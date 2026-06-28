using UrreaHub.Domain.Common;

namespace UrreaHub.Domain.CoreRH;

/// <summary>Datos personales sensibles (LFPDPPP). Separados del núcleo operacional.</summary>
public class ColaboradorDatosSensibles : BaseEntity
{
    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public string? Rfc { get; set; }
    public string? Curp { get; set; }
    public string? Nss { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string? Genero { get; set; }
    public string? EstadoCivil { get; set; }
    public string? Domicilio { get; set; }
    public string? CodigoPostal { get; set; }
    public string? Ciudad { get; set; }
    public string? Estado { get; set; }
    public string? Pais { get; set; } = "México";
    public bool Enmascarado { get; set; } = true;
}
