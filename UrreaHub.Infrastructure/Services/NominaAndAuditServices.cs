using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UrreaHub.Application.Nomina;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Domain.Auditoria;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.CoreRH;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly UrreaHubDbContext _context;

    public AuditService(UrreaHubDbContext context) => _context = context;

    public async Task LogEventoAsync(string modulo, string accion, string entidad, Guid entidadId, string? usuario, string? detalle, CancellationToken cancellationToken = default)
    {
        _context.BitacoraEventos.Add(new BitacoraEvento
        {
            Id = Guid.NewGuid(),
            Modulo = modulo,
            Accion = accion,
            Entidad = entidad,
            EntidadId = entidadId,
            Usuario = usuario,
            Detalle = detalle,
            FechaEvento = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        });
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task LogCambioEstadoAsync(string entidad, Guid entidadId, string anterior, string nuevo, string? usuario, string? motivo, CancellationToken cancellationToken = default)
    {
        _context.CambiosEstado.Add(new CambioEstado
        {
            Id = Guid.NewGuid(),
            Entidad = entidad,
            EntidadId = entidadId,
            EstadoAnterior = anterior,
            EstadoNuevo = nuevo,
            Usuario = usuario,
            Motivo = motivo,
            FechaCambio = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        });
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class StubNominaSyncAdapter : INominaSyncAdapter
{
    public Task<IReadOnlyList<ColaboradorNominaRecord>> FetchColaboradoresAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ColaboradorNominaRecord>>(Array.Empty<ColaboradorNominaRecord>());
}

public class CsvNominaSyncAdapter : INominaSyncAdapter
{
    private readonly string _csvPath;

    public CsvNominaSyncAdapter(IOptions<NominaSyncSettings> settings)
    {
        _csvPath = settings.Value.CsvPath ?? Path.Combine(AppContext.BaseDirectory, "Data", "nomina.csv");
    }

    public async Task<IReadOnlyList<ColaboradorNominaRecord>> FetchColaboradoresAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_csvPath))
            return Array.Empty<ColaboradorNominaRecord>();

        var lines = await File.ReadAllLinesAsync(_csvPath, cancellationToken);
        var records = new List<ColaboradorNominaRecord>();
        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var parts = line.Split(',');
            if (parts.Length < 6) continue;
            records.Add(new ColaboradorNominaRecord(
                parts[0].Trim(),
                parts[1].Trim(),
                parts[2].Trim(),
                parts.Length > 3 ? parts[3].Trim() : null,
                parts[4].Trim(),
                parts.Length > 5 ? parts[5].Trim() : null,
                DateTime.Parse(parts[6].Trim()),
                parts.Length > 7 && !string.IsNullOrWhiteSpace(parts[7]) ? DateTime.Parse(parts[7].Trim()) : null));
        }
        return records;
    }
}

public class NominaSyncService : INominaSyncService
{
    private readonly UrreaHubDbContext _context;
    private readonly INominaSyncAdapter _adapter;
    private readonly IAuditService _audit;

    public NominaSyncService(UrreaHubDbContext context, INominaSyncAdapter adapter, IAuditService audit)
    {
        _context = context;
        _adapter = adapter;
        _audit = audit;
    }

    public async Task<int> SyncAsync(CancellationToken cancellationToken = default)
    {
        var records = await _adapter.FetchColaboradoresAsync(cancellationToken);
        var count = 0;
        foreach (var record in records)
        {
            var colaborador = await _context.Colaboradores
                .Include(c => c.DatosSensibles)
                .FirstOrDefaultAsync(c => c.NumeroEmpleado == record.NumeroEmpleado, cancellationToken);

            if (colaborador is null)
                continue;

            if (colaborador.IsManualOverride)
                continue;

            colaborador.Email = record.Email;
            colaborador.FechaIngreso = record.FechaIngreso;
            colaborador.FechaBaja = record.FechaBaja;
            colaborador.NominaSyncAt = DateTime.UtcNow;
            colaborador.ExternalSource = EmployeeExternalSource.Nomina;
            colaborador.SyncStatus = EmployeeSyncStatus.Synced;
            colaborador.UpdatedAt = DateTime.UtcNow;

            if (colaborador.DatosSensibles is null)
            {
                colaborador.DatosSensibles = new ColaboradorDatosSensibles
                {
                    Id = Guid.NewGuid(),
                    ColaboradorId = colaborador.Id,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                };
                _context.ColaboradoresDatosSensibles.Add(colaborador.DatosSensibles);
            }

            if (!string.IsNullOrWhiteSpace(record.Rfc))
                colaborador.DatosSensibles.Rfc = record.Rfc;

            if (record.FechaBaja.HasValue && record.FechaBaja.Value.Date <= DateTime.UtcNow.Date)
                colaborador.IsActive = false;

            count++;
        }

        if (count > 0)
            await _context.SaveChangesAsync(cancellationToken);

        await _audit.LogEventoAsync("Nomina", "Sync", "Colaborador", Guid.Empty, "system",
            $"Registros sincronizados: {count}", cancellationToken);

        return count;
    }
}

public class NominaSyncBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<NominaSyncSettings> _settings;
    private readonly ILogger<NominaSyncBackgroundService> _logger;

    public NominaSyncBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<NominaSyncSettings> settings,
        ILogger<NominaSyncBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _settings = settings;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.Value.Enabled) return;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var sync = scope.ServiceProvider.GetRequiredService<INominaSyncService>();
                var count = await sync.SyncAsync(stoppingToken);
                _logger.LogInformation("Nomina sync completed: {Count} records", count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nomina sync failed");
            }

            await Task.Delay(TimeSpan.FromMinutes(_settings.Value.IntervalMinutes), stoppingToken);
        }
    }
}
