using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Admin;
using UrreaHub.Domain.Common;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class GenericAdminService : IGenericAdminService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly UrreaHubDbContext _db;

    public GenericAdminService(UrreaHubDbContext db) => _db = db;

    public IReadOnlyList<EntityRegistryDto> ListEntities() =>
        _db.Model.GetEntityTypes()
            .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType) && !e.IsOwned())
            .OrderBy(e => e.GetSchema()).ThenBy(e => e.GetTableName())
            .Select(e => new EntityRegistryDto(
                e.ClrType.Name,
                e.GetSchema() ?? "dbo",
                e.GetTableName() ?? e.ClrType.Name,
                e.GetProperties().Count(),
                $"/api/v1/admin/entities/{e.ClrType.Name}"))
            .ToList();

    public Task<EntityPageDto> ListRecordsAsync(
        string entityName, int page, int pageSize, bool includeInactive, CancellationToken ct = default) =>
        InvokeGeneric<EntityPageDto>(nameof(ListRecordsCoreAsync), entityName, page, pageSize, includeInactive, ct);

    public Task<object?> GetRecordAsync(string entityName, Guid id, CancellationToken ct = default) =>
        InvokeGeneric<object?>(nameof(GetRecordCoreAsync), entityName, id, ct);

    public Task<Guid> CreateRecordAsync(string entityName, object payload, CancellationToken ct = default) =>
        InvokeGeneric<Guid>(nameof(CreateRecordCoreAsync), entityName, payload, ct);

    public Task UpdateRecordAsync(string entityName, Guid id, object payload, CancellationToken ct = default) =>
        InvokeGenericTask(nameof(UpdateRecordCoreAsync), entityName, id, payload, ct);

    public Task DeleteRecordAsync(string entityName, Guid id, CancellationToken ct = default) =>
        InvokeGenericTask(nameof(DeleteRecordCoreAsync), entityName, id, ct);

    private async Task<EntityPageDto> ListRecordsCoreAsync<T>(
        int page, int pageSize, bool includeInactive, CancellationToken ct) where T : BaseEntity
    {
        var query = _db.Set<T>().AsNoTracking().AsQueryable();
        if (!includeInactive)
            query = query.Where(e => e.IsActive);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new EntityPageDto(typeof(T).Name, total, page, pageSize, items.Cast<object>().ToList());
    }

    private async Task<object?> GetRecordCoreAsync<T>(Guid id, CancellationToken ct) where T : BaseEntity =>
        await _db.Set<T>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);

    private async Task<Guid> CreateRecordCoreAsync<T>(object payload, CancellationToken ct) where T : BaseEntity
    {
        var json = JsonSerializer.Serialize(payload, JsonOptions);
        var entity = JsonSerializer.Deserialize<T>(json, JsonOptions)!;
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _db.Set<T>().Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.Id;
    }

    private async Task UpdateRecordCoreAsync<T>(Guid id, object payload, CancellationToken ct) where T : BaseEntity
    {
        var existing = await _db.Set<T>().FirstOrDefaultAsync(e => e.Id == id, ct)
            ?? throw new KeyNotFoundException($"Registro {id} no encontrado en {typeof(T).Name}.");

        var json = JsonSerializer.Serialize(payload, JsonOptions);
        var updated = JsonSerializer.Deserialize<T>(json, JsonOptions)!;
        _db.Entry(existing).CurrentValues.SetValues(updated);
        existing.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private async Task DeleteRecordCoreAsync<T>(Guid id, CancellationToken ct) where T : BaseEntity
    {
        var existing = await _db.Set<T>().FirstOrDefaultAsync(e => e.Id == id, ct)
            ?? throw new KeyNotFoundException($"Registro {id} no encontrado en {typeof(T).Name}.");

        existing.IsActive = false;
        existing.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    private Task InvokeGenericTask(string methodName, string entityName, params object?[] args)
    {
        var clrType = ResolveEntityType(entityName);
        var method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!
            .MakeGenericMethod(clrType);
        return (Task)method.Invoke(this, args)!;
    }

    private Task<TResult> InvokeGeneric<TResult>(string methodName, string entityName, params object?[] args)
    {
        var clrType = ResolveEntityType(entityName);
        var method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!
            .MakeGenericMethod(clrType);
        return (Task<TResult>)method.Invoke(this, args)!;
    }

    private Type ResolveEntityType(string entityName)
    {
        var match = _db.Model.GetEntityTypes()
            .FirstOrDefault(e => e.ClrType.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase));
        if (match is null || !typeof(BaseEntity).IsAssignableFrom(match.ClrType))
            throw new ArgumentException($"Entidad '{entityName}' no encontrada o no administrable.");
        return match.ClrType;
    }
}
