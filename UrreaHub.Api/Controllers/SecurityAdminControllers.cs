using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrreaHub.Api.Routing;
using UrreaHub.Application.Seguridad;

namespace UrreaHub.Api.Controllers;

[ApiController]
[Route(ApiRoutes.SecurityAdmin)]
[Route(ApiRoutes.SecurityAdminLegacy)]
[Authorize(Policy = "TiAdmin")]
public class SecurityAdminController : ControllerBase
{
    private readonly ISecurityAdminService _security;

    public SecurityAdminController(ISecurityAdminService security) => _security = security;

    private string PerformedBy => User.FindFirstValue(ClaimTypes.Name) ?? "ti-admin";

    [HttpGet("roles")]
    public Task<IReadOnlyList<RolDto>> ListRoles(CancellationToken ct) => _security.ListRolesAsync(ct);

    [HttpPost("roles")]
    public Task<RolDto> CreateRole([FromBody] UpsertRolDto dto, CancellationToken ct) =>
        _security.UpsertRoleAsync(null, dto, ct);

    [HttpPut("roles/{id:guid}")]
    public Task<RolDto> UpdateRole(Guid id, [FromBody] UpsertRolDto dto, CancellationToken ct) =>
        _security.UpsertRoleAsync(id, dto, ct);

    [HttpGet("permisos")]
    public Task<IReadOnlyList<PermisoDto>> ListPermissions([FromQuery] string? modulo, CancellationToken ct) =>
        _security.ListPermissionsAsync(modulo, ct);

    [HttpGet("matriz")]
    public Task<RolePermissionMatrixDto> Matrix(CancellationToken ct) =>
        _security.GetRolePermissionMatrixAsync(ct);

    [HttpPut("roles/{rolId:guid}/permisos")]
    public async Task<IActionResult> UpdateRolePermissions(Guid rolId, [FromBody] UpdateRolePermissionsDto dto, CancellationToken ct)
    {
        await _security.UpdateRolePermissionsAsync(rolId, dto, PerformedBy, ct);
        return NoContent();
    }

    [HttpGet("colaboradores")]
    public Task<IReadOnlyList<ColaboradorAccessSummaryDto>> SearchColaboradores([FromQuery] string? q, CancellationToken ct) =>
        _security.SearchColaboradoresAsync(q, ct);

    [HttpGet("colaboradores/{id:guid}/acceso")]
    public async Task<ActionResult<ColaboradorAccessDetailDto>> GetColaboradorAccess(Guid id, CancellationToken ct)
    {
        var detail = await _security.GetColaboradorAccessAsync(id, ct);
        return detail is null ? NotFound() : Ok(detail);
    }

    [HttpPost("colaboradores/{colaboradorId:guid}/roles/{rolId:guid}")]
    public async Task<IActionResult> AssignRole(Guid colaboradorId, Guid rolId, CancellationToken ct)
    {
        await _security.AssignRoleAsync(colaboradorId, rolId, PerformedBy, ct);
        return NoContent();
    }

    [HttpDelete("colaboradores/{colaboradorId:guid}/roles/{rolId:guid}")]
    public async Task<IActionResult> RemoveRole(Guid colaboradorId, Guid rolId, CancellationToken ct)
    {
        await _security.RemoveRoleAsync(colaboradorId, rolId, PerformedBy, ct);
        return NoContent();
    }
}
