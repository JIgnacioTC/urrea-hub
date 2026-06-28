using UrreaHub.Application.Common;

namespace UrreaHub.Application.TI;

public record ColumnSchemaDto(string Name, string ClrType, bool IsKey, bool IsNullable, int? MaxLength);

public record ForeignKeySchemaDto(string Column, string ReferencedTable, string ReferencedColumn);

public record TableSchemaDto(
    string Schema,
    string Table,
    string Entity,
    IReadOnlyList<ColumnSchemaDto> Columns,
    IReadOnlyList<ForeignKeySchemaDto> ForeignKeys);

public record DatabaseSchemaDto(
    string Database,
    string MigrationId,
    int TableCount,
    IReadOnlyList<TableSchemaDto> Tables);

public record ApiEndpointDto(
    string Method,
    string Route,
    string Controller,
    string Action,
    IReadOnlyList<string> AuthPolicies);

public record ApiCatalogDto(
    int EndpointCount,
    IReadOnlyList<ApiEndpointDto> Endpoints);

public record EnvironmentInfoDto(
    string EnvironmentName,
    string ApplicationVersion,
    string MigrationId,
    string DatabaseName,
    string ServerTimeUtc,
    IReadOnlyList<string> Roles);

public record MetadatoSnapshotDto(
    Guid Id,
    string Tipo,
    string Origen,
    string Etiqueta,
    string? VersionTag,
    string? Notas,
    string? MigracionId,
    DateTime CreatedAt);

public record MetadatoSnapshotDetailDto(
    Guid Id,
    string Tipo,
    string Origen,
    string Etiqueta,
    string? VersionTag,
    string? Notas,
    string ContenidoJson,
    string? MigracionId,
    DateTime CreatedAt);

public record SaveSnapshotRequest(
    string Tipo,
    string Origen,
    string Etiqueta,
    string? VersionTag,
    string? Notas,
    bool IncludeLiveSchema,
    bool IncludeLiveApis,
    bool IncludeEnvironment);

public interface ITiMetadataService
{
    Task<DatabaseSchemaDto> GetLiveSchemaAsync(CancellationToken ct = default);
    Task<ApiCatalogDto> GetLiveApisAsync(CancellationToken ct = default);
    Task<EnvironmentInfoDto> GetEnvironmentAsync(CancellationToken ct = default);
    Task<IReadOnlyList<MetadatoSnapshotDto>> ListSnapshotsAsync(string? tipo = null, CancellationToken ct = default);
    Task<MetadatoSnapshotDetailDto?> GetSnapshotAsync(Guid id, CancellationToken ct = default);
    Task<MetadatoSnapshotDetailDto> SaveSnapshotAsync(SaveSnapshotRequest request, Guid? creadoPorId, CancellationToken ct = default);
}
