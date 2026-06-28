using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Common;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Application.Onboarding;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.Onboarding;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class OnboardingAdminService : IOnboardingAdminService
{
    private readonly UrreaHubDbContext _context;
    private readonly IAuditService _audit;

    private record PlantillaDef(string Codigo, string Nombre, string Descripcion, (string Titulo, string Rol, int DiasLimite)[] Tareas);

    private static readonly PlantillaDef[] Plantillas =
    {
        new("ADMIN", "Administrativo", "Incorporación estándar oficina corporativa", new[]
        {
            ("Documentos de ingreso", "RH", 2),
            ("Firma de contrato", "Legal", 4),
            ("Alta de accesos TI", "TI", 5),
            ("Inducción corporativa", "Capacitación", 7),
            ("Examen médico", "Salud", 3),
            ("Asignación de equipo", "Facilities", 5),
        }),
        new("PLANTA", "Planta / Operaciones", "Incorporación personal de manufactura", new[]
        {
            ("Documentos de ingreso", "RH", 2),
            ("Examen médico y aptitud", "Salud", 3),
            ("Curso seguridad e higiene", "Capacitación", 5),
            ("Entrega EPP", "Seguridad", 4),
            ("Asignación área y turno", "Jefe", 3),
            ("Alta biométrico", "RH", 6),
        }),
        new("COMERCIAL", "Fuerza de ventas", "Incorporación ejecutivos comerciales", new[]
        {
            ("Documentos de ingreso", "RH", 2),
            ("Firma de contrato", "Legal", 4),
            ("Alta CRM y accesos", "TI", 5),
            ("Capacitación producto", "Comercial", 10),
            ("Asignación territorio", "Jefe", 7),
            ("Vehículo / herramientas", "Operaciones", 8),
        }),
        new("PRACTICANTE", "Practicante / Becario", "Plan reducido para practicantes", new[]
        {
            ("Carta de prácticas", "RH", 3),
            ("Buddy asignado", "Jefe", 5),
            ("Inducción corporativa", "Capacitación", 7),
            ("Encuesta 30 días", "RH", 30),
        }),
    };

    public OnboardingAdminService(UrreaHubDbContext context, IAuditService audit)
    {
        _context = context;
        _audit = audit;
    }

    public async Task<OnboardingDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var plans = await _context.PlanesOnboarding.AsNoTracking()
            .Include(p => p.Colaborador)
            .Include(p => p.Tareas).ThenInclude(t => t.FechaCompromiso)
            .Where(p => p.IsActive)
            .ToListAsync(cancellationToken);

        var activos = plans.Count(p => p.Estado == EstadoSolicitud.Pendiente);
        var completados = plans.Count(p => p.Estado == EstadoSolicitud.Aprobada);
        var today = DateTime.UtcNow.Date;

        var allTasks = plans.SelectMany(p => p.Tareas.Where(t => t.IsActive)).ToList();
        var pendientes = allTasks.Count(t => !t.Completada);
        var vencidas = allTasks.Count(t => !t.Completada && t.FechaCompromiso != null && t.FechaCompromiso.FechaCompromiso.Date < today);
        var promedio = plans.Count == 0 ? 0 : (int)Math.Round(plans.Average(p => OnboardingService.MapResumen(p).AvancePorcentaje));

        var recientes = plans.OrderByDescending(p => p.FechaInicio).Take(8).Select(OnboardingService.MapResumen).ToList();

        return new OnboardingDashboardDto(activos, completados, pendientes, vencidas, promedio, recientes);
    }

    public async Task<IReadOnlyList<PlanOnboardingResumenDto>> ListPlansAsync(EstadoSolicitud? estado, CancellationToken cancellationToken = default)
    {
        var query = _context.PlanesOnboarding.AsNoTracking()
            .Include(p => p.Colaborador)
            .Include(p => p.Tareas).ThenInclude(t => t.FechaCompromiso)
            .Where(p => p.IsActive);

        if (estado.HasValue)
            query = query.Where(p => p.Estado == estado.Value);

        var plans = await query.OrderByDescending(p => p.FechaInicio).ToListAsync(cancellationToken);
        return plans.Select(OnboardingService.MapResumen).ToList();
    }

    public async Task<PlanOnboardingDto?> GetPlanAsync(Guid planId, CancellationToken cancellationToken = default)
    {
        var plan = await _context.PlanesOnboarding.AsNoTracking()
            .Include(p => p.Colaborador).ThenInclude(c => c.Puesto)
            .Include(p => p.Colaborador).ThenInclude(c => c.Departamento)
            .Include(p => p.Tareas).ThenInclude(t => t.Responsables).ThenInclude(r => r.Colaborador)
            .Include(p => p.Tareas).ThenInclude(t => t.FechaCompromiso)
            .Include(p => p.Tareas).ThenInclude(t => t.Evidencias)
            .Include(p => p.Checklists)
            .FirstOrDefaultAsync(p => p.Id == planId && p.IsActive, cancellationToken);

        return plan is null ? null : OnboardingService.MapPlan(plan);
    }

    public Task<IReadOnlyList<PlantillaOnboardingDto>> ListTemplatesAsync(CancellationToken cancellationToken = default)
    {
        var items = Plantillas.Select(p => new PlantillaOnboardingDto(
            p.Codigo,
            p.Nombre,
            p.Descripcion,
            p.Tareas.Length,
            p.Tareas.Select(t => t.Titulo).Take(4).ToList())).ToList();
        return Task.FromResult<IReadOnlyList<PlantillaOnboardingDto>>(items);
    }

    public async Task<Result<PlanOnboardingDto>> CreatePlanAsync(CrearPlanOnboardingDto dto, string performedBy, CancellationToken cancellationToken = default)
    {
        var plantilla = Plantillas.FirstOrDefault(p => p.Codigo.Equals(dto.PlantillaCodigo, StringComparison.OrdinalIgnoreCase));
        if (plantilla is null)
            return Result<PlanOnboardingDto>.Fail("Plantilla no encontrada.");

        var colaborador = await _context.Colaboradores
            .Include(c => c.JefeDirecto)
            .FirstOrDefaultAsync(c => c.Id == dto.ColaboradorId && c.IsActive, cancellationToken);
        if (colaborador is null)
            return Result<PlanOnboardingDto>.Fail("Colaborador no encontrado.");

        var activePlan = await _context.PlanesOnboarding.AnyAsync(p =>
            p.ColaboradorId == dto.ColaboradorId && p.IsActive
            && p.Estado != EstadoSolicitud.Aprobada && p.Estado != EstadoSolicitud.Cancelada, cancellationToken);
        if (activePlan)
            return Result<PlanOnboardingDto>.Fail("El colaborador ya tiene un plan de onboarding activo.");

        var rhAdmin = await _context.Colaboradores.FirstOrDefaultAsync(c => c.NumeroEmpleado == "1005", cancellationToken);
        var now = DateTime.UtcNow;
        var planId = Guid.NewGuid();

        var plan = new PlanOnboarding
        {
            Id = planId,
            ColaboradorId = colaborador.Id,
            Nombre = dto.Nombre ?? $"Onboarding — {plantilla.Nombre}",
            Descripcion = plantilla.Descripcion,
            FechaInicio = dto.FechaInicio.Date,
            Estado = EstadoSolicitud.Pendiente,
            CreatedAt = now,
            IsActive = true,
        };

        var orden = 1;
        foreach (var (titulo, rol, diasLimite) in plantilla.Tareas)
        {
            var tareaId = Guid.NewGuid();
            var responsableId = await ResolveResponsableAsync(rol, colaborador, rhAdmin?.Id, cancellationToken);

            var tarea = new TareaOnboarding
            {
                Id = tareaId,
                PlanId = planId,
                Titulo = titulo,
                Descripcion = $"Responsable: {rol}",
                Orden = orden++,
                Completada = false,
                CreatedAt = now,
                IsActive = true,
                FechaCompromiso = new FechaCompromisoOnboarding
                {
                    Id = Guid.NewGuid(),
                    TareaId = tareaId,
                    FechaCompromiso = dto.FechaInicio.Date.AddDays(diasLimite),
                    CreatedAt = now,
                    IsActive = true,
                },
            };

            if (responsableId.HasValue)
            {
                tarea.Responsables.Add(new ResponsableOnboarding
                {
                    Id = Guid.NewGuid(),
                    TareaId = tareaId,
                    ColaboradorId = responsableId.Value,
                    Rol = rol,
                    CreatedAt = now,
                    IsActive = true,
                });
            }

            plan.Tareas.Add(tarea);
            plan.Checklists.Add(new ChecklistOnboarding
            {
                Id = Guid.NewGuid(),
                PlanId = planId,
                Item = titulo,
                Completado = false,
                CreatedAt = now,
                IsActive = true,
            });
        }

        _context.PlanesOnboarding.Add(plan);
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Onboarding", "CrearPlan", "PlanOnboarding", planId, performedBy, plantilla.Codigo, cancellationToken);

        var created = await GetPlanAsync(planId, cancellationToken);
        return Result<PlanOnboardingDto>.Ok(created!);
    }

    private async Task<Guid?> ResolveResponsableAsync(string rol, Domain.CoreRH.Colaborador colaborador, Guid? rhAdminId, CancellationToken cancellationToken)
    {
        return rol switch
        {
            "Jefe" => colaborador.JefeDirectoId ?? rhAdminId,
            "RH" or "Legal" or "Capacitación" or "Salud" or "Seguridad" => rhAdminId,
            "TI" => await _context.Colaboradores.Where(c => c.IsActive && c.NumeroEmpleado == "1005").Select(c => (Guid?)c.Id).FirstOrDefaultAsync(cancellationToken) ?? rhAdminId,
            _ => rhAdminId,
        };
    }
}
