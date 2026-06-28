using UrreaHub.Domain.Common;

namespace UrreaHub.Domain.CoreRH;

public class Puesto : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int NivelJerarquico { get; set; }

    public ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();
}
