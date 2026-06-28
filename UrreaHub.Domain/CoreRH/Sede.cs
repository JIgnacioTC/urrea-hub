using UrreaHub.Domain.Common;

namespace UrreaHub.Domain.CoreRH;

public class Sede : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? Ciudad { get; set; }
    public string? Pais { get; set; }

    public ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();
    public ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();
}
