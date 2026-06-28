using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Catalogs;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class CatalogService : ICatalogService
{
    private readonly UrreaHubDbContext _db;

    public CatalogService(UrreaHubDbContext db) => _db = db;

    public async Task<IReadOnlyList<MinimumCatalogDto>> GetMinimumCatalogManifestAsync(CancellationToken ct = default)
    {
        var areasCount = await _db.Areas.CountAsync(ct);
        var subareasCount = await _db.Departamentos.CountAsync(ct);
        var cargosCount = await _db.Puestos.CountAsync(ct);
        var estadosCount = await _db.CatalogosEstados.CountAsync(ct);
        var municipiosCount = await _db.CatalogosMunicipios.CountAsync(ct);
        var razonesCount = await _db.RazonesTermino.CountAsync(r => r.Activo, ct);
        var civilesCount = await _db.CatalogosEstadosCiviles.CountAsync(ct);
        var patronalesCount = await _db.RegistrosPatronales.CountAsync(r => r.Activo, ct);
        var centrosCount = await _db.CentrosCosto.CountAsync(ct);
        var jerarquiasCount = await _db.CatalogosJerarquias.CountAsync(ct);
        var politicasCount = await _db.PoliticasVacaciones.CountAsync(ct);

        return new[]
        {
            new MinimumCatalogDto("areas-y-subareas", "Áreas y Subáreas", "/api/v1/catalogs/areas", areasCount + subareasCount, areasCount > 0 && subareasCount > 0),
            new MinimumCatalogDto("cargos", "Cargos", "/api/v1/catalogs/positions", cargosCount, cargosCount > 0),
            new MinimumCatalogDto("estados", "Estados", "/api/v1/catalogs/states", estadosCount, estadosCount >= 32),
            new MinimumCatalogDto("municipios", "Municipios", "/api/v1/catalogs/municipalities", municipiosCount, municipiosCount > 0),
            new MinimumCatalogDto("razones-termino", "Razones de Término", "/api/v1/catalogs/termination-reasons", razonesCount, razonesCount > 0),
            new MinimumCatalogDto("estados-civiles", "Estados Civiles", "/api/v1/catalogs/marital-statuses", civilesCount, civilesCount > 0),
            new MinimumCatalogDto("registros-patronales", "Registros Patronales", "/api/v1/catalogs/employer-registrations", patronalesCount, patronalesCount > 0),
            new MinimumCatalogDto("centros-costo", "Centro de Costos", "/api/v1/catalogs/cost-centers", centrosCount, centrosCount > 0),
            new MinimumCatalogDto("jerarquias", "Jerarquías", "/api/v1/catalogs/hierarchies", jerarquiasCount, jerarquiasCount > 0),
            new MinimumCatalogDto("politicas-vacaciones", "Políticas de vacaciones", "/api/v1/catalogs/vacation-policies", politicasCount, politicasCount > 0),
        };
    }

    public async Task<IReadOnlyList<AreaConSubareasDto>> GetAreasConSubareasAsync(CancellationToken ct = default)
    {
        var areas = await _db.Areas
            .AsNoTracking()
            .Include(a => a.Departamentos)
            .OrderBy(a => a.Nombre)
            .ToListAsync(ct);

        return areas.Select(a => new AreaConSubareasDto(
            a.Id,
            a.Codigo,
            a.Nombre,
            a.Descripcion,
            a.Departamentos
                .OrderBy(d => d.Nombre)
                .Select(d => new CatalogItemDto(d.Id, d.Codigo, d.Nombre, d.Descripcion))
                .ToList()
        )).ToList();
    }

    public async Task<IReadOnlyList<CatalogItemDto>> GetCargosAsync(CancellationToken ct = default) =>
        await _db.Puestos
            .AsNoTracking()
            .OrderBy(p => p.Nombre)
            .Select(p => new CatalogItemDto(p.Id, p.Codigo, p.Nombre, p.Descripcion))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CatalogoEstadoDto>> GetEstadosAsync(CancellationToken ct = default) =>
        await _db.CatalogosEstados
            .AsNoTracking()
            .OrderBy(e => e.Nombre)
            .Select(e => new CatalogoEstadoDto(e.Id, e.Codigo, e.Nombre, e.Pais))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CatalogoMunicipioDto>> GetMunicipiosAsync(Guid? estadoId = null, CancellationToken ct = default)
    {
        var query = _db.CatalogosMunicipios
            .AsNoTracking()
            .Include(m => m.Estado)
            .AsQueryable();

        if (estadoId.HasValue)
            query = query.Where(m => m.EstadoId == estadoId.Value);

        return await query
            .OrderBy(m => m.Estado!.Nombre)
            .ThenBy(m => m.Nombre)
            .Select(m => new CatalogoMunicipioDto(m.Id, m.Codigo, m.Nombre, m.EstadoId, m.Estado!.Nombre))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CatalogItemDto>> GetRazonesTerminoAsync(CancellationToken ct = default) =>
        await _db.RazonesTermino
            .AsNoTracking()
            .Where(r => r.Activo)
            .OrderBy(r => r.Nombre)
            .Select(r => new CatalogItemDto(r.Id, r.Codigo, r.Nombre, r.Descripcion))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CatalogItemDto>> GetEstadosCivilesAsync(CancellationToken ct = default) =>
        await _db.CatalogosEstadosCiviles
            .AsNoTracking()
            .OrderBy(e => e.Orden)
            .Select(e => new CatalogItemDto(e.Id, e.Codigo, e.Nombre, null))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CatalogItemDto>> GetRegistrosPatronalesAsync(CancellationToken ct = default) =>
        await _db.RegistrosPatronales
            .AsNoTracking()
            .Where(r => r.Activo)
            .OrderBy(r => r.RazonSocial)
            .Select(r => new CatalogItemDto(r.Id, r.Codigo, r.RazonSocial, r.NumeroImss))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CatalogItemDto>> GetCentrosCostoAsync(CancellationToken ct = default) =>
        await _db.CentrosCosto
            .AsNoTracking()
            .OrderBy(c => c.Nombre)
            .Select(c => new CatalogItemDto(c.Id, c.Codigo, c.Nombre, c.Descripcion))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CatalogItemDto>> GetJerarquiasAsync(CancellationToken ct = default) =>
        await _db.CatalogosJerarquias
            .AsNoTracking()
            .OrderBy(j => j.NivelOrden)
            .Select(j => new CatalogItemDto(j.Id, j.Codigo, j.Nombre, j.Descripcion))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CatalogItemDto>> GetPoliticasVacacionesAsync(CancellationToken ct = default) =>
        await _db.PoliticasVacaciones
            .AsNoTracking()
            .OrderBy(p => p.Nombre)
            .Select(p => new CatalogItemDto(p.Id, p.Nombre, p.Nombre, p.Descripcion))
            .ToListAsync(ct);
}
