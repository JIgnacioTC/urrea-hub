using UrreaHub.Application.Common;
using UrreaHub.Domain.Common;

namespace UrreaHub.Application.Onboarding;

public record TareaOnboardingDto(
    Guid Id,
    string Titulo,
    string? Descripcion,
    int Orden,
    bool Completada,
    DateTime? FechaCompromiso,
    bool Vencida,
    IReadOnlyList<ResponsableOnboardingDto> Responsables,
    int EvidenciasCount);

public record ResponsableOnboardingDto(Guid ColaboradorId, string Nombre, string Rol);

public record ChecklistItemDto(Guid Id, string Item, bool Completado, DateTime? FechaCompletado);

public record PlanOnboardingDto(
    Guid Id,
    Guid ColaboradorId,
    string ColaboradorNombre,
    string? Puesto,
    string? Departamento,
    string Nombre,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime? FechaFin,
    string Estado,
    int AvancePorcentaje,
    int TareasPendientes,
    int TareasVencidas,
    IReadOnlyList<TareaOnboardingDto> Tareas,
    IReadOnlyList<ChecklistItemDto> Checklist);

public record OnboardingSummaryDto(
    PlanOnboardingDto? PlanActivo,
    int TareasPendientes,
    int TareasCompletadas,
    int TareasVencidas);

public record PlantillaOnboardingDto(
    string Codigo,
    string Nombre,
    string Descripcion,
    int TareasCount,
    IReadOnlyList<string> TareasPreview);

public record CrearPlanOnboardingDto(
    Guid ColaboradorId,
    string PlantillaCodigo,
    DateTime FechaInicio,
    string? Nombre);

public record CompletarTareaDto(string? Comentario, string? EvidenciaNombre);

public record OnboardingDashboardDto(
    int PlanesActivos,
    int PlanesCompletados,
    int TareasPendientes,
    int TareasVencidas,
    int PromedioAvance,
    IReadOnlyList<PlanOnboardingResumenDto> PlanesRecientes);

public record PlanOnboardingResumenDto(
    Guid Id,
    string ColaboradorNombre,
    string Nombre,
    string Estado,
    int AvancePorcentaje,
    DateTime FechaInicio,
    int TareasVencidas);

public interface IOnboardingService
{
    Task<OnboardingSummaryDto> GetMySummaryAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
    Task<PlanOnboardingDto?> GetMyPlanAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
    Task<Result<TareaOnboardingDto>> CompleteTaskAsync(Guid colaboradorId, Guid taskId, CompletarTareaDto dto, bool isRhAdmin, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PlanOnboardingResumenDto>> GetTeamPlansAsync(Guid jefeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TareaOnboardingDto>> GetTeamPendingTasksAsync(Guid jefeId, CancellationToken cancellationToken = default);
}

public interface IOnboardingAdminService
{
    Task<OnboardingDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PlanOnboardingResumenDto>> ListPlansAsync(EstadoSolicitud? estado, CancellationToken cancellationToken = default);
    Task<PlanOnboardingDto?> GetPlanAsync(Guid planId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PlantillaOnboardingDto>> ListTemplatesAsync(CancellationToken cancellationToken = default);
    Task<Result<PlanOnboardingDto>> CreatePlanAsync(CrearPlanOnboardingDto dto, string performedBy, CancellationToken cancellationToken = default);
}
