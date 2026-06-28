using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Auth;
using UrreaHub.Application.Common;
using UrreaHub.Domain.CoreRH;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class RoleResolutionService : IRoleResolutionService
{
    private readonly UrreaHubDbContext _context;

    public RoleResolutionService(UrreaHubDbContext context) => _context = context;

    public async Task<IReadOnlyList<string>> ResolveRolesAsync(
        Colaborador colaborador,
        CancellationToken cancellationToken = default)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { AppRoles.Colaborador };

        var dbRoles = await _context.ColaboradorRoles.AsNoTracking()
            .Where(cr => cr.ColaboradorId == colaborador.Id && cr.IsActive)
            .Join(_context.Roles.Where(r => r.IsActive),
                cr => cr.RolId,
                r => r.Id,
                (_, r) => r.Codigo)
            .Distinct()
            .ToListAsync(cancellationToken);

        foreach (var role in dbRoles)
            set.Add(role);

        await MergeLegacyAdminRolesAsync(colaborador, set, cancellationToken);

        if (!set.Contains(AppRoles.Jefe))
        {
            var tieneSubordinados = await _context.Colaboradores
                .AnyAsync(c => c.JefeDirectoId == colaborador.Id && c.IsActive, cancellationToken);
            if (tieneSubordinados)
                set.Add(AppRoles.Jefe);
        }

        return set.ToList();
    }

    public async Task<IReadOnlyList<string>> ResolvePermissionsAsync(
        IReadOnlyList<string> roles,
        Guid colaboradorId,
        CancellationToken cancellationToken = default)
    {
        var set = new HashSet<string>(RolePermissionMap.Resolve(roles), StringComparer.OrdinalIgnoreCase);

        var dbPermissions = await _context.ColaboradorRoles.AsNoTracking()
            .Where(cr => cr.ColaboradorId == colaboradorId && cr.IsActive)
            .Join(_context.RolPermisos.Where(rp => rp.IsActive),
                cr => cr.RolId,
                rp => rp.RolId,
                (_, rp) => rp.PermisoId)
            .Join(_context.Permisos.Where(p => p.IsActive),
                permisoId => permisoId,
                p => p.Id,
                (_, p) => p.Codigo)
            .Distinct()
            .ToListAsync(cancellationToken);

        foreach (var permission in dbPermissions)
            set.Add(permission);

        return set.ToList();
    }

    private async Task MergeLegacyAdminRolesAsync(
        Colaborador colaborador,
        HashSet<string> roles,
        CancellationToken cancellationToken)
    {
        var cuenta = colaborador.CuentaAcceso
            ?? await _context.CuentasAcceso
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ColaboradorId == colaborador.Id && c.IsActive, cancellationToken);

        if (cuenta?.EsRhAdmin == true)
            roles.Add(AppRoles.RhAdmin);
        if (cuenta?.EsTiAdmin == true)
            roles.Add(AppRoles.TiAdmin);
    }
}
