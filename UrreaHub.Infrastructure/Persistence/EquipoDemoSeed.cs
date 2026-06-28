using Microsoft.EntityFrameworkCore;
using UrreaHub.Domain.Capacitacion;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.Desempeno;

namespace UrreaHub.Infrastructure.Persistence;

public static class EquipoDemoSeed
{
    public static async Task SeedAsync(UrreaHubDbContext context)
    {
        if (await context.Cursos.AnyAsync())
            return;

        var jefe = await context.Colaboradores.FirstOrDefaultAsync(c => c.NumeroEmpleado == "1002");
        var maria = await context.Colaboradores.FirstOrDefaultAsync(c => c.NumeroEmpleado == "1003");
        var carlos = await context.Colaboradores.FirstOrDefaultAsync(c => c.NumeroEmpleado == "1004");
        if (jefe is null || maria is null || carlos is null)
            return;

        var now = DateTime.UtcNow;
        var cursoSeg = new Curso
        {
            Id = Guid.NewGuid(), Codigo = "SEG-001", Nombre = "Seguridad industrial URREA",
            Descripcion = "Normas de seguridad en planta", DuracionHoras = 8, Modalidad = "Presencial",
            CreatedAt = now, IsActive = true,
        };
        var cursoLider = new Curso
        {
            Id = Guid.NewGuid(), Codigo = "LID-101", Nombre = "Liderazgo colaborativo",
            Descripcion = "Habilidades de liderazgo para mandos medios", DuracionHoras = 16, Modalidad = "Híbrido",
            CreatedAt = now, IsActive = true,
        };
        var cursoExcel = new Curso
        {
            Id = Guid.NewGuid(), Codigo = "TEC-220", Nombre = "Excel avanzado para analistas",
            Descripcion = "Análisis de datos con Excel", DuracionHoras = 12, Modalidad = "En línea",
            CreatedAt = now, IsActive = true,
        };
        context.Cursos.AddRange(cursoSeg, cursoLider, cursoExcel);

        var insMaria1 = CreateInscripcion(maria.Id, cursoSeg.Id, now.AddMonths(-2), now.AddMonths(-2).AddDays(3), EstadoSolicitud.Aprobada, 92, true);
        var insMaria2 = CreateInscripcion(maria.Id, cursoExcel.Id, now.AddMonths(-1), null, EstadoSolicitud.Pendiente, null, null);
        var insCarlos1 = CreateInscripcion(carlos.Id, cursoSeg.Id, now.AddMonths(-3), now.AddMonths(-3).AddDays(2), EstadoSolicitud.Aprobada, 88, true);
        var insCarlos2 = CreateInscripcion(carlos.Id, cursoLider.Id, now.AddDays(-10), null, EstadoSolicitud.Pendiente, null, null);
        context.InscripcionesCurso.AddRange(insMaria1, insMaria2, insCarlos1, insCarlos2);

        context.PlanesAccion.AddRange(
            new PlanAccion
            {
                Id = Guid.NewGuid(), ColaboradorId = maria.Id, CreadoPorId = jefe.Id,
                Titulo = "Mejorar tiempos de respuesta en reportes",
                Descripcion = "Reducir el SLA de entrega de reportes semanales de 5 a 3 días.",
                FechaInicio = now.Date, FechaFin = now.Date.AddMonths(2),
                Estado = EstadoPlanAccion.EnProgreso, Avance = 35, Prioridad = "Alta",
                CreatedAt = now, IsActive = true,
            },
            new PlanAccion
            {
                Id = Guid.NewGuid(), ColaboradorId = carlos.Id, CreadoPorId = jefe.Id,
                Titulo = "Certificación en seguridad",
                Descripcion = "Completar curso SEG-001 y entregar evidencia.",
                FechaInicio = now.Date.AddDays(-15), FechaFin = now.Date.AddDays(30),
                Estado = EstadoPlanAccion.EnProgreso, Avance = 60, Prioridad = "Media",
                CreatedAt = now, IsActive = true,
            });

        context.FeedbacksEquipo.AddRange(
            new FeedbackEquipo
            {
                Id = Guid.NewGuid(), ColaboradorId = maria.Id, AutorId = jefe.Id,
                Tipo = "Reconocimiento", Comentario = "Excelente apoyo en el cierre del proyecto Q2.",
                Fecha = now.AddDays(-5), CreatedAt = now, IsActive = true,
            },
            new FeedbackEquipo
            {
                Id = Guid.NewGuid(), ColaboradorId = carlos.Id, AutorId = jefe.Id,
                Tipo = "Constructivo", Comentario = "Refuerza la puntualidad en las juntas de seguimiento semanal.",
                Fecha = now.AddDays(-2), CreatedAt = now, IsActive = true,
            });

        await context.SaveChangesAsync();
    }

    private static InscripcionCurso CreateInscripcion(
        Guid colaboradorId, Guid cursoId, DateTime inscripcion, DateTime? completado,
        EstadoSolicitud estado, decimal? score, bool? aprobado)
    {
        var ins = new InscripcionCurso
        {
            Id = Guid.NewGuid(),
            ColaboradorId = colaboradorId,
            CursoId = cursoId,
            FechaInscripcion = inscripcion,
            FechaCompletado = completado,
            Estado = estado,
            CreatedAt = inscripcion,
            IsActive = true,
        };

        if (score.HasValue && aprobado.HasValue)
        {
            ins.Evaluacion = new EvaluacionCapacitacion
            {
                Id = Guid.NewGuid(),
                InscripcionId = ins.Id,
                Puntuacion = score.Value,
                Aprobado = aprobado.Value,
                FechaEvaluacion = completado ?? inscripcion,
                CreatedAt = inscripcion,
                IsActive = true,
            };
        }

        return ins;
    }
}
