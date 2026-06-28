using UrreaHub.Domain.Common;
using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Domain.Compensaciones;

public class SolicitudAjusteCompensacion : BaseEntity
{
    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public Guid SolicitanteId { get; set; }
    public Colaborador Solicitante { get; set; } = null!;

    public TipoAjusteCompensacion TipoAjuste { get; set; }
    public EstadoAjusteCompensacion Estado { get; set; } = EstadoAjusteCompensacion.Borrador;
    public string? ValorAnterior { get; set; }
    public string ValorNuevo { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaDecision { get; set; }
    public bool RequiereFinanzas { get; set; }
    public decimal? MontoReferencia { get; set; }

    public ICollection<AprobacionAjusteCompensacion> Aprobaciones { get; set; } = new List<AprobacionAjusteCompensacion>();
    public ICollection<HistorialAjusteCompensacion> Historial { get; set; } = new List<HistorialAjusteCompensacion>();
}

public class AprobacionAjusteCompensacion : BaseEntity
{
    public int Orden { get; set; }
    public EstadoAjusteCompensacion Decision { get; set; } = EstadoAjusteCompensacion.EnRevisionDh;
    public string? Comentario { get; set; }
    public DateTime? FechaDecision { get; set; }
    public string RolAprobador { get; set; } = string.Empty;

    public Guid SolicitudId { get; set; }
    public SolicitudAjusteCompensacion Solicitud { get; set; } = null!;

    public Guid AprobadorId { get; set; }
    public Colaborador Aprobador { get; set; } = null!;
}

public class HistorialAjusteCompensacion : BaseEntity
{
    public string Accion { get; set; } = string.Empty;
    public string? Detalle { get; set; }
    public DateTime FechaAccion { get; set; }

    public Guid SolicitudId { get; set; }
    public SolicitudAjusteCompensacion Solicitud { get; set; } = null!;

    public Guid? UsuarioId { get; set; }
    public Colaborador? Usuario { get; set; }
}

public class Tabulador : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Moneda { get; set; } = "MXN";
    public DateTime VigenciaDesde { get; set; }
    public DateTime? VigenciaHasta { get; set; }

    public ICollection<BandaSalarial> Bandas { get; set; } = new List<BandaSalarial>();
}

public class BandaSalarial : BaseEntity
{
    public string Nivel { get; set; } = string.Empty;
    public decimal Minimo { get; set; }
    public decimal Medio { get; set; }
    public decimal Maximo { get; set; }

    public Guid TabuladorId { get; set; }
    public Tabulador Tabulador { get; set; } = null!;
}

public class ConceptoCompensacion : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public bool ImpactaNomina { get; set; }
    public string? Descripcion { get; set; }
}
