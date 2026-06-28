using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrreaHub.Api.Routing;
using UrreaHub.Application.CoreRH;

namespace UrreaHub.Api.Controllers;

[ApiController]
[Route(ApiRoutes.DhAdminOrg)]
[Route(ApiRoutes.DhAdminOrgLegacy)]
[Authorize(Policy = "RhAdmin")]
public class DhOrgAdminController : ControllerBase
{
    private readonly IDhOrgAdminService _org;

    public DhOrgAdminController(IDhOrgAdminService org) => _org = org;

    private string PerformedBy => User.FindFirstValue(ClaimTypes.Name) ?? "rh-admin";

    [HttpGet("catalogo")]
    public Task<OrgCatalogDto> Catalog(CancellationToken ct) => _org.GetCatalogAsync(ct);

    [HttpPost("areas")]
    public Task<OrgItemDto> CreateArea([FromBody] UpsertOrgItemDto dto, CancellationToken ct) =>
        _org.UpsertAreaAsync(null, dto, PerformedBy, ct);

    [HttpPut("areas/{id:guid}")]
    public Task<OrgItemDto> UpdateArea(Guid id, [FromBody] UpsertOrgItemDto dto, CancellationToken ct) =>
        _org.UpsertAreaAsync(id, dto, PerformedBy, ct);

    [HttpPost("departamentos")]
    public Task<OrgDepartamentoDto> CreateDepartamento([FromBody] UpsertDepartamentoDto dto, CancellationToken ct) =>
        _org.UpsertDepartamentoAsync(null, dto, PerformedBy, ct);

    [HttpPut("departamentos/{id:guid}")]
    public Task<OrgDepartamentoDto> UpdateDepartamento(Guid id, [FromBody] UpsertDepartamentoDto dto, CancellationToken ct) =>
        _org.UpsertDepartamentoAsync(id, dto, PerformedBy, ct);

    [HttpPost("puestos")]
    public Task<OrgItemDto> CreatePuesto([FromBody] UpsertOrgItemDto dto, CancellationToken ct) =>
        _org.UpsertPuestoAsync(null, dto, PerformedBy, ct);

    [HttpPut("puestos/{id:guid}")]
    public Task<OrgItemDto> UpdatePuesto(Guid id, [FromBody] UpsertOrgItemDto dto, CancellationToken ct) =>
        _org.UpsertPuestoAsync(id, dto, PerformedBy, ct);

    [HttpPost("sedes")]
    public Task<OrgSedeDto> CreateSede([FromBody] UpsertSedeDto dto, CancellationToken ct) =>
        _org.UpsertSedeAsync(null, dto, PerformedBy, ct);

    [HttpPut("sedes/{id:guid}")]
    public Task<OrgSedeDto> UpdateSede(Guid id, [FromBody] UpsertSedeDto dto, CancellationToken ct) =>
        _org.UpsertSedeAsync(id, dto, PerformedBy, ct);

    [HttpPost("centros-costo")]
    public Task<OrgItemDto> CreateCentroCosto([FromBody] UpsertOrgItemDto dto, CancellationToken ct) =>
        _org.UpsertCentroCostoAsync(null, dto, PerformedBy, ct);

    [HttpPut("centros-costo/{id:guid}")]
    public Task<OrgItemDto> UpdateCentroCosto(Guid id, [FromBody] UpsertOrgItemDto dto, CancellationToken ct) =>
        _org.UpsertCentroCostoAsync(id, dto, PerformedBy, ct);

    [HttpGet("jefes")]
    public Task<IReadOnlyList<ManagerAssignmentDto>> ListManagers([FromQuery] string? q, CancellationToken ct) =>
        _org.ListManagerAssignmentsAsync(q, ct);

    [HttpPut("jefes/{colaboradorId:guid}")]
    public async Task<IActionResult> AssignManager(Guid colaboradorId, [FromBody] AssignManagerDto dto, CancellationToken ct)
    {
        await _org.AssignManagerAsync(colaboradorId, dto, PerformedBy, ct);
        return NoContent();
    }
}
