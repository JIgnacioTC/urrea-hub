using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using UrreaHub.Application.Common;
using UrreaHub.Application.Portal;
using UrreaHub.Domain.Beneficios;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.Portal;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class PortalContentService : IPortalContentService
{
    private static readonly IReadOnlyDictionary<string, string> CategoriaLabels = new Dictionary<string, string>
    {
        ["reglamentos"] = "Reglamentos internos",
        ["politicas"] = "Políticas y procedimientos",
        ["etica"] = "Código de ética",
        ["prevision"] = "Previsión social",
        ["prestaciones"] = "Prestaciones",
        ["puntualidad"] = "Puntualidad",
    };

    private readonly UrreaHubDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public PortalContentService(UrreaHubDbContext context, IConfiguration configuration, IHostEnvironment environment)
    {
        _context = context;
        _configuration = configuration;
        _environment = environment;
    }

    public async Task<IReadOnlyList<FeedPostDto>> GetFeedAsync(Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        var posts = await _context.PublicacionesPortal.AsNoTracking()
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.FechaPublicacion)
            .ToListAsync(cancellationToken);

        var postIds = posts.Select(p => p.Id).ToList();
        var likedIds = (await _context.ReaccionesPublicacion.AsNoTracking()
            .Where(r => r.ColaboradorId == colaboradorId && postIds.Contains(r.PublicacionId))
            .Select(r => r.PublicacionId)
            .ToListAsync(cancellationToken))
            .ToHashSet();

        return posts.Select(p => MapFeed(p, colaboradorId, likedIds.Contains(p.Id))).ToList();
    }

    public async Task<Result<FeedPostDto>> CrearPublicacionAsync(Guid colaboradorId, CrearPublicacionDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Contenido))
            return Result<FeedPostDto>.Fail("El contenido no puede estar vacío.");

        var colaborador = await _context.Colaboradores.AsNoTracking()
            .Include(c => c.Puesto)
            .Include(c => c.Departamento)
            .FirstOrDefaultAsync(c => c.Id == colaboradorId, cancellationToken);
        if (colaborador is null)
            return Result<FeedPostDto>.Fail("Colaborador no encontrado.");

        var now = DateTime.UtcNow;
        var entity = new PublicacionPortal
        {
            Id = Guid.NewGuid(),
            AutorNombre = $"{colaborador.Nombre} {colaborador.ApellidoPaterno}".Trim(),
            AutorRol = colaborador.Puesto?.Nombre ?? "",
            AutorIniciales = BuildInitials(colaborador.Nombre, colaborador.ApellidoPaterno),
            Departamento = colaborador.Departamento?.Nombre ?? "",
            Contenido = dto.Contenido.Trim(),
            GradienteImagen = dto.GradienteImagen,
            Likes = 0,
            Comentarios = 0,
            Compartidos = 0,
            FechaPublicacion = now,
            Tipo = dto.Tipo,
            ColaboradorId = colaboradorId,
            CreatedAt = now,
            IsActive = true,
        };
        _context.PublicacionesPortal.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<FeedPostDto>.Ok(MapFeed(entity, colaboradorId, likedByMe: false));
    }

    public async Task<Result<bool>> EliminarPublicacionPropiaAsync(Guid colaboradorId, Guid publicacionId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.PublicacionesPortal
            .FirstOrDefaultAsync(p => p.Id == publicacionId && p.IsActive, cancellationToken);
        if (entity is null)
            return Result<bool>.Fail("Publicación no encontrada.");
        if (entity.ColaboradorId != colaboradorId)
            return Result<bool>.Fail("No puedes eliminar publicaciones de otros colaboradores.");

        entity.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Ok(true);
    }

    public async Task<Result<ToggleReaccionResultDto>> ToggleReaccionAsync(Guid colaboradorId, Guid publicacionId, CancellationToken cancellationToken = default)
    {
        var publicacion = await _context.PublicacionesPortal
            .FirstOrDefaultAsync(p => p.Id == publicacionId && p.IsActive, cancellationToken);
        if (publicacion is null)
            return Result<ToggleReaccionResultDto>.Fail("Publicación no encontrada.");

        var reaccion = await _context.ReaccionesPublicacion
            .FirstOrDefaultAsync(r => r.PublicacionId == publicacionId && r.ColaboradorId == colaboradorId, cancellationToken);

        bool nowLiked;
        if (reaccion is null)
        {
            _context.ReaccionesPublicacion.Add(new ReaccionPublicacion
            {
                Id = Guid.NewGuid(),
                PublicacionId = publicacionId,
                ColaboradorId = colaboradorId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            });
            publicacion.Likes += 1;
            nowLiked = true;
        }
        else
        {
            _context.ReaccionesPublicacion.Remove(reaccion);
            publicacion.Likes = Math.Max(0, publicacion.Likes - 1);
            nowLiked = false;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result<ToggleReaccionResultDto>.Ok(new ToggleReaccionResultDto(nowLiked, publicacion.Likes));
    }

    public async Task<Result<IReadOnlyList<ComentarioDto>>> GetComentariosAsync(Guid colaboradorId, Guid publicacionId, CancellationToken cancellationToken = default)
    {
        var existe = await _context.PublicacionesPortal.AsNoTracking()
            .AnyAsync(p => p.Id == publicacionId && p.IsActive, cancellationToken);
        if (!existe)
            return Result<IReadOnlyList<ComentarioDto>>.Fail("Publicación no encontrada.");

        var comentarios = await _context.ComentariosPublicacion.AsNoTracking()
            .Where(c => c.PublicacionId == publicacionId && c.IsActive)
            .Include(c => c.Colaborador)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        var dtos = comentarios.Select(c => MapComentario(c, colaboradorId)).ToList();
        return Result<IReadOnlyList<ComentarioDto>>.Ok(dtos);
    }

    public async Task<Result<ComentarioDto>> CrearComentarioAsync(Guid colaboradorId, Guid publicacionId, CrearComentarioDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Contenido))
            return Result<ComentarioDto>.Fail("El comentario no puede estar vacío.");

        var publicacion = await _context.PublicacionesPortal
            .FirstOrDefaultAsync(p => p.Id == publicacionId && p.IsActive, cancellationToken);
        if (publicacion is null)
            return Result<ComentarioDto>.Fail("Publicación no encontrada.");

        var colaborador = await _context.Colaboradores.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == colaboradorId, cancellationToken);
        if (colaborador is null)
            return Result<ComentarioDto>.Fail("Colaborador no encontrado.");

        var now = DateTime.UtcNow;
        var entity = new ComentarioPublicacion
        {
            Id = Guid.NewGuid(),
            PublicacionId = publicacionId,
            ColaboradorId = colaboradorId,
            Contenido = dto.Contenido.Trim(),
            CreatedAt = now,
            IsActive = true,
        };
        _context.ComentariosPublicacion.Add(entity);
        publicacion.Comentarios += 1;
        await _context.SaveChangesAsync(cancellationToken);

        var authorName = $"{colaborador.Nombre} {colaborador.ApellidoPaterno}".Trim();
        return Result<ComentarioDto>.Ok(new ComentarioDto(
            entity.Id, authorName, BuildInitials(colaborador.Nombre, colaborador.ApellidoPaterno),
            entity.Contenido, entity.CreatedAt, IsOwnComment: true));
    }

    public async Task<BeneficiosCatalogoDto> GetBeneficiosCatalogoAsync(int? anioFestivos, CancellationToken cancellationToken = default)
    {
        var anio = anioFestivos ?? DateTime.UtcNow.Year;
        var documentos = await _context.DocumentosCorporativos.AsNoTracking()
            .Where(d => d.IsActive)
            .OrderBy(d => d.Orden).ThenBy(d => d.Titulo)
            .Select(d => MapDocumento(d))
            .ToListAsync(cancellationToken);

        var festivos = await _context.DiasFestivosCorporativos.AsNoTracking()
            .Where(d => d.IsActive && d.Anio == anio)
            .OrderBy(d => d.Fecha)
            .Select(d => new DiaFestivoDto(d.Fecha, d.Nombre, d.Tipo == TipoDiaFestivo.Oficial ? "Oficial" : "Empresa"))
            .ToListAsync(cancellationToken);

        var convenios = await _context.ConveniosProveedores.AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Orden).ThenBy(c => c.Proveedor)
            .Select(c => MapConvenio(c))
            .ToListAsync(cancellationToken);

        var productos = await _context.ProductosTiendaInterna.AsNoTracking()
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.Destacado).ThenBy(p => p.Orden)
            .Select(p => MapProducto(p))
            .ToListAsync(cancellationToken);

        return new BeneficiosCatalogoDto(CategoriaLabels, documentos, festivos, convenios, productos);
    }

    public async Task<SaldoPuntosDto> GetSaldoPuntosAsync(Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        var saldo = await EnsureSaldoAsync(colaboradorId, cancellationToken);
        return new SaldoPuntosDto(saldo.Puntos);
    }

    public async Task<Result<CanjeTiendaResultDto>> CanjearProductoAsync(Guid colaboradorId, Guid productoId, CancellationToken cancellationToken = default)
    {
        var producto = await _context.ProductosTiendaInterna
            .FirstOrDefaultAsync(p => p.Id == productoId && p.IsActive, cancellationToken);
        if (producto is null)
            return Result<CanjeTiendaResultDto>.Fail("Producto no encontrado.");
        if (producto.Stock <= 0)
            return Result<CanjeTiendaResultDto>.Fail("Sin stock disponible.");

        var saldo = await EnsureSaldoAsync(colaboradorId, cancellationToken);
        if (saldo.Puntos < producto.PuntosRequeridos)
            return Result<CanjeTiendaResultDto>.Fail("Puntos insuficientes.");

        saldo.Puntos -= producto.PuntosRequeridos;
        producto.Stock -= 1;
        var canjeId = Guid.NewGuid();
        _context.CanjesTiendaInterna.Add(new CanjeTiendaInterna
        {
            Id = canjeId,
            ColaboradorId = colaboradorId,
            ProductoId = productoId,
            PuntosUsados = producto.PuntosRequeridos,
            FechaCanje = DateTime.UtcNow,
            Estado = "PendienteEntrega",
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        });
        await _context.SaveChangesAsync(cancellationToken);
        return Result<CanjeTiendaResultDto>.Ok(new CanjeTiendaResultDto(canjeId, saldo.Puntos, producto.Stock));
    }

    public async Task<ModuloPortalDto?> GetModuloAsync(string codigoModulo, CancellationToken cancellationToken = default)
    {
        var modulo = await _context.ContenidosModuloPortal.AsNoTracking()
            .FirstOrDefaultAsync(m => m.IsActive && m.CodigoModulo == codigoModulo && m.Publicado, cancellationToken);
        return modulo is null ? null : MapModulo(modulo);
    }

    public async Task<PortalAdminEstadoDto> GetAdminEstadoAsync(CancellationToken cancellationToken = default)
    {
        var dbOk = await _context.Database.CanConnectAsync(cancellationToken);
        var cs = _configuration.GetConnectionString("UrreaHubDb") ?? "";
        var masked = MaskConnectionString(cs);

        var integraciones = await _context.Integraciones.AsNoTracking()
            .Where(i => i.IsActive)
            .OrderBy(i => i.Nombre)
            .Select(i => new IntegracionEstadoDto(
                i.Id, i.Nombre, i.SistemaExterno, i.Endpoint, i.Estado.ToString(), i.UltimaEjecucion))
            .ToListAsync(cancellationToken);

        return new PortalAdminEstadoDto(
            dbOk,
            _environment.EnvironmentName,
            masked,
            await _context.PublicacionesPortal.CountAsync(p => p.IsActive, cancellationToken),
            await _context.DocumentosCorporativos.CountAsync(d => d.IsActive, cancellationToken),
            await _context.ConveniosProveedores.CountAsync(c => c.IsActive, cancellationToken),
            await _context.ProductosTiendaInterna.CountAsync(p => p.IsActive, cancellationToken),
            await _context.ContenidosModuloPortal.CountAsync(m => m.IsActive, cancellationToken),
            await _context.Colaboradores.CountAsync(c => c.IsActive, cancellationToken),
            integraciones);
    }

    public async Task<IReadOnlyList<FeedPostDto>> GetAdminPublicacionesAsync(CancellationToken cancellationToken = default)
        => await GetFeedAsync(Guid.Empty, cancellationToken);

    public async Task<FeedPostDto> UpsertPublicacionAsync(Guid? id, UpsertPublicacionDto dto, CancellationToken cancellationToken = default)
    {
        PublicacionPortal entity;
        if (id is Guid existingId)
        {
            entity = await _context.PublicacionesPortal.FirstAsync(p => p.Id == existingId, cancellationToken);
        }
        else
        {
            entity = new PublicacionPortal { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, IsActive = true };
            _context.PublicacionesPortal.Add(entity);
        }

        entity.AutorNombre = dto.AutorNombre;
        entity.AutorRol = dto.AutorRol;
        entity.AutorIniciales = dto.AutorIniciales;
        entity.Departamento = dto.Departamento;
        entity.Contenido = dto.Contenido;
        entity.GradienteImagen = dto.GradienteImagen;
        entity.Likes = dto.Likes;
        entity.Comentarios = dto.Comentarios;
        entity.Compartidos = dto.Compartidos;
        entity.FechaPublicacion = dto.FechaPublicacion;
        entity.Tipo = dto.Tipo;
        await _context.SaveChangesAsync(cancellationToken);
        return MapFeed(entity, Guid.Empty, likedByMe: false);
    }

    public async Task<bool> DeletePublicacionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.PublicacionesPortal.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<DocumentoCorporativoDto> UpsertDocumentoAsync(Guid? id, UpsertDocumentoDto dto, CancellationToken cancellationToken = default)
    {
        DocumentoCorporativo entity;
        if (id is Guid existingId)
            entity = await _context.DocumentosCorporativos.FirstAsync(d => d.Id == existingId, cancellationToken);
        else
        {
            entity = new DocumentoCorporativo { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, IsActive = true };
            _context.DocumentosCorporativos.Add(entity);
        }

        entity.Codigo = dto.Codigo;
        entity.Categoria = dto.Categoria;
        entity.Titulo = dto.Titulo;
        entity.Descripcion = dto.Descripcion;
        entity.Icono = dto.Icono;
        entity.Paginas = dto.Paginas;
        entity.UrlDocumento = dto.UrlDocumento;
        entity.FechaActualizacion = dto.FechaActualizacion;
        entity.Orden = dto.Orden;
        await _context.SaveChangesAsync(cancellationToken);
        return MapDocumento(entity);
    }

    public async Task<bool> DeleteDocumentoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.DocumentosCorporativos.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ConvenioProveedorDto> UpsertConvenioAsync(Guid? id, UpsertConvenioDto dto, CancellationToken cancellationToken = default)
    {
        ConvenioProveedor entity;
        if (id is Guid existingId)
            entity = await _context.ConveniosProveedores.FirstAsync(c => c.Id == existingId, cancellationToken);
        else
        {
            entity = new ConvenioProveedor { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, IsActive = true };
            _context.ConveniosProveedores.Add(entity);
        }

        entity.Codigo = dto.Codigo;
        entity.Proveedor = dto.Proveedor;
        entity.Categoria = dto.Categoria;
        entity.Descuento = dto.Descuento;
        entity.Descripcion = dto.Descripcion;
        entity.Icono = dto.Icono;
        entity.Vigencia = dto.Vigencia;
        entity.CodigoPromocional = dto.CodigoPromocional;
        entity.Orden = dto.Orden;
        await _context.SaveChangesAsync(cancellationToken);
        return MapConvenio(entity);
    }

    public async Task<bool> DeleteConvenioAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ConveniosProveedores.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ProductoTiendaDto> UpsertProductoAsync(Guid? id, UpsertProductoDto dto, CancellationToken cancellationToken = default)
    {
        ProductoTiendaInterna entity;
        if (id is Guid existingId)
            entity = await _context.ProductosTiendaInterna.FirstAsync(p => p.Id == existingId, cancellationToken);
        else
        {
            entity = new ProductoTiendaInterna { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, IsActive = true };
            _context.ProductosTiendaInterna.Add(entity);
        }

        entity.Codigo = dto.Codigo;
        entity.Nombre = dto.Nombre;
        entity.Categoria = dto.Categoria;
        entity.PuntosRequeridos = dto.PuntosRequeridos;
        entity.Stock = dto.Stock;
        entity.Icono = dto.Icono;
        entity.Gradiente = dto.Gradiente;
        entity.Destacado = dto.Destacado;
        entity.Orden = dto.Orden;
        await _context.SaveChangesAsync(cancellationToken);
        return MapProducto(entity);
    }

    public async Task<bool> DeleteProductoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ProductosTiendaInterna.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (entity is null) return false;
        entity.IsActive = false;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<ModuloPortalDto>> GetAdminModulosAsync(CancellationToken cancellationToken = default)
        => await _context.ContenidosModuloPortal.AsNoTracking()
            .Where(m => m.IsActive)
            .OrderBy(m => m.Orden)
            .Select(m => MapModulo(m))
            .ToListAsync(cancellationToken);

    public async Task<ModuloPortalDto> UpsertModuloAsync(UpsertModuloDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _context.ContenidosModuloPortal
            .FirstOrDefaultAsync(m => m.CodigoModulo == dto.CodigoModulo, cancellationToken);
        if (entity is null)
        {
            entity = new ContenidoModuloPortal
            {
                Id = Guid.NewGuid(),
                CodigoModulo = dto.CodigoModulo,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };
            _context.ContenidosModuloPortal.Add(entity);
        }

        entity.Titulo = dto.Titulo;
        entity.Subtitulo = dto.Subtitulo;
        entity.Descripcion = dto.Descripcion;
        entity.Icono = dto.Icono;
        entity.Orden = dto.Orden;
        entity.Publicado = dto.Publicado;
        await _context.SaveChangesAsync(cancellationToken);
        return MapModulo(entity);
    }

    private async Task<SaldoPuntosColaborador> EnsureSaldoAsync(Guid colaboradorId, CancellationToken cancellationToken)
    {
        var saldo = await _context.SaldosPuntosColaboradores
            .FirstOrDefaultAsync(s => s.ColaboradorId == colaboradorId, cancellationToken);
        if (saldo is not null) return saldo;

        saldo = new SaldoPuntosColaborador
        {
            Id = Guid.NewGuid(),
            ColaboradorId = colaboradorId,
            Puntos = 850,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };
        _context.SaldosPuntosColaboradores.Add(saldo);
        await _context.SaveChangesAsync(cancellationToken);
        return saldo;
    }

    private static FeedPostDto MapFeed(PublicacionPortal p, Guid colaboradorId, bool likedByMe) => new(
        p.Id, p.AutorNombre, p.AutorRol, p.AutorIniciales, p.Departamento, p.Contenido,
        p.GradienteImagen, p.Likes, p.Comentarios, p.Compartidos, p.FechaPublicacion,
        p.Tipo switch
        {
            TipoPublicacionPortal.Announcement => "announcement",
            TipoPublicacionPortal.Recognition => "recognition",
            TipoPublicacionPortal.Event => "event",
            _ => "general",
        },
        likedByMe,
        p.ColaboradorId.HasValue && p.ColaboradorId == colaboradorId);

    private static ComentarioDto MapComentario(ComentarioPublicacion c, Guid colaboradorId) => new(
        c.Id,
        $"{c.Colaborador.Nombre} {c.Colaborador.ApellidoPaterno}".Trim(),
        BuildInitials(c.Colaborador.Nombre, c.Colaborador.ApellidoPaterno),
        c.Contenido,
        c.CreatedAt,
        c.ColaboradorId == colaboradorId);

    private static string BuildInitials(string nombre, string apellidoPaterno)
    {
        var a = nombre.Length > 0 ? nombre[..1] : "";
        var b = apellidoPaterno.Length > 0 ? apellidoPaterno[..1] : "";
        return (a + b).ToUpperInvariant();
    }

    private static DocumentoCorporativoDto MapDocumento(DocumentoCorporativo d) => new(
        d.Id, d.Codigo, CategoriaToSlug(d.Categoria), d.Titulo, d.Descripcion, d.Icono,
        d.FechaActualizacion, d.Paginas, d.UrlDocumento);

    private static ConvenioProveedorDto MapConvenio(ConvenioProveedor c) => new(
        c.Id, c.Codigo, c.Proveedor, c.Categoria, c.Descuento, c.Descripcion, c.Icono,
        c.Vigencia, c.CodigoPromocional);

    private static ProductoTiendaDto MapProducto(ProductoTiendaInterna p) => new(
        p.Id, p.Codigo, p.Nombre, p.Categoria, p.PuntosRequeridos, p.Stock, p.Icono, p.Gradiente, p.Destacado);

    private static ModuloPortalDto MapModulo(ContenidoModuloPortal m) => new(
        m.CodigoModulo, m.Titulo, m.Subtitulo, m.Descripcion, m.Icono, m.Publicado);

    private static string CategoriaToSlug(CategoriaDocumentoCorporativo c) => c switch
    {
        CategoriaDocumentoCorporativo.Reglamentos => "reglamentos",
        CategoriaDocumentoCorporativo.Politicas => "politicas",
        CategoriaDocumentoCorporativo.Etica => "etica",
        CategoriaDocumentoCorporativo.Prevision => "prevision",
        CategoriaDocumentoCorporativo.Prestaciones => "prestaciones",
        _ => "puntualidad",
    };

    private static string MaskConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) return "No configurada";
        var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(";", parts.Select(p =>
        {
            if (p.TrimStart().StartsWith("Password=", StringComparison.OrdinalIgnoreCase) ||
                p.TrimStart().StartsWith("Pwd=", StringComparison.OrdinalIgnoreCase))
                return "Password=***";
            return p;
        }));
    }
}
