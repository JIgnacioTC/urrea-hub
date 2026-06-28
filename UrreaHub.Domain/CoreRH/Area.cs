using UrreaHub.Domain.Common;

namespace UrreaHub.Domain.CoreRH;

public class Area : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();
}
