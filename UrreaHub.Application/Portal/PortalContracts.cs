using UrreaHub.Application.Common;
using UrreaHub.Domain.Common;

namespace UrreaHub.Application.Portal;

public record FeedPostDto(
    Guid Id,
    string AuthorName,
    string AuthorRole,
    string AuthorInitials,
    string Department,
    string Content,
    string? ImageGradient,
    int Likes,
    int Comments,
    int Shares,
    DateTime CreatedAt,
    string Type,
    bool LikedByMe,
    bool IsOwnPost);

public record CrearPublicacionDto(string Contenido, string? GradienteImagen, TipoPublicacionPortal Tipo);

public record ComentarioDto(
    Guid Id,
    string AuthorName,
    string AuthorInitials,
    string Content,
    DateTime CreatedAt,
    bool IsOwnComment);

public record CrearComentarioDto(string Contenido);

public record ToggleReaccionResultDto(bool Liked, int TotalLikes);

public record DocumentoCorporativoDto(
    Guid Id,
    string Codigo,
    string Categoria,
    string Titulo,
    string Descripcion,
    string? Icono,
    DateTime Actualizado,
    int? Paginas,
    string? UrlDocumento);

public record DiaFestivoDto(DateTime Fecha, string Nombre, string Tipo);

public record ConvenioProveedorDto(
    Guid Id,
    string Codigo,
    string Proveedor,
    string Categoria,
    string Descuento,
    string Descripcion,
    string? Icono,
    string Vigencia,
    string? CodigoPromocional);

public record ProductoTiendaDto(
    Guid Id,
    string Codigo,
    string Nombre,
    string Categoria,
    int Puntos,
    int Stock,
    string? Icono,
    string? Gradiente,
    bool Destacado);

public record BeneficiosCatalogoDto(
    IReadOnlyDictionary<string, string> CategoriaLabels,
    IReadOnlyList<DocumentoCorporativoDto> Documentos,
    IReadOnlyList<DiaFestivoDto> Festivos,
    IReadOnlyList<ConvenioProveedorDto> Convenios,
    IReadOnlyList<ProductoTiendaDto> Productos);

public record SaldoPuntosDto(int Puntos);

public record CanjeTiendaResultDto(Guid CanjeId, int PuntosRestantes, int StockRestante);

public record ModuloPortalDto(
    string CodigoModulo,
    string Titulo,
    string? Subtitulo,
    string? Descripcion,
    string? Icono,
    bool Publicado);

public record IntegracionEstadoDto(
    Guid Id,
    string Nombre,
    string SistemaExterno,
    string? Endpoint,
    string Estado,
    DateTime? UltimaEjecucion);

public record PortalAdminEstadoDto(
    bool DatabaseOk,
    string Entorno,
    string ConnectionInfo,
    int Publicaciones,
    int Documentos,
    int Convenios,
    int Productos,
    int Modulos,
    int ColaboradoresActivos,
    IReadOnlyList<IntegracionEstadoDto> Integraciones);

public record UpsertPublicacionDto(
    string AutorNombre,
    string AutorRol,
    string AutorIniciales,
    string Departamento,
    string Contenido,
    string? GradienteImagen,
    int Likes,
    int Comentarios,
    int Compartidos,
    DateTime FechaPublicacion,
    TipoPublicacionPortal Tipo);

public record UpsertDocumentoDto(
    string Codigo,
    CategoriaDocumentoCorporativo Categoria,
    string Titulo,
    string Descripcion,
    string? Icono,
    int? Paginas,
    string? UrlDocumento,
    DateTime FechaActualizacion,
    int Orden);

public record UpsertConvenioDto(
    string Codigo,
    string Proveedor,
    string Categoria,
    string Descuento,
    string Descripcion,
    string? Icono,
    string Vigencia,
    string? CodigoPromocional,
    int Orden);

public record UpsertProductoDto(
    string Codigo,
    string Nombre,
    string Categoria,
    int PuntosRequeridos,
    int Stock,
    string? Icono,
    string? Gradiente,
    bool Destacado,
    int Orden);

public record UpsertModuloDto(
    string CodigoModulo,
    string Titulo,
    string? Subtitulo,
    string? Descripcion,
    string? Icono,
    int Orden,
    bool Publicado);

public interface IPortalContentService
{
    Task<IReadOnlyList<FeedPostDto>> GetFeedAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
    Task<Result<FeedPostDto>> CrearPublicacionAsync(Guid colaboradorId, CrearPublicacionDto dto, CancellationToken cancellationToken = default);
    Task<Result<bool>> EliminarPublicacionPropiaAsync(Guid colaboradorId, Guid publicacionId, CancellationToken cancellationToken = default);
    Task<Result<ToggleReaccionResultDto>> ToggleReaccionAsync(Guid colaboradorId, Guid publicacionId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ComentarioDto>>> GetComentariosAsync(Guid colaboradorId, Guid publicacionId, CancellationToken cancellationToken = default);
    Task<Result<ComentarioDto>> CrearComentarioAsync(Guid colaboradorId, Guid publicacionId, CrearComentarioDto dto, CancellationToken cancellationToken = default);
    Task<BeneficiosCatalogoDto> GetBeneficiosCatalogoAsync(int? anioFestivos, CancellationToken cancellationToken = default);
    Task<SaldoPuntosDto> GetSaldoPuntosAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
    Task<Result<CanjeTiendaResultDto>> CanjearProductoAsync(Guid colaboradorId, Guid productoId, CancellationToken cancellationToken = default);
    Task<ModuloPortalDto?> GetModuloAsync(string codigoModulo, CancellationToken cancellationToken = default);

    Task<PortalAdminEstadoDto> GetAdminEstadoAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeedPostDto>> GetAdminPublicacionesAsync(CancellationToken cancellationToken = default);
    Task<FeedPostDto> UpsertPublicacionAsync(Guid? id, UpsertPublicacionDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeletePublicacionAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DocumentoCorporativoDto> UpsertDocumentoAsync(Guid? id, UpsertDocumentoDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteDocumentoAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ConvenioProveedorDto> UpsertConvenioAsync(Guid? id, UpsertConvenioDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteConvenioAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductoTiendaDto> UpsertProductoAsync(Guid? id, UpsertProductoDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteProductoAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ModuloPortalDto>> GetAdminModulosAsync(CancellationToken cancellationToken = default);
    Task<ModuloPortalDto> UpsertModuloAsync(UpsertModuloDto dto, CancellationToken cancellationToken = default);
}
