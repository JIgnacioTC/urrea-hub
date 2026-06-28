using Microsoft.EntityFrameworkCore;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.Onboarding;

namespace UrreaHub.Infrastructure.Persistence;

public static class OnboardingSeed
{
    public static async Task SeedAsync(UrreaHubDbContext context)
    {
        if (await context.PlanesOnboarding.AnyAsync())
            return;

        var maria = await context.Colaboradores.FirstOrDefaultAsync(c => c.NumeroEmpleado == "1003");
        var carlos = await context.Colaboradores.FirstOrDefaultAsync(c => c.NumeroEmpleado == "1004");
        var patricia = await context.Colaboradores.FirstOrDefaultAsync(c => c.NumeroEmpleado == "1005");
        var luis = await context.Colaboradores.FirstOrDefaultAsync(c => c.NumeroEmpleado == "1002");
        if (maria is null || patricia is null) return;

        await CreateDemoPlan(context, maria.Id, patricia.Id, luis?.Id, "Onboarding — Administrativo", new DateTime(2026, 6, 10), new[]
        {
            ("Documentos de ingreso", patricia.Id, "RH", 2, true),
            ("Firma de contrato", patricia.Id, "Legal", 4, true),
            ("Alta de accesos TI", patricia.Id, "TI", 5, false),
            ("Inducción corporativa", patricia.Id, "Capacitación", 7, false),
            ("Examen médico", patricia.Id, "Salud", 3, false),
        });

        if (carlos is not null && luis is not null)
        {
            await CreateDemoPlan(context, carlos.Id, patricia.Id, luis.Id, "Onboarding — Planta", new DateTime(2026, 6, 16), new[]
            {
                ("Documentos de ingreso", patricia.Id, "RH", 2, true),
                ("Examen médico y aptitud", patricia.Id, "Salud", 3, false),
                ("Curso seguridad e higiene", patricia.Id, "Capacitación", 5, false),
                ("Entrega EPP", luis.Id, "Jefe", 4, false),
            });
        }

        await context.SaveChangesAsync();
    }

    private static Task CreateDemoPlan(
        UrreaHubDbContext context,
        Guid colaboradorId,
        Guid rhId,
        Guid? jefeId,
        string nombre,
        DateTime fechaInicio,
        (string Titulo, Guid ResponsableId, string Rol, int DiasLimite, bool Completada)[] tareas)
    {
        var now = DateTime.UtcNow;
        var planId = Guid.NewGuid();
        var plan = new PlanOnboarding
        {
            Id = planId,
            ColaboradorId = colaboradorId,
            Nombre = nombre,
            Descripcion = "Plan demo Fase 4 — incorporación",
            FechaInicio = fechaInicio,
            Estado = EstadoSolicitud.Pendiente,
            CreatedAt = now,
            IsActive = true,
        };

        var orden = 1;
        foreach (var (titulo, responsableId, rol, diasLimite, completada) in tareas)
        {
            var tareaId = Guid.NewGuid();
            var tarea = new TareaOnboarding
            {
                Id = tareaId,
                PlanId = planId,
                Titulo = titulo,
                Orden = orden++,
                Completada = completada,
                CreatedAt = now,
                IsActive = true,
                FechaCompromiso = new FechaCompromisoOnboarding
                {
                    Id = Guid.NewGuid(),
                    TareaId = tareaId,
                    FechaCompromiso = fechaInicio.AddDays(diasLimite),
                    FechaCumplimiento = completada ? fechaInicio.AddDays(1) : null,
                    CreatedAt = now,
                    IsActive = true,
                },
            };
            tarea.Responsables.Add(new ResponsableOnboarding
            {
                Id = Guid.NewGuid(),
                TareaId = tareaId,
                ColaboradorId = rol == "Jefe" && jefeId.HasValue ? jefeId.Value : responsableId,
                Rol = rol,
                CreatedAt = now,
                IsActive = true,
            });
            plan.Tareas.Add(tarea);
            plan.Checklists.Add(new ChecklistOnboarding
            {
                Id = Guid.NewGuid(),
                PlanId = planId,
                Item = titulo,
                Completado = completada,
                FechaCompletado = completada ? fechaInicio.AddDays(1) : null,
                CreatedAt = now,
                IsActive = true,
            });
        }

        context.PlanesOnboarding.Add(plan);
        return Task.CompletedTask;
    }
}
