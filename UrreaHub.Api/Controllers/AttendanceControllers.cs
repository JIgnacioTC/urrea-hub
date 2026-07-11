using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrreaHub.Api.Routing;
using UrreaHub.Application.Asistencia;
using UrreaHub.Application.Common;
using UrreaHub.Domain.Asistencia;

namespace UrreaHub.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Attendance)]
[Route(ApiRoutes.AttendanceLegacy)]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendance;
    private readonly IAttendanceAdminService _admin;
    private readonly ICurrentUser _currentUser;

    public AttendanceController(IAttendanceService attendance, IAttendanceAdminService admin, ICurrentUser currentUser)
    {
        _attendance = attendance;
        _admin = admin;
        _currentUser = currentUser;
    }

    [HttpGet("my-summary")]
    public Task<AttendanceSummaryDto> MySummary(CancellationToken ct)
        => _attendance.GetMySummaryAsync(_currentUser.ColaboradorId, ct);

    [HttpGet("my-records")]
    public Task<IReadOnlyList<RegistroAsistenciaDto>> MyRecords([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken ct)
        => _attendance.GetMyRecordsAsync(_currentUser.ColaboradorId, desde, hasta, ct);

    [HttpPost("check-in")]
    public async Task<ActionResult<RegistroAsistenciaDto>> CheckIn([FromBody] CheckInOutDto dto, CancellationToken ct)
    {
        var result = await _attendance.CheckInAsync(_currentUser.ColaboradorId, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("check-out")]
    public async Task<ActionResult<RegistroAsistenciaDto>> CheckOut([FromBody] CheckInOutDto dto, CancellationToken ct)
    {
        var result = await _attendance.CheckOutAsync(_currentUser.ColaboradorId, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("checador/verificar")]
    [AllowAnonymous]
    public Task<ChecadorResultDto> VerifyAndRegisterChecador([FromBody] ChecadorVerifyDto dto, CancellationToken ct)
        => _attendance.VerifyAndRegisterChecadorAsync(dto, ct);

    [HttpPost("corrections")]
    public async Task<ActionResult<CorreccionDto>> CreateCorrection([FromBody] CrearCorreccionDto dto, CancellationToken ct)
    {
        var result = await _attendance.CreateCorrectionAsync(_currentUser.ColaboradorId, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpGet("corrections/my")]
    public Task<IReadOnlyList<CorreccionDto>> MyCorrections(CancellationToken ct)
        => _attendance.GetMyCorrectionsAsync(_currentUser.ColaboradorId, ct);

    [HttpGet("team/summary")]
    [Authorize(Policy = "ManagerApproval")]
    public Task<TeamAttendanceSummaryDto> TeamSummary([FromQuery] DateTime? fecha, CancellationToken ct)
        => _attendance.GetTeamSummaryAsync(_currentUser.ColaboradorId, fecha ?? DateTime.UtcNow.Date, ct);

    [HttpGet("team/incidents")]
    [Authorize(Policy = "ManagerApproval")]
    public Task<IReadOnlyList<IncidenciaDto>> TeamIncidents([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken ct)
        => _attendance.GetTeamIncidentsAsync(_currentUser.ColaboradorId, desde, hasta, ct);

    [HttpGet("team/corrections/pending")]
    [Authorize(Policy = "ManagerApproval")]
    public Task<IReadOnlyList<CorreccionDto>> PendingCorrections(CancellationToken ct)
        => _attendance.GetPendingCorrectionsAsync(_currentUser.ColaboradorId, ct);

    [HttpPost("corrections/{correctionId:guid}/approve")]
    [Authorize(Policy = "ManagerApproval")]
    public async Task<ActionResult<CorreccionDto>> ApproveCorrection(Guid correctionId, [FromBody] DecisionCorreccionDto dto, CancellationToken ct)
    {
        var result = await _attendance.ApproveCorrectionAsync(_currentUser.ColaboradorId, correctionId, dto, _currentUser.IsRhAdmin, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("corrections/{correctionId:guid}/reject")]
    [Authorize(Policy = "ManagerApproval")]
    public async Task<ActionResult<CorreccionDto>> RejectCorrection(Guid correctionId, [FromBody] DecisionCorreccionDto dto, CancellationToken ct)
    {
        var result = await _attendance.RejectCorrectionAsync(_currentUser.ColaboradorId, correctionId, dto, _currentUser.IsRhAdmin, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpGet("commercial/records")]
    [Authorize(Policy = "RhAdmin")]
    public Task<IReadOnlyList<RegistroAsistenciaDto>> CommercialRecords([FromQuery] DateTime? fecha, CancellationToken ct)
        => _admin.ListCommercialRecordsAsync(fecha, ct);

    [HttpPost("commercial/check-in")]
    public async Task<ActionResult<RegistroAsistenciaDto>> CommercialCheckIn([FromBody] CheckInOutDto dto, CancellationToken ct)
    {
        dto = dto with { Fuente = FuenteRegistroAsistencia.Comercial };
        var result = await _attendance.CheckInAsync(_currentUser.ColaboradorId, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("commercial/check-out")]
    public async Task<ActionResult<RegistroAsistenciaDto>> CommercialCheckOut([FromBody] CheckInOutDto dto, CancellationToken ct)
    {
        dto = dto with { Fuente = FuenteRegistroAsistencia.Comercial };
        var result = await _attendance.CheckOutAsync(_currentUser.ColaboradorId, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpGet("available-shifts")]
    public Task<IReadOnlyList<TurnoDto>> AvailableShifts(CancellationToken ct)
        => _attendance.GetAvailableShiftsAsync(ct);

    [HttpGet("my-shifts")]
    public Task<IReadOnlyList<AsignacionTurnoDto>> MyShifts(CancellationToken ct)
        => _attendance.GetMyShiftHistoryAsync(_currentUser.ColaboradorId, ct);

    [HttpPost("shift-change-requests")]
    public async Task<ActionResult<SolicitudCambioHorarioDto>> CreateShiftChangeRequest([FromBody] CrearSolicitudCambioHorarioDto dto, CancellationToken ct)
    {
        var result = await _attendance.CreateShiftChangeRequestAsync(_currentUser.ColaboradorId, dto, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpGet("shift-change-requests/my")]
    public Task<IReadOnlyList<SolicitudCambioHorarioDto>> MyShiftChangeRequests(CancellationToken ct)
        => _attendance.GetMyShiftChangeRequestsAsync(_currentUser.ColaboradorId, ct);

    [HttpGet("shift-change-requests/pending")]
    [Authorize(Policy = "ManagerApproval")]
    public Task<IReadOnlyList<SolicitudCambioHorarioDto>> PendingShiftChangeRequests(CancellationToken ct)
        => _attendance.GetPendingShiftChangeRequestsAsync(_currentUser.ColaboradorId, ct);

    [HttpPost("shift-change-requests/{id:guid}/approve")]
    [Authorize(Policy = "ManagerApproval")]
    public async Task<ActionResult<SolicitudCambioHorarioDto>> ApproveShiftChangeRequest(Guid id, [FromBody] DecisionCambioHorarioDto dto, CancellationToken ct)
    {
        var result = await _attendance.ApproveShiftChangeRequestAsync(_currentUser.ColaboradorId, id, dto, _currentUser.IsRhAdmin, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("shift-change-requests/{id:guid}/reject")]
    [Authorize(Policy = "ManagerApproval")]
    public async Task<ActionResult<SolicitudCambioHorarioDto>> RejectShiftChangeRequest(Guid id, [FromBody] DecisionCambioHorarioDto dto, CancellationToken ct)
    {
        var result = await _attendance.RejectShiftChangeRequestAsync(_currentUser.ColaboradorId, id, dto, _currentUser.IsRhAdmin, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }
}

[ApiController]
[Route(ApiRoutes.Attendance + "/admin")]
[Route(ApiRoutes.AttendanceLegacy + "/admin")]
[Authorize(Policy = "RhAdmin")]
public class AttendanceAdminController : ControllerBase
{
    private readonly IAttendanceAdminService _admin;

    public AttendanceAdminController(IAttendanceAdminService admin) => _admin = admin;

    private string PerformedBy => User.Identity?.Name ?? "rh-admin";

    [HttpGet("dashboard")]
    public Task<AttendanceDashboardDto> Dashboard(CancellationToken ct) => _admin.GetDashboardAsync(ct);

    [HttpGet("records")]
    public Task<IReadOnlyList<RegistroAsistenciaDto>> Records([FromQuery] DateTime? fecha, [FromQuery] Guid? sedeId, [FromQuery] Guid? departamentoId, CancellationToken ct)
        => _admin.ListRecordsAsync(fecha, sedeId, departamentoId, ct);

    [HttpGet("incidents")]
    public Task<IReadOnlyList<IncidenciaDto>> Incidents([FromQuery] EstadoIncidenciaAsistencia? estado, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, CancellationToken ct)
        => _admin.ListIncidentsAsync(estado, desde, hasta, ct);

    [HttpGet("corrections")]
    public Task<IReadOnlyList<CorreccionDto>> Corrections([FromQuery] EstadoCorreccionAsistencia? estado, CancellationToken ct)
        => _admin.ListCorrectionsAsync(estado, ct);

    [HttpPost("incidents/generate")]
    public async Task<ActionResult<object>> GenerateIncidents([FromQuery] DateTime? fecha, CancellationToken ct)
    {
        var count = await _admin.GenerateDailyIncidentsAsync(fecha, PerformedBy, ct);
        return Ok(new { generadas = count });
    }

    [HttpGet("payroll-incidents")]
    public Task<IReadOnlyList<IncidenciaNominaAsistenciaDto>> PayrollIncidents(CancellationToken ct)
        => _admin.ListPayrollIncidentsAsync(ct);

    [HttpPost("payroll-incidents/generate")]
    public async Task<ActionResult<object>> GeneratePayroll([FromBody] GeneratePayrollRequest body, CancellationToken ct)
    {
        var count = await _admin.GeneratePayrollCutAsync(body.Periodo ?? DateTime.UtcNow.ToString("yyyy-MM"), PerformedBy, ct);
        return Ok(new { generadas = count });
    }

    [HttpPost("payroll-incidents/send")]
    public async Task<IActionResult> SendPayroll(CancellationToken ct)
    {
        await _admin.SendPayrollCutAsync(PerformedBy, ct);
        return NoContent();
    }

    [HttpGet("shifts")]
    public Task<IReadOnlyList<TurnoDto>> Shifts(CancellationToken ct) => _admin.ListShiftsAsync(ct);

    [HttpPost("shifts")]
    public Task<TurnoDto> CreateShift([FromBody] UpsertTurnoDto dto, CancellationToken ct)
        => _admin.UpsertShiftAsync(null, dto, ct);

    [HttpPut("shifts/{id:guid}")]
    public Task<TurnoDto> UpdateShift(Guid id, [FromBody] UpsertTurnoDto dto, CancellationToken ct)
        => _admin.UpsertShiftAsync(id, dto, ct);

    [HttpGet("shift-assignments")]
    public Task<IReadOnlyList<AsignacionTurnoDto>> ShiftAssignments(CancellationToken ct)
        => _admin.ListShiftAssignmentsAsync(ct);

    [HttpPost("shift-assignments")]
    public Task<AsignacionTurnoDto> AssignShift([FromBody] UpsertAsignacionTurnoDto dto, CancellationToken ct)
        => _admin.AssignShiftAsync(dto, ct);

    [HttpGet("rules")]
    public Task<ReglasAsistenciaDto> GetRules([FromQuery] Guid? sedeId, CancellationToken ct)
        => _admin.GetRulesAsync(sedeId, ct);

    [HttpPut("rules")]
    public Task<ReglasAsistenciaDto> UpdateRules([FromBody] ReglasAsistenciaDto dto, CancellationToken ct)
        => _admin.UpdateRulesAsync(dto, ct);

    [HttpPost("records/manual")]
    public async Task<ActionResult<RegistroAsistenciaDto>> ManualRecord([FromBody] ManualRecordRequest body, CancellationToken ct)
    {
        var result = await _admin.CreateManualRecordAsync(body.ColaboradorId, body.Fecha, body.Entrada, body.Salida, PerformedBy, ct);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }
}

public record GeneratePayrollRequest(string? Periodo);
public record ManualRecordRequest(Guid ColaboradorId, DateTime Fecha, DateTime? Entrada, DateTime? Salida);
