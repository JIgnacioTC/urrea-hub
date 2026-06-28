using System.ComponentModel.DataAnnotations.Schema;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Domain.Documentos;

public class TipoDocumento : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool RequiereVigencia { get; set; }
    public bool RequiereFirma { get; set; }
    public NivelConfidencialidad NivelConfidencialidad { get; set; } = NivelConfidencialidad.Interno;

    public ICollection<Documento> Documentos { get; set; } = new List<Documento>();
}

public class Expediente : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public ICollection<Documento> Documentos { get; set; } = new List<Documento>();
}

public class Documento : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string NombreArchivo { get; set; } = string.Empty;
    public string RutaArchivo { get; set; } = string.Empty;
    public int VersionActual { get; set; } = 1;

    public Guid TipoDocumentoId { get; set; }
    public TipoDocumento TipoDocumento { get; set; } = null!;

    public Guid ExpedienteId { get; set; }
    public Expediente Expediente { get; set; } = null!;

    public VigenciaDocumento? Vigencia { get; set; }
    public ICollection<FirmaDocumento> Firmas { get; set; } = new List<FirmaDocumento>();
    public ICollection<VersionDocumento> Versiones { get; set; } = new List<VersionDocumento>();
}

public class VigenciaDocumento : BaseEntity
{
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }

    [NotMapped]
    public bool Vigente => !FechaFin.HasValue || FechaFin.Value >= DateTime.UtcNow;

    public Guid DocumentoId { get; set; }
    public Documento Documento { get; set; } = null!;
}

public class FirmaDocumento : BaseEntity
{
    public string Firmante { get; set; } = string.Empty;
    public DateTime FechaFirma { get; set; }
    public string? MetodoFirma { get; set; }

    public Guid DocumentoId { get; set; }
    public Documento Documento { get; set; } = null!;
}

public class VersionDocumento : BaseEntity
{
    public int NumeroVersion { get; set; }
    public string RutaArchivo { get; set; } = string.Empty;
    public string? ComentarioCambio { get; set; }
    public DateTime FechaVersion { get; set; }

    public Guid DocumentoId { get; set; }
    public Documento Documento { get; set; } = null!;
}
