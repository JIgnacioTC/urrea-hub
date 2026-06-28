using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrreaHub.Api.Routing;
using UrreaHub.Application.Auth;
using UrreaHub.Application.Common;
using UrreaHub.Application.Portal;

namespace UrreaHub.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Portal)]
[Route(ApiRoutes.PortalLegacy)]
[Authorize]
public class PortalContentController : ControllerBase
{
    private readonly IPortalContentService _portalService;
    private readonly ICurrentUser _currentUser;

    public PortalContentController(IPortalContentService portalService, ICurrentUser currentUser)
    {
        _portalService = portalService;
        _currentUser = currentUser;
    }

    [HttpGet("feed")]
    public async Task<ActionResult<IReadOnlyList<FeedPostDto>>> GetFeed(CancellationToken cancellationToken)
        => Ok(await _portalService.GetFeedAsync(cancellationToken));

    [HttpGet("beneficios/catalogo")]
    public async Task<ActionResult<BeneficiosCatalogoDto>> GetBeneficiosCatalogo(
        [FromQuery] int? anio,
        CancellationToken cancellationToken)
        => Ok(await _portalService.GetBeneficiosCatalogoAsync(anio, cancellationToken));

    [HttpGet("beneficios/tienda/saldo")]
    public async Task<ActionResult<SaldoPuntosDto>> GetSaldo(CancellationToken cancellationToken)
        => Ok(await _portalService.GetSaldoPuntosAsync(_currentUser.ColaboradorId, cancellationToken));

    [HttpPost("beneficios/tienda/canjear")]
    public async Task<ActionResult<CanjeTiendaResultDto>> Canjear(
        [FromBody] CanjearProductoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _portalService.CanjearProductoAsync(_currentUser.ColaboradorId, request.ProductoId, cancellationToken);
        return result.Success ? Ok(result.Data) : BadRequest(new { error = result.Error });
    }

    [HttpGet("modulos/{codigo}")]
    public async Task<ActionResult<ModuloPortalDto>> GetModulo(string codigo, CancellationToken cancellationToken)
    {
        var modulo = await _portalService.GetModuloAsync(codigo, cancellationToken);
        return modulo is null ? NotFound() : Ok(modulo);
    }
}

public record CanjearProductoRequest(Guid ProductoId);

[ApiController]
[Route(ApiRoutes.RhAdminPortal)]
[Route(ApiRoutes.RhAdminPortalLegacy)]
[Authorize(Policy = "RhAdmin")]
public class AdminPortalController : ControllerBase
{
    private readonly IPortalContentService _portalService;

    public AdminPortalController(IPortalContentService portalService) => _portalService = portalService;

    [HttpGet("estado")]
    [HttpGet("status")]
    public async Task<ActionResult<PortalAdminEstadoDto>> GetEstado(CancellationToken cancellationToken)
        => Ok(await _portalService.GetAdminEstadoAsync(cancellationToken));

    [HttpGet("publicaciones")]
    [HttpGet("posts")]
    public async Task<ActionResult<IReadOnlyList<FeedPostDto>>> GetPublicaciones(CancellationToken cancellationToken)
        => Ok(await _portalService.GetAdminPublicacionesAsync(cancellationToken));

    [HttpPost("publicaciones")]
    [HttpPost("posts")]
    public async Task<ActionResult<FeedPostDto>> CrearPublicacion(
        [FromBody] UpsertPublicacionDto dto,
        CancellationToken cancellationToken)
        => Ok(await _portalService.UpsertPublicacionAsync(null, dto, cancellationToken));

    [HttpPut("publicaciones/{id:guid}")]
    public async Task<ActionResult<FeedPostDto>> ActualizarPublicacion(
        Guid id,
        [FromBody] UpsertPublicacionDto dto,
        CancellationToken cancellationToken)
        => Ok(await _portalService.UpsertPublicacionAsync(id, dto, cancellationToken));

    [HttpPut("posts/{postId:guid}")]
    public Task<ActionResult<FeedPostDto>> ActualizarPost(Guid postId, [FromBody] UpsertPublicacionDto dto, CancellationToken cancellationToken)
        => ActualizarPublicacion(postId, dto, cancellationToken);

    [HttpDelete("publicaciones/{id:guid}")]
    public async Task<IActionResult> EliminarPublicacion(Guid id, CancellationToken cancellationToken)
        => await _portalService.DeletePublicacionAsync(id, cancellationToken) ? NoContent() : NotFound();

    [HttpDelete("posts/{postId:guid}")]
    public Task<IActionResult> EliminarPost(Guid postId, CancellationToken cancellationToken)
        => EliminarPublicacion(postId, cancellationToken);

    [HttpPost("documentos")]
    public async Task<ActionResult<DocumentoCorporativoDto>> CrearDocumento(
        [FromBody] UpsertDocumentoDto dto,
        CancellationToken cancellationToken)
        => Ok(await _portalService.UpsertDocumentoAsync(null, dto, cancellationToken));

    [HttpPut("documentos/{id:guid}")]
    public async Task<ActionResult<DocumentoCorporativoDto>> ActualizarDocumento(
        Guid id,
        [FromBody] UpsertDocumentoDto dto,
        CancellationToken cancellationToken)
        => Ok(await _portalService.UpsertDocumentoAsync(id, dto, cancellationToken));

    [HttpDelete("documentos/{id:guid}")]
    public async Task<IActionResult> EliminarDocumento(Guid id, CancellationToken cancellationToken)
        => await _portalService.DeleteDocumentoAsync(id, cancellationToken) ? NoContent() : NotFound();

    [HttpPost("convenios")]
    public async Task<ActionResult<ConvenioProveedorDto>> CrearConvenio(
        [FromBody] UpsertConvenioDto dto,
        CancellationToken cancellationToken)
        => Ok(await _portalService.UpsertConvenioAsync(null, dto, cancellationToken));

    [HttpPut("convenios/{id:guid}")]
    public async Task<ActionResult<ConvenioProveedorDto>> ActualizarConvenio(
        Guid id,
        [FromBody] UpsertConvenioDto dto,
        CancellationToken cancellationToken)
        => Ok(await _portalService.UpsertConvenioAsync(id, dto, cancellationToken));

    [HttpDelete("convenios/{id:guid}")]
    public async Task<IActionResult> EliminarConvenio(Guid id, CancellationToken cancellationToken)
        => await _portalService.DeleteConvenioAsync(id, cancellationToken) ? NoContent() : NotFound();

    [HttpPost("productos")]
    public async Task<ActionResult<ProductoTiendaDto>> CrearProducto(
        [FromBody] UpsertProductoDto dto,
        CancellationToken cancellationToken)
        => Ok(await _portalService.UpsertProductoAsync(null, dto, cancellationToken));

    [HttpPut("productos/{id:guid}")]
    public async Task<ActionResult<ProductoTiendaDto>> ActualizarProducto(
        Guid id,
        [FromBody] UpsertProductoDto dto,
        CancellationToken cancellationToken)
        => Ok(await _portalService.UpsertProductoAsync(id, dto, cancellationToken));

    [HttpDelete("productos/{id:guid}")]
    public async Task<IActionResult> EliminarProducto(Guid id, CancellationToken cancellationToken)
        => await _portalService.DeleteProductoAsync(id, cancellationToken) ? NoContent() : NotFound();

    [HttpGet("modulos")]
    public async Task<ActionResult<IReadOnlyList<ModuloPortalDto>>> GetModulos(CancellationToken cancellationToken)
        => Ok(await _portalService.GetAdminModulosAsync(cancellationToken));

    [HttpPut("modulos")]
    public async Task<ActionResult<ModuloPortalDto>> ActualizarModulo(
        [FromBody] UpsertModuloDto dto,
        CancellationToken cancellationToken)
        => Ok(await _portalService.UpsertModuloAsync(dto, cancellationToken));
}
