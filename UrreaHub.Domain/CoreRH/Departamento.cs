using UrreaHub.Domain.Common;

namespace UrreaHub.Domain.CoreRH;

public class Departamento : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public Guid SubareaId { get; set; }
    public Subarea Subarea { get; set; } = null!;

    public Guid? SedeId { get; set; }
    public Sede? Sede { get; set; }

    public ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();
}
