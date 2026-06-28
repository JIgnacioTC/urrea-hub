using UrreaHub.Domain.Common;
using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Domain.Onboarding;

public class PlanOnboarding : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public ICollection<TareaOnboarding> Tareas { get; set; } = new List<TareaOnboarding>();
    public ICollection<ChecklistOnboarding> Checklists { get; set; } = new List<ChecklistOnboarding>();
}

public class TareaOnboarding : BaseEntity
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Orden { get; set; }
    public bool Completada { get; set; }

    public Guid PlanId { get; set; }
    public PlanOnboarding Plan { get; set; } = null!;

    public ICollection<ResponsableOnboarding> Responsables { get; set; } = new List<ResponsableOnboarding>();
    public ICollection<EvidenciaOnboarding> Evidencias { get; set; } = new List<EvidenciaOnboarding>();
    public FechaCompromisoOnboarding? FechaCompromiso { get; set; }
}

public class ResponsableOnboarding : BaseEntity
{
    public string Rol { get; set; } = string.Empty;

    public Guid TareaId { get; set; }
    public TareaOnboarding Tarea { get; set; } = null!;

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;
}

public class EvidenciaOnboarding : BaseEntity
{
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public DateTime FechaCarga { get; set; }
    public string? Comentario { get; set; }

    public Guid TareaId { get; set; }
    public TareaOnboarding Tarea { get; set; } = null!;
}

public class ChecklistOnboarding : BaseEntity
{
    public string Item { get; set; } = string.Empty;
    public bool Completado { get; set; }
    public DateTime? FechaCompletado { get; set; }

    public Guid PlanId { get; set; }
    public PlanOnboarding Plan { get; set; } = null!;
}

public class FechaCompromisoOnboarding : BaseEntity
{
    public DateTime FechaCompromiso { get; set; }
    public DateTime? FechaCumplimiento { get; set; }
    public string? Notas { get; set; }

    public Guid TareaId { get; set; }
    public TareaOnboarding Tarea { get; set; } = null!;
}
