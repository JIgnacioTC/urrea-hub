using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using UrreaHub.Application.Common;
using UrreaHub.Application.TI;
using UrreaHub.Domain.Plataforma;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class TiMetadataService : ITiMetadataService
{
    private readonly UrreaHubDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public TiMetadataService(UrreaHubDbContext db, IWebHostEnvironment env, IConfiguration config)
    {
        _db = db;
        _env = env;
        _config = config;
    }

    public Task<DatabaseSchemaDto> GetLiveSchemaAsync(CancellationToken ct = default)
    {
        var migrationId = _db.Database.GetMigrations().LastOrDefault() ?? "none";
        var tables = _db.Model.GetEntityTypes()
            .Where(e => !e.IsOwned())
            .OrderBy(e => e.GetSchema()).ThenBy(e => e.GetTableName())
            .Select(MapTable)
            .ToList();

        var dbName = _db.Database.GetDbConnection().Database;
        return Task.FromResult(new DatabaseSchemaDto(dbName, migrationId, tables.Count, tables));
    }

    public Task<ApiCatalogDto> GetLiveApisAsync(CancellationToken ct = default)
    {
        var apiAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == "UrreaHub.Api")
            ?? typeof(TiMetadataService).Assembly;

        var endpoints = ApiEndpointDiscovery.DiscoverFromAssembly(apiAssembly, v1Only: true);
        return Task.FromResult(new ApiCatalogDto(endpoints.Count, endpoints));
    }

    public async Task<EnvironmentInfoDto> GetEnvironmentAsync(CancellationToken ct = default)
    {
        var migrationId = (await _db.Database.GetAppliedMigrationsAsync(ct)).LastOrDefault() ?? "none";
        var conn = _db.Database.GetDbConnection();
        return new EnvironmentInfoDto(
            _env.EnvironmentName,
            "1.0.0-mvp",
            migrationId,
            conn.Database,
            DateTime.UtcNow.ToString("O"),
            new[] { AppRoles.RhAdmin, AppRoles.TiAdmin, AppRoles.Jefe, AppRoles.Colaborador });
    }

    public async Task<IReadOnlyList<MetadatoSnapshotDto>> ListSnapshotsAsync(string? tipo = null, CancellationToken ct = default)
    {
        var query = _db.MetadatosPlataforma.AsNoTracking().Where(m => m.IsActive);
        if (!string.IsNullOrWhiteSpace(tipo) && Enum.TryParse<TipoMetadatoPlataforma>(tipo, true, out var parsed))
            query = query.Where(m => m.Tipo == parsed);

        return await query
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new MetadatoSnapshotDto(
                m.Id, m.Tipo.ToString(), m.Origen.ToString(), m.Etiqueta,
                m.VersionTag, m.Notas, m.MigracionId, m.CreatedAt))
            .ToListAsync(ct);
    }

    public async Task<MetadatoSnapshotDetailDto?> GetSnapshotAsync(Guid id, CancellationToken ct = default)
    {
        var m = await _db.MetadatosPlataforma.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.IsActive, ct);
        if (m is null) return null;
        return new MetadatoSnapshotDetailDto(
            m.Id, m.Tipo.ToString(), m.Origen.ToString(), m.Etiqueta,
            m.VersionTag, m.Notas, m.ContenidoJson, m.MigracionId, m.CreatedAt);
    }

    public async Task<MetadatoSnapshotDetailDto> SaveSnapshotAsync(
        SaveSnapshotRequest request, Guid? creadoPorId, CancellationToken ct = default)
    {
        if (!Enum.TryParse<TipoMetadatoPlataforma>(request.Tipo, true, out var tipo))
            tipo = TipoMetadatoPlataforma.Combined;
        if (!Enum.TryParse<OrigenMetadato>(request.Origen, true, out var origen))
            origen = OrigenMetadato.Manual;

        var payload = new Dictionary<string, object?>();
        if (request.IncludeEnvironment)
            payload["environment"] = await GetEnvironmentAsync(ct);
        if (request.IncludeLiveSchema)
            payload["schema"] = await GetLiveSchemaAsync(ct);
        if (request.IncludeLiveApis)
            payload["apis"] = await GetLiveApisAsync(ct);

        var migrationId = (await _db.Database.GetAppliedMigrationsAsync(ct)).LastOrDefault();
        var entity = new MetadatoPlataforma
        {
            Id = Guid.NewGuid(),
            Tipo = tipo,
            Origen = origen,
            Etiqueta = request.Etiqueta,
            VersionTag = request.VersionTag,
            Notas = request.Notas,
            ContenidoJson = System.Text.Json.JsonSerializer.Serialize(payload),
            MigracionId = migrationId,
            CreadoPorColaboradorId = creadoPorId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };

        _db.MetadatosPlataforma.Add(entity);
        await _db.SaveChangesAsync(ct);

        return new MetadatoSnapshotDetailDto(
            entity.Id, entity.Tipo.ToString(), entity.Origen.ToString(), entity.Etiqueta,
            entity.VersionTag, entity.Notas, entity.ContenidoJson, entity.MigracionId, entity.CreatedAt);
    }

    private static TableSchemaDto MapTable(IEntityType entityType)
    {
        var columns = entityType.GetProperties().Select(p =>
        {
            var maxLen = p.GetMaxLength();
            return new ColumnSchemaDto(
                p.Name,
                p.ClrType.Name,
                p.IsPrimaryKey(),
                p.IsNullable,
                maxLen);
        }).ToList();

        var fks = entityType.GetForeignKeys().SelectMany(fk =>
            fk.Properties.Select(p => new ForeignKeySchemaDto(
                p.Name,
                fk.PrincipalEntityType.GetTableName() ?? fk.PrincipalEntityType.ClrType.Name,
                fk.PrincipalKey.Properties[0].Name)))
            .ToList();

        return new TableSchemaDto(
            entityType.GetSchema() ?? "dbo",
            entityType.GetTableName() ?? entityType.ClrType.Name,
            entityType.ClrType.Name,
            columns,
            fks);
    }
}
