using UrreaHub.Domain.Common;
using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Domain.Beneficios;

public class Beneficio : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal? MontoMaximo { get; set; }
    public string Moneda { get; set; } = "MXN";

    public ICollection<ElegibilidadBeneficio> Elegibilidades { get; set; } = new List<ElegibilidadBeneficio>();
    public ICollection<SolicitudBeneficio> Solicitudes { get; set; } = new List<SolicitudBeneficio>();
}

public class ElegibilidadBeneficio : BaseEntity
{
    public string Criterio { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;

    public Guid BeneficioId { get; set; }
    public Beneficio Beneficio { get; set; } = null!;
}

public class SolicitudBeneficio : BaseEntity
{
    public DateTime FechaSolicitud { get; set; }
    public decimal? MontoSolicitado { get; set; }
    public string? Justificacion { get; set; }
    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;

    public Guid BeneficioId { get; set; }
    public Beneficio Beneficio { get; set; } = null!;

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public ICollection<AprobacionBeneficio> Aprobaciones { get; set; } = new List<AprobacionBeneficio>();
}

public class AprobacionBeneficio : BaseEntity
{
    public EstadoSolicitud Decision { get; set; }
    public string? Comentario { get; set; }
    public DateTime FechaDecision { get; set; }

    public Guid SolicitudId { get; set; }
    public SolicitudBeneficio Solicitud { get; set; } = null!;

    public Guid AprobadorId { get; set; }
    public Colaborador Aprobador { get; set; } = null!;
}

public class DocumentoCorporativo : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public CategoriaDocumentoCorporativo Categoria { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Icono { get; set; }
    public int? Paginas { get; set; }
    public string? UrlDocumento { get; set; }
    public DateTime FechaActualizacion { get; set; }
    public int Orden { get; set; }
}

public class DiaFestivoCorporativo : BaseEntity
{
    public DateTime Fecha { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoDiaFestivo Tipo { get; set; }
    public int Anio { get; set; }
}

public class ConvenioProveedor : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Proveedor { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Descuento { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Icono { get; set; }
    public string Vigencia { get; set; } = string.Empty;
    public string? CodigoPromocional { get; set; }
    public int Orden { get; set; }
}

public class ProductoTiendaInterna : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int PuntosRequeridos { get; set; }
    public int Stock { get; set; }
    public string? Icono { get; set; }
    public string? Gradiente { get; set; }
    public bool Destacado { get; set; }
    public int Orden { get; set; }
}

public class SaldoPuntosColaborador : BaseEntity
{
    public int Puntos { get; set; }
    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;
}

public class CanjeTiendaInterna : BaseEntity
{
    public int PuntosUsados { get; set; }
    public DateTime FechaCanje { get; set; }
    public string Estado { get; set; } = "PendienteEntrega";

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public Guid ProductoId { get; set; }
    public ProductoTiendaInterna Producto { get; set; } = null!;
}
