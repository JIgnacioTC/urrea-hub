using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Common;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Application.Onboarding;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.Onboarding;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class OnboardingService : IOnboardingService
{
    private readonly UrreaHubDbContext _context;
    private readonly IAuditService _audit;

    public OnboardingService(UrreaHubDbContext context, IAuditService audit)
    {
        _context = context;
        _audit = audit;
    }

    public async Task<OnboardingSummaryDto> GetMySummaryAsync(Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        var plan = await LoadActivePlanQuery(colaboradorId).FirstOrDefaultAsync(cancellationToken);
        if (plan is null)
            return new OnboardingSummaryDto(null, 0, 0, 0);

        var dto = MapPlan(plan);
        return new OnboardingSummaryDto(
            dto,
            dto.Tareas.Count(t => !t.Completada),
            dto.Tareas.Count(t => t.Completada),
            dto.TareasVencidas);
    }

    public async Task<PlanOnboardingDto?> GetMyPlanAsync(Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        var plan = await LoadActivePlanQuery(colaboradorId).FirstOrDefaultAsync(cancellationToken);
        return plan is null ? null : MapPlan(plan);
    }

    public async Task<Result<TareaOnboardingDto>> CompleteTaskAsync(
        Guid colaboradorId, Guid taskId, CompletarTareaDto dto, bool isRhAdmin, CancellationToken cancellationToken = default)
    {
        var tarea = await _context.TareasOnboarding
            .Include(t => t.Plan).ThenInclude(p => p.Colaborador)
            .Include(t => t.Responsables).ThenInclude(r => r.Colaborador)
            .Include(t => t.FechaCompromiso)
            .Include(t => t.Evidencias)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.IsActive, cancellationToken);

        if (tarea is null)
            return Result<TareaOnboardingDto>.Fail("Tarea no encontrada.");

        if (tarea.Completada)
            return Result<TareaOnboardingDto>.Fail("La tarea ya está completada.");

        var canComplete = isRhAdmin
            || tarea.Plan.ColaboradorId == colaboradorId
            || tarea.Responsables.Any(r => r.ColaboradorId == colaboradorId && r.IsActive);

        if (!canComplete)
            return Result<TareaOnboardingDto>.Fail("No tienes permiso para completar esta tarea.");

        var now = DateTime.UtcNow;
        tarea.Completada = true;
        tarea.UpdatedAt = now;

        if (!string.IsNullOrWhiteSpace(dto.EvidenciaNombre))
        {
            tarea.Evidencias.Add(new EvidenciaOnboarding
            {
                Id = Guid.NewGuid(),
                TareaId = tarea.Id,
                NombreArchivo = dto.EvidenciaNombre!,
                RutaArchivo = $"/evidencias/onboarding/{tarea.Id}/{dto.EvidenciaNombre}",
                FechaCarga = now,
                Comentario = dto.Comentario,
                CreatedAt = now,
                IsActive = true,
            });
        }

        if (tarea.FechaCompromiso is not null)
            tarea.FechaCompromiso.FechaCumplimiento = now;

        await SyncChecklistForTaskAsync(tarea.PlanId, tarea.Titulo, now, cancellationToken);
        await TryCompletePlanAsync(tarea.Plan, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Onboarding", "CompletarTarea", "TareaOnboarding", tarea.Id, colaboradorId.ToString(), dto.Comentario, cancellationToken);

        return Result<TareaOnboardingDto>.Ok(MapTarea(tarea));
    }

    public async Task<IReadOnlyList<PlanOnboardingResumenDto>> GetTeamPlansAsync(Guid jefeId, CancellationToken cancellationToken = default)
    {
        var subordinados = await _context.Colaboradores.AsNoTracking()
            .Where(c => c.JefeDirectoId == jefeId && c.IsActive)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var plans = await _context.PlanesOnboarding.AsNoTracking()
            .Include(p => p.Colaborador)
            .Include(p => p.Tareas).ThenInclude(t => t.FechaCompromiso)
            .Where(p => p.IsActive && subordinados.Contains(p.ColaboradorId))
            .OrderByDescending(p => p.FechaInicio)
            .ToListAsync(cancellationToken);

        return plans.Select(MapResumen).ToList();
    }

    public async Task<IReadOnlyList<TareaOnboardingDto>> GetTeamPendingTasksAsync(Guid jefeId, CancellationToken cancellationToken = default)
    {
        var subordinados = await _context.Colaboradores.AsNoTracking()
            .Where(c => c.JefeDirectoId == jefeId && c.IsActive)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        var tareas = await _context.TareasOnboarding.AsNoTracking()
            .Include(t => t.Responsables).ThenInclude(r => r.Colaborador)
            .Include(t => t.FechaCompromiso)
            .Include(t => t.Evidencias)
            .Include(t => t.Plan)
            .Where(t => t.IsActive && !t.Completada && t.Plan.IsActive
                && (subordinados.Contains(t.Plan.ColaboradorId) || t.Responsables.Any(r => r.ColaboradorId == jefeId)))
            .OrderBy(t => t.Plan.FechaInicio).ThenBy(t => t.Orden)
            .ToListAsync(cancellationToken);

        return tareas.Select(MapTarea).ToList();
    }

    private IQueryable<PlanOnboarding> LoadActivePlanQuery(Guid colaboradorId) =>
        _context.PlanesOnboarding.AsNoTracking()
            .Include(p => p.Colaborador).ThenInclude(c => c.Puesto)
            .Include(p => p.Colaborador).ThenInclude(c => c.Departamento)
            .Include(p => p.Tareas).ThenInclude(t => t.Responsables).ThenInclude(r => r.Colaborador)
            .Include(p => p.Tareas).ThenInclude(t => t.FechaCompromiso)
            .Include(p => p.Tareas).ThenInclude(t => t.Evidencias)
            .Include(p => p.Checklists)
            .Where(p => p.ColaboradorId == colaboradorId && p.IsActive
                && p.Estado != EstadoSolicitud.Aprobada && p.Estado != EstadoSolicitud.Cancelada)
            .OrderByDescending(p => p.FechaInicio);

    private async Task SyncChecklistForTaskAsync(Guid planId, string tituloTarea, DateTime now, CancellationToken cancellationToken)
    {
        var item = await _context.ChecklistsOnboarding
            .FirstOrDefaultAsync(c => c.PlanId == planId && c.Item == tituloTarea && c.IsActive, cancellationToken);
        if (item is null) return;
        item.Completado = true;
        item.FechaCompletado = now;
        item.UpdatedAt = now;
    }

    private async Task TryCompletePlanAsync(PlanOnboarding plan, CancellationToken cancellationToken)
    {
        var tareas = await _context.TareasOnboarding
            .Where(t => t.PlanId == plan.Id && t.IsActive)
            .ToListAsync(cancellationToken);

        if (tareas.Count == 0 || tareas.Any(t => !t.Completada)) return;

        plan.Estado = EstadoSolicitud.Aprobada;
        plan.FechaFin = DateTime.UtcNow;
        plan.UpdatedAt = DateTime.UtcNow;
    }

    internal static PlanOnboardingDto MapPlan(PlanOnboarding p)
    {
        var tareas = p.Tareas.Where(t => t.IsActive).OrderBy(t => t.Orden).Select(MapTarea).ToList();
        var total = tareas.Count;
        var done = tareas.Count(t => t.Completada);
        var avance = total == 0 ? 0 : (int)Math.Round(done * 100.0 / total);
        var vencidas = tareas.Count(t => t.Vencida);

        return new PlanOnboardingDto(
            p.Id,
            p.ColaboradorId,
            $"{p.Colaborador.Nombre} {p.Colaborador.ApellidoPaterno}",
            p.Colaborador.Puesto?.Nombre,
            p.Colaborador.Departamento?.Nombre,
            p.Nombre,
            p.Descripcion,
            p.FechaInicio,
            p.FechaFin,
            p.Estado.ToString(),
            avance,
            total - done,
            vencidas,
            tareas,
            p.Checklists.Where(c => c.IsActive).Select(c => new ChecklistItemDto(c.Id, c.Item, c.Completado, c.FechaCompletado)).ToList());
    }

    internal static PlanOnboardingResumenDto MapResumen(PlanOnboarding p)
    {
        var tareas = p.Tareas.Where(t => t.IsActive).ToList();
        var total = tareas.Count;
        var done = tareas.Count(t => t.Completada);
        var avance = total == 0 ? 0 : (int)Math.Round(done * 100.0 / total);
        var today = DateTime.UtcNow.Date;
        var vencidas = tareas.Count(t => !t.Completada && t.FechaCompromiso != null && t.FechaCompromiso.FechaCompromiso.Date < today);

        return new PlanOnboardingResumenDto(
            p.Id,
            $"{p.Colaborador.Nombre} {p.Colaborador.ApellidoPaterno}",
            p.Nombre,
            p.Estado.ToString(),
            avance,
            p.FechaInicio,
            vencidas);
    }

    internal static TareaOnboardingDto MapTarea(TareaOnboarding t)
    {
        var today = DateTime.UtcNow.Date;
        var vencida = !t.Completada && t.FechaCompromiso != null && t.FechaCompromiso.FechaCompromiso.Date < today;
        return new TareaOnboardingDto(
            t.Id,
            t.Titulo,
            t.Descripcion,
            t.Orden,
            t.Completada,
            t.FechaCompromiso?.FechaCompromiso,
            vencida,
            t.Responsables.Where(r => r.IsActive).Select(r => new ResponsableOnboardingDto(
                r.ColaboradorId,
                $"{r.Colaborador.Nombre} {r.Colaborador.ApellidoPaterno}",
                r.Rol)).ToList(),
            t.Evidencias.Count(e => e.IsActive));
    }
}
