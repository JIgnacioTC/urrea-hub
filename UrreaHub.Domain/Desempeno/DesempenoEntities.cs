using UrreaHub.Domain.Common;
using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Domain.Desempeno;

public class CicloDesempeno : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public int Anio { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activo { get; set; }

    public ICollection<EvaluacionDesempeno> Evaluaciones { get; set; } = new List<EvaluacionDesempeno>();
}

public class Competencia : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Peso { get; set; } = 1;
}

public class Objetivo : BaseEntity
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Meta { get; set; }
    public decimal Avance { get; set; }
    public decimal Peso { get; set; } = 1;

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public Guid CicloId { get; set; }
    public CicloDesempeno Ciclo { get; set; } = null!;
}

public class EvaluacionDesempeno : BaseEntity
{
    public decimal PuntuacionFinal { get; set; }
    public string? ComentarioGeneral { get; set; }
    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;

    public Guid CicloId { get; set; }
    public CicloDesempeno Ciclo { get; set; } = null!;

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public Guid EvaluadorId { get; set; }
    public Colaborador Evaluador { get; set; } = null!;

    public ResultadoDesempeno? Resultado { get; set; }
    public ICollection<FeedbackDesempeno> Feedbacks { get; set; } = new List<FeedbackDesempeno>();
}

public class FeedbackDesempeno : BaseEntity
{
    public string Comentario { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Tipo { get; set; } = string.Empty;

    public Guid EvaluacionId { get; set; }
    public EvaluacionDesempeno Evaluacion { get; set; } = null!;
}

public class ResultadoDesempeno : BaseEntity
{
    public string Calificacion { get; set; } = string.Empty;
    public string? Recomendacion { get; set; }
    public DateTime FechaResultado { get; set; }

    public Guid EvaluacionId { get; set; }
    public EvaluacionDesempeno Evaluacion { get; set; } = null!;
}

public class PlanAccion : BaseEntity
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public EstadoPlanAccion Estado { get; set; } = EstadoPlanAccion.Pendiente;
    public decimal Avance { get; set; }
    public string Prioridad { get; set; } = "Media";

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public Guid CreadoPorId { get; set; }
    public Colaborador CreadoPor { get; set; } = null!;
}

public class FeedbackEquipo : BaseEntity
{
    public string Tipo { get; set; } = string.Empty;
    public string Comentario { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public Guid AutorId { get; set; }
    public Colaborador Autor { get; set; } = null!;
}
