using Microsoft.EntityFrameworkCore;
using UrreaHub.Domain.Asistencia;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Persistence;

public static class AttendanceSeed
{
    public static async Task SeedAsync(UrreaHubDbContext context)
    {
        Turno? turno = null;
        if (!await context.Turnos.AnyAsync())
        {
            turno = new Turno
            {
                Id = Guid.NewGuid(),
                Codigo = "ADMIN",
                Nombre = "Turno Administrativo",
                HoraEntrada = new TimeSpan(8, 30, 0),
                HoraSalida = new TimeSpan(18, 0, 0),
                MinutosToleranciaEntrada = 10,
                MinutosToleranciaSalida = 10,
                MinutosComida = 60,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };
            context.Turnos.Add(turno);
        }
        else
        {
            turno = await context.Turnos.FirstAsync(t => t.IsActive);
        }

        if (!await context.ReglasAsistencia.AnyAsync())
        {
            context.ReglasAsistencia.Add(new ReglasAsistencia
            {
                Id = Guid.NewGuid(),
                MinutosToleranciaRetardo = 10,
                MinutosParaFalta = 60,
                GeneraIncidenciaNominaRetardo = true,
                RequiereValidacionLider = true,
                PermitirRegistroMovil = true,
                RequiereGeolocalizacion = false,
                RadioMetrosSede = 500,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            });
        }

        var colaboradores = await context.Colaboradores.Where(c => c.IsActive).Select(c => c.Id).ToListAsync();
        foreach (var colaboradorId in colaboradores)
        {
            var hasAssignment = await context.AsignacionesTurno.AnyAsync(a =>
                a.ColaboradorId == colaboradorId && a.IsActive);
            if (hasAssignment) continue;

            context.AsignacionesTurno.Add(new AsignacionTurno
            {
                Id = Guid.NewGuid(),
                ColaboradorId = colaboradorId,
                TurnoId = turno!.Id,
                FechaInicio = new DateTime(2026, 1, 1),
                Origen = "Seed",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            });
        }

        await context.SaveChangesAsync();
    }
}
