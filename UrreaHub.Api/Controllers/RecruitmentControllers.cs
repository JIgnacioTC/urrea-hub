using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrreaHub.Api.Routing;
using UrreaHub.Application.Reclutamiento;
using UrreaHub.Domain.Common;

namespace UrreaHub.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Recruitment)]
[Route(ApiRoutes.RecruitmentLegacy)]
[Authorize(Policy = "RhAdmin")]
public class RecruitmentController : ControllerBase
{
    private readonly IRecruitmentService _recruitment;

    public RecruitmentController(IRecruitmentService recruitment) => _recruitment = recruitment;

    private string PerformedBy => User.Identity?.Name ?? "rh-admin";

    [HttpGet("dashboard")]
    public Task<RecruitmentDashboardDto> Dashboard(CancellationToken ct) => _recruitment.GetDashboardAsync(ct);

    [HttpGet("vacancies")]
    public Task<IReadOnlyList<VacanteDto>> Vacancies([FromQuery] EstadoVacante? estado, CancellationToken ct)
        => _recruitment.ListVacanciesAsync(estado, ct);

    [HttpGet("vacancies/{id:guid}")]
    public Task<VacanteDto?> Vacancy(Guid id, CancellationToken ct) => _recruitment.GetVacancyAsync(id, ct);

    [HttpGet("pipeline")]
    public Task<IReadOnlyList<PostulacionDto>> Pipeline([FromQuery] Guid? vacanteId, CancellationToken ct)
        => _recruitment.ListPipelineAsync(vacanteId, ct);

    [HttpGet("candidates/{id:guid}")]
    public Task<CandidatoDto?> Candidate(Guid id, CancellationToken ct) => _recruitment.GetCandidateAsync(id, ct);

    [HttpPost("candidates")]
    public async Task<ActionResult<PostulacionDto>> CreateCandidate([FromBody] CrearCandidatoDto dto, CancellationToken ct)
    {
        var result = await _recruitment.CreateCandidateAndApplyAsync(dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("applications/{id:guid}/stage")]
    public async Task<ActionResult<PostulacionDto>> ChangeStage(Guid id, [FromBody] CambiarEtapaDto dto, CancellationToken ct)
    {
        var result = await _recruitment.ChangeStageAsync(id, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("applications/{id:guid}/interviews")]
    public async Task<ActionResult<EntrevistaDto>> ScheduleInterview(Guid id, [FromBody] CrearEntrevistaDto dto, CancellationToken ct)
    {
        var result = await _recruitment.ScheduleInterviewAsync(id, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("applications/{id:guid}/offers")]
    public async Task<ActionResult<OfertaDto>> CreateOffer(Guid id, [FromBody] CrearOfertaDto dto, CancellationToken ct)
    {
        var result = await _recruitment.CreateOfferAsync(id, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpGet("offers")]
    public Task<IReadOnlyList<OfertaDto>> Offers(CancellationToken ct) => _recruitment.ListOffersAsync(ct);

    [HttpPost("offers/{id:guid}/accept-and-onboard")]
    public async Task<ActionResult<EnviarOnboardingResultDto>> AcceptAndOnboard(Guid id, CancellationToken ct)
    {
        var result = await _recruitment.AcceptOfferAndSendToOnboardingAsync(id, PerformedBy, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }
}
