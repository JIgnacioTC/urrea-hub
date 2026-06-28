namespace UrreaHub.Api.Routing;

/// <summary>Rutas versionadas y legacy. Preferir <see cref="V1"/> en consumidores nuevos.</summary>
public static class ApiRoutes
{
    public const string V1 = "/api/v1";
    public const string Legacy = "/api";

    public const string Auth = V1 + "/auth";
    public const string AuthLegacy = Legacy + "/auth";

    public const string Hcm = V1 + "/hcm";
    public const string HcmLegacy = Legacy + "/hcm";

    public const string Catalogs = V1 + "/catalogs";
    public const string CatalogsLegacy = Legacy + "/catalogs";

    public const string Integrations = V1 + "/integrations";
    public const string IntegrationsLegacy = Legacy + "/integrations";

    public const string TiMetadata = V1 + "/ti/metadata";
    public const string TiMetadataLegacy = Legacy + "/ti/metadata";

    public const string AdminEntities = V1 + "/admin/entities";
    public const string AdminEntitiesLegacy = Legacy + "/admin/entities";

    public const string Portal = V1 + "/portal";
    public const string PortalLegacy = Legacy + "/portal";

    public const string RhAdminPortal = V1 + "/rh/admin/portal";
    public const string RhAdminPortalLegacy = Legacy + "/rh/admin/portal";

    public const string Absence = V1 + "/absence";
    public const string AbsenceLegacy = Legacy + "/vacaciones";

    public const string Rh = V1 + "/rh";
    public const string RhLegacy = Legacy + "/rh";

    public const string Team = V1 + "/team";
    public const string TeamLegacy = Legacy + "/portal/equipo";

    public const string SecurityAdmin = V1 + "/ti/admin/security";
    public const string SecurityAdminLegacy = Legacy + "/ti/admin/security";

    public const string DhAdminOrg = V1 + "/rh/admin/org";
    public const string DhAdminOrgLegacy = Legacy + "/rh/admin/org";

    public const string Attendance = V1 + "/attendance";
    public const string AttendanceLegacy = Legacy + "/asistencia";

    public const string Onboarding = V1 + "/onboarding";
    public const string OnboardingLegacy = Legacy + "/onboarding";

    public const string Requisitions = V1 + "/requisitions";
    public const string RequisitionsLegacy = Legacy + "/requisiciones";

    public const string Recruitment = V1 + "/recruitment";
    public const string RecruitmentLegacy = Legacy + "/reclutamiento";

    public const string Compensation = V1 + "/compensation";
    public const string CompensationLegacy = Legacy + "/compensacion";

    public const string BenefitsAdmin = V1 + "/benefits/admin";
    public const string BenefitsAdminLegacy = Legacy + "/beneficios/admin";
}
