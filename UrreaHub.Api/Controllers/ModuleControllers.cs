using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrreaHub.Api.Routing;
using UrreaHub.Domain.Organizacion;
using UrreaHub.Domain.Vacaciones;
using UrreaHub.Domain.Requisiciones;
using UrreaHub.Domain.Reclutamiento;
using UrreaHub.Domain.Onboarding;
using UrreaHub.Domain.Documentos;
using UrreaHub.Domain.Desempeno;
using UrreaHub.Domain.Capacitacion;
using UrreaHub.Domain.Beneficios;
using UrreaHub.Domain.Auditoria;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Api.Controllers;

[Route("api/organizacion/[controller]")]
public class OrganigramasController : CrudControllerBase<Organigrama>
{
    public OrganigramasController(UrreaHubDbContext context) : base(context) { }
}

[Route("api/v1/absence/types")]
[Route("api/vacaciones/[controller]")]
public class TiposAusenciaController : CrudControllerBase<TipoAusencia>
{
    public TiposAusenciaController(UrreaHubDbContext context) : base(context) { }

    [AllowAnonymous]
    public override Task<ActionResult<IEnumerable<TipoAusencia>>> GetAll(CancellationToken cancellationToken)
        => base.GetAll(cancellationToken);
}

// SolicitudesAusencia CRUD removed — use /api/vacaciones/solicitudes workflow endpoints

[Route("api/requisiciones/[controller]")]
public class RequisicionesPersonalController : CrudControllerBase<RequisicionPersonal>
{
    public RequisicionesPersonalController(UrreaHubDbContext context) : base(context) { }
}

[Route("api/reclutamiento/[controller]")]
public class VacantesReclutamientoController : CrudControllerBase<VacanteReclutamiento>
{
    public VacantesReclutamientoController(UrreaHubDbContext context) : base(context) { }
}

[Route("api/reclutamiento/[controller]")]
public class CandidatosController : CrudControllerBase<Candidato>
{
    public CandidatosController(UrreaHubDbContext context) : base(context) { }
}

[Route("api/onboarding/[controller]")]
public class PlanesOnboardingController : CrudControllerBase<PlanOnboarding>
{
    public PlanesOnboardingController(UrreaHubDbContext context) : base(context) { }
}

[Route("api/documentos/[controller]")]
public class ExpedientesController : CrudControllerBase<Expediente>
{
    public ExpedientesController(UrreaHubDbContext context) : base(context) { }
}

[Route("api/desempeno/[controller]")]
public class CiclosDesempenoController : CrudControllerBase<CicloDesempeno>
{
    public CiclosDesempenoController(UrreaHubDbContext context) : base(context) { }
}

[Route("api/capacitacion/[controller]")]
public class CursosController : CrudControllerBase<Curso>
{
    public CursosController(UrreaHubDbContext context) : base(context) { }
}

[Route("api/beneficios/[controller]")]
public class BeneficiosController : CrudControllerBase<Beneficio>
{
    public BeneficiosController(UrreaHubDbContext context) : base(context) { }
}

[Route("api/auditoria/[controller]")]
public class BitacoraEventosController : CrudControllerBase<BitacoraEvento>
{
    public BitacoraEventosController(UrreaHubDbContext context) : base(context) { }
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "TiMetadataRead")]
public class ModulosController : ControllerBase
{
    [HttpGet]
    public IActionResult GetModulos()
    {
        var modulos = new[]
        {
            new { id = "core-rh", nombre = "Core RH", ruta = "/core-rh", tablas = new[] { "Colaborador", "Puesto", "Área", "Departamento", "Sede", "Centro de costo", "Relación laboral" } },
            new { id = "organizacion", nombre = "Organización", ruta = "/organizacion", tablas = new[] { "Organigrama", "Posición", "Vacante", "Movimiento organizacional" } },
            new { id = "vacaciones", nombre = "Vacaciones/Permisos", ruta = "/vacaciones", tablas = new[] { "Política", "Saldo", "Solicitud", "Tipo de ausencia", "Calendario laboral", "Días inhábiles", "Aprobación" } },
            new { id = "requisiciones", nombre = "Requisiciones", ruta = "/requisiciones", tablas = new[] { "Requisición de personal", "Justificación", "Presupuesto", "Perfil", "Aprobadores", "Historial" } },
            new { id = "reclutamiento", nombre = "Reclutamiento", ruta = "/reclutamiento", tablas = new[] { "Vacante", "Publicación", "Candidato", "Postulación", "CV", "Entrevista", "Evaluación", "Oferta" } },
            new { id = "onboarding", nombre = "Onboarding", ruta = "/onboarding", tablas = new[] { "Plan onboarding", "Tarea onboarding", "Responsable", "Evidencia", "Checklist", "Fecha compromiso" } },
            new { id = "documentos", nombre = "Documentos", ruta = "/documentos", tablas = new[] { "Expediente", "Documento", "Tipo documento", "Vigencia", "Firma", "Versión", "Confidencialidad" } },
            new { id = "desempeno", nombre = "Desempeño", ruta = "/desempeno", tablas = new[] { "Ciclo", "Objetivo", "Competencia", "Evaluación", "Feedback", "Resultado" } },
            new { id = "capacitacion", nombre = "Capacitación", ruta = "/capacitacion", tablas = new[] { "Curso", "Inscripción", "Evidencia", "Evaluación", "Constancia" } },
            new { id = "beneficios", nombre = "Beneficios", ruta = "/beneficios", tablas = new[] { "Beneficio", "Solicitud beneficio", "Elegibilidad", "Aprobación" } },
            new { id = "auditoria", nombre = "Auditoría", ruta = "/auditoria", tablas = new[] { "Bitácora evento", "Cambio estado", "Notificación enviada", "Integración", "Error integración" } }
        };

        return Ok(modulos);
    }
}

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class HealthController : ControllerBase
{
    [HttpGet]
    [HttpGet("/health/live")]
    public IActionResult Get() => Ok(new { status = "healthy", app = "URREA Hub", version = "1.0.0-mvp" });

    [HttpGet("ready")]
    [HttpGet("/health/ready")]
    public IActionResult Ready() => Ok(new { status = "ready", app = "URREA Hub" });
}

[ApiController]
[Route("api/v1/health")]
[Authorize(Policy = "TiMetadataRead")]
public class HealthDetailsController : ControllerBase
{
    [HttpGet("details")]
    public IActionResult Details() => Ok(new
    {
        status = "healthy",
        app = "URREA Hub",
        version = "1.0.0-mvp",
        environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
    });
}
