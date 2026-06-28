namespace UrreaHub.Application.Seguridad;

public record RolDto(Guid Id, string Codigo, string Nombre, string? Descripcion, bool IsActive, int PermisoCount, int ColaboradorCount);

public record UpsertRolDto(string Codigo, string Nombre, string? Descripcion, bool IsActive = true);

public record PermisoDto(Guid Id, string Codigo, string Modulo, string Nombre, string? Descripcion, bool IsActive);

public record RolePermissionMatrixDto(
    IReadOnlyList<RolDto> Roles,
    IReadOnlyList<PermisoDto> Permisos,
    IReadOnlyList<RolePermissionRowDto> Rows);

public record RolePermissionRowDto(Guid PermisoId, string Codigo, string Modulo, string Nombre, IReadOnlyDictionary<string, bool> ByRoleCodigo);

public record UpdateRolePermissionsDto(IReadOnlyList<Guid> PermisoIds);

public record ColaboradorAccessSummaryDto(
    Guid Id,
    string NumeroEmpleado,
    string NombreCompleto,
    string Puesto,
    string Departamento,
    IReadOnlyList<string> Roles);

public record ColaboradorAccessDetailDto(
    Guid Id,
    string NumeroEmpleado,
    string NombreCompleto,
    string Puesto,
    string Departamento,
    string? Area,
    IReadOnlyList<RolDto> Roles,
    IReadOnlyList<string> PermisosEfectivos);

public interface ISecurityAdminService
{
    Task<IReadOnlyList<RolDto>> ListRolesAsync(CancellationToken cancellationToken = default);
    Task<RolDto> UpsertRoleAsync(Guid? id, UpsertRolDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PermisoDto>> ListPermissionsAsync(string? modulo, CancellationToken cancellationToken = default);
    Task<RolePermissionMatrixDto> GetRolePermissionMatrixAsync(CancellationToken cancellationToken = default);
    Task UpdateRolePermissionsAsync(Guid rolId, UpdateRolePermissionsDto dto, string performedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ColaboradorAccessSummaryDto>> SearchColaboradoresAsync(string? search, CancellationToken cancellationToken = default);
    Task<ColaboradorAccessDetailDto?> GetColaboradorAccessAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
    Task AssignRoleAsync(Guid colaboradorId, Guid rolId, string performedBy, CancellationToken cancellationToken = default);
    Task RemoveRoleAsync(Guid colaboradorId, Guid rolId, string performedBy, CancellationToken cancellationToken = default);
}
