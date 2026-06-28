using UrreaHub.Domain.Common;
using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Domain.Portal;

public class PublicacionPortal : BaseEntity
{
    public string AutorNombre { get; set; } = string.Empty;
    public string AutorRol { get; set; } = string.Empty;
    public string AutorIniciales { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public string? GradienteImagen { get; set; }
    public int Likes { get; set; }
    public int Comentarios { get; set; }
    public int Compartidos { get; set; }
    public DateTime FechaPublicacion { get; set; }
    public TipoPublicacionPortal Tipo { get; set; } = TipoPublicacionPortal.General;
    public Guid? ColaboradorId { get; set; }
    public Colaborador? Colaborador { get; set; }
}

public class ContenidoModuloPortal : BaseEntity
{
    public string CodigoModulo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string? Subtitulo { get; set; }
    public string? Descripcion { get; set; }
    public string? Icono { get; set; }
    public int Orden { get; set; }
    public bool Publicado { get; set; } = true;
}
