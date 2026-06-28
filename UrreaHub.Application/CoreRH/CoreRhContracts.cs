using UrreaHub.Application.Common;

namespace UrreaHub.Application.CoreRH;

public record ColaboradorPerfilDto(
    Guid Id,
    string NumeroEmpleado,
    string Nombre,
    string ApellidoPaterno,
    string? ApellidoMaterno,
    string Email,
    string? Rfc,
    string? Telefono,
    DateTime FechaIngreso,
    DateTime? FechaBaja,
    DateTime? NominaSyncAt,
    string Puesto,
    string Departamento,
    string? Sede,
    string? JefeDirecto,
    IReadOnlyList<ColaboradorResumenDto> Subordinados);

public record ColaboradorResumenDto(
    Guid Id,
    string NumeroEmpleado,
    string NombreCompleto,
    string Puesto,
    string? JefeDirectoId);

public record ColaboradorRhDto(
    Guid Id,
    string NumeroEmpleado,
    string Nombre,
    string ApellidoPaterno,
    string? ApellidoMaterno,
    string Email,
    string? Rfc,
    string? Telefono,
    DateTime FechaIngreso,
    DateTime? FechaBaja,
    Guid PuestoId,
    Guid DepartamentoId,
    Guid? SedeId,
    Guid? CentroCostoId,
    Guid RelacionLaboralId,
    Guid? JefeDirectoId,
    bool IsActive);

public record ColaboradorRhUpdateDto(
    string Nombre,
    string ApellidoPaterno,
    string? ApellidoMaterno,
    string? Telefono,
    Guid PuestoId,
    Guid DepartamentoId,
    Guid? SedeId,
    Guid? CentroCostoId,
    Guid RelacionLaboralId,
    Guid? JefeDirectoId);

public record OrganigramaNodoDto(
    Guid Id,
    string NumeroEmpleado,
    string NombreCompleto,
    string Puesto,
    string Departamento,
    IReadOnlyList<OrganigramaNodoDto> Subordinados);

public interface IColaboradorService
{
    Task<ColaboradorPerfilDto?> GetPerfilAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
    Task<PagedResult<ColaboradorResumenDto>> ListAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<ColaboradorRhDto?> UpdateRhFieldsAsync(Guid id, ColaboradorRhUpdateDto dto, CancellationToken cancellationToken = default);
}

public interface IOrganigramaService
{
    Task<OrganigramaNodoDto?> GetArbolAsync(Guid? raizId, Guid? sedeId, Guid? departamentoId, CancellationToken cancellationToken = default);
}
