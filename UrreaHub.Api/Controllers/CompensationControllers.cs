using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrreaHub.Api.Routing;
using UrreaHub.Application.Auth;
using UrreaHub.Application.Common;
using UrreaHub.Application.Compensaciones;
using UrreaHub.Domain.Compensaciones;

namespace UrreaHub.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Compensation)]
[Route(ApiRoutes.CompensationLegacy)]
[Authorize]
public class CompensationController : ControllerBase
{
    private readonly ICompensationService _compensation;
    private readonly IBenefitsAdminService _benefits;
    private readonly ICurrentUser _currentUser;

    public CompensationController(ICompensationService compensation, IBenefitsAdminService benefits, ICurrentUser currentUser)
    {
        _compensation = compensation;
        _benefits = benefits;
        _currentUser = currentUser;
    }

    [HttpGet("my-package")]
    [Authorize(Policy = "CompensationRead")]
    public Task<MiCompensacionDto> MyPackage(CancellationToken ct)
        => _compensation.GetMyPackageAsync(_currentUser.ColaboradorId, ct);

    [HttpPost("benefit-requests")]
    [Authorize(Policy = "CompensationRequestCreate")]
    public async Task<ActionResult<SolicitudBeneficioAdminDto>> CreateBenefitRequest(
        [FromBody] CrearSolicitudBeneficioDto dto, CancellationToken ct)
    {
        var result = await _benefits.CreateBenefitRequestAsync(
            _currentUser.ColaboradorId, dto.BeneficioId, dto.Monto, dto.Justificacion, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }
}

public record CrearSolicitudBeneficioDto(Guid BeneficioId, decimal? Monto, string? Justificacion);

[ApiController]
[Route(ApiRoutes.Compensation + "/admin")]
[Route(ApiRoutes.CompensationLegacy + "/admin")]
[Authorize(Policy = "RhAdmin")]
public class CompensationAdminController : ControllerBase
{
    private readonly ICompensationService _compensation;
    private readonly ICurrentUser _currentUser;

    public CompensationAdminController(ICompensationService compensation, ICurrentUser currentUser)
    {
        _compensation = compensation;
        _currentUser = currentUser;
    }

    private string PerformedBy => User.Identity?.Name ?? "rh-admin";

    [HttpGet("dashboard")]
    public Task<CompensacionDashboardDto> Dashboard(CancellationToken ct)
        => _compensation.GetDashboardAsync(ct);

    [HttpGet("colaboradores")]
    public Task<IReadOnlyList<CompensacionColaboradorDto>> Colaboradores(CancellationToken ct)
        => _compensation.ListColaboradoresAsync(ct);

    [HttpGet("colaboradores/{id:guid}")]
    public Task<CompensacionColaboradorDto?> Colaborador(Guid id, CancellationToken ct)
        => _compensation.GetColaboradorAsync(id, ct);

    [HttpGet("tabuladores")]
    public Task<IReadOnlyList<TabuladorDto>> Tabuladores(CancellationToken ct)
        => _compensation.ListTabuladoresAsync(ct);

    [HttpGet("adjustments")]
    public Task<IReadOnlyList<SolicitudAjusteDto>> Adjustments([FromQuery] EstadoAjusteCompensacion? estado, CancellationToken ct)
        => _compensation.ListAdjustmentRequestsAsync(estado, ct);

    [HttpPost("adjustments")]
    public async Task<ActionResult<SolicitudAjusteDto>> CreateAdjustment([FromBody] CrearSolicitudAjusteDto dto, CancellationToken ct)
    {
        var result = await _compensation.CreateAdjustmentRequestAsync(_currentUser.ColaboradorId, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("adjustments/{id:guid}/submit")]
    public async Task<ActionResult<SolicitudAjusteDto>> SubmitAdjustment(Guid id, CancellationToken ct)
    {
        var result = await _compensation.SubmitAdjustmentAsync(_currentUser.ColaboradorId, id, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("adjustments/{id:guid}/approve")]
    public async Task<ActionResult<SolicitudAjusteDto>> ApproveAdjustment(Guid id, [FromBody] DecisionAjusteDto dto, CancellationToken ct)
    {
        var isFinanzas = User.HasClaim(AppPermissions.ClaimType, AppPermissions.HcmReadSalary);
        var result = await _compensation.ApproveAdjustmentAsync(_currentUser.ColaboradorId, id, dto, isFinanzas, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("adjustments/{id:guid}/reject")]
    public async Task<ActionResult<SolicitudAjusteDto>> RejectAdjustment(Guid id, [FromBody] DecisionAjusteDto dto, CancellationToken ct)
    {
        var result = await _compensation.RejectAdjustmentAsync(_currentUser.ColaboradorId, id, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("adjustments/{id:guid}/apply")]
    public async Task<ActionResult<SolicitudAjusteDto>> ApplyAdjustment(Guid id, CancellationToken ct)
    {
        var result = await _compensation.ApplyAdjustmentAsync(id, PerformedBy, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }
}

[ApiController]
[Route(ApiRoutes.BenefitsAdmin)]
[Route(ApiRoutes.BenefitsAdminLegacy)]
[Authorize(Policy = "RhAdmin")]
public class BenefitsAdminController : ControllerBase
{
    private readonly IBenefitsAdminService _benefits;
    private readonly ICurrentUser _currentUser;

    public BenefitsAdminController(IBenefitsAdminService benefits, ICurrentUser currentUser)
    {
        _benefits = benefits;
        _currentUser = currentUser;
    }

    [HttpGet("benefits")]
    public Task<IReadOnlyList<BeneficioDisponibleDto>> Benefits(CancellationToken ct)
        => _benefits.ListBenefitsAsync(ct);

    [HttpGet("requests")]
    public Task<IReadOnlyList<SolicitudBeneficioAdminDto>> Requests(
        [FromQuery] Domain.Common.EstadoSolicitud? estado, CancellationToken ct)
        => _benefits.ListBenefitRequestsAsync(estado, ct);

    [HttpPost("requests/{id:guid}/approve")]
    public async Task<ActionResult<SolicitudBeneficioAdminDto>> Approve(Guid id, [FromBody] DecisionAjusteDto dto, CancellationToken ct)
    {
        var result = await _benefits.ApproveBenefitRequestAsync(_currentUser.ColaboradorId, id, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("requests/{id:guid}/reject")]
    public async Task<ActionResult<SolicitudBeneficioAdminDto>> Reject(Guid id, [FromBody] DecisionAjusteDto dto, CancellationToken ct)
    {
        var result = await _benefits.RejectBenefitRequestAsync(_currentUser.ColaboradorId, id, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }
}
