using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrreaHub.Domain.Auditoria;
using UrreaHub.Domain.Beneficios;
using UrreaHub.Domain.Capacitacion;
using UrreaHub.Domain.Desempeno;
using UrreaHub.Domain.Documentos;
using UrreaHub.Domain.Onboarding;
using UrreaHub.Domain.Organizacion;
using UrreaHub.Domain.Portal;
using UrreaHub.Domain.Reclutamiento;
using UrreaHub.Domain.Requisiciones;
using UrreaHub.Domain.Vacaciones;

namespace UrreaHub.Infrastructure.Persistence.Configurations;

public class OrganizacionConfigurations :
    IEntityTypeConfiguration<Organigrama>,
    IEntityTypeConfiguration<PosicionOrganizacional>,
    IEntityTypeConfiguration<VacanteOrganizacional>,
    IEntityTypeConfiguration<MovimientoOrganizacional>
{
    public void Configure(EntityTypeBuilder<Organigrama> builder) => builder.ToTable("Organigramas", "Organizacion");
    public void Configure(EntityTypeBuilder<PosicionOrganizacional> builder) => builder.ToTable("Posiciones", "Organizacion");
    public void Configure(EntityTypeBuilder<VacanteOrganizacional> builder) => builder.ToTable("Vacantes", "Organizacion");
    public void Configure(EntityTypeBuilder<MovimientoOrganizacional> builder) => builder.ToTable("MovimientosOrganizacionales", "Organizacion");
}

public class VacacionesConfigurations :
    IEntityTypeConfiguration<PoliticaVacaciones>,
    IEntityTypeConfiguration<TipoAusencia>,
    IEntityTypeConfiguration<SaldoVacaciones>,
    IEntityTypeConfiguration<CalendarioLaboral>,
    IEntityTypeConfiguration<DiaInhabil>,
    IEntityTypeConfiguration<SolicitudAusencia>,
    IEntityTypeConfiguration<AprobacionAusencia>,
    IEntityTypeConfiguration<AjusteSaldo>,
    IEntityTypeConfiguration<IncidenciaNomina>
{
    public void Configure(EntityTypeBuilder<PoliticaVacaciones> builder) => builder.ToTable("Politicas", "Vacaciones");
    public void Configure(EntityTypeBuilder<TipoAusencia> builder) => builder.ToTable("TiposAusencia", "Vacaciones");
    public void Configure(EntityTypeBuilder<SaldoVacaciones> builder) => builder.ToTable("Saldos", "Vacaciones");
    public void Configure(EntityTypeBuilder<CalendarioLaboral> builder) => builder.ToTable("CalendariosLaborales", "Vacaciones");
    public void Configure(EntityTypeBuilder<DiaInhabil> builder) => builder.ToTable("DiasInhabiles", "Vacaciones");
    public void Configure(EntityTypeBuilder<SolicitudAusencia> builder) => builder.ToTable("Solicitudes", "Vacaciones");
    public void Configure(EntityTypeBuilder<AprobacionAusencia> builder) => builder.ToTable("Aprobaciones", "Vacaciones");
    public void Configure(EntityTypeBuilder<AjusteSaldo> builder) => builder.ToTable("AjustesSaldo", "Vacaciones");
    public void Configure(EntityTypeBuilder<IncidenciaNomina> builder) => builder.ToTable("IncidenciasNomina", "Vacaciones");
}

public class RequisicionesConfigurations :
    IEntityTypeConfiguration<RequisicionPersonal>,
    IEntityTypeConfiguration<JustificacionRequisicion>,
    IEntityTypeConfiguration<PresupuestoRequisicion>,
    IEntityTypeConfiguration<PerfilRequisicion>,
    IEntityTypeConfiguration<AprobadorRequisicion>,
    IEntityTypeConfiguration<HistorialRequisicion>
{
    public void Configure(EntityTypeBuilder<RequisicionPersonal> builder)
    {
        builder.ToTable("Requisiciones", "Requisiciones");
        builder.HasIndex(r => r.Folio).IsUnique();
    }

    public void Configure(EntityTypeBuilder<JustificacionRequisicion> builder) => builder.ToTable("Justificaciones", "Requisiciones");
    public void Configure(EntityTypeBuilder<PresupuestoRequisicion> builder) => builder.ToTable("Presupuestos", "Requisiciones");
    public void Configure(EntityTypeBuilder<PerfilRequisicion> builder) => builder.ToTable("Perfiles", "Requisiciones");
    public void Configure(EntityTypeBuilder<AprobadorRequisicion> builder) => builder.ToTable("Aprobadores", "Requisiciones");
    public void Configure(EntityTypeBuilder<HistorialRequisicion> builder) => builder.ToTable("Historial", "Requisiciones");
}

public class ReclutamientoConfigurations :
    IEntityTypeConfiguration<VacanteReclutamiento>,
    IEntityTypeConfiguration<PublicacionVacante>,
    IEntityTypeConfiguration<Candidato>,
    IEntityTypeConfiguration<CurriculumVitae>,
    IEntityTypeConfiguration<Postulacion>,
    IEntityTypeConfiguration<Entrevista>,
    IEntityTypeConfiguration<EvaluacionReclutamiento>,
    IEntityTypeConfiguration<OfertaLaboral>
{
    public void Configure(EntityTypeBuilder<VacanteReclutamiento> builder) => builder.ToTable("Vacantes", "Reclutamiento");
    public void Configure(EntityTypeBuilder<PublicacionVacante> builder) => builder.ToTable("Publicaciones", "Reclutamiento");
    public void Configure(EntityTypeBuilder<Candidato> builder) => builder.ToTable("Candidatos", "Reclutamiento");
    public void Configure(EntityTypeBuilder<CurriculumVitae> builder) => builder.ToTable("CVs", "Reclutamiento");
    public void Configure(EntityTypeBuilder<Postulacion> builder) => builder.ToTable("Postulaciones", "Reclutamiento");
    public void Configure(EntityTypeBuilder<Entrevista> builder) => builder.ToTable("Entrevistas", "Reclutamiento");
    public void Configure(EntityTypeBuilder<EvaluacionReclutamiento> builder) => builder.ToTable("Evaluaciones", "Reclutamiento");
    public void Configure(EntityTypeBuilder<OfertaLaboral> builder) => builder.ToTable("Ofertas", "Reclutamiento");
}

public class OnboardingConfigurations :
    IEntityTypeConfiguration<PlanOnboarding>,
    IEntityTypeConfiguration<TareaOnboarding>,
    IEntityTypeConfiguration<ResponsableOnboarding>,
    IEntityTypeConfiguration<EvidenciaOnboarding>,
    IEntityTypeConfiguration<ChecklistOnboarding>,
    IEntityTypeConfiguration<FechaCompromisoOnboarding>
{
    public void Configure(EntityTypeBuilder<PlanOnboarding> builder) => builder.ToTable("Planes", "Onboarding");
    public void Configure(EntityTypeBuilder<TareaOnboarding> builder) => builder.ToTable("Tareas", "Onboarding");
    public void Configure(EntityTypeBuilder<ResponsableOnboarding> builder) => builder.ToTable("Responsables", "Onboarding");
    public void Configure(EntityTypeBuilder<EvidenciaOnboarding> builder) => builder.ToTable("Evidencias", "Onboarding");
    public void Configure(EntityTypeBuilder<ChecklistOnboarding> builder) => builder.ToTable("Checklists", "Onboarding");
    public void Configure(EntityTypeBuilder<FechaCompromisoOnboarding> builder) => builder.ToTable("FechasCompromiso", "Onboarding");
}

public class DocumentosConfigurations :
    IEntityTypeConfiguration<TipoDocumento>,
    IEntityTypeConfiguration<Expediente>,
    IEntityTypeConfiguration<Documento>,
    IEntityTypeConfiguration<VigenciaDocumento>,
    IEntityTypeConfiguration<FirmaDocumento>,
    IEntityTypeConfiguration<VersionDocumento>
{
    public void Configure(EntityTypeBuilder<TipoDocumento> builder) => builder.ToTable("TiposDocumento", "Documentos");
    public void Configure(EntityTypeBuilder<Expediente> builder) => builder.ToTable("Expedientes", "Documentos");
    public void Configure(EntityTypeBuilder<Documento> builder) => builder.ToTable("Documentos", "Documentos");
    public void Configure(EntityTypeBuilder<VigenciaDocumento> builder) => builder.ToTable("Vigencias", "Documentos");
    public void Configure(EntityTypeBuilder<FirmaDocumento> builder) => builder.ToTable("Firmas", "Documentos");
    public void Configure(EntityTypeBuilder<VersionDocumento> builder) => builder.ToTable("Versiones", "Documentos");
}

public class DesempenoConfigurations :
    IEntityTypeConfiguration<CicloDesempeno>,
    IEntityTypeConfiguration<Competencia>,
    IEntityTypeConfiguration<Objetivo>,
    IEntityTypeConfiguration<EvaluacionDesempeno>,
    IEntityTypeConfiguration<FeedbackDesempeno>,
    IEntityTypeConfiguration<ResultadoDesempeno>,
    IEntityTypeConfiguration<PlanAccion>,
    IEntityTypeConfiguration<FeedbackEquipo>
{
    public void Configure(EntityTypeBuilder<CicloDesempeno> builder) => builder.ToTable("Ciclos", "Desempeno");
    public void Configure(EntityTypeBuilder<Competencia> builder) => builder.ToTable("Competencias", "Desempeno");
    public void Configure(EntityTypeBuilder<Objetivo> builder) => builder.ToTable("Objetivos", "Desempeno");
    public void Configure(EntityTypeBuilder<EvaluacionDesempeno> builder) => builder.ToTable("Evaluaciones", "Desempeno");
    public void Configure(EntityTypeBuilder<FeedbackDesempeno> builder) => builder.ToTable("Feedbacks", "Desempeno");
    public void Configure(EntityTypeBuilder<ResultadoDesempeno> builder) => builder.ToTable("Resultados", "Desempeno");
    public void Configure(EntityTypeBuilder<PlanAccion> builder) => builder.ToTable("PlanesAccion", "Desempeno");
    public void Configure(EntityTypeBuilder<FeedbackEquipo> builder) => builder.ToTable("FeedbacksEquipo", "Desempeno");
}

public class CapacitacionConfigurations :
    IEntityTypeConfiguration<Curso>,
    IEntityTypeConfiguration<InscripcionCurso>,
    IEntityTypeConfiguration<EvidenciaCapacitacion>,
    IEntityTypeConfiguration<EvaluacionCapacitacion>,
    IEntityTypeConfiguration<ConstanciaCapacitacion>
{
    public void Configure(EntityTypeBuilder<Curso> builder) => builder.ToTable("Cursos", "Capacitacion");
    public void Configure(EntityTypeBuilder<InscripcionCurso> builder) => builder.ToTable("Inscripciones", "Capacitacion");
    public void Configure(EntityTypeBuilder<EvidenciaCapacitacion> builder) => builder.ToTable("Evidencias", "Capacitacion");
    public void Configure(EntityTypeBuilder<EvaluacionCapacitacion> builder) => builder.ToTable("Evaluaciones", "Capacitacion");
    public void Configure(EntityTypeBuilder<ConstanciaCapacitacion> builder) => builder.ToTable("Constancias", "Capacitacion");
}

public class BeneficiosConfigurations :
    IEntityTypeConfiguration<Beneficio>,
    IEntityTypeConfiguration<ElegibilidadBeneficio>,
    IEntityTypeConfiguration<SolicitudBeneficio>,
    IEntityTypeConfiguration<AprobacionBeneficio>,
    IEntityTypeConfiguration<DocumentoCorporativo>,
    IEntityTypeConfiguration<DiaFestivoCorporativo>,
    IEntityTypeConfiguration<ConvenioProveedor>,
    IEntityTypeConfiguration<ProductoTiendaInterna>,
    IEntityTypeConfiguration<SaldoPuntosColaborador>,
    IEntityTypeConfiguration<CanjeTiendaInterna>
{
    public void Configure(EntityTypeBuilder<Beneficio> builder) => builder.ToTable("Beneficios", "Beneficios");
    public void Configure(EntityTypeBuilder<ElegibilidadBeneficio> builder) => builder.ToTable("Elegibilidades", "Beneficios");
    public void Configure(EntityTypeBuilder<SolicitudBeneficio> builder) => builder.ToTable("Solicitudes", "Beneficios");
    public void Configure(EntityTypeBuilder<AprobacionBeneficio> builder) => builder.ToTable("Aprobaciones", "Beneficios");
    public void Configure(EntityTypeBuilder<DocumentoCorporativo> builder)
    {
        builder.ToTable("DocumentosCorporativos", "Beneficios");
        builder.HasIndex(d => d.Codigo).IsUnique();
    }
    public void Configure(EntityTypeBuilder<DiaFestivoCorporativo> builder) => builder.ToTable("DiasFestivos", "Beneficios");
    public void Configure(EntityTypeBuilder<ConvenioProveedor> builder)
    {
        builder.ToTable("ConveniosProveedores", "Beneficios");
        builder.HasIndex(c => c.Codigo).IsUnique();
    }
    public void Configure(EntityTypeBuilder<ProductoTiendaInterna> builder)
    {
        builder.ToTable("ProductosTienda", "Beneficios");
        builder.HasIndex(p => p.Codigo).IsUnique();
    }
    public void Configure(EntityTypeBuilder<SaldoPuntosColaborador> builder)
    {
        builder.ToTable("SaldosPuntos", "Beneficios");
        builder.HasIndex(s => s.ColaboradorId).IsUnique();
    }
    public void Configure(EntityTypeBuilder<CanjeTiendaInterna> builder) => builder.ToTable("CanjesTienda", "Beneficios");
}

public class PortalConfigurations :
    IEntityTypeConfiguration<PublicacionPortal>,
    IEntityTypeConfiguration<ContenidoModuloPortal>,
    IEntityTypeConfiguration<ReaccionPublicacion>,
    IEntityTypeConfiguration<ComentarioPublicacion>
{
    public void Configure(EntityTypeBuilder<PublicacionPortal> builder) => builder.ToTable("Publicaciones", "Portal");
    public void Configure(EntityTypeBuilder<ContenidoModuloPortal> builder)
    {
        builder.ToTable("ContenidosModulo", "Portal");
        builder.HasIndex(c => c.CodigoModulo).IsUnique();
    }

    public void Configure(EntityTypeBuilder<ReaccionPublicacion> builder)
    {
        builder.ToTable("Reacciones", "Portal");
        builder.HasIndex(r => new { r.PublicacionId, r.ColaboradorId }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<ComentarioPublicacion> builder)
    {
        builder.ToTable("Comentarios", "Portal");
        builder.HasIndex(c => c.PublicacionId);
    }
}

public class AuditoriaConfigurations :
    IEntityTypeConfiguration<BitacoraEvento>,
    IEntityTypeConfiguration<CambioEstado>,
    IEntityTypeConfiguration<NotificacionEnviada>,
    IEntityTypeConfiguration<Integracion>,
    IEntityTypeConfiguration<ErrorIntegracion>
{
    public void Configure(EntityTypeBuilder<BitacoraEvento> builder) => builder.ToTable("BitacoraEventos", "Auditoria");
    public void Configure(EntityTypeBuilder<CambioEstado> builder) => builder.ToTable("CambiosEstado", "Auditoria");
    public void Configure(EntityTypeBuilder<NotificacionEnviada> builder) => builder.ToTable("NotificacionesEnviadas", "Auditoria");
    public void Configure(EntityTypeBuilder<Integracion> builder) => builder.ToTable("Integraciones", "Auditoria");
    public void Configure(EntityTypeBuilder<ErrorIntegracion> builder) => builder.ToTable("ErroresIntegracion", "Auditoria");
}
