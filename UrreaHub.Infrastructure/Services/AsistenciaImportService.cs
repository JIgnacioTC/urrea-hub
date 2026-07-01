using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Asistencia;
using UrreaHub.Application.Common;
using UrreaHub.Domain.Asistencia;
using UrreaHub.Domain.CoreRH;
using UrreaHub.Domain.Vacaciones;
using UrreaHub.Domain.Common;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class AsistenciaImportService : IAsistenciaImportService
{
    private readonly UrreaHubDbContext _context;

    public AsistenciaImportService(UrreaHubDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ImportResultDto>> ImportarExcelAsistenciasAsync(Stream excelStream, string performedBy, CancellationToken cancellationToken = default)
    {
        var errores = new List<string>();
        int totalFilas = 0;
        int detectadas = 0;
        int justificadas = 0;
        int porRevisar = 0;

        try
        {
            using var workbook = new XLWorkbook(excelStream);
            var ws = workbook.Worksheets.FirstOrDefault();
            if (ws == null) return Result<ImportResultDto>.Fail("El archivo Excel está vacío.");

            var rows = ws.RangeUsed().RowsUsed().Skip(1); // Skip header

            var processedDates = new HashSet<DateTime>();
            var colaboradoresDict = await _context.Colaboradores.AsNoTracking()
                .Where(c => c.IsActive)
                .ToDictionaryAsync(c => c.NumeroEmpleado, c => c.Id, cancellationToken);

            var registrosAInsertar = new List<RegistroAsistencia>();

            foreach (var row in rows)
            {
                totalFilas++;
                var numEmpleado = row.Cell(1).GetString().Trim();
                if (string.IsNullOrEmpty(numEmpleado)) continue;

                if (!colaboradoresDict.TryGetValue(numEmpleado, out var colabId))
                {
                    errores.Add($"Fila {row.RowNumber()}: No se encontró el número de empleado {numEmpleado}.");
                    continue;
                }

                if (!DateTime.TryParse(row.Cell(3).GetString(), out var fecha))
                {
                    errores.Add($"Fila {row.RowNumber()}: Fecha inválida.");
                    continue;
                }
                fecha = fecha.Date;
                processedDates.Add(fecha);

                // Try parse times
                DateTime? entrada = null;
                DateTime? salida = null;
                if (DateTime.TryParse(row.Cell(4).GetString(), out var e)) entrada = e;
                if (DateTime.TryParse(row.Cell(5).GetString(), out var s)) salida = s;

                // Solo guardamos el registro si hay al menos una entrada o salida. 
                // Si están vacías, asumimos inasistencia.
                if (entrada.HasValue || salida.HasValue)
                {
                    registrosAInsertar.Add(new RegistroAsistencia
                    {
                        Id = Guid.NewGuid(),
                        ColaboradorId = colabId,
                        Fecha = fecha,
                        HoraEntrada = entrada,
                        HoraSalida = salida,
                        Fuente = FuenteRegistroAsistencia.Biometrico,
                        TipoRegistro = "Oficina",
                        Estado = entrada.HasValue && salida.HasValue ? EstadoRegistroAsistencia.Completo : EstadoRegistroAsistencia.EntradaSinSalida,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }

            if (registrosAInsertar.Any())
            {
                // Limpiar registros existentes para esas fechas y empleados, para evitar duplicados si se reimporta
                var newFechas = registrosAInsertar.Select(r => r.Fecha).Distinct().ToList();
                var newColabs = registrosAInsertar.Select(r => r.ColaboradorId).Distinct().ToList();
                var existing = await _context.RegistrosAsistencia
                    .Where(r => r.IsActive && newFechas.Contains(r.Fecha) && newColabs.Contains(r.ColaboradorId) && r.Fuente == FuenteRegistroAsistencia.Biometrico)
                    .ToListAsync(cancellationToken);
                
                _context.RegistrosAsistencia.RemoveRange(existing);
                _context.RegistrosAsistencia.AddRange(registrosAInsertar);
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Paso 2: Para cada fecha procesada, verificar quién faltó.
            foreach (var date in processedDates)
            {
                var registrosDia = await _context.RegistrosAsistencia.AsNoTracking()
                    .Where(r => r.Fecha == date && r.IsActive)
                    .Select(r => r.ColaboradorId)
                    .ToListAsync(cancellationToken);

                var ausentes = colaboradoresDict.Values.Except(registrosDia).ToList();

                foreach (var ausenteId in ausentes)
                {
                    // Check if missing incidence already exists
                    var existingInc = await _context.IncidenciasAsistencia.FirstOrDefaultAsync(i =>
                        i.ColaboradorId == ausenteId && i.Fecha == date && i.Tipo == TipoIncidenciaAsistencia.AusenciaInjustificada && i.IsActive, cancellationToken);
                    
                    if (existingInc != null) continue;

                    // Revisar si hay un permiso o vacación aprobada para ese día
                    var tienePermiso = await _context.SolicitudesAusencia.AsNoTracking().AnyAsync(s =>
                        s.ColaboradorId == ausenteId && s.IsActive &&
                        s.Estado == EstadoSolicitud.Aprobada &&
                        s.FechaInicio.Date <= date && s.FechaFin.Date >= date, cancellationToken);

                    detectadas++;
                    var inc = new IncidenciaAsistencia
                    {
                        Id = Guid.NewGuid(),
                        ColaboradorId = ausenteId,
                        Fecha = date,
                        Tipo = tienePermiso ? TipoIncidenciaAsistencia.PermisoRelacionado : TipoIncidenciaAsistencia.AusenciaInjustificada,
                        Estado = tienePermiso ? EstadoIncidenciaAsistencia.Justificada : EstadoIncidenciaAsistencia.Detectada,
                        Descripcion = tienePermiso ? "Ausencia justificada por permiso/vacación" : "Ausencia detectada por importación de asistencia",
                        GeneraPrenomina = !tienePermiso, // Si no tiene permiso, genera descuento en prenomina
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    
                    if (tienePermiso) justificadas++;
                    else porRevisar++;

                    _context.IncidenciasAsistencia.Add(inc);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            var result = new ImportResultDto(totalFilas, detectadas, justificadas, porRevisar, errores);
            return Result<ImportResultDto>.Ok(result);
        }
        catch (Exception ex)
        {
            return Result<ImportResultDto>.Fail($"Error al procesar el archivo: {ex.Message}");
        }
    }
}
