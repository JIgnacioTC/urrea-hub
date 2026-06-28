using UrreaHub.Domain.Common;

namespace UrreaHub.Domain.Auditoria;

public class BitacoraEvento : BaseEntity
{
    public string Modulo { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string Entidad { get; set; } = string.Empty;
    public Guid EntidadId { get; set; }
    public string? Usuario { get; set; }
    public string? Detalle { get; set; }
    public DateTime FechaEvento { get; set; } = DateTime.UtcNow;
    public string? IpOrigen { get; set; }
}

public class CambioEstado : BaseEntity
{
    public string Entidad { get; set; } = string.Empty;
    public Guid EntidadId { get; set; }
    public string EstadoAnterior { get; set; } = string.Empty;
    public string EstadoNuevo { get; set; } = string.Empty;
    public string? Usuario { get; set; }
    public DateTime FechaCambio { get; set; } = DateTime.UtcNow;
    public string? Motivo { get; set; }
}

public class NotificacionEnviada : BaseEntity
{
    public string Canal { get; set; } = string.Empty;
    public string Destinatario { get; set; } = string.Empty;
    public string Asunto { get; set; } = string.Empty;
    public string? Contenido { get; set; }
    public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
    public bool Exitosa { get; set; }
    public string? Error { get; set; }
}

public class Integracion : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string SistemaExterno { get; set; } = string.Empty;
    public string? Endpoint { get; set; }
    public EstadoIntegracion Estado { get; set; } = EstadoIntegracion.Pendiente;
    public DateTime? UltimaEjecucion { get; set; }

    public ICollection<ErrorIntegracion> Errores { get; set; } = new List<ErrorIntegracion>();
}

public class ErrorIntegracion : BaseEntity
{
    public string CodigoError { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string? Payload { get; set; }
    public DateTime FechaError { get; set; } = DateTime.UtcNow;

    public Guid IntegracionId { get; set; }
    public Integracion Integracion { get; set; } = null!;
}
