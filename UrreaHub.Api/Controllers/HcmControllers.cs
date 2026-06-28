using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrreaHub.Api.Routing;
using UrreaHub.Application.Auth;
using UrreaHub.Application.Common;
using UrreaHub.Application.CoreRH;
using UrreaHub.Application.HCM;

namespace UrreaHub.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Hcm)]
[Route(ApiRoutes.HcmLegacy)]
public class HcmEmployeesController : ControllerBase
{
    private readonly IHcmEmployeeService _employees;
    private readonly IOrganigramaService _organigrama;
    private readonly ICurrentUser _currentUser;

    public HcmEmployeesController(
        IHcmEmployeeService employees,
        IOrganigramaService organigrama,
        ICurrentUser currentUser)
    {
        _employees = employees;
        _organigrama = organigrama;
        _currentUser = currentUser;
    }

    [HttpGet("employees")]
    [Authorize(Policy = "HcmRead")]
    public async Task<ActionResult<PagedResult<EmployeeListDto>>> GetEmployees(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? search = null,
        [FromQuery] Guid? departmentId = null,
        [FromQuery] Guid? locationId = null,
        [FromQuery] string? status = null,
        [FromQuery] string? externalSource = null,
        [FromQuery] string? syncStatus = null,
        [FromQuery] Guid? positionId = null,
        [FromQuery] Guid? costCenterId = null,
        [FromQuery] Guid? contractTypeId = null,
        CancellationToken cancellationToken = default)
        => Ok(await _employees.GetEmployeesAsync(page, pageSize, search, departmentId, locationId, status, externalSource, syncStatus, positionId, costCenterId, contractTypeId, cancellationToken));

    [HttpGet("employees/{employeeId:guid}")]
    [Authorize(Policy = "HcmRead")]
    public async Task<ActionResult<EmployeeDetailDto>> GetEmployee(Guid employeeId, CancellationToken cancellationToken)
    {
        var includeSensitive = User.HasClaim(AppPermissions.ClaimType, AppPermissions.HcmReadSensitive);
        var employee = await _employees.GetEmployeeAsync(employeeId, includeSensitive, cancellationToken);
        return employee is null ? NotFound() : Ok(employee);
    }

    [HttpGet("employees/{employeeId:guid}/movements")]
    [Authorize(Policy = "HcmRead")]
    public async Task<ActionResult<IReadOnlyList<EmployeeMovementDto>>> GetMovements(Guid employeeId, CancellationToken cancellationToken)
        => Ok(await _employees.GetMovementsAsync(employeeId, cancellationToken));

    [HttpGet("employees/{employeeId:guid}/audit-log")]
    [Authorize(Policy = "AuditRead")]
    public async Task<ActionResult<IReadOnlyList<EmployeeAuditLogDto>>> GetAuditLog(Guid employeeId, CancellationToken cancellationToken)
        => Ok(await _employees.GetAuditLogAsync(employeeId, cancellationToken));

    [HttpGet("employees/{employeeId:guid}/organization")]
    [Authorize(Policy = "HcmRead")]
    public async Task<ActionResult<EmployeeOrganizationDto>> GetOrganization(Guid employeeId, CancellationToken cancellationToken)
    {
        var org = await _employees.GetOrganizationAsync(employeeId, cancellationToken);
        return org is null ? NotFound() : Ok(org);
    }

    [HttpGet("employees/{employeeId:guid}/vacation-summary")]
    [Authorize(Policy = "HcmRead")]
    public async Task<ActionResult<EmployeeVacationSummaryDto>> GetVacationSummary(Guid employeeId, [FromQuery] int? year, CancellationToken cancellationToken)
    {
        var summary = await _employees.GetVacationSummaryAsync(employeeId, year, cancellationToken);
        return summary is null ? NotFound() : Ok(summary);
    }

    [HttpGet("employees/{employeeId:guid}/documents")]
    [Authorize(Policy = "HcmRead")]
    public async Task<ActionResult<IReadOnlyList<EmployeeDocumentDto>>> GetDocuments(Guid employeeId, CancellationToken cancellationToken)
        => Ok(await _employees.GetDocumentsAsync(employeeId, cancellationToken));

    [HttpGet("employees/{employeeId:guid}/module-links")]
    [Authorize(Policy = "HcmRead")]
    public async Task<ActionResult<IReadOnlyList<EmployeeModuleLinkDto>>> GetModuleLinks(Guid employeeId, CancellationToken cancellationToken)
        => Ok(await _employees.GetModuleLinksAsync(employeeId, cancellationToken));

    [HttpPut("employees/{employeeId:guid}")]
    [Authorize(Policy = "HcmWrite")]
    public async Task<ActionResult<EmployeeDetailDto>> UpdateEmployee(Guid employeeId, [FromBody] EmployeeUpdateDto dto, CancellationToken cancellationToken)
    {
        var updated = await _employees.UpdateEmployeeAsync(employeeId, dto, _currentUser.ColaboradorId.ToString(), cancellationToken);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("employees/{employeeId:guid}/sync")]
    [Authorize(Policy = "IntegrationRunSync")]
    public async Task<ActionResult> SyncEmployee(Guid employeeId, CancellationToken cancellationToken)
    {
        var ok = await _employees.TriggerEmployeeSyncAsync(employeeId, cancellationToken);
        return ok ? Ok(new { synced = true }) : BadRequest(new { error = "Sync bloqueado o colaborador no encontrado." });
    }

    [HttpGet("dashboard")]
    [Authorize(Policy = "HcmRead")]
    public async Task<ActionResult<HcmDashboardDto>> Dashboard(CancellationToken cancellationToken)
        => Ok(await _employees.GetDashboardAsync(cancellationToken));

    [HttpGet("catalogs")]
    [Authorize(Policy = "HcmRead")]
    public async Task<ActionResult<HcmCatalogsDto>> Catalogs(CancellationToken cancellationToken)
        => Ok(await _employees.GetCatalogsAsync(cancellationToken));

    [HttpGet("data-quality")]
    [Authorize(Policy = "RhAdmin")]
    public async Task<ActionResult<HcmDataQualityDto>> DataQuality(CancellationToken cancellationToken)
        => Ok(await _employees.GetDataQualityAsync(cancellationToken));

    [HttpGet("data-quality/report")]
    [Authorize(Policy = "RhAdmin")]
    public async Task<ActionResult<HcmDataQualityReportDto>> DataQualityReport(CancellationToken cancellationToken)
        => Ok(await _employees.GetDataQualityReportAsync(cancellationToken));

    [HttpGet("org-chart")]
    [HttpGet("organigrama")]
    [Authorize(Policy = "HcmRead")]
    public async Task<ActionResult<OrganigramaNodoDto>> OrgChart(
        [FromQuery] Guid? raizId,
        [FromQuery] Guid? sedeId,
        [FromQuery] Guid? departamentoId,
        CancellationToken cancellationToken = default)
    {
        var arbol = await _organigrama.GetArbolAsync(raizId, sedeId, departamentoId, cancellationToken);
        return arbol is null ? NotFound() : Ok(arbol);
    }
}

[ApiController]
[Route(ApiRoutes.Integrations)]
[Route(ApiRoutes.IntegrationsLegacy)]
[Authorize(Policy = "IntegrationRead")]
public class IntegrationsController : ControllerBase
{
    private readonly IIntegrationService _integrations;
    private readonly ICurrentUser _currentUser;

    public IntegrationsController(IIntegrationService integrations, ICurrentUser currentUser)
    {
        _integrations = integrations;
        _currentUser = currentUser;
    }

    [HttpGet("sync-runs")]
    public async Task<ActionResult<IReadOnlyList<SyncRunDto>>> SyncRuns(CancellationToken cancellationToken)
        => Ok(await _integrations.GetSyncRunsAsync(cancellationToken));

    [HttpPost("payroll/sync")]
    [HttpPost("nomina/sync")]
    [Authorize(Policy = "IntegrationRunSync")]
    public async Task<ActionResult<object>> RunNominaSync(CancellationToken cancellationToken)
    {
        var count = await _integrations.RunNominaSyncAsync(cancellationToken);
        return Ok(new { recordsSynced = count });
    }

    [HttpPost("cdm/employees/upsert")]
    [Authorize(Policy = "IntegrationRunSync")]
    public async Task<ActionResult<CdmUpsertResultDto>> UpsertEmployee(
        [FromBody] CdmEmployeeUpsertDto payload,
        CancellationToken cancellationToken)
    {
        var user = _currentUser.ColaboradorId.ToString();
        var result = await _integrations.UpsertEmployeeFromCdmAsync(payload, user, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
