using UrreaHub.Domain.Common;

namespace UrreaHub.Domain.Catalogos;

/// <summary>Entidad federativa (estado) — catálogo geográfico México.</summary>
public class CatalogoEstado : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Pais { get; set; } = "MEX";

    public ICollection<CatalogoMunicipio> Municipios { get; set; } = new List<CatalogoMunicipio>();
}

/// <summary>Municipio / alcaldía vinculado a un estado.</summary>
public class CatalogoMunicipio : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    public Guid EstadoId { get; set; }
    public CatalogoEstado Estado { get; set; } = null!;
}

/// <summary>Estado civil — diccionario RH.</summary>
public class CatalogoEstadoCivil : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Orden { get; set; }
}

/// <summary>Motivo de baja / término de relación laboral.</summary>
public class RazonTermino : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool RequiereComprobante { get; set; }
    public bool Activo { get; set; } = true;
}

/// <summary>Registro patronal IMSS asociado a la empresa.</summary>
public class RegistroPatronal : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string NumeroImss { get; set; } = string.Empty;
    public string RazonSocial { get; set; } = string.Empty;
    public string? Rfc { get; set; }
    public Guid? EstadoId { get; set; }
    public CatalogoEstado? Estado { get; set; }
    public bool Activo { get; set; } = true;
}

/// <summary>Nivel jerárquico organizacional (Director, Gerente, etc.).</summary>
public class CatalogoJerarquia : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int NivelOrden { get; set; }
    public string? Descripcion { get; set; }
}
