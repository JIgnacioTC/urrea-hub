using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Auth;
using UrreaHub.Application.Common;
using UrreaHub.Domain.Seguridad;

namespace UrreaHub.Infrastructure.Persistence;

/// <summary>Catálogo RBAC persistente + bridge desde banderas legacy en CuentasAcceso.</summary>
public static class SecuritySeed
{
    private static readonly (string Codigo, string Modulo, string Nombre)[] PermisoCatalog =
    {
        (AppPermissions.HcmRead, "HCM", "Consulta HCM"),
        (AppPermissions.HcmWrite, "HCM", "Edición HCM"),
        (AppPermissions.HcmReadSensitive, "HCM", "Datos sensibles HCM"),
        (AppPermissions.HcmReadSalary, "HCM", "Compensación HCM"),
        (AppPermissions.VacationsApprove, "Vacaciones", "Aprobar vacaciones"),
        (AppPermissions.VacationsApprovePayroll, "Vacaciones", "Aprobar solicitudes (nivel nómina)"),
        (AppPermissions.AttendanceValidate, "Asistencia", "Validar asistencia"),
        (AppPermissions.AttendanceRead, "Asistencia", "Consultar asistencia"),
        (AppPermissions.AttendanceCheckIn, "Asistencia", "Registrar entrada"),
        (AppPermissions.AttendanceCheckOut, "Asistencia", "Registrar salida"),
        (AppPermissions.AttendanceCorrectionCreate, "Asistencia", "Solicitar corrección"),
        (AppPermissions.AttendanceCorrectionApprove, "Asistencia", "Aprobar corrección"),
        (AppPermissions.AttendanceTeamRead, "Asistencia", "Asistencia del equipo"),
        (AppPermissions.AttendanceAdminRead, "Asistencia", "Administrar asistencia"),
        (AppPermissions.AttendanceAdminWrite, "Asistencia", "Configurar asistencia"),
        (AppPermissions.AttendancePayrollGenerate, "Asistencia", "Generar prenómina asistencia"),
        (AppPermissions.AttendancePayrollSend, "Asistencia", "Enviar prenómina asistencia"),
        (AppPermissions.AttendanceCommercialRead, "Asistencia", "Consultar comercial"),
        (AppPermissions.AttendanceCommercialWrite, "Asistencia", "Registrar comercial"),
        (AppPermissions.OnboardingRead, "Onboarding", "Consultar onboarding"),
        (AppPermissions.OnboardingTaskComplete, "Onboarding", "Completar tareas"),
        (AppPermissions.OnboardingTeamRead, "Onboarding", "Onboarding del equipo"),
        (AppPermissions.OnboardingAdminRead, "Onboarding", "Administrar onboarding"),
        (AppPermissions.OnboardingAdminWrite, "Onboarding", "Configurar onboarding"),
        (AppPermissions.RequisitionsRead, "Requisiciones", "Consultar requisiciones"),
        (AppPermissions.RequisitionsCreate, "Requisiciones", "Crear requisiciones"),
        (AppPermissions.RequisitionsApprove, "Requisiciones", "Aprobar requisiciones"),
        (AppPermissions.RequisitionsAdmin, "Requisiciones", "Administrar requisiciones"),
        (AppPermissions.RecruitmentRead, "Reclutamiento", "Consultar reclutamiento"),
        (AppPermissions.RecruitmentWrite, "Reclutamiento", "Operar reclutamiento"),
        (AppPermissions.CompensationRead, "Compensaciones", "Consultar compensación"),
        (AppPermissions.CompensationRequestCreate, "Compensaciones", "Solicitar ajustes/beneficios"),
        (AppPermissions.CompensationAdminRead, "Compensaciones", "Administrar compensación"),
        (AppPermissions.CompensationAdminWrite, "Compensaciones", "Operar compensación"),
        (AppPermissions.CompensationApprove, "Compensaciones", "Aprobar ajustes"),
        (AppPermissions.BenefitsAdminRead, "Beneficios", "Administrar beneficios"),
        (AppPermissions.BenefitsAdminWrite, "Beneficios", "Operar beneficios"),
        (AppPermissions.DocumentsUpload, "Documentos", "Cargar documentos"),
        (AppPermissions.EthicsReadCase, "Ética", "Consultar casos ética"),
        (AppPermissions.IntegrationRunSync, "Integraciones", "Ejecutar sincronizaciones"),
        (AppPermissions.IntegrationRead, "Integraciones", "Consultar integraciones"),
        (AppPermissions.AnalyticsExecutiveView, "Analytics", "Vista ejecutiva"),
        (AppPermissions.TiMetadataRead, "TI", "Consulta metadatos TI"),
        (AppPermissions.TiMetadataWrite, "TI", "Edición metadatos TI"),
        (AppPermissions.AdminEntitiesRead, "Admin", "Consulta entidades admin"),
        (AppPermissions.AdminEntitiesWrite, "Admin", "Edición entidades admin"),
        (AppPermissions.AdminEntitiesDelete, "Admin", "Eliminación entidades admin"),
        (AppPermissions.AuditRead, "Auditoría", "Consulta auditoría"),
        (AppPermissions.ManagerApproval, "Gestión", "Aprobaciones de jefe"),
    };

    private static readonly (string Codigo, string Nombre, string? Descripcion)[] RolCatalog =
    {
        (AppRoles.Colaborador, "Colaborador", "Acceso base al portal"),
        (AppRoles.Jefe, "Jefe de equipo", "Gestión de subordinados directos"),
        (AppRoles.RhAdmin, "Administrador RH", "Administración de recursos humanos"),
        (AppRoles.TiAdmin, "Administrador TI", "Administración de plataforma y metadatos"),
        (AppRoles.NominaAdmin, "Administrador de Nómina", "Aprobación de solicitudes en el nivel de nómina"),
    };

    public static async Task SeedAsync(UrreaHubDbContext context)
    {
        await UpsertPermisosAsync(context);
        await UpsertRolesAsync(context);
        await context.SaveChangesAsync();

        await UpsertRolPermisosAsync(context);
        await SyncColaboradorRolesFromLegacyFlagsAsync(context);
        await context.SaveChangesAsync();
    }

    private static async Task UpsertPermisosAsync(UrreaHubDbContext context)
    {
        var existing = await context.Permisos.ToDictionaryAsync(p => p.Codigo);
        foreach (var (codigo, modulo, nombre) in PermisoCatalog)
        {
            if (existing.TryGetValue(codigo, out var permiso))
            {
                permiso.Modulo = modulo;
                permiso.Nombre = nombre;
                permiso.IsActive = true;
                permiso.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                context.Permisos.Add(new Permiso
                {
                    Id = Guid.NewGuid(),
                    Codigo = codigo,
                    Modulo = modulo,
                    Nombre = nombre,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                });
            }
        }
    }

    private static async Task UpsertRolesAsync(UrreaHubDbContext context)
    {
        var existing = await context.Roles.ToDictionaryAsync(r => r.Codigo);
        foreach (var (codigo, nombre, descripcion) in RolCatalog)
        {
            if (existing.TryGetValue(codigo, out var rol))
            {
                rol.Nombre = nombre;
                rol.Descripcion = descripcion;
                rol.IsActive = true;
                rol.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                context.Roles.Add(new Rol
                {
                    Id = Guid.NewGuid(),
                    Codigo = codigo,
                    Nombre = nombre,
                    Descripcion = descripcion,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                });
            }
        }
    }

    private static async Task UpsertRolPermisosAsync(UrreaHubDbContext context)
    {
        var roles = await context.Roles.ToDictionaryAsync(r => r.Codigo);
        var permisos = await context.Permisos.ToDictionaryAsync(p => p.Codigo);
        var existing = await context.RolPermisos
            .Select(rp => new { rp.RolId, rp.PermisoId })
            .ToListAsync();
        var existingSet = existing.Select(x => (x.RolId, x.PermisoId)).ToHashSet();

        foreach (var rolEntry in roles)
        {
            var permissionCodes = RolePermissionMap.ForRole(rolEntry.Key);
            if (!roles.TryGetValue(rolEntry.Key, out var rol)) continue;

            foreach (var code in permissionCodes)
            {
                if (!permisos.TryGetValue(code, out var permiso)) continue;
                if (existingSet.Contains((rol.Id, permiso.Id))) continue;

                context.RolPermisos.Add(new RolPermiso
                {
                    Id = Guid.NewGuid(),
                    RolId = rol.Id,
                    PermisoId = permiso.Id,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                });
            }
        }
    }

    private static async Task SyncColaboradorRolesFromLegacyFlagsAsync(UrreaHubDbContext context)
    {
        var roles = await context.Roles.ToDictionaryAsync(r => r.Codigo);
        if (!roles.TryGetValue(AppRoles.Colaborador, out var rolColaborador)) return;

        var cuentas = await context.CuentasAcceso
            .Where(c => c.IsActive)
            .ToListAsync();

        var assigned = await context.ColaboradorRoles
            .Select(cr => new { cr.ColaboradorId, cr.RolId })
            .ToListAsync();
        var assignedSet = assigned.Select(x => (x.ColaboradorId, x.RolId)).ToHashSet();

        foreach (var cuenta in cuentas)
        {
            EnsureAssignment(context, assignedSet, cuenta.ColaboradorId, rolColaborador.Id);

            if (cuenta.EsRhAdmin && roles.TryGetValue(AppRoles.RhAdmin, out var rolRh))
                EnsureAssignment(context, assignedSet, cuenta.ColaboradorId, rolRh.Id);

            if (cuenta.EsTiAdmin && roles.TryGetValue(AppRoles.TiAdmin, out var rolTi))
                EnsureAssignment(context, assignedSet, cuenta.ColaboradorId, rolTi.Id);
        }

        // Jefe: subordinados activos
        if (roles.TryGetValue(AppRoles.Jefe, out var rolJefe))
        {
            var jefeIds = await context.Colaboradores
                .Where(c => c.IsActive && c.JefeDirectoId != null)
                .Select(c => c.JefeDirectoId!.Value)
                .Distinct()
                .ToListAsync();

            foreach (var jefeId in jefeIds)
                EnsureAssignment(context, assignedSet, jefeId, rolJefe.Id);
        }
    }

    private static void EnsureAssignment(
        UrreaHubDbContext context,
        HashSet<(Guid ColaboradorId, Guid RolId)> assigned,
        Guid colaboradorId,
        Guid rolId)
    {
        if (!assigned.Add((colaboradorId, rolId))) return;

        context.ColaboradorRoles.Add(new ColaboradorRol
        {
            Id = Guid.NewGuid(),
            ColaboradorId = colaboradorId,
            RolId = rolId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        });
    }
}
