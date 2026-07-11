using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrreaHub.Api.Routing;
using UrreaHub.Application.Vacaciones;
using UrreaHub.Domain.Common;

namespace UrreaHub.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Absence + "/admin")]
[Route(ApiRoutes.AbsenceLegacy + "/admin")]
[Authorize(Policy = "RhAdmin")]
public class AbsenceAdminController : ControllerBase
{
    private readonly IAbsenceAdminService _admin;

    public AbsenceAdminController(IAbsenceAdminService admin) => _admin = admin;

    private string PerformedBy => User.FindFirstValue(ClaimTypes.Name) ?? "rh-admin";

    [HttpGet("policies")]
    public Task<IReadOnlyList<PoliticaVacacionesDto>> Policies(CancellationToken ct) => _admin.ListPoliciesAsync(ct);

    [HttpPost("policies")]
    public Task<PoliticaVacacionesDto> CreatePolicy([FromBody] UpsertPoliticaDto dto, CancellationToken ct) =>
        _admin.UpsertPolicyAsync(null, dto, ct);

    [HttpPut("policies/{id:guid}")]
    public Task<PoliticaVacacionesDto> UpdatePolicy(Guid id, [FromBody] UpsertPoliticaDto dto, CancellationToken ct) =>
        _admin.UpsertPolicyAsync(id, dto, ct);

    [HttpGet("types")]
    public Task<IReadOnlyList<TipoAusenciaDto>> Types(CancellationToken ct) => _admin.ListTypesAsync(ct);

    [HttpPost("types")]
    public Task<TipoAusenciaDto> CreateType([FromBody] UpsertTipoAusenciaDto dto, CancellationToken ct) =>
        _admin.UpsertTypeAsync(null, dto, ct);

    [HttpPut("types/{id:guid}")]
    public Task<TipoAusenciaDto> UpdateType(Guid id, [FromBody] UpsertTipoAusenciaDto dto, CancellationToken ct) =>
        _admin.UpsertTypeAsync(id, dto, ct);

    [HttpDelete("types/{id:guid}")]
    public async Task<IActionResult> DeleteType(Guid id, CancellationToken ct)
    {
        await _admin.DeleteTypeAsync(id, ct);
        return NoContent();
    }

    [HttpGet("calendars")]
    public Task<IReadOnlyList<CalendarioLaboralDto>> Calendars([FromQuery] int? anio, CancellationToken ct) =>
        _admin.ListCalendarsAsync(anio, ct);

    [HttpPost("calendars")]
    public Task<CalendarioLaboralDto> CreateCalendar([FromBody] UpsertCalendarioDto dto, CancellationToken ct) =>
        _admin.UpsertCalendarAsync(null, dto, ct);

    [HttpPut("calendars/{id:guid}")]
    public Task<CalendarioLaboralDto> UpdateCalendar(Guid id, [FromBody] UpsertCalendarioDto dto, CancellationToken ct) =>
        _admin.UpsertCalendarAsync(id, dto, ct);

    [HttpPost("calendars/{id:guid}/holidays")]
    public Task<DiaInhabilDto> AddHoliday(Guid id, [FromBody] UpsertDiaInhabilDto dto, CancellationToken ct) =>
        _admin.AddHolidayAsync(id, dto, ct);

    [HttpGet("balances")]
    public Task<IReadOnlyList<AdminBalanceDto>> Balances([FromQuery] int? anio, [FromQuery] string? q, CancellationToken ct) =>
        _admin.ListBalancesAsync(anio, q, ct);

    [HttpPost("balances/{colaboradorId:guid}/adjust")]
    public async Task<IActionResult> AdjustBalance(Guid colaboradorId, [FromBody] AdjustBalanceDto dto, [FromQuery] int? anio, CancellationToken ct)
    {
        try
        {
            await _admin.AdjustBalanceAsync(colaboradorId, dto, anio, PerformedBy, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("balances/recalculate")]
    public async Task<IActionResult> Recalculate([FromBody] RecalculateBalanceDto dto, CancellationToken ct)
    {
        await _admin.RecalculateBalancesAsync(dto, PerformedBy, ct);
        return NoContent();
    }

    [HttpGet("requests")]
    public Task<IReadOnlyList<AdminSolicitudDto>> Requests(
        [FromQuery] EstadoSolicitud? estado,
        [FromQuery] Guid? sedeId,
        [FromQuery] Guid? tipoId,
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta,
        [FromQuery] string? q,
        CancellationToken ct) =>
        _admin.ListRequestsAsync(estado, sedeId, tipoId, desde, hasta, q, ct);

    [HttpPost("requests/{id:guid}/cancel")]
    public async Task<IActionResult> CancelAdministratively(Guid id, [FromBody] AprobacionRequestDto dto, CancellationToken ct)
    {
        try
        {
            await _admin.CancelAdministrativelyAsync(id, dto.Comentario ?? "Cancelación administrativa", PerformedBy, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("payroll-incidents")]
    public Task<IReadOnlyList<IncidenciaNominaDto>> PayrollIncidents(CancellationToken ct) =>
        _admin.ListPayrollIncidentsAsync(ct);
}
