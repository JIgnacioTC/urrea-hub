using UrreaHub.Domain.Common;
using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Domain.Requisiciones;

public class RequisicionPersonal : BaseEntity
{
    public string Folio { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public int VacantesSolicitadas { get; set; }
    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Borrador;
    public DateTime FechaSolicitud { get; set; }

    public Guid SolicitanteId { get; set; }
    public Colaborador Solicitante { get; set; } = null!;

    public Guid? DepartamentoId { get; set; }
    public Departamento? Departamento { get; set; }

    public JustificacionRequisicion? Justificacion { get; set; }
    public PresupuestoRequisicion? Presupuesto { get; set; }
    public PerfilRequisicion? Perfil { get; set; }
    public ICollection<AprobadorRequisicion> Aprobadores { get; set; } = new List<AprobadorRequisicion>();
    public ICollection<HistorialRequisicion> Historial { get; set; } = new List<HistorialRequisicion>();
}

public class JustificacionRequisicion : BaseEntity
{
    public string Motivo { get; set; } = string.Empty;
    public string? ImpactoNegocio { get; set; }
    public string? AlternativasConsideradas { get; set; }

    public Guid RequisicionId { get; set; }
    public RequisicionPersonal Requisicion { get; set; } = null!;
}

public class PresupuestoRequisicion : BaseEntity
{
    public decimal MontoAutorizado { get; set; }
    public string Moneda { get; set; } = "MXN";
    public string? CentroCostoCodigo { get; set; }
    public string? Notas { get; set; }

    public Guid RequisicionId { get; set; }
    public RequisicionPersonal Requisicion { get; set; } = null!;
}

public class PerfilRequisicion : BaseEntity
{
    public string DescripcionPuesto { get; set; } = string.Empty;
    public string? ExperienciaRequerida { get; set; }
    public string? EducacionRequerida { get; set; }
    public string? CompetenciasRequeridas { get; set; }

    public Guid RequisicionId { get; set; }
    public RequisicionPersonal Requisicion { get; set; } = null!;
}

public class AprobadorRequisicion : BaseEntity
{
    public int Orden { get; set; }
    public EstadoSolicitud Decision { get; set; } = EstadoSolicitud.Pendiente;
    public DateTime? FechaDecision { get; set; }
    public string? Comentario { get; set; }

    public Guid RequisicionId { get; set; }
    public RequisicionPersonal Requisicion { get; set; } = null!;

    public Guid AprobadorId { get; set; }
    public Colaborador Aprobador { get; set; } = null!;
}

public class HistorialRequisicion : BaseEntity
{
    public string Accion { get; set; } = string.Empty;
    public string? Detalle { get; set; }
    public DateTime FechaAccion { get; set; }

    public Guid RequisicionId { get; set; }
    public RequisicionPersonal Requisicion { get; set; } = null!;

    public Guid? UsuarioId { get; set; }
    public Colaborador? Usuario { get; set; }
}
