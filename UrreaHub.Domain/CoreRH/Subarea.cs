using UrreaHub.Domain.Common;

namespace UrreaHub.Domain.CoreRH;

public class Subarea : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public Guid AreaId { get; set; }
    public Area Area { get; set; } = null!;

    public ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();
}
