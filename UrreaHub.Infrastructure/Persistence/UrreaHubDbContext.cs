using Microsoft.EntityFrameworkCore;
using UrreaHub.Domain.Auditoria;
using UrreaHub.Domain.Catalogos;
using UrreaHub.Domain.Beneficios;
using UrreaHub.Domain.Capacitacion;
using UrreaHub.Domain.CoreRH;
using UrreaHub.Domain.Desempeno;
using UrreaHub.Domain.Documentos;
using UrreaHub.Domain.Onboarding;
using UrreaHub.Domain.Organizacion;
using UrreaHub.Domain.Portal;
using UrreaHub.Domain.Plataforma;
using UrreaHub.Domain.Reclutamiento;
using UrreaHub.Domain.Requisiciones;
using UrreaHub.Domain.Seguridad;
using UrreaHub.Domain.Asistencia;
using UrreaHub.Domain.Compensaciones;
using UrreaHub.Domain.Vacaciones;

namespace UrreaHub.Infrastructure.Persistence;

public class UrreaHubDbContext : DbContext
{
    public UrreaHubDbContext(DbContextOptions<UrreaHubDbContext> options) : base(options)
    {
    }

    // Core RH
    public DbSet<Colaborador> Colaboradores => Set<Colaborador>();
    public DbSet<Puesto> Puestos => Set<Puesto>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<Subarea> Subareas => Set<Subarea>();
    public DbSet<Departamento> Departamentos => Set<Departamento>();
    public DbSet<Sede> Sedes => Set<Sede>();
    public DbSet<CentroCosto> CentrosCosto => Set<CentroCosto>();
    public DbSet<RelacionLaboral> RelacionesLaborales => Set<RelacionLaboral>();
    public DbSet<CuentaAcceso> CuentasAcceso => Set<CuentaAcceso>();
    public DbSet<ColaboradorDatosSensibles> ColaboradoresDatosSensibles => Set<ColaboradorDatosSensibles>();
    public DbSet<ColaboradorDatosLaborales> ColaboradoresDatosLaborales => Set<ColaboradorDatosLaborales>();
    public DbSet<MovimientoColaborador> MovimientosColaborador => Set<MovimientoColaborador>();

    // Seguridad / RBAC
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Permiso> Permisos => Set<Permiso>();
    public DbSet<RolPermiso> RolPermisos => Set<RolPermiso>();
    public DbSet<ColaboradorRol> ColaboradorRoles => Set<ColaboradorRol>();

    // Catálogos (diccionarios RH — mínimos BUK)
    public DbSet<CatalogoEstado> CatalogosEstados => Set<CatalogoEstado>();
    public DbSet<CatalogoMunicipio> CatalogosMunicipios => Set<CatalogoMunicipio>();
    public DbSet<CatalogoEstadoCivil> CatalogosEstadosCiviles => Set<CatalogoEstadoCivil>();
    public DbSet<RazonTermino> RazonesTermino => Set<RazonTermino>();
    public DbSet<RegistroPatronal> RegistrosPatronales => Set<RegistroPatronal>();
    public DbSet<CatalogoJerarquia> CatalogosJerarquias => Set<CatalogoJerarquia>();

    // Organización
    public DbSet<Organigrama> Organigramas => Set<Organigrama>();
    public DbSet<PosicionOrganizacional> PosicionesOrganizacionales => Set<PosicionOrganizacional>();
    public DbSet<VacanteOrganizacional> VacantesOrganizacionales => Set<VacanteOrganizacional>();
    public DbSet<MovimientoOrganizacional> MovimientosOrganizacionales => Set<MovimientoOrganizacional>();

    // Vacaciones
    public DbSet<PoliticaVacaciones> PoliticasVacaciones => Set<PoliticaVacaciones>();
    public DbSet<SaldoVacaciones> SaldosVacaciones => Set<SaldoVacaciones>();
    public DbSet<SolicitudAusencia> SolicitudesAusencia => Set<SolicitudAusencia>();
    public DbSet<TipoAusencia> TiposAusencia => Set<TipoAusencia>();
    public DbSet<CalendarioLaboral> CalendariosLaborales => Set<CalendarioLaboral>();
    public DbSet<DiaInhabil> DiasInhabiles => Set<DiaInhabil>();
    public DbSet<AprobacionAusencia> AprobacionesAusencia => Set<AprobacionAusencia>();
    public DbSet<AjusteSaldo> AjustesSaldo => Set<AjusteSaldo>();
    public DbSet<IncidenciaNomina> IncidenciasNomina => Set<IncidenciaNomina>();

    // Asistencia
    public DbSet<RegistroAsistencia> RegistrosAsistencia => Set<RegistroAsistencia>();
    public DbSet<IncidenciaAsistencia> IncidenciasAsistencia => Set<IncidenciaAsistencia>();
    public DbSet<CorreccionAsistencia> CorreccionesAsistencia => Set<CorreccionAsistencia>();
    public DbSet<Turno> Turnos => Set<Turno>();
    public DbSet<AsignacionTurno> AsignacionesTurno => Set<AsignacionTurno>();
    public DbSet<ReglasAsistencia> ReglasAsistencia => Set<ReglasAsistencia>();
    public DbSet<IncidenciaNominaAsistencia> IncidenciasNominaAsistencia => Set<IncidenciaNominaAsistencia>();
    public DbSet<SolicitudCambioHorario> SolicitudesCambioHorario => Set<SolicitudCambioHorario>();

    // Requisiciones
    public DbSet<RequisicionPersonal> RequisicionesPersonal => Set<RequisicionPersonal>();
    public DbSet<JustificacionRequisicion> JustificacionesRequisicion => Set<JustificacionRequisicion>();
    public DbSet<PresupuestoRequisicion> PresupuestosRequisicion => Set<PresupuestoRequisicion>();
    public DbSet<PerfilRequisicion> PerfilesRequisicion => Set<PerfilRequisicion>();
    public DbSet<AprobadorRequisicion> AprobadoresRequisicion => Set<AprobadorRequisicion>();
    public DbSet<HistorialRequisicion> HistorialRequisiciones => Set<HistorialRequisicion>();

    // Reclutamiento
    public DbSet<VacanteReclutamiento> VacantesReclutamiento => Set<VacanteReclutamiento>();
    public DbSet<PublicacionVacante> PublicacionesVacante => Set<PublicacionVacante>();
    public DbSet<Candidato> Candidatos => Set<Candidato>();
    public DbSet<Postulacion> Postulaciones => Set<Postulacion>();
    public DbSet<CurriculumVitae> CurriculumsVitae => Set<CurriculumVitae>();
    public DbSet<Entrevista> Entrevistas => Set<Entrevista>();
    public DbSet<EvaluacionReclutamiento> EvaluacionesReclutamiento => Set<EvaluacionReclutamiento>();
    public DbSet<OfertaLaboral> OfertasLaborales => Set<OfertaLaboral>();

    // Onboarding
    public DbSet<PlanOnboarding> PlanesOnboarding => Set<PlanOnboarding>();
    public DbSet<TareaOnboarding> TareasOnboarding => Set<TareaOnboarding>();
    public DbSet<ResponsableOnboarding> ResponsablesOnboarding => Set<ResponsableOnboarding>();
    public DbSet<EvidenciaOnboarding> EvidenciasOnboarding => Set<EvidenciaOnboarding>();
    public DbSet<ChecklistOnboarding> ChecklistsOnboarding => Set<ChecklistOnboarding>();
    public DbSet<FechaCompromisoOnboarding> FechasCompromisoOnboarding => Set<FechaCompromisoOnboarding>();

    // Compensaciones
    public DbSet<SolicitudAjusteCompensacion> SolicitudesAjusteCompensacion => Set<SolicitudAjusteCompensacion>();
    public DbSet<AprobacionAjusteCompensacion> AprobacionesAjusteCompensacion => Set<AprobacionAjusteCompensacion>();
    public DbSet<HistorialAjusteCompensacion> HistorialAjusteCompensacion => Set<HistorialAjusteCompensacion>();
    public DbSet<Tabulador> Tabuladores => Set<Tabulador>();
    public DbSet<BandaSalarial> BandasSalariales => Set<BandaSalarial>();
    public DbSet<ConceptoCompensacion> ConceptosCompensacion => Set<ConceptoCompensacion>();

    // Documentos
    public DbSet<Expediente> Expedientes => Set<Expediente>();
    public DbSet<Documento> Documentos => Set<Documento>();
    public DbSet<TipoDocumento> TiposDocumento => Set<TipoDocumento>();
    public DbSet<VigenciaDocumento> VigenciasDocumento => Set<VigenciaDocumento>();
    public DbSet<FirmaDocumento> FirmasDocumento => Set<FirmaDocumento>();
    public DbSet<VersionDocumento> VersionesDocumento => Set<VersionDocumento>();

    // Desempeño
    public DbSet<CicloDesempeno> CiclosDesempeno => Set<CicloDesempeno>();
    public DbSet<Objetivo> Objetivos => Set<Objetivo>();
    public DbSet<Competencia> Competencias => Set<Competencia>();
    public DbSet<EvaluacionDesempeno> EvaluacionesDesempeno => Set<EvaluacionDesempeno>();
    public DbSet<FeedbackDesempeno> FeedbacksDesempeno => Set<FeedbackDesempeno>();
    public DbSet<ResultadoDesempeno> ResultadosDesempeno => Set<ResultadoDesempeno>();
    public DbSet<PlanAccion> PlanesAccion => Set<PlanAccion>();
    public DbSet<FeedbackEquipo> FeedbacksEquipo => Set<FeedbackEquipo>();

    // Capacitación
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<InscripcionCurso> InscripcionesCurso => Set<InscripcionCurso>();
    public DbSet<EvidenciaCapacitacion> EvidenciasCapacitacion => Set<EvidenciaCapacitacion>();
    public DbSet<EvaluacionCapacitacion> EvaluacionesCapacitacion => Set<EvaluacionCapacitacion>();
    public DbSet<ConstanciaCapacitacion> ConstanciasCapacitacion => Set<ConstanciaCapacitacion>();

    // Beneficios
    public DbSet<Beneficio> Beneficios => Set<Beneficio>();
    public DbSet<SolicitudBeneficio> SolicitudesBeneficio => Set<SolicitudBeneficio>();
    public DbSet<ElegibilidadBeneficio> ElegibilidadesBeneficio => Set<ElegibilidadBeneficio>();
    public DbSet<AprobacionBeneficio> AprobacionesBeneficio => Set<AprobacionBeneficio>();
    public DbSet<DocumentoCorporativo> DocumentosCorporativos => Set<DocumentoCorporativo>();
    public DbSet<DiaFestivoCorporativo> DiasFestivosCorporativos => Set<DiaFestivoCorporativo>();
    public DbSet<ConvenioProveedor> ConveniosProveedores => Set<ConvenioProveedor>();
    public DbSet<ProductoTiendaInterna> ProductosTiendaInterna => Set<ProductoTiendaInterna>();
    public DbSet<SaldoPuntosColaborador> SaldosPuntosColaboradores => Set<SaldoPuntosColaborador>();
    public DbSet<CanjeTiendaInterna> CanjesTiendaInterna => Set<CanjeTiendaInterna>();

    // Portal
    public DbSet<PublicacionPortal> PublicacionesPortal => Set<PublicacionPortal>();
    public DbSet<ContenidoModuloPortal> ContenidosModuloPortal => Set<ContenidoModuloPortal>();

    public DbSet<MetadatoPlataforma> MetadatosPlataforma => Set<MetadatoPlataforma>();
    public DbSet<ConfiguracionGlobal> ConfiguracionesGlobales => Set<ConfiguracionGlobal>();

    // Auditoría
    public DbSet<BitacoraEvento> BitacoraEventos => Set<BitacoraEvento>();
    public DbSet<CambioEstado> CambiosEstado => Set<CambioEstado>();
    public DbSet<NotificacionEnviada> NotificacionesEnviadas => Set<NotificacionEnviada>();
    public DbSet<Integracion> Integraciones => Set<Integracion>();
    public DbSet<ErrorIntegracion> ErroresIntegracion => Set<ErrorIntegracion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UrreaHubDbContext).Assembly);

        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
            .SelectMany(entity => entity.GetForeignKeys())
            .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
        }

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            modelBuilder.Entity(entityType.ClrType)
                .Property("CreatedAt")
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
