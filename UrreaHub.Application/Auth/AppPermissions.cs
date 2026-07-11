using UrreaHub.Application.Common;

namespace UrreaHub.Application.Auth;

/// <summary>Permisos granulares por módulo (RBAC producción).</summary>
public static class AppPermissions
{
    public const string ClaimType = "permission";

    public const string HcmRead = "hcm.read";
    public const string HcmWrite = "hcm.write";
    public const string HcmReadSensitive = "hcm.read_sensitive";
    public const string HcmReadSalary = "hcm.read_salary";
    public const string VacationsApprove = "vacations.approve";
    public const string VacationsApprovePayroll = "vacations.approve_payroll";
    public const string AttendanceValidate = "attendance.validate";
    public const string AttendanceRead = "attendance.read";
    public const string AttendanceCheckIn = "attendance.checkin";
    public const string AttendanceCheckOut = "attendance.checkout";
    public const string AttendanceCorrectionCreate = "attendance.correction.create";
    public const string AttendanceCorrectionApprove = "attendance.correction.approve";
    public const string AttendanceTeamRead = "attendance.team.read";
    public const string AttendanceAdminRead = "attendance.admin.read";
    public const string AttendanceAdminWrite = "attendance.admin.write";
    public const string AttendancePayrollGenerate = "attendance.payroll.generate";
    public const string AttendancePayrollSend = "attendance.payroll.send";
    public const string AttendanceCommercialRead = "attendance.commercial.read";
    public const string AttendanceCommercialWrite = "attendance.commercial.write";
    public const string OnboardingRead = "onboarding.read";
    public const string OnboardingTaskComplete = "onboarding.task.complete";
    public const string OnboardingTeamRead = "onboarding.team.read";
    public const string OnboardingAdminRead = "onboarding.admin.read";
    public const string OnboardingAdminWrite = "onboarding.admin.write";
    public const string RequisitionsRead = "requisitions.read";
    public const string RequisitionsCreate = "requisitions.create";
    public const string RequisitionsApprove = "requisitions.approve";
    public const string RequisitionsAdmin = "requisitions.admin";
    public const string RecruitmentRead = "recruitment.read";
    public const string RecruitmentWrite = "recruitment.write";
    public const string CompensationRead = "compensation.read";
    public const string CompensationRequestCreate = "compensation.request.create";
    public const string CompensationAdminRead = "compensation.admin.read";
    public const string CompensationAdminWrite = "compensation.admin.write";
    public const string CompensationApprove = "compensation.approve";
    public const string BenefitsAdminRead = "benefits.admin.read";
    public const string BenefitsAdminWrite = "benefits.admin.write";
    public const string DocumentsUpload = "documents.upload";
    public const string EthicsReadCase = "ethics.read_case";
    public const string IntegrationRunSync = "integrations.run_sync";
    public const string IntegrationRead = "integrations.read";
    public const string AnalyticsExecutiveView = "analytics.executive_view";
    public const string TiMetadataRead = "ti.metadata.read";
    public const string TiMetadataWrite = "ti.metadata.write";
    public const string AdminEntitiesRead = "admin.entities.read";
    public const string AdminEntitiesWrite = "admin.entities.write";
    public const string AdminEntitiesDelete = "admin.entities.delete";
    public const string AuditRead = "audit.read";
    public const string ManagerApproval = "manager.approval";
}

public static class RolePermissionMap
{
    public static IReadOnlyList<string> Resolve(IEnumerable<string> roles)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var role in roles)
        {
            foreach (var p in ForRole(role))
                set.Add(p);
        }
        return set.ToList();
    }

    public static IReadOnlyList<string> ForRole(string role) => role switch
    {
        AppRoles.RhAdmin => new[]
        {
            AppPermissions.HcmRead, AppPermissions.HcmWrite, AppPermissions.HcmReadSensitive,
            AppPermissions.VacationsApprove, AppPermissions.AttendanceValidate,
            AppPermissions.AttendanceRead, AppPermissions.AttendanceCheckIn, AppPermissions.AttendanceCheckOut,
            AppPermissions.AttendanceCorrectionCreate, AppPermissions.AttendanceCorrectionApprove,
            AppPermissions.AttendanceTeamRead, AppPermissions.AttendanceAdminRead, AppPermissions.AttendanceAdminWrite,
            AppPermissions.AttendancePayrollGenerate, AppPermissions.AttendancePayrollSend,
            AppPermissions.AttendanceCommercialRead, AppPermissions.AttendanceCommercialWrite,
            AppPermissions.OnboardingRead, AppPermissions.OnboardingTaskComplete,
            AppPermissions.OnboardingTeamRead, AppPermissions.OnboardingAdminRead, AppPermissions.OnboardingAdminWrite,
            AppPermissions.RequisitionsRead, AppPermissions.RequisitionsCreate, AppPermissions.RequisitionsApprove,
            AppPermissions.RequisitionsAdmin, AppPermissions.RecruitmentRead, AppPermissions.RecruitmentWrite,
            AppPermissions.CompensationRead, AppPermissions.CompensationRequestCreate,
            AppPermissions.CompensationAdminRead, AppPermissions.CompensationAdminWrite, AppPermissions.CompensationApprove,
            AppPermissions.BenefitsAdminRead, AppPermissions.BenefitsAdminWrite,
            AppPermissions.DocumentsUpload, AppPermissions.IntegrationRunSync, AppPermissions.IntegrationRead,
            AppPermissions.AnalyticsExecutiveView,
            AppPermissions.TiMetadataRead, AppPermissions.TiMetadataWrite,
            AppPermissions.AdminEntitiesRead, AppPermissions.AdminEntitiesWrite, AppPermissions.AdminEntitiesDelete,
            AppPermissions.AuditRead, AppPermissions.ManagerApproval,
        },
        AppRoles.TiAdmin => new[]
        {
            AppPermissions.TiMetadataRead, AppPermissions.TiMetadataWrite,
            AppPermissions.IntegrationRunSync, AppPermissions.IntegrationRead,
            AppPermissions.AdminEntitiesRead, AppPermissions.AdminEntitiesWrite, AppPermissions.AdminEntitiesDelete,
            AppPermissions.AuditRead, AppPermissions.AttendanceAdminRead,
        },
        AppRoles.Jefe => new[]
        {
            AppPermissions.HcmRead, AppPermissions.VacationsApprove, AppPermissions.ManagerApproval,
            AppPermissions.AttendanceTeamRead, AppPermissions.AttendanceCorrectionApprove,
            AppPermissions.AttendanceRead,
            AppPermissions.OnboardingTeamRead, AppPermissions.OnboardingTaskComplete, AppPermissions.OnboardingRead,
            AppPermissions.RequisitionsRead, AppPermissions.RequisitionsCreate, AppPermissions.RequisitionsApprove,
            AppPermissions.RequisitionsAdmin, AppPermissions.RecruitmentRead, AppPermissions.RecruitmentWrite,
            AppPermissions.CompensationRead, AppPermissions.CompensationRequestCreate,
        },
        AppRoles.NominaAdmin => new[]
        {
            AppPermissions.VacationsApprovePayroll, AppPermissions.ManagerApproval,
            AppPermissions.AttendancePayrollGenerate, AppPermissions.AttendancePayrollSend,
        },
        AppRoles.Colaborador => new[]
        {
            AppPermissions.HcmRead,
            AppPermissions.AttendanceRead, AppPermissions.AttendanceCheckIn, AppPermissions.AttendanceCheckOut,
            AppPermissions.AttendanceCorrectionCreate,
            AppPermissions.OnboardingRead, AppPermissions.OnboardingTaskComplete,
            AppPermissions.RequisitionsRead, AppPermissions.RequisitionsCreate,
            AppPermissions.CompensationRead, AppPermissions.CompensationRequestCreate,
        },
        _ => Array.Empty<string>(),
    };
}
