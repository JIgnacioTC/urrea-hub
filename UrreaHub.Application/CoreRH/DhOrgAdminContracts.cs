namespace UrreaHub.Application.CoreRH;

public record OrgCatalogDto(
    IReadOnlyList<OrgItemDto> Areas,
    IReadOnlyList<OrgSubareaDto> Subareas,
    IReadOnlyList<OrgDepartamentoDto> Departamentos,
    IReadOnlyList<OrgPuestoDto> Puestos,
    IReadOnlyList<OrgSedeDto> Sedes,
    IReadOnlyList<OrgItemDto> CentrosCosto);

public record OrgItemDto(Guid Id, string Codigo, string Nombre, string? Descripcion, bool IsActive);

public record OrgSubareaDto(Guid Id, string Codigo, string Nombre, string? Descripcion, Guid AreaId, string AreaNombre, bool IsActive);

public record OrgDepartamentoDto(
    Guid Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    Guid SubareaId,
    string SubareaNombre,
    Guid AreaId,
    string AreaNombre,
    Guid? SedeId,
    string? SedeNombre,
    bool IsActive);

public record OrgSedeDto(Guid Id, string Codigo, string Nombre, string? Ciudad, string? Pais, bool IsActive);

public record OrgPuestoDto(
    Guid Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    int NivelJerarquico,
    int? GradoMercer,
    string? Impacto,
    string? Comunicacion,
    string? Innovacion,
    string? EducacionRequerida,
    int? ExperienciaAnios,
    decimal? PresupuestoAnual,
    int? PersonalCargoDirecto,
    int? PersonalCargoIndirecto,
    bool IsActive);

public record UpsertOrgItemDto(string Codigo, string Nombre, string? Descripcion, bool IsActive = true);

public record UpsertSubareaDto(string Codigo, string Nombre, string? Descripcion, Guid AreaId, bool IsActive = true);

public record UpsertDepartamentoDto(string Codigo, string Nombre, string? Descripcion, Guid SubareaId, Guid? SedeId, bool IsActive = true);

public record UpsertSedeDto(string Codigo, string Nombre, string? Ciudad, string? Pais, bool IsActive = true);

public record UpsertPuestoDto(
    string Codigo,
    string Nombre,
    string? Descripcion,
    int NivelJerarquico,
    int? GradoMercer,
    string? Impacto,
    string? Comunicacion,
    string? Innovacion,
    string? EducacionRequerida,
    int? ExperienciaAnios,
    decimal? PresupuestoAnual,
    int? PersonalCargoDirecto,
    int? PersonalCargoIndirecto,
    bool IsActive = true);

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
    Task<OrgSubareaDto> UpsertSubareaAsync(Guid? id, UpsertSubareaDto dto, string performedBy, CancellationToken cancellationToken = default);
    Task<OrgDepartamentoDto> UpsertDepartamentoAsync(Guid? id, UpsertDepartamentoDto dto, string performedBy, CancellationToken cancellationToken = default);
    Task<OrgPuestoDto> UpsertPuestoAsync(Guid? id, UpsertPuestoDto dto, string performedBy, CancellationToken cancellationToken = default);
    Task<OrgSedeDto> UpsertSedeAsync(Guid? id, UpsertSedeDto dto, string performedBy, CancellationToken cancellationToken = default);
    Task<OrgItemDto> UpsertCentroCostoAsync(Guid? id, UpsertOrgItemDto dto, string performedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManagerAssignmentDto>> ListManagerAssignmentsAsync(string? search, CancellationToken cancellationToken = default);
    Task AssignManagerAsync(Guid colaboradorId, AssignManagerDto dto, string performedBy, CancellationToken cancellationToken = default);
    
    // Deletion methods
    Task DeleteAreaAsync(Guid id, string performedBy, CancellationToken cancellationToken = default);
    Task DeleteSubareaAsync(Guid id, string performedBy, CancellationToken cancellationToken = default);
    Task DeleteDepartamentoAsync(Guid id, string performedBy, CancellationToken cancellationToken = default);
    Task DeletePuestoAsync(Guid id, string performedBy, CancellationToken cancellationToken = default);
    Task DeleteSedeAsync(Guid id, string performedBy, CancellationToken cancellationToken = default);
    Task DeleteCentroCostoAsync(Guid id, string performedBy, CancellationToken cancellationToken = default);
}
