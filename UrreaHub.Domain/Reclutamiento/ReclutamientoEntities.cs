using UrreaHub.Domain.Common;
using UrreaHub.Domain.Requisiciones;

namespace UrreaHub.Domain.Reclutamiento;

public class VacanteReclutamiento : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public EstadoVacante Estado { get; set; } = EstadoVacante.Abierta;
    public DateTime FechaPublicacion { get; set; }
    public DateTime? FechaCierre { get; set; }

    public Guid? RequisicionId { get; set; }
    public RequisicionPersonal? Requisicion { get; set; }

    public ICollection<PublicacionVacante> Publicaciones { get; set; } = new List<PublicacionVacante>();
    public ICollection<Postulacion> Postulaciones { get; set; } = new List<Postulacion>();
}

public class PublicacionVacante : BaseEntity
{
    public string Canal { get; set; } = string.Empty;
    public string? Url { get; set; }
    public DateTime FechaPublicacion { get; set; }
    public DateTime? FechaExpiracion { get; set; }

    public Guid VacanteId { get; set; }
    public VacanteReclutamiento Vacante { get; set; } = null!;
}

public class Candidato : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string ApellidoPaterno { get; set; } = string.Empty;
    public string? ApellidoMaterno { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? LinkedIn { get; set; }

    public CurriculumVitae? Curriculum { get; set; }
    public ICollection<Postulacion> Postulaciones { get; set; } = new List<Postulacion>();
}

public class CurriculumVitae : BaseEntity
{
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public string? Resumen { get; set; }
    public DateTime FechaCarga { get; set; }

    public Guid CandidatoId { get; set; }
    public Candidato Candidato { get; set; } = null!;
}

public class Postulacion : BaseEntity
{
    public DateTime FechaPostulacion { get; set; }
    public EstadoPostulacion Estado { get; set; } = EstadoPostulacion.Recibida;
    public string? Notas { get; set; }

    public Guid VacanteId { get; set; }
    public VacanteReclutamiento Vacante { get; set; } = null!;

    public Guid CandidatoId { get; set; }
    public Candidato Candidato { get; set; } = null!;

    public ICollection<Entrevista> Entrevistas { get; set; } = new List<Entrevista>();
    public ICollection<EvaluacionReclutamiento> Evaluaciones { get; set; } = new List<EvaluacionReclutamiento>();
    public OfertaLaboral? Oferta { get; set; }
}

public class Entrevista : BaseEntity
{
    public DateTime FechaHora { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string? Ubicacion { get; set; }
    public string? Notas { get; set; }

    public Guid PostulacionId { get; set; }
    public Postulacion Postulacion { get; set; } = null!;
}

public class EvaluacionReclutamiento : BaseEntity
{
    public string Evaluador { get; set; } = string.Empty;
    public decimal Puntuacion { get; set; }
    public string? Comentarios { get; set; }
    public DateTime FechaEvaluacion { get; set; }

    public Guid PostulacionId { get; set; }
    public Postulacion Postulacion { get; set; } = null!;
}

public class OfertaLaboral : BaseEntity
{
    public decimal SalarioOfrecido { get; set; }
    public string Moneda { get; set; } = "MXN";
    public DateTime FechaOferta { get; set; }
    public DateTime? FechaRespuesta { get; set; }
    public bool Aceptada { get; set; }

    public Guid PostulacionId { get; set; }
    public Postulacion Postulacion { get; set; } = null!;
}
