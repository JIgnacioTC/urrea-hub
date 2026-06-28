using UrreaHub.Domain.Common;

namespace UrreaHub.Domain.CoreRH;

public class ColaboradorDatosLaborales : BaseEntity
{
    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public string? Jornada { get; set; }
    public string? Turno { get; set; }
    public string? GrupoNomina { get; set; }
    public bool Sindicalizado { get; set; }
    public string? NivelSalarial { get; set; }
    public string NivelVisibilidadCompensacion { get; set; } = "Restricted";
}
