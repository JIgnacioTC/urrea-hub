using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Common;
using UrreaHub.Application.CoreRH;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class ColaboradorService : IColaboradorService
{
    private readonly UrreaHubDbContext _context;
    private readonly IAuditService _audit;

    public ColaboradorService(UrreaHubDbContext context, IAuditService audit)
    {
        _context = context;
        _audit = audit;
    }

    public async Task<ColaboradorPerfilDto?> GetPerfilAsync(Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        var c = await _context.Colaboradores
            .AsNoTracking()
            .Include(x => x.Puesto)
            .Include(x => x.Departamento)
            .Include(x => x.Sede)
            .Include(x => x.JefeDirecto)
            .Include(x => x.DatosSensibles)
            .Include(x => x.Subordinados.Where(s => s.IsActive))
                .ThenInclude(s => s.Puesto)
            .FirstOrDefaultAsync(x => x.Id == colaboradorId && x.IsActive, cancellationToken);

        if (c is null) return null;

        var jefeNombre = c.JefeDirecto is null
            ? null
            : $"{c.JefeDirecto.Nombre} {c.JefeDirecto.ApellidoPaterno}";

        var rfc = c.DatosSensibles?.Rfc;

        return new ColaboradorPerfilDto(
            c.Id, c.NumeroEmpleado, c.Nombre, c.ApellidoPaterno, c.ApellidoMaterno,
            c.Email, rfc, c.Telefono, c.FechaIngreso, c.FechaBaja, c.NominaSyncAt,
            c.Puesto.Nombre, c.Departamento.Nombre, c.Sede?.Nombre, jefeNombre,
            c.Subordinados.Select(s => new ColaboradorResumenDto(
                s.Id, s.NumeroEmpleado, $"{s.Nombre} {s.ApellidoPaterno}", s.Puesto.Nombre, s.JefeDirectoId?.ToString()
            )).ToList());
    }

    public async Task<PagedResult<ColaboradorResumenDto>> ListAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _context.Colaboradores.AsNoTracking()
            .Include(c => c.Puesto)
            .Where(c => c.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(c =>
                c.NumeroEmpleado.ToLower().Contains(term) ||
                c.Nombre.ToLower().Contains(term) ||
                c.ApellidoPaterno.ToLower().Contains(term) ||
                c.Email.ToLower().Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(c => c.ApellidoPaterno).ThenBy(c => c.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ColaboradorResumenDto(
                c.Id, c.NumeroEmpleado, $"{c.Nombre} {c.ApellidoPaterno}", c.Puesto.Nombre, c.JefeDirectoId.ToString()))
            .ToListAsync(cancellationToken);

        return new PagedResult<ColaboradorResumenDto> { Items = items, Total = total, Page = page, PageSize = pageSize };
    }

    public async Task<ColaboradorRhDto?> UpdateRhFieldsAsync(Guid id, ColaboradorRhUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var c = await _context.Colaboradores
            .Include(x => x.DatosSensibles)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);
        if (c is null) return null;

        c.Nombre = dto.Nombre;
        c.ApellidoPaterno = dto.ApellidoPaterno;
        c.ApellidoMaterno = dto.ApellidoMaterno;
        c.Telefono = dto.Telefono;
        c.PuestoId = dto.PuestoId;
        c.DepartamentoId = dto.DepartamentoId;
        c.SedeId = dto.SedeId;
        c.CentroCostoId = dto.CentroCostoId;
        c.RelacionLaboralId = dto.RelacionLaboralId;
        c.JefeDirectoId = dto.JefeDirectoId;
        c.IsManualOverride = true;
        c.SyncStatus = Domain.CoreRH.EmployeeSyncStatus.ManualOverride;
        c.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("HCM", "UpdateRhFields", "Colaborador", id, "rh-admin", "Actualización manual de campos RH", cancellationToken);

        return new ColaboradorRhDto(
            c.Id, c.NumeroEmpleado, c.Nombre, c.ApellidoPaterno, c.ApellidoMaterno,
            c.Email, c.DatosSensibles?.Rfc, c.Telefono, c.FechaIngreso, c.FechaBaja,
            c.PuestoId, c.DepartamentoId, c.SedeId, c.CentroCostoId, c.RelacionLaboralId, c.JefeDirectoId, c.IsActive);
    }
}

public class OrganigramaService : IOrganigramaService
{
    private readonly UrreaHubDbContext _context;

    public OrganigramaService(UrreaHubDbContext context) => _context = context;

    public async Task<OrganigramaNodoDto?> GetArbolAsync(Guid? raizId, Guid? sedeId, Guid? departamentoId, CancellationToken cancellationToken = default)
    {
        var colaboradores = await _context.Colaboradores.AsNoTracking()
            .Include(c => c.Puesto)
            .Include(c => c.Departamento)
            .Where(c => c.IsActive)
            .Where(c => !sedeId.HasValue || c.SedeId == sedeId)
            .Where(c => !departamentoId.HasValue || c.DepartamentoId == departamentoId)
            .ToListAsync(cancellationToken);

        if (colaboradores.Count == 0) return null;

        var lookup = colaboradores.ToLookup(c => c.JefeDirectoId);
        OrganigramaNodoDto Build(Domain.CoreRH.Colaborador c) => new(
            c.Id, c.NumeroEmpleado, $"{c.Nombre} {c.ApellidoPaterno}", c.Puesto.Nombre, c.Departamento.Nombre,
            lookup[c.Id].Select(Build).ToList());

        if (raizId.HasValue)
        {
            var raiz = colaboradores.FirstOrDefault(c => c.Id == raizId);
            return raiz is null ? null : Build(raiz);
        }

        var topLevel = colaboradores.Where(c => c.JefeDirectoId is null || colaboradores.All(x => x.Id != c.JefeDirectoId)).ToList();
        if (topLevel.Count == 1) return Build(topLevel[0]);

        return new OrganigramaNodoDto(Guid.Empty, "ROOT", "Organización URREA", "", "",
            topLevel.Select(Build).ToList());
    }
}
