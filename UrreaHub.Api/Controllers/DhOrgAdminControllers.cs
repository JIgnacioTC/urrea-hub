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

    [HttpPost("subareas")]
    public Task<OrgSubareaDto> CreateSubarea([FromBody] UpsertSubareaDto dto, CancellationToken ct) =>
        _org.UpsertSubareaAsync(null, dto, PerformedBy, ct);

    [HttpPut("subareas/{id:guid}")]
    public Task<OrgSubareaDto> UpdateSubarea(Guid id, [FromBody] UpsertSubareaDto dto, CancellationToken ct) =>
        _org.UpsertSubareaAsync(id, dto, PerformedBy, ct);

    [HttpPost("departamentos")]
    public Task<OrgDepartamentoDto> CreateDepartamento([FromBody] UpsertDepartamentoDto dto, CancellationToken ct) =>
        _org.UpsertDepartamentoAsync(null, dto, PerformedBy, ct);

    [HttpPut("departamentos/{id:guid}")]
    public Task<OrgDepartamentoDto> UpdateDepartamento(Guid id, [FromBody] UpsertDepartamentoDto dto, CancellationToken ct) =>
        _org.UpsertDepartamentoAsync(id, dto, PerformedBy, ct);

    [HttpPost("puestos")]
    public Task<OrgPuestoDto> CreatePuesto([FromBody] UpsertPuestoDto dto, CancellationToken ct) =>
        _org.UpsertPuestoAsync(null, dto, PerformedBy, ct);

    [HttpPut("puestos/{id:guid}")]
    public Task<OrgPuestoDto> UpdatePuesto(Guid id, [FromBody] UpsertPuestoDto dto, CancellationToken ct) =>
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

    [HttpDelete("areas/{id:guid}")]
    public async Task<IActionResult> DeleteArea(Guid id, CancellationToken ct)
    {
        await _org.DeleteAreaAsync(id, PerformedBy, ct);
        return NoContent();
    }

    [HttpDelete("subareas/{id:guid}")]
    public async Task<IActionResult> DeleteSubarea(Guid id, CancellationToken ct)
    {
        await _org.DeleteSubareaAsync(id, PerformedBy, ct);
        return NoContent();
    }

    [HttpDelete("departamentos/{id:guid}")]
    public async Task<IActionResult> DeleteDepartamento(Guid id, CancellationToken ct)
    {
        await _org.DeleteDepartamentoAsync(id, PerformedBy, ct);
        return NoContent();
    }

    [HttpDelete("puestos/{id:guid}")]
    public async Task<IActionResult> DeletePuesto(Guid id, CancellationToken ct)
    {
        await _org.DeletePuestoAsync(id, PerformedBy, ct);
        return NoContent();
    }

    [HttpDelete("sedes/{id:guid}")]
    public async Task<IActionResult> DeleteSede(Guid id, CancellationToken ct)
    {
        await _org.DeleteSedeAsync(id, PerformedBy, ct);
        return NoContent();
    }

    [HttpDelete("centros-costo/{id:guid}")]
    public async Task<IActionResult> DeleteCentroCosto(Guid id, CancellationToken ct)
    {
        await _org.DeleteCentroCostoAsync(id, PerformedBy, ct);
        return NoContent();
    }
}
