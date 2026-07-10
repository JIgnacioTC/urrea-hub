using Microsoft.EntityFrameworkCore;
using UrreaHub.Domain.Asistencia;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Persistence;

public static class AttendanceSeed
{
    public static async Task SeedAsync(UrreaHubDbContext context)
    {
        var shiftsToSeed = new List<Turno>
        {
            new Turno { Codigo = "ADMIN_730", Nombre = "Administrativo 7:30 - 17:00", HoraEntrada = new TimeSpan(7, 30, 0), HoraSalida = new TimeSpan(17, 0, 0), MinutosToleranciaEntrada = 10, MinutosToleranciaSalida = 10, MinutosComida = 60, AplicaLunes = true, AplicaMartes = true, AplicaMiercoles = true, AplicaJueves = true, AplicaViernes = true, AplicaSabado = false, AplicaDomingo = false, IsActive = true },
            new Turno { Codigo = "ADMIN_800", Nombre = "Administrativo 8:00 - 17:30", HoraEntrada = new TimeSpan(8, 0, 0), HoraSalida = new TimeSpan(17, 30, 0), MinutosToleranciaEntrada = 10, MinutosToleranciaSalida = 10, MinutosComida = 60, AplicaLunes = true, AplicaMartes = true, AplicaMiercoles = true, AplicaJueves = true, AplicaViernes = true, AplicaSabado = false, AplicaDomingo = false, IsActive = true },
            new Turno { Codigo = "ADMIN_830", Nombre = "Administrativo 8:30 - 18:00", HoraEntrada = new TimeSpan(8, 30, 0), HoraSalida = new TimeSpan(18, 0, 0), MinutosToleranciaEntrada = 10, MinutosToleranciaSalida = 10, MinutosComida = 60, AplicaLunes = true, AplicaMartes = true, AplicaMiercoles = true, AplicaJueves = true, AplicaViernes = true, AplicaSabado = false, AplicaDomingo = false, IsActive = true },
            new Turno { Codigo = "ADMIN_900", Nombre = "Administrativo 9:00 - 18:30", HoraEntrada = new TimeSpan(9, 0, 0), HoraSalida = new TimeSpan(18, 30, 0), MinutosToleranciaEntrada = 10, MinutosToleranciaSalida = 10, MinutosComida = 60, AplicaLunes = true, AplicaMartes = true, AplicaMiercoles = true, AplicaJueves = true, AplicaViernes = true, AplicaSabado = false, AplicaDomingo = false, IsActive = true },
            new Turno { Codigo = "ADMIN_930", Nombre = "Administrativo 9:30 - 19:00", HoraEntrada = new TimeSpan(9, 30, 0), HoraSalida = new TimeSpan(19, 0, 0), MinutosToleranciaEntrada = 10, MinutosToleranciaSalida = 10, MinutosComida = 60, AplicaLunes = true, AplicaMartes = true, AplicaMiercoles = true, AplicaJueves = true, AplicaViernes = true, AplicaSabado = false, AplicaDomingo = false, IsActive = true },
            new Turno { Codigo = "OP_VESPERTINO", Nombre = "Operador Vespertino (14:00 - 22:00)", HoraEntrada = new TimeSpan(14, 0, 0), HoraSalida = new TimeSpan(22, 0, 0), MinutosToleranciaEntrada = 15, MinutosToleranciaSalida = 10, MinutosComida = 30, AplicaLunes = true, AplicaMartes = true, AplicaMiercoles = true, AplicaJueves = true, AplicaViernes = true, AplicaSabado = true, AplicaDomingo = false, IsActive = true },
            new Turno { Codigo = "OP_NOCTURNO", Nombre = "Operador Nocturno (22:00 - 06:00)", HoraEntrada = new TimeSpan(22, 0, 0), HoraSalida = new TimeSpan(6, 0, 0), MinutosToleranciaEntrada = 15, MinutosToleranciaSalida = 10, MinutosComida = 30, AplicaLunes = true, AplicaMartes = true, AplicaMiercoles = true, AplicaJueves = true, AplicaViernes = true, AplicaSabado = false, AplicaDomingo = false, IsActive = true },
            new Turno { Codigo = "OP_FIN_SEMANA", Nombre = "Operador Fin de Semana (08:00 - 20:00)", HoraEntrada = new TimeSpan(8, 0, 0), HoraSalida = new TimeSpan(20, 0, 0), MinutosToleranciaEntrada = 15, MinutosToleranciaSalida = 15, MinutosComida = 60, AplicaLunes = false, AplicaMartes = false, AplicaMiercoles = false, AplicaJueves = false, AplicaViernes = false, AplicaSabado = true, AplicaDomingo = true, IsActive = true }
        };

        Turno? defaultTurno = null;
        foreach (var t in shiftsToSeed)
        {
            var existing = await context.Turnos.FirstOrDefaultAsync(x => x.Codigo == t.Codigo);
            if (existing == null)
            {
                t.Id = Guid.NewGuid();
                t.CreatedAt = DateTime.UtcNow;
                context.Turnos.Add(t);
                if (t.Codigo == "ADMIN_830") defaultTurno = t;
            }
            else if (t.Codigo == "ADMIN_830")
            {
                defaultTurno = existing;
            }
        }

        await context.SaveChangesAsync();

        if (defaultTurno == null)
        {
            defaultTurno = await context.Turnos.FirstOrDefaultAsync(t => t.Codigo == "ADMIN_830" || t.Codigo == "ADMIN");
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
                TurnoId = defaultTurno!.Id,
                FechaInicio = new DateTime(2026, 1, 1),
                Origen = "Seed",
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            });
        }

        await context.SaveChangesAsync();
    }
}
