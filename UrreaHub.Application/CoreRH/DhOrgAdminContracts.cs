namespace UrreaHub.Application.CoreRH;

public record OrgCatalogDto(
    IReadOnlyList<OrgItemDto> Areas,
    IReadOnlyList<OrgDepartamentoDto> Departamentos,
    IReadOnlyList<OrgItemDto> Puestos,
    IReadOnlyList<OrgSedeDto> Sedes,
    IReadOnlyList<OrgItemDto> CentrosCosto);

public record OrgItemDto(Guid Id, string Codigo, string Nombre, string? Descripcion, bool IsActive);

public record OrgDepartamentoDto(Guid Id, string Codigo, string Nombre, string? Descripcion, Guid AreaId, string AreaNombre, Guid? SedeId, string? SedeNombre, bool IsActive);

public record OrgSedeDto(Guid Id, string Codigo, string Nombre, string? Ciudad, string? Pais, bool IsActive);

public record UpsertOrgItemDto(string Codigo, string Nombre, string? Descripcion, bool IsActive = true);

public record UpsertDepartamentoDto(string Codigo, string Nombre, string? Descripcion, Guid AreaId, Guid? SedeId, bool IsActive = true);

public record UpsertSedeDto(string Codigo, string Nombre, string? Ciudad, string? Pais, bool IsActive = true);

public record ManagerAssignmentDto(
    Guid ColaboradorId,
    string NumeroEmpleado,
    string NombreCompleto,
    string Puesto,
    string Departamento,
    Guid? JefeDirectoId,
    string? JefeDirectoNombre,
    bool IsManualOverride,
    string SyncStatus,
    string? ExternalSource);

public record AssignManagerDto(Guid? JefeDirectoId, string? Motivo, DateTime? FechaEfectiva);

public interface IDhOrgAdminService
{
    Task<OrgCatalogDto> GetCatalogAsync(CancellationToken cancellationToken = default);
    Task<OrgItemDto> UpsertAreaAsync(Guid? id, UpsertOrgItemDto dto, string performedBy, CancellationToken cancellationToken = default);
    Task<OrgDepartamentoDto> UpsertDepartamentoAsync(Guid? id, UpsertDepartamentoDto dto, string performedBy, CancellationToken cancellationToken = default);
    Task<OrgItemDto> UpsertPuestoAsync(Guid? id, UpsertOrgItemDto dto, string performedBy, CancellationToken cancellationToken = default);
    Task<OrgSedeDto> UpsertSedeAsync(Guid? id, UpsertSedeDto dto, string performedBy, CancellationToken cancellationToken = default);
    Task<OrgItemDto> UpsertCentroCostoAsync(Guid? id, UpsertOrgItemDto dto, string performedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManagerAssignmentDto>> ListManagerAssignmentsAsync(string? search, CancellationToken cancellationToken = default);
    Task AssignManagerAsync(Guid colaboradorId, AssignManagerDto dto, string performedBy, CancellationToken cancellationToken = default);
}
