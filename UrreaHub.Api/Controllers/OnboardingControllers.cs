using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrreaHub.Api.Routing;
using UrreaHub.Application.Common;
using UrreaHub.Application.Onboarding;
using UrreaHub.Domain.Common;

namespace UrreaHub.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Onboarding)]
[Route(ApiRoutes.OnboardingLegacy)]
[Authorize]
public class OnboardingController : ControllerBase
{
    private readonly IOnboardingService _onboarding;
    private readonly ICurrentUser _currentUser;

    public OnboardingController(IOnboardingService onboarding, ICurrentUser currentUser)
    {
        _onboarding = onboarding;
        _currentUser = currentUser;
    }

    [HttpGet("my-summary")]
    public Task<OnboardingSummaryDto> MySummary(CancellationToken ct)
        => _onboarding.GetMySummaryAsync(_currentUser.ColaboradorId, ct);

    [HttpGet("my-plan")]
    public Task<PlanOnboardingDto?> MyPlan(CancellationToken ct)
        => _onboarding.GetMyPlanAsync(_currentUser.ColaboradorId, ct);

    [HttpPost("tasks/{taskId:guid}/complete")]
    public async Task<ActionResult<TareaOnboardingDto>> CompleteTask(Guid taskId, [FromBody] CompletarTareaDto dto, CancellationToken ct)
    {
        var result = await _onboarding.CompleteTaskAsync(_currentUser.ColaboradorId, taskId, dto, _currentUser.IsRhAdmin, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpGet("team/plans")]
    [Authorize(Policy = "ManagerApproval")]
    public Task<IReadOnlyList<PlanOnboardingResumenDto>> TeamPlans(CancellationToken ct)
        => _onboarding.GetTeamPlansAsync(_currentUser.ColaboradorId, ct);

    [HttpGet("team/tasks/pending")]
    [Authorize(Policy = "ManagerApproval")]
    public Task<IReadOnlyList<TareaOnboardingDto>> TeamPendingTasks(CancellationToken ct)
        => _onboarding.GetTeamPendingTasksAsync(_currentUser.ColaboradorId, ct);
}

[ApiController]
[Route(ApiRoutes.Onboarding + "/admin")]
[Route(ApiRoutes.OnboardingLegacy + "/admin")]
[Authorize(Policy = "RhAdmin")]
public class OnboardingAdminController : ControllerBase
{
    private readonly IOnboardingAdminService _admin;

    public OnboardingAdminController(IOnboardingAdminService admin) => _admin = admin;

    private string PerformedBy => User.Identity?.Name ?? "rh-admin";

    [HttpGet("dashboard")]
    public Task<OnboardingDashboardDto> Dashboard(CancellationToken ct) => _admin.GetDashboardAsync(ct);

    [HttpGet("plans")]
    public Task<IReadOnlyList<PlanOnboardingResumenDto>> Plans([FromQuery] EstadoSolicitud? estado, CancellationToken ct)
        => _admin.ListPlansAsync(estado, ct);

    [HttpGet("plans/{planId:guid}")]
    public Task<PlanOnboardingDto?> Plan(Guid planId, CancellationToken ct) => _admin.GetPlanAsync(planId, ct);

    [HttpGet("templates")]
    public Task<IReadOnlyList<PlantillaOnboardingDto>> Templates(CancellationToken ct) => _admin.ListTemplatesAsync(ct);

    [HttpPost("plans")]
    public async Task<ActionResult<PlanOnboardingDto>> CreatePlan([FromBody] CrearPlanOnboardingDto dto, CancellationToken ct)
    {
        var result = await _admin.CreatePlanAsync(dto, PerformedBy, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }
}
