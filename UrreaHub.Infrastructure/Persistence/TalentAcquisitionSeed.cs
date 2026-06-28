using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Requisiciones;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.Reclutamiento;
using UrreaHub.Domain.Requisiciones;

namespace UrreaHub.Infrastructure.Persistence;

public static class TalentAcquisitionSeed
{
    public static async Task SeedAsync(UrreaHubDbContext context, IRequisitionAdminService requisitionAdmin)
    {
        if (await context.RequisicionesPersonal.AnyAsync())
            return;

        var luis = await context.Colaboradores.FirstOrDefaultAsync(c => c.NumeroEmpleado == "1002");
        var patricia = await context.Colaboradores.FirstOrDefaultAsync(c => c.NumeroEmpleado == "1005");
        var depto = await context.Departamentos.FirstOrDefaultAsync();
        if (luis is null || patricia is null || depto is null) return;

        var now = DateTime.UtcNow;
        var reqId = Guid.NewGuid();

        var req = new RequisicionPersonal
        {
            Id = reqId,
            Folio = $"REQ-{now.Year}-0001",
            Titulo = "Analista de Compensaciones",
            VacantesSolicitadas = 1,
            Estado = EstadoSolicitud.Aprobada,
            FechaSolicitud = now.AddDays(-14),
            SolicitanteId = luis.Id,
            DepartamentoId = depto.Id,
            CreatedAt = now.AddDays(-14),
            IsActive = true,
            Justificacion = new JustificacionRequisicion
            {
                Id = Guid.NewGuid(), RequisicionId = reqId,
                Motivo = "Crecimiento del área de DH y proyectos de tabuladores",
                ImpactoNegocio = "Reducir tiempo de respuesta en ajustes salariales",
                CreatedAt = now, IsActive = true,
            },
            Presupuesto = new PresupuestoRequisicion
            {
                Id = Guid.NewGuid(), RequisicionId = reqId,
                MontoAutorizado = 420000, Moneda = "MXN", CentroCostoCodigo = "CC001",
                CreatedAt = now, IsActive = true,
            },
            Perfil = new PerfilRequisicion
            {
                Id = Guid.NewGuid(), RequisicionId = reqId,
                DescripcionPuesto = "Analista para administración de compensación referencial y beneficios",
                ExperienciaRequerida = "3+ años en nómina o compensaciones",
                EducacionRequerida = "Licenciatura en Administración, Contabilidad o afín",
                CreatedAt = now, IsActive = true,
            },
        };

        req.Aprobadores.Add(new AprobadorRequisicion
        {
            Id = Guid.NewGuid(), RequisicionId = reqId, AprobadorId = patricia.Id, Orden = 1,
            Decision = EstadoSolicitud.Aprobada, FechaDecision = now.AddDays(-10), CreatedAt = now, IsActive = true,
        });

        context.RequisicionesPersonal.Add(req);
        await context.SaveChangesAsync();

        var convert = await requisitionAdmin.ConvertToVacancyAsync(reqId, "seed");
        if (!convert.Success || convert.Data == Guid.Empty) return;

        var vacante = await context.VacantesReclutamiento.FirstAsync(v => v.Id == convert.Data);
        var candId = Guid.NewGuid();
        var postId = Guid.NewGuid();
        var cand = new Candidato
        {
            Id = candId, Nombre = "Elena", ApellidoPaterno = "Ríos", Email = "elena.rios@gmail.com",
            CreatedAt = now, IsActive = true,
        };
        var post = new Postulacion
        {
            Id = postId, VacanteId = vacante.Id, CandidatoId = candId,
            FechaPostulacion = now.AddDays(-5), Estado = EstadoPostulacion.Entrevista,
            CreatedAt = now, IsActive = true,
        };

        context.Candidatos.Add(cand);
        context.Postulaciones.Add(post);
        context.Entrevistas.Add(new Entrevista
        {
            Id = Guid.NewGuid(), PostulacionId = postId,
            FechaHora = now.AddDays(2), Tipo = "Presencial", CreatedAt = now, IsActive = true,
        });
        await context.SaveChangesAsync();
    }
}
