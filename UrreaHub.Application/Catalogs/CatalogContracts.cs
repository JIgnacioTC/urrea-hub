namespace UrreaHub.Application.Catalogs;

public record CatalogItemDto(Guid Id, string Codigo, string Nombre, string? Descripcion = null);

public record CatalogoEstadoDto(Guid Id, string Codigo, string Nombre, string Pais);

public record CatalogoMunicipioDto(Guid Id, string Codigo, string Nombre, Guid EstadoId, string EstadoNombre);

public record AreaConSubareasDto(
    Guid Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    IReadOnlyList<CatalogItemDto> Subareas);

public record MinimumCatalogDto(
    string Key,
    string Nombre,
    string Endpoint,
    int Count,
    bool CumpleMinimo);

public interface ICatalogService
{
    Task<IReadOnlyList<MinimumCatalogDto>> GetMinimumCatalogManifestAsync(CancellationToken ct = default);

    Task<IReadOnlyList<AreaConSubareasDto>> GetAreasConSubareasAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CatalogItemDto>> GetCargosAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CatalogoEstadoDto>> GetEstadosAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CatalogoMunicipioDto>> GetMunicipiosAsync(Guid? estadoId = null, CancellationToken ct = default);
    Task<IReadOnlyList<CatalogItemDto>> GetRazonesTerminoAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CatalogItemDto>> GetEstadosCivilesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CatalogItemDto>> GetRegistrosPatronalesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CatalogItemDto>> GetCentrosCostoAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CatalogItemDto>> GetJerarquiasAsync(CancellationToken ct = default);
    Task<IReadOnlyList<CatalogItemDto>> GetPoliticasVacacionesAsync(CancellationToken ct = default);
}
