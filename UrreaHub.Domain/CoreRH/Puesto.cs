using UrreaHub.Domain.Common;

namespace UrreaHub.Domain.CoreRH;

public class Puesto : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int NivelJerarquico { get; set; }

    // Mercer job evaluation parameters
    public int? GradoMercer { get; set; }
    public string? Impacto { get; set; }
    public string? Comunicacion { get; set; }
    public string? Innovacion { get; set; }
    public string? EducacionRequerida { get; set; }
    public int? ExperienciaAnios { get; set; }
    public decimal? PresupuestoAnual { get; set; }
    public int? PersonalCargoDirecto { get; set; }
    public int? PersonalCargoIndirecto { get; set; }

    public ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();
}
