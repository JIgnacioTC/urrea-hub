using UrreaHub.Domain.Common;
using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Domain.Seguridad;

public class Rol : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
    public ICollection<ColaboradorRol> ColaboradorRoles { get; set; } = new List<ColaboradorRol>();
}

public class Permiso : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Modulo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
}

public class RolPermiso : BaseEntity
{
    public Guid RolId { get; set; }
    public Rol Rol { get; set; } = null!;

    public Guid PermisoId { get; set; }
    public Permiso Permiso { get; set; } = null!;
}

public class ColaboradorRol : BaseEntity
{
    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public Guid RolId { get; set; }
    public Rol Rol { get; set; } = null!;
}
