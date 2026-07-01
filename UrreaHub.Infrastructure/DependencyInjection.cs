using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using UrreaHub.Application;
using UrreaHub.Application.Auth;
using UrreaHub.Application.Common;
using UrreaHub.Application.CoreRH;
using UrreaHub.Application.Seguridad;
using UrreaHub.Application.Nomina;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Application.Equipo;
using UrreaHub.Application.Admin;
using UrreaHub.Application.Catalogs;
using UrreaHub.Application.HCM;
using UrreaHub.Application.TI;
using UrreaHub.Application.Portal;
using UrreaHub.Application.Vacaciones;
using UrreaHub.Application.Asistencia;
using UrreaHub.Application.Onboarding;
using UrreaHub.Application.Requisiciones;
using UrreaHub.Application.Reclutamiento;
using UrreaHub.Application.Compensaciones;
using UrreaHub.Infrastructure.Persistence;
using UrreaHub.Infrastructure.Services;

namespace UrreaHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string connectionString)
    {
        services.AddDbContext<UrreaHubDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(UrreaHubDbContext).Assembly.FullName);
                sql.EnableRetryOnFailure(3);
            }));

        services.AddApplication();
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<NominaSyncSettings>(configuration.GetSection(NominaSyncSettings.SectionName));
        services.Configure<AzureAdSettings>(configuration.GetSection(AzureAdSettings.SectionName));

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IRoleResolutionService, RoleResolutionService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ISecurityAdminService, SecurityAdminService>();
        services.AddScoped<IDhOrgAdminService, DhOrgAdminService>();
        services.AddScoped<IColaboradorService, ColaboradorService>();
        services.AddScoped<IHcmEmployeeService, HcmEmployeeService>();
        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<ITiMetadataService, TiMetadataService>();
        services.AddScoped<IGenericAdminService, GenericAdminService>();
        services.AddScoped<IIntegrationService, IntegrationService>();
        services.AddScoped<IOrganigramaService, OrganigramaService>();
        services.AddScoped<IEquipoService, EquipoService>();
        services.AddScoped<IPortalContentService, PortalContentService>();
        services.AddScoped<ISolicitudAusenciaService, SolicitudAusenciaService>();
        services.AddScoped<ISaldoVacacionesService, SaldoVacacionesService>();
        services.AddScoped<IAbsenceAdminService, AbsenceAdminService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<IAttendanceAdminService, AttendanceAdminService>();
        services.AddScoped<IAsistenciaImportService, AsistenciaImportService>();
        services.AddScoped<IOnboardingService, OnboardingService>();
        services.AddScoped<IOnboardingAdminService, OnboardingAdminService>();
        services.AddScoped<IRequisitionService, RequisitionService>();
        services.AddScoped<IRequisitionAdminService, RequisitionAdminService>();
        services.AddScoped<IRecruitmentService, RecruitmentService>();
        services.AddScoped<ICompensationService, CompensationService>();
        services.AddScoped<IBenefitsAdminService, BenefitsAdminService>();
        services.AddScoped<ICalendarioLaboralService, CalendarioLaboralService>();
        services.AddScoped<INominaSyncService, NominaSyncService>();
        services.AddScoped<IAuditService, AuditService>();

        var nominaAdapter = configuration.GetSection(NominaSyncSettings.SectionName).Get<NominaSyncSettings>()?.Adapter ?? "Stub";
        if (nominaAdapter.Equals("Csv", StringComparison.OrdinalIgnoreCase))
            services.AddScoped<INominaSyncAdapter, CsvNominaSyncAdapter>();
        else
            services.AddScoped<INominaSyncAdapter, StubNominaSyncAdapter>();

        var azure = configuration.GetSection(AzureAdSettings.SectionName).Get<AzureAdSettings>();
        if (!string.IsNullOrEmpty(azure?.TenantId) && !string.IsNullOrEmpty(azure.ClientId))
            services.AddScoped<INotificationSender, GraphNotificationSender>();
        else
            services.AddScoped<INotificationSender, LoggingNotificationSender>();

        services.AddHostedService<NominaSyncBackgroundService>();

        var jwt = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret))
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("RhAdmin", policy => policy.RequireRole(AppRoles.RhAdmin));
            options.AddPolicy("TiAdmin", policy => policy.RequireRole(AppRoles.TiAdmin, AppRoles.RhAdmin));
            options.AddPolicy("JefeOrRh", policy => policy.RequireRole(AppRoles.Jefe, AppRoles.RhAdmin));
            options.AddPolicy("HcmRead", policy => policy.RequireClaim(AppPermissions.ClaimType, AppPermissions.HcmRead));
            options.AddPolicy("HcmWrite", policy => policy.RequireClaim(AppPermissions.ClaimType, AppPermissions.HcmWrite));
            options.AddPolicy("HcmReadSensitive", policy => policy.RequireClaim(AppPermissions.ClaimType, AppPermissions.HcmReadSensitive));
            options.AddPolicy("IntegrationRunSync", policy => policy.RequireClaim(AppPermissions.ClaimType, AppPermissions.IntegrationRunSync));
            options.AddPolicy("IntegrationRead", policy => policy.RequireClaim(AppPermissions.ClaimType, AppPermissions.IntegrationRead));
            options.AddPolicy("TiMetadataRead", policy => policy.RequireClaim(AppPermissions.ClaimType, AppPermissions.TiMetadataRead));
            options.AddPolicy("TiMetadataWrite", policy => policy.RequireClaim(AppPermissions.ClaimType, AppPermissions.TiMetadataWrite));
            options.AddPolicy("AdminEntitiesRead", policy => policy.RequireClaim(AppPermissions.ClaimType, AppPermissions.AdminEntitiesRead));
            options.AddPolicy("AdminEntitiesWrite", policy => policy.RequireClaim(AppPermissions.ClaimType, AppPermissions.AdminEntitiesWrite));
            options.AddPolicy("AdminEntitiesDelete", policy => policy.RequireClaim(AppPermissions.ClaimType, AppPermissions.AdminEntitiesDelete));
            options.AddPolicy("AuditRead", policy => policy.RequireClaim(AppPermissions.ClaimType, AppPermissions.AuditRead));
            options.AddPolicy("ManagerApproval", policy => policy.RequireClaim(AppPermissions.ClaimType, AppPermissions.ManagerApproval));
            options.AddPolicy("CompensationRead", policy => policy.RequireClaim(AppPermissions.ClaimType, AppPermissions.CompensationRead));
            options.AddPolicy("CompensationRequestCreate", policy => policy.RequireClaim(AppPermissions.ClaimType, AppPermissions.CompensationRequestCreate));
        });

        return services;
    }
}
