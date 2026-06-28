using UrreaHub.Domain.Common;
using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Domain.Asistencia;

public class RegistroAsistencia : BaseEntity
{
    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public DateTime Fecha { get; set; }
    public DateTime? HoraEntrada { get; set; }
    public DateTime? HoraSalida { get; set; }
    public FuenteRegistroAsistencia Fuente { get; set; }
    public string TipoRegistro { get; set; } = "Oficina";
    public EstadoRegistroAsistencia Estado { get; set; } = EstadoRegistroAsistencia.EntradaSinSalida;

    public decimal? LatitudEntrada { get; set; }
    public decimal? LongitudEntrada { get; set; }
    public decimal? LatitudSalida { get; set; }
    public decimal? LongitudSalida { get; set; }
    public string? ClienteComercial { get; set; }
    public string? UbicacionComercial { get; set; }
    public string? Observaciones { get; set; }

    public ICollection<IncidenciaAsistencia> Incidencias { get; set; } = new List<IncidenciaAsistencia>();
}

public class IncidenciaAsistencia : BaseEntity
{
    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public Guid? RegistroId { get; set; }
    public RegistroAsistencia? Registro { get; set; }

    public DateTime Fecha { get; set; }
    public TipoIncidenciaAsistencia Tipo { get; set; }
    public string Severidad { get; set; } = "Media";
    public EstadoIncidenciaAsistencia Estado { get; set; } = EstadoIncidenciaAsistencia.Detectada;
    public string? Descripcion { get; set; }
    public bool RequiereValidacion { get; set; } = true;
    public bool GeneraPrenomina { get; set; }
    public DateTime FechaDeteccion { get; set; } = DateTime.UtcNow;

    public ICollection<CorreccionAsistencia> Correcciones { get; set; } = new List<CorreccionAsistencia>();
}

public class CorreccionAsistencia : BaseEntity
{
    public Guid IncidenciaId { get; set; }
    public IncidenciaAsistencia Incidencia { get; set; } = null!;

    public Guid SolicitanteId { get; set; }
    public Colaborador Solicitante { get; set; } = null!;

    public Guid? AprobadorId { get; set; }
    public Colaborador? Aprobador { get; set; }

    public TipoCorreccionAsistencia TipoCorreccion { get; set; }
    public DateTime? HoraEntradaSolicitada { get; set; }
    public DateTime? HoraSalidaSolicitada { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? EvidenciaRef { get; set; }
    public EstadoCorreccionAsistencia Estado { get; set; } = EstadoCorreccionAsistencia.Solicitada;
    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
    public DateTime? FechaDecision { get; set; }
    public string? ComentarioDecision { get; set; }
}

public class Turno : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public TimeSpan HoraEntrada { get; set; }
    public TimeSpan HoraSalida { get; set; }
    public int MinutosToleranciaEntrada { get; set; }
    public int MinutosToleranciaSalida { get; set; }
    public int MinutosComida { get; set; }
    public bool AplicaLunes { get; set; } = true;
    public bool AplicaMartes { get; set; } = true;
    public bool AplicaMiercoles { get; set; } = true;
    public bool AplicaJueves { get; set; } = true;
    public bool AplicaViernes { get; set; } = true;
    public bool AplicaSabado { get; set; }
    public bool AplicaDomingo { get; set; }
    public Guid? SedeId { get; set; }
    public Sede? Sede { get; set; }
    public Guid? AreaId { get; set; }
    public Area? Area { get; set; }
}

public class AsignacionTurno : BaseEntity
{
    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public Guid TurnoId { get; set; }
    public Turno Turno { get; set; } = null!;

    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string Origen { get; set; } = "Manual";
}

public class ReglasAsistencia : BaseEntity
{
    public Guid? SedeId { get; set; }
    public Sede? Sede { get; set; }

    public int MinutosToleranciaRetardo { get; set; } = 10;
    public int MinutosParaFalta { get; set; } = 60;
    public bool GeneraIncidenciaNominaRetardo { get; set; } = true;
    public bool RequiereValidacionLider { get; set; } = true;
    public bool PermitirRegistroMovil { get; set; } = true;
    public bool RequiereGeolocalizacion { get; set; }
    public int RadioMetrosSede { get; set; } = 500;
}

public class IncidenciaNominaAsistencia : BaseEntity
{
    public Guid IncidenciaId { get; set; }
    public IncidenciaAsistencia Incidencia { get; set; } = null!;

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public string Periodo { get; set; } = string.Empty;
    public string TipoConcepto { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public string Unidad { get; set; } = "horas";
    public EstadoIncidenciaNominaAsistencia Estado { get; set; } = EstadoIncidenciaNominaAsistencia.Pendiente;
    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaEnvioNomina { get; set; }
    public DateTime? NominaSyncAt { get; set; }
    public string? ErrorNomina { get; set; }
    public string? ValidadoPor { get; set; }
}
