using UrreaHub.Domain.Common;

namespace UrreaHub.Domain.Plataforma;

public enum TipoMetadatoPlataforma
{
    Schema,
    Api,
    Environment,
    Combined,
}

public enum OrigenMetadato
{
    Dev,
    Staging,
    Produccion,
    Manual,
}

/// <summary>Snapshot versionado de schema, APIs o ambiente para alinear dev/producción.</summary>
public class MetadatoPlataforma : BaseEntity
{
    public TipoMetadatoPlataforma Tipo { get; set; }
    public OrigenMetadato Origen { get; set; }
    public string Etiqueta { get; set; } = string.Empty;
    public string? VersionTag { get; set; }
    public string? Notas { get; set; }
    public string ContenidoJson { get; set; } = "{}";
    public string? MigracionId { get; set; }
    public Guid? CreadoPorColaboradorId { get; set; }
}

public class ConfiguracionGlobal : BaseEntity
{
    public string Clave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
