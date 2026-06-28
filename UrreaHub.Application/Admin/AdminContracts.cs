using UrreaHub.Application.Common;

namespace UrreaHub.Application.Admin;

public record EntityRegistryDto(
    string Name,
    string Schema,
    string Table,
    int ColumnCount,
    string AdminRoute);

public record EntityPageDto(
    string Entity,
    int Total,
    int Page,
    int PageSize,
    IReadOnlyList<object> Items);

public interface IGenericAdminService
{
    IReadOnlyList<EntityRegistryDto> ListEntities();
    Task<EntityPageDto> ListRecordsAsync(string entityName, int page, int pageSize, bool includeInactive, CancellationToken ct = default);
    Task<object?> GetRecordAsync(string entityName, Guid id, CancellationToken ct = default);
    Task<Guid> CreateRecordAsync(string entityName, object payload, CancellationToken ct = default);
    Task UpdateRecordAsync(string entityName, Guid id, object payload, CancellationToken ct = default);
    Task DeleteRecordAsync(string entityName, Guid id, CancellationToken ct = default);
}
