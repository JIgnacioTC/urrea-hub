using UrreaHub.Domain.Common;
using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Domain.Capacitacion;

public class Curso : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int DuracionHoras { get; set; }
    public string? Modalidad { get; set; }

    public ICollection<InscripcionCurso> Inscripciones { get; set; } = new List<InscripcionCurso>();
}

public class InscripcionCurso : BaseEntity
{
    public DateTime FechaInscripcion { get; set; }
    public DateTime? FechaCompletado { get; set; }
    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;

    public Guid CursoId { get; set; }
    public Curso Curso { get; set; } = null!;

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public ICollection<EvidenciaCapacitacion> Evidencias { get; set; } = new List<EvidenciaCapacitacion>();
    public EvaluacionCapacitacion? Evaluacion { get; set; }
    public ConstanciaCapacitacion? Constancia { get; set; }
}

public class EvidenciaCapacitacion : BaseEntity
{
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public DateTime FechaCarga { get; set; }

    public Guid InscripcionId { get; set; }
    public InscripcionCurso Inscripcion { get; set; } = null!;
}

public class EvaluacionCapacitacion : BaseEntity
{
    public decimal Puntuacion { get; set; }
    public bool Aprobado { get; set; }
    public DateTime FechaEvaluacion { get; set; }
    public string? Comentarios { get; set; }

    public Guid InscripcionId { get; set; }
    public InscripcionCurso Inscripcion { get; set; } = null!;
}

public class ConstanciaCapacitacion : BaseEntity
{
    public string Folio { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }

    public Guid InscripcionId { get; set; }
    public InscripcionCurso Inscripcion { get; set; } = null!;
}
