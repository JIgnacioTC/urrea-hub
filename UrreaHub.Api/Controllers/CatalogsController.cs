using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrreaHub.Api.Routing;
using UrreaHub.Application.Catalogs;

namespace UrreaHub.Api.Controllers;

/// <summary>Catálogos mínimos BUK — diccionarios RH unificados (API-first).</summary>
[ApiController]
[Route(ApiRoutes.Catalogs)]
[Route(ApiRoutes.CatalogsLegacy)]
public class CatalogsController : ControllerBase
{
    private readonly ICatalogService _catalogs;

    public CatalogsController(ICatalogService catalogs) => _catalogs = catalogs;

    [HttpGet("minimum")]
    [Authorize(Policy = "HcmRead")]
    public Task<IReadOnlyList<MinimumCatalogDto>> Minimum(CancellationToken ct) =>
        _catalogs.GetMinimumCatalogManifestAsync(ct);

    [HttpGet("areas")]
    [Authorize(Policy = "HcmRead")]
    public Task<IReadOnlyList<AreaConSubareasDto>> Areas(CancellationToken ct) =>
        _catalogs.GetAreasConSubareasAsync(ct);

    [HttpGet("cargos")]
    [HttpGet("positions")]
    [Authorize(Policy = "HcmRead")]
    public Task<IReadOnlyList<CatalogItemDto>> Cargos(CancellationToken ct) =>
        _catalogs.GetCargosAsync(ct);

    [HttpGet("estados")]
    [HttpGet("states")]
    [Authorize(Policy = "HcmRead")]
    public Task<IReadOnlyList<CatalogoEstadoDto>> Estados(CancellationToken ct) =>
        _catalogs.GetEstadosAsync(ct);

    [HttpGet("municipios")]
    [HttpGet("municipalities")]
    [Authorize(Policy = "HcmRead")]
    public Task<IReadOnlyList<CatalogoMunicipioDto>> Municipios([FromQuery] Guid? estadoId, [FromQuery] Guid? stateId, CancellationToken ct) =>
        _catalogs.GetMunicipiosAsync(estadoId ?? stateId, ct);

    [HttpGet("razones-termino")]
    [HttpGet("termination-reasons")]
    [Authorize(Policy = "HcmRead")]
    public Task<IReadOnlyList<CatalogItemDto>> RazonesTermino(CancellationToken ct) =>
        _catalogs.GetRazonesTerminoAsync(ct);

    [HttpGet("estados-civiles")]
    [HttpGet("marital-statuses")]
    [Authorize(Policy = "HcmRead")]
    public Task<IReadOnlyList<CatalogItemDto>> EstadosCiviles(CancellationToken ct) =>
        _catalogs.GetEstadosCivilesAsync(ct);

    [HttpGet("registros-patronales")]
    [HttpGet("employer-registrations")]
    [Authorize(Policy = "HcmRead")]
    public Task<IReadOnlyList<CatalogItemDto>> RegistrosPatronales(CancellationToken ct) =>
        _catalogs.GetRegistrosPatronalesAsync(ct);

    [HttpGet("centros-costo")]
    [HttpGet("cost-centers")]
    [Authorize(Policy = "HcmRead")]
    public Task<IReadOnlyList<CatalogItemDto>> CentrosCosto(CancellationToken ct) =>
        _catalogs.GetCentrosCostoAsync(ct);

    [HttpGet("jerarquias")]
    [HttpGet("hierarchies")]
    [Authorize(Policy = "HcmRead")]
    public Task<IReadOnlyList<CatalogItemDto>> Jerarquias(CancellationToken ct) =>
        _catalogs.GetJerarquiasAsync(ct);

    [HttpGet("politicas-vacaciones")]
    [HttpGet("vacation-policies")]
    [Authorize(Policy = "HcmRead")]
    public Task<IReadOnlyList<CatalogItemDto>> PoliticasVacaciones(CancellationToken ct) =>
        _catalogs.GetPoliticasVacacionesAsync(ct);
}
