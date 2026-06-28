using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrreaHub.Api.Routing;
using UrreaHub.Application.Common;
using UrreaHub.Application.Requisiciones;
using UrreaHub.Domain.Common;

namespace UrreaHub.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Requisitions)]
[Route(ApiRoutes.RequisitionsLegacy)]
[Authorize]
public class RequisitionController : ControllerBase
{
    private readonly IRequisitionService _requisitions;
    private readonly ICurrentUser _currentUser;

    public RequisitionController(IRequisitionService requisitions, ICurrentUser currentUser)
    {
        _requisitions = requisitions;
        _currentUser = currentUser;
    }

    [HttpGet]
    public Task<IReadOnlyList<RequisicionResumenDto>> List(CancellationToken ct)
        => _requisitions.ListMyAsync(_currentUser.ColaboradorId, ct);

    [HttpGet("{id:guid}")]
    public Task<RequisicionDto?> Get(Guid id, CancellationToken ct)
        => _requisitions.GetAsync(id, ct);

    [HttpPost]
    public async Task<ActionResult<RequisicionDto>> Create([FromBody] CrearRequisicionDto dto, CancellationToken ct)
    {
        var result = await _requisitions.CreateAsync(_currentUser.ColaboradorId, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RequisicionDto>> Update(Guid id, [FromBody] ActualizarRequisicionDto dto, CancellationToken ct)
    {
        var result = await _requisitions.UpdateAsync(_currentUser.ColaboradorId, id, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult<RequisicionDto>> Submit(Guid id, CancellationToken ct)
    {
        var result = await _requisitions.SubmitAsync(_currentUser.ColaboradorId, id, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpGet("pending-approvals")]
    [Authorize(Policy = "ManagerApproval")]
    public Task<IReadOnlyList<RequisicionResumenDto>> PendingApprovals(CancellationToken ct)
        => _requisitions.ListPendingApprovalsAsync(_currentUser.ColaboradorId, ct);

    [HttpPost("{id:guid}/approve")]
    [Authorize(Policy = "ManagerApproval")]
    public async Task<ActionResult<RequisicionDto>> Approve(Guid id, [FromBody] DecisionRequisicionDto dto, CancellationToken ct)
    {
        var result = await _requisitions.ApproveAsync(_currentUser.ColaboradorId, id, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/reject")]
    [Authorize(Policy = "ManagerApproval")]
    public async Task<ActionResult<RequisicionDto>> Reject(Guid id, [FromBody] DecisionRequisicionDto dto, CancellationToken ct)
    {
        var result = await _requisitions.RejectAsync(_currentUser.ColaboradorId, id, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("{id:guid}/request-changes")]
    [Authorize(Policy = "ManagerApproval")]
    public async Task<ActionResult<RequisicionDto>> RequestChanges(Guid id, [FromBody] DecisionRequisicionDto dto, CancellationToken ct)
    {
        var result = await _requisitions.RequestChangesAsync(_currentUser.ColaboradorId, id, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }
}

[ApiController]
[Route(ApiRoutes.Requisitions + "/admin")]
[Route(ApiRoutes.RequisitionsLegacy + "/admin")]
[Authorize(Policy = "RhAdmin")]
public class RequisitionAdminController : ControllerBase
{
    private readonly IRequisitionAdminService _admin;

    public RequisitionAdminController(IRequisitionAdminService admin) => _admin = admin;

    private string PerformedBy => User.Identity?.Name ?? "rh-admin";

    [HttpGet("dashboard")]
    public Task<RequisicionDashboardDto> Dashboard(CancellationToken ct) => _admin.GetDashboardAsync(ct);

    [HttpGet("all")]
    public Task<IReadOnlyList<RequisicionResumenDto>> All([FromQuery] EstadoSolicitud? estado, [FromQuery] Guid? departamentoId, CancellationToken ct)
        => _admin.ListAllAsync(estado, departamentoId, ct);

    [HttpPost("{id:guid}/convert-to-vacancy")]
    public async Task<ActionResult<object>> ConvertToVacancy(Guid id, CancellationToken ct)
    {
        var result = await _admin.ConvertToVacancyAsync(id, PerformedBy, ct);
        return result.Success ? Ok(new { vacanteId = result.Data }) : BadRequest(new { error = result.Error });
    }
}
