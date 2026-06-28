using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrreaHub.Api.Routing;
using UrreaHub.Application.Auth;
using UrreaHub.Application.Common;
using UrreaHub.Application.CoreRH;
using UrreaHub.Application.Equipo;
using UrreaHub.Application.Nomina;
using UrreaHub.Application.Vacaciones;

namespace UrreaHub.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Auth)]
[Route(ApiRoutes.AuthLegacy)]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);
        if (result is null)
            return Unauthorized(new { error = "Credenciales inválidas." });
        return Ok(result);
    }
}

[ApiController]
[Route(ApiRoutes.Portal)]
[Route(ApiRoutes.PortalLegacy)]
[Authorize]
public class PortalController : ControllerBase
{
    private readonly IColaboradorService _colaboradorService;
    private readonly ICurrentUser _currentUser;

    public PortalController(IColaboradorService colaboradorService, ICurrentUser currentUser)
    {
        _colaboradorService = colaboradorService;
        _currentUser = currentUser;
    }

    [HttpGet("me")]
    public async Task<ActionResult<ColaboradorPerfilDto>> GetMe(CancellationToken cancellationToken)
    {
        var perfil = await _colaboradorService.GetPerfilAsync(_currentUser.ColaboradorId, cancellationToken);
        return perfil is null ? NotFound() : Ok(perfil);
    }
}

[ApiController]
[Route(ApiRoutes.Team)]
[Route(ApiRoutes.TeamLegacy)]
[Authorize(Policy = "JefeOrRh")]
public class EquipoController : ControllerBase
{
    private readonly IEquipoService _equipoService;
    private readonly ICurrentUser _currentUser;

    public EquipoController(IEquipoService equipoService, ICurrentUser currentUser)
    {
        _equipoService = equipoService;
        _currentUser = currentUser;
    }

    [HttpGet("colaboradores")]
    [HttpGet("employees")]
    public async Task<ActionResult<IReadOnlyList<EquipoMiembroDto>>> Colaboradores(CancellationToken cancellationToken)
        => Ok(await _equipoService.GetMiEquipoAsync(_currentUser.ColaboradorId, cancellationToken));

    [HttpGet("colaboradores/{id:guid}")]
    public async Task<ActionResult<ColaboradorPerfilDto>> FichaColaborador(Guid id, CancellationToken cancellationToken)
    {
        var ficha = await _equipoService.GetFichaSubordinadoAsync(_currentUser.ColaboradorId, id, cancellationToken);
        return ficha is null ? NotFound() : Ok(ficha);
    }

    [HttpGet("employees/{employeeId:guid}")]
    public Task<ActionResult<ColaboradorPerfilDto>> FichaEmployee(Guid employeeId, CancellationToken cancellationToken)
        => FichaColaborador(employeeId, cancellationToken);

    [HttpGet("planes-accion")]
    [HttpGet("action-plans")]
    public async Task<ActionResult<IReadOnlyList<PlanAccionDto>>> PlanesAccion(CancellationToken cancellationToken)
        => Ok(await _equipoService.GetPlanesAccionAsync(_currentUser.ColaboradorId, cancellationToken));

    [HttpPost("planes-accion")]
    [HttpPost("action-plans")]
    public async Task<ActionResult<PlanAccionDto>> CrearPlan([FromBody] CrearPlanAccionDto dto, CancellationToken cancellationToken)
    {
        var result = await _equipoService.CrearPlanAccionAsync(_currentUser.ColaboradorId, dto, cancellationToken);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpGet("feedback")]
    public async Task<ActionResult<IReadOnlyList<FeedbackEquipoDto>>> Feedback(CancellationToken cancellationToken)
        => Ok(await _equipoService.GetFeedbackAsync(_currentUser.ColaboradorId, cancellationToken));

    [HttpPost("feedback")]
    public async Task<ActionResult<FeedbackEquipoDto>> CrearFeedback([FromBody] CrearFeedbackEquipoDto dto, CancellationToken cancellationToken)
    {
        var result = await _equipoService.CrearFeedbackAsync(_currentUser.ColaboradorId, dto, cancellationToken);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpGet("capacitaciones")]
    public async Task<ActionResult<IReadOnlyList<CapacitacionEquipoDto>>> Capacitaciones(CancellationToken cancellationToken)
        => Ok(await _equipoService.GetCapacitacionesEquipoAsync(_currentUser.ColaboradorId, cancellationToken));
}

[ApiController]
[Route("api/core-rh")]
[Authorize]
public class OrganigramaController : ControllerBase
{
    private readonly IOrganigramaService _organigramaService;

    public OrganigramaController(IOrganigramaService organigramaService) => _organigramaService = organigramaService;

    [HttpGet("organigrama")]
    public async Task<ActionResult<OrganigramaNodoDto>> GetOrganigrama(
        [FromQuery] Guid? raizId,
        [FromQuery] Guid? sedeId,
        [FromQuery] Guid? departamentoId,
        CancellationToken cancellationToken)
    {
        var arbol = await _organigramaService.GetArbolAsync(raizId, sedeId, departamentoId, cancellationToken);
        return arbol is null ? NotFound() : Ok(arbol);
    }
}

[ApiController]
[Route(ApiRoutes.Absence)]
[Route(ApiRoutes.AbsenceLegacy)]
[Authorize]
public class VacacionesWorkflowController : ControllerBase
{
    private readonly ISolicitudAusenciaService _solicitudService;
    private readonly ISaldoVacacionesService _saldoService;
    private readonly ICurrentUser _currentUser;

    public VacacionesWorkflowController(
        ISolicitudAusenciaService solicitudService,
        ISaldoVacacionesService saldoService,
        ICurrentUser currentUser)
    {
        _solicitudService = solicitudService;
        _saldoService = saldoService;
        _currentUser = currentUser;
    }

    [HttpPost("solicitudes")]
    [HttpPost("requests")]
    public async Task<ActionResult<SolicitudAusenciaDto>> Crear([FromBody] CrearSolicitudDto dto, CancellationToken cancellationToken)
    {
        var result = await _solicitudService.CrearAsync(_currentUser.ColaboradorId, dto, cancellationToken);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("solicitudes/{id:guid}/enviar")]
    [HttpPost("requests/{id:guid}/submit")]
    public async Task<ActionResult<SolicitudAusenciaDto>> Enviar(Guid id, CancellationToken cancellationToken)
    {
        var result = await _solicitudService.EnviarAsync(_currentUser.ColaboradorId, id, cancellationToken);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("solicitudes/{id:guid}/cancelar")]
    public async Task<ActionResult<SolicitudAusenciaDto>> Cancelar(Guid id, CancellationToken cancellationToken)
    {
        var result = await _solicitudService.CancelarAsync(_currentUser.ColaboradorId, id, cancellationToken);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("requests/{requestId:guid}/cancel")]
    public Task<ActionResult<SolicitudAusenciaDto>> CancelRequest(Guid requestId, CancellationToken cancellationToken)
        => Cancelar(requestId, cancellationToken);

    [HttpPost("solicitudes/{id:guid}/aprobar")]
    [Authorize(Policy = "ManagerApproval")]
    public async Task<ActionResult<SolicitudAusenciaDto>> Aprobar(Guid id, [FromBody] AprobacionRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _solicitudService.AprobarAsync(_currentUser.ColaboradorId, id, dto, _currentUser.IsRhAdmin, cancellationToken);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("requests/{requestId:guid}/approve")]
    [Authorize(Policy = "ManagerApproval")]
    public Task<ActionResult<SolicitudAusenciaDto>> Approve(Guid requestId, [FromBody] AprobacionRequestDto dto, CancellationToken cancellationToken)
        => Aprobar(requestId, dto, cancellationToken);

    [HttpPost("solicitudes/{id:guid}/rechazar")]
    [Authorize(Policy = "ManagerApproval")]
    public async Task<ActionResult<SolicitudAusenciaDto>> Rechazar(Guid id, [FromBody] AprobacionRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _solicitudService.RechazarAsync(_currentUser.ColaboradorId, id, dto, _currentUser.IsRhAdmin, cancellationToken);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpPost("requests/{requestId:guid}/reject")]
    [Authorize(Policy = "ManagerApproval")]
    public Task<ActionResult<SolicitudAusenciaDto>> Reject(Guid requestId, [FromBody] AprobacionRequestDto dto, CancellationToken cancellationToken)
        => Rechazar(requestId, dto, cancellationToken);

    [HttpGet("mis-solicitudes")]
    [HttpGet("my-requests")]
    public async Task<ActionResult<IReadOnlyList<SolicitudAusenciaDto>>> MisSolicitudes(CancellationToken cancellationToken)
        => Ok(await _solicitudService.GetMisSolicitudesAsync(_currentUser.ColaboradorId, cancellationToken));

    [HttpGet("pendientes-aprobacion")]
    [HttpGet("pending-approvals")]
    [Authorize(Policy = "ManagerApproval")]
    public async Task<ActionResult<IReadOnlyList<PendingApprovalDto>>> Pendientes(CancellationToken cancellationToken)
        => Ok(await _solicitudService.GetPendientesAprobacionAsync(_currentUser.ColaboradorId, cancellationToken));

    [HttpGet("requests/{id:guid}")]
    public async Task<ActionResult<SolicitudAusenciaDto>> GetRequest(Guid id, CancellationToken cancellationToken)
    {
        var dto = await _solicitudService.GetByIdAsync(id, _currentUser.ColaboradorId, _currentUser.IsRhAdmin, cancellationToken);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("team-calendar")]
    public async Task<ActionResult<TeamCalendarDto>> TeamCalendar(
        [FromQuery] DateTime desde,
        [FromQuery] DateTime hasta,
        CancellationToken cancellationToken)
        => Ok(await _solicitudService.GetTeamCalendarAsync(_currentUser.ColaboradorId, desde, hasta, cancellationToken));

    [HttpPost("requests/calculate-days")]
    public async Task<ActionResult<CalculateDaysResultDto>> CalculateDaysPost(
        [FromBody] CalculateDaysDto body,
        CancellationToken cancellationToken)
        => Ok(await _solicitudService.PreviewDaysAsync(_currentUser.ColaboradorId, body, cancellationToken));

    [HttpGet("saldos/mi-saldo")]
    [HttpGet("my-balance")]
    public async Task<ActionResult<SaldoVacacionesDto>> MiSaldo([FromQuery] int? anio, [FromQuery] int? year, CancellationToken cancellationToken)
    {
        var saldo = await _saldoService.GetMiSaldoAsync(_currentUser.ColaboradorId, anio ?? year, cancellationToken);
        return saldo is null ? NotFound() : Ok(saldo);
    }

    [HttpGet("calendario/ausencias")]
    public async Task<ActionResult<IReadOnlyList<CalendarioAusenciaDto>>> Calendario(
        [FromQuery] DateTime desde,
        [FromQuery] DateTime hasta,
        CancellationToken cancellationToken)
        => Ok(await _solicitudService.GetCalendarioAusenciasAsync(_currentUser.ColaboradorId, desde, hasta, cancellationToken));

    [HttpGet("calcular-dias")]
    public async Task<ActionResult<decimal>> CalcularDias(
        [FromQuery] DateTime inicio,
        [FromQuery] DateTime fin,
        CancellationToken cancellationToken)
        => Ok(await _solicitudService.CalcularDiasHabilesAsync(_currentUser.ColaboradorId, inicio, fin, cancellationToken));

    [HttpGet("permisos/tipos")]
    [HttpGet("permission-types")]
    public async Task<ActionResult<IReadOnlyList<TipoAusenciaDto>>> TiposPermiso(CancellationToken cancellationToken)
        => Ok(await _solicitudService.GetTiposPermisoAsync(cancellationToken));

    [HttpGet("permisos/resumen")]
    [HttpGet("permission-types/summary")]
    public async Task<ActionResult<IReadOnlyList<ResumenTipoPermisoDto>>> ResumenPermisos(
        [FromQuery] int? anio,
        CancellationToken cancellationToken)
        => Ok(await _solicitudService.GetResumenPermisosAsync(_currentUser.ColaboradorId, anio, cancellationToken));
}

[ApiController]
[Route(ApiRoutes.Rh)]
[Route(ApiRoutes.RhLegacy)]
[Authorize(Policy = "RhAdmin")]
public class RhController : ControllerBase
{
    private readonly ISolicitudAusenciaService _solicitudService;
    private readonly IColaboradorService _colaboradorService;
    private readonly INominaSyncService _nominaSyncService;

    public RhController(
        ISolicitudAusenciaService solicitudService,
        IColaboradorService colaboradorService,
        INominaSyncService nominaSyncService)
    {
        _solicitudService = solicitudService;
        _colaboradorService = colaboradorService;
        _nominaSyncService = nominaSyncService;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<RhDashboardDto>> Dashboard(CancellationToken cancellationToken)
        => Ok(await _solicitudService.GetDashboardAsync(cancellationToken));

    [HttpGet("colaboradores")]
    public async Task<ActionResult<PagedResult<ColaboradorResumenDto>>> Colaboradores(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
        => Ok(await _colaboradorService.ListAsync(page, pageSize, search, cancellationToken));

    [HttpPut("colaboradores/{id:guid}")]
    public async Task<ActionResult<ColaboradorRhDto>> UpdateColaborador(Guid id, [FromBody] ColaboradorRhUpdateDto dto, CancellationToken cancellationToken)
    {
        var result = await _colaboradorService.UpdateRhFieldsAsync(id, dto, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("reportes/ausencias")]
    public async Task<IActionResult> ReporteAusencias(
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta,
        [FromQuery] Guid? sedeId,
        [FromQuery] Guid? tipoId,
        [FromQuery] Domain.Common.EstadoSolicitud? estado,
        [FromQuery] string format = "json",
        CancellationToken cancellationToken = default)
    {
        var data = await _solicitudService.GetReporteAusenciasAsync(desde, hasta, sedeId, tipoId, estado, cancellationToken);
        if (format.Equals("csv", StringComparison.OrdinalIgnoreCase))
        {
            var csv = "NumeroEmpleado,Colaborador,TipoAusencia,FechaInicio,FechaFin,Dias,Estado,Aprobador\n" +
                string.Join("\n", data.Select(r =>
                    $"{r.NumeroEmpleado},{r.Colaborador},{r.TipoAusencia},{r.FechaInicio:yyyy-MM-dd},{r.FechaFin:yyyy-MM-dd},{r.Dias},{r.Estado},{r.Aprobador}"));
            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "ausencias.csv");
        }
        return Ok(data);
    }

    [HttpPost("nomina/sync")]
    [HttpPost("payroll/sync")]
    [Authorize(Policy = "IntegrationRunSync")]
    public async Task<ActionResult<object>> SyncNomina(CancellationToken cancellationToken)
    {
        var count = await _nominaSyncService.SyncAsync(cancellationToken);
        return Ok(new { sincronizados = count });
    }
}
