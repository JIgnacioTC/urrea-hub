using UrreaHub.Application.Common;

namespace UrreaHub.Application.HCM;

public record EmployeeListDto(
    Guid Id,
    string EmployeeNumber,
    string LegalFullName,
    string? PreferredName,
    string Position,
    string Department,
    string? Area,
    string? CostCenter,
    string? ManagerName,
    string? Location,
    string ContractType,
    string Status,
    DateTime HireDate,
    int TenureYears,
    string? ExternalSource,
    string SyncStatus,
    DateTime? LastSyncAt,
    bool IsManualOverride);

public record EmployeeDetailDto(
    Guid Id,
    string EmployeeNumber,
    string LegalFirstName,
    string LegalLastName,
    string? LegalMiddleName,
    string? PreferredName,
    string WorkEmail,
    string? Phone,
    string Status,
    DateTime HireDate,
    DateTime? TerminationDate,
    string Position,
    string Department,
    string? Area,
    string? Location,
    string? CostCenter,
    string ContractType,
    string? ManagerName,
    int TenureYears,
    string? ExternalSource,
    string? ExternalEmployeeId,
    string SyncStatus,
    DateTime? LastSyncAt,
    bool IsManualOverride,
    EmployeePersonalDto? Personal,
    EmployeeEmploymentDto? Employment,
    IReadOnlyList<EmployeeSummaryDto> DirectReports);

public record EmployeePersonalDto(
    string? Rfc,
    string? Curp,
    string? Nss,
    DateTime? BirthDate,
    string? Address,
    bool IsMasked);

public record EmployeeEmploymentDto(
    string? WorkSchedule,
    string? Shift,
    string? PayrollGroup,
    bool Unionized,
    string CompensationVisibility);

public record EmployeeSummaryDto(
    Guid Id,
    string EmployeeNumber,
    string FullName,
    string Position);

public record EmployeeMovementDto(
    Guid Id,
    string MovementType,
    DateTime EffectiveDate,
    string? PreviousValue,
    string? NewValue,
    string Source,
    string? ExternalReference,
    string CreatedBy,
    DateTime CreatedAt);

public record HcmDashboardDto(
    int ActiveEmployees,
    int CostCenters,
    int OrganizationUnits,
    int Locations,
    DateTime? LastNominaSync);

public record HcmCatalogsDto(
    IReadOnlyList<HcmCatalogItemDto> Locations,
    IReadOnlyList<HcmCatalogItemDto> Departments,
    IReadOnlyList<HcmCatalogItemDto> Positions,
    IReadOnlyList<HcmCatalogItemDto> CostCenters,
    IReadOnlyList<HcmCatalogItemDto> ContractTypes);

public record HcmCatalogItemDto(Guid Id, string Name, string? Code = null);

public record HcmDataQualityDto(
    int WithoutManager,
    int WithoutCostCenter,
    int MissingRfc,
    int MissingCurp,
    int MissingNss,
    int PendingSync,
    int ManualOverrideConflicts,
    int TotalActive);

public record HcmDataQualityIssueDto(
    string Code,
    string Label,
    int Count,
    IReadOnlyList<EmployeeSummaryDto> Samples);

public record HcmDataQualityReportDto(
    HcmDataQualityDto Summary,
    IReadOnlyList<HcmDataQualityIssueDto> Issues);

public record EmployeeAuditLogDto(
    DateTime OccurredAt,
    string Module,
    string Action,
    string? User,
    string? Detail);

public record EmployeeVacationSummaryDto(
    int Year,
    decimal DaysAssigned,
    decimal DaysUsed,
    decimal DaysPending,
    int RequestsThisYear,
    int PendingApproval);

public record EmployeeDocumentDto(
    Guid Id,
    string Name,
    string DocumentType,
    int Version,
    string Status,
    DateTime? ValidUntil);

public record EmployeeModuleLinkDto(
    string Module,
    string Label,
    int RecordCount,
    bool ModuleAvailable,
    string StatusMessage);

public record EmployeeUpdateDto(
    string? PreferredName,
    string? Phone);

public record EmployeeOrganizationDto(
    Guid? ManagerId,
    string? ManagerName,
    string? ManagerEmployeeNumber,
    Guid DepartmentId,
    string Department,
    string? Area,
    Guid? LocationId,
    string? Location,
    Guid? CostCenterId,
    string? CostCenter,
    Guid PositionId,
    string Position,
    int DirectReportsCount);

public record SyncRunDto(
    Guid Id,
    string Name,
    string ExternalSystem,
    string Status,
    DateTime? LastRunAt);

public record CdmEmployeeUpsertDto(
    string Source,
    string SourceSystem,
    string ExternalEmployeeId,
    string EmployeeNumber,
    string LegalFirstName,
    string LegalLastName,
    string? PreferredName,
    string? WorkEmail,
    string Status,
    DateTime HireDate,
    string? PositionCode,
    string? PositionName,
    string? AreaCode,
    string? AreaName,
    string? CostCenterCode,
    string? ManagerEmployeeNumber,
    string? LocationCode,
    string? ContractType,
    string? MovementType,
    DateTime? EffectiveDate,
    string? Rfc,
    string? Curp,
    string? Nss);

public interface IHcmEmployeeService
{
    Task<HcmDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<HcmCatalogsDto> GetCatalogsAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<EmployeeListDto>> GetEmployeesAsync(
        int page, int pageSize, string? search,
        Guid? departmentId, Guid? locationId, string? status,
        string? externalSource, string? syncStatus,
        Guid? positionId, Guid? costCenterId, Guid? contractTypeId,
        CancellationToken cancellationToken = default);
    Task<EmployeeDetailDto?> GetEmployeeAsync(Guid id, bool includeSensitive, CancellationToken cancellationToken = default);
    Task<EmployeeOrganizationDto?> GetOrganizationAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmployeeMovementDto>> GetMovementsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmployeeAuditLogDto>> GetAuditLogAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EmployeeVacationSummaryDto?> GetVacationSummaryAsync(Guid id, int? year, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmployeeDocumentDto>> GetDocumentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EmployeeModuleLinkDto>> GetModuleLinksAsync(Guid id, CancellationToken cancellationToken = default);
    Task<HcmDataQualityDto> GetDataQualityAsync(CancellationToken cancellationToken = default);
    Task<HcmDataQualityReportDto> GetDataQualityReportAsync(CancellationToken cancellationToken = default);
    Task<bool> TriggerEmployeeSyncAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EmployeeDetailDto?> UpdateEmployeeAsync(Guid id, EmployeeUpdateDto dto, string performedBy, CancellationToken cancellationToken = default);
}

public interface IIntegrationService
{
    Task<IReadOnlyList<SyncRunDto>> GetSyncRunsAsync(CancellationToken cancellationToken = default);
    Task<int> RunNominaSyncAsync(CancellationToken cancellationToken = default);
    Task<CdmUpsertResultDto> UpsertEmployeeFromCdmAsync(CdmEmployeeUpsertDto payload, string performedBy, CancellationToken cancellationToken = default);
}

public record CdmUpsertResultDto(
    bool Success,
    Guid? EmployeeId,
    string Action,
    string? Error);
