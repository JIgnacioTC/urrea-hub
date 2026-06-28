using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Auth;
using UrreaHub.Application.Common;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Application.Seguridad;
using UrreaHub.Domain.Seguridad;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class SecurityAdminService : ISecurityAdminService
{
    private static readonly HashSet<string> ProtectedRoleCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        AppRoles.Colaborador, AppRoles.Jefe, AppRoles.RhAdmin, AppRoles.TiAdmin,
    };

    private readonly UrreaHubDbContext _context;
    private readonly IRoleResolutionService _roleResolution;
    private readonly IAuditService _audit;

    public SecurityAdminService(UrreaHubDbContext context, IRoleResolutionService roleResolution, IAuditService audit)
    {
        _context = context;
        _roleResolution = roleResolution;
        _audit = audit;
    }

    public async Task<IReadOnlyList<RolDto>> ListRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles.AsNoTracking()
            .OrderBy(r => r.Codigo)
            .Select(r => new RolDto(
                r.Id,
                r.Codigo,
                r.Nombre,
                r.Descripcion,
                r.IsActive,
                r.RolPermisos.Count(rp => rp.IsActive),
                r.ColaboradorRoles.Count(cr => cr.IsActive)))
            .ToListAsync(cancellationToken);
    }

    public async Task<RolDto> UpsertRoleAsync(Guid? id, UpsertRolDto dto, CancellationToken cancellationToken = default)
    {
        Rol rol;
        if (id.HasValue)
        {
            rol = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken)
                ?? throw new InvalidOperationException("Rol no encontrado.");
            if (ProtectedRoleCodes.Contains(rol.Codigo) && !string.Equals(rol.Codigo, dto.Codigo, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("No se puede renombrar un rol del sistema.");

            rol.Codigo = dto.Codigo.Trim();
            rol.Nombre = dto.Nombre.Trim();
            rol.Descripcion = dto.Descripcion;
            rol.IsActive = dto.IsActive;
            rol.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            rol = new Rol
            {
                Id = Guid.NewGuid(),
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
            };
            _context.Roles.Add(rol);
        }

        await _context.SaveChangesAsync(cancellationToken);
        var listed = await ListRolesAsync(cancellationToken);
        return listed.First(r => r.Id == rol.Id);
    }

    public async Task<IReadOnlyList<PermisoDto>> ListPermissionsAsync(string? modulo, CancellationToken cancellationToken = default)
    {
        var query = _context.Permisos.AsNoTracking().Where(p => p.IsActive);
        if (!string.IsNullOrWhiteSpace(modulo))
            query = query.Where(p => p.Modulo == modulo);

        return await query
            .OrderBy(p => p.Modulo).ThenBy(p => p.Codigo)
            .Select(p => new PermisoDto(p.Id, p.Codigo, p.Modulo, p.Nombre, p.Descripcion, p.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<RolePermissionMatrixDto> GetRolePermissionMatrixAsync(CancellationToken cancellationToken = default)
    {
        var roles = await ListRolesAsync(cancellationToken);
        var permisos = await ListPermissionsAsync(null, cancellationToken);
        var links = await _context.RolPermisos.AsNoTracking()
            .Where(rp => rp.IsActive)
            .Select(rp => new { rp.RolId, rp.PermisoId })
            .ToListAsync(cancellationToken);

        var roleById = roles.ToDictionary(r => r.Id, r => r.Codigo);
        var linkSet = links
            .Where(l => roleById.ContainsKey(l.RolId))
            .Select(l => (l.RolId, l.PermisoId))
            .ToHashSet();

        var rows = permisos.Select(p =>
        {
            var byRole = roles.ToDictionary(
                r => r.Codigo,
                r => linkSet.Contains((r.Id, p.Id)),
                StringComparer.OrdinalIgnoreCase);
            return new RolePermissionRowDto(p.Id, p.Codigo, p.Modulo, p.Nombre, byRole);
        }).ToList();

        return new RolePermissionMatrixDto(roles, permisos, rows);
    }

    public async Task UpdateRolePermissionsAsync(
        Guid rolId,
        UpdateRolePermissionsDto dto,
        string performedBy,
        CancellationToken cancellationToken = default)
    {
        var rol = await _context.Roles.FirstOrDefaultAsync(r => r.Id == rolId, cancellationToken)
            ?? throw new InvalidOperationException("Rol no encontrado.");

        var desired = dto.PermisoIds.Distinct().ToHashSet();
        var existing = await _context.RolPermisos.Where(rp => rp.RolId == rolId).ToListAsync(cancellationToken);

        foreach (var rp in existing)
        {
            var active = desired.Contains(rp.PermisoId);
            if (rp.IsActive != active)
            {
                rp.IsActive = active;
                rp.UpdatedAt = DateTime.UtcNow;
            }
            desired.Remove(rp.PermisoId);
        }

        foreach (var permisoId in desired)
        {
            _context.RolPermisos.Add(new RolPermiso
            {
                Id = Guid.NewGuid(),
                RolId = rolId,
                PermisoId = permisoId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Seguridad", "UpdateRolePermissions", "Rol", rolId, performedBy,
            $"Permisos actualizados para rol {rol.Codigo}", cancellationToken);
    }

    public async Task<IReadOnlyList<ColaboradorAccessSummaryDto>> SearchColaboradoresAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Colaboradores.AsNoTracking()
            .Include(c => c.Puesto)
            .Include(c => c.Departamento)
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

        var colaboradores = await query
            .OrderBy(c => c.ApellidoPaterno).ThenBy(c => c.Nombre)
            .Take(50)
            .ToListAsync(cancellationToken);

        var ids = colaboradores.Select(c => c.Id).ToList();
        var roleRows = await _context.ColaboradorRoles.AsNoTracking()
            .Where(cr => ids.Contains(cr.ColaboradorId) && cr.IsActive)
            .Join(_context.Roles.Where(r => r.IsActive), cr => cr.RolId, r => r.Id, (cr, r) => new { cr.ColaboradorId, r.Codigo })
            .ToListAsync(cancellationToken);

        var rolesByColaborador = roleRows
            .GroupBy(x => x.ColaboradorId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<string>)g.Select(x => x.Codigo).Distinct().ToList());

        return colaboradores.Select(c => new ColaboradorAccessSummaryDto(
            c.Id,
            c.NumeroEmpleado,
            $"{c.Nombre} {c.ApellidoPaterno}".Trim(),
            c.Puesto.Nombre,
            c.Departamento.Nombre,
            rolesByColaborador.GetValueOrDefault(c.Id) ?? Array.Empty<string>())).ToList();
    }

    public async Task<ColaboradorAccessDetailDto?> GetColaboradorAccessAsync(
        Guid colaboradorId,
        CancellationToken cancellationToken = default)
    {
        var c = await _context.Colaboradores.AsNoTracking()
            .Include(x => x.Puesto)
            .Include(x => x.Departamento).ThenInclude(d => d.Area)
            .Include(x => x.CuentaAcceso)
            .FirstOrDefaultAsync(x => x.Id == colaboradorId && x.IsActive, cancellationToken);

        if (c is null) return null;

        var roles = await ListRolesForColaboradorAsync(colaboradorId, cancellationToken);
        var resolvedRoles = await _roleResolution.ResolveRolesAsync(c, cancellationToken);
        var permisos = await _roleResolution.ResolvePermissionsAsync(resolvedRoles, colaboradorId, cancellationToken);

        return new ColaboradorAccessDetailDto(
            c.Id,
            c.NumeroEmpleado,
            $"{c.Nombre} {c.ApellidoPaterno}".Trim(),
            c.Puesto.Nombre,
            c.Departamento.Nombre,
            c.Departamento.Area?.Nombre,
            roles,
            permisos);
    }

    public async Task AssignRoleAsync(Guid colaboradorId, Guid rolId, string performedBy, CancellationToken cancellationToken = default)
    {
        var exists = await _context.ColaboradorRoles
            .AnyAsync(cr => cr.ColaboradorId == colaboradorId && cr.RolId == rolId && cr.IsActive, cancellationToken);
        if (exists) return;

        var inactive = await _context.ColaboradorRoles
            .FirstOrDefaultAsync(cr => cr.ColaboradorId == colaboradorId && cr.RolId == rolId && !cr.IsActive, cancellationToken);
        if (inactive is not null)
        {
            inactive.IsActive = true;
            inactive.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _context.ColaboradorRoles.Add(new ColaboradorRol
            {
                Id = Guid.NewGuid(),
                ColaboradorId = colaboradorId,
                RolId = rolId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Seguridad", "AssignRole", "Colaborador", colaboradorId, performedBy,
            $"Rol asignado: {rolId}", cancellationToken);
    }

    public async Task RemoveRoleAsync(Guid colaboradorId, Guid rolId, string performedBy, CancellationToken cancellationToken = default)
    {
        var rol = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == rolId, cancellationToken);
        if (rol is not null && string.Equals(rol.Codigo, AppRoles.Colaborador, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("No se puede quitar el rol base Colaborador.");

        var assignment = await _context.ColaboradorRoles
            .FirstOrDefaultAsync(cr => cr.ColaboradorId == colaboradorId && cr.RolId == rolId && cr.IsActive, cancellationToken);
        if (assignment is null) return;

        assignment.IsActive = false;
        assignment.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Seguridad", "RemoveRole", "Colaborador", colaboradorId, performedBy,
            $"Rol removido: {rolId}", cancellationToken);
    }

    private async Task<IReadOnlyList<RolDto>> ListRolesForColaboradorAsync(Guid colaboradorId, CancellationToken cancellationToken)
    {
        return await _context.ColaboradorRoles.AsNoTracking()
            .Where(cr => cr.ColaboradorId == colaboradorId && cr.IsActive)
            .Join(_context.Roles.Where(r => r.IsActive), cr => cr.RolId, r => r.Id, (cr, r) => r)
            .Select(r => new RolDto(
                r.Id, r.Codigo, r.Nombre, r.Descripcion, r.IsActive,
                r.RolPermisos.Count(rp => rp.IsActive),
                r.ColaboradorRoles.Count(cr => cr.IsActive)))
            .ToListAsync(cancellationToken);
    }
}
