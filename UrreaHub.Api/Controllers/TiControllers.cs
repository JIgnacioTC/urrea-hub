using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrreaHub.Api.Routing;
using UrreaHub.Application.Admin;
using UrreaHub.Application.TI;

namespace UrreaHub.Api.Controllers;

[ApiController]
[Route(ApiRoutes.TiMetadata)]
[Route(ApiRoutes.TiMetadataLegacy)]
[Authorize(Policy = "TiMetadataRead")]
public class TiMetadataController : ControllerBase
{
    private readonly ITiMetadataService _metadata;

    public TiMetadataController(ITiMetadataService metadata) => _metadata = metadata;

    [HttpGet("schema")]
    public Task<DatabaseSchemaDto> Schema(CancellationToken ct) => _metadata.GetLiveSchemaAsync(ct);

    [HttpGet("apis")]
    public Task<ApiCatalogDto> Apis(CancellationToken ct) => _metadata.GetLiveApisAsync(ct);

    [HttpGet("environment")]
    public Task<EnvironmentInfoDto> Environment(CancellationToken ct) => _metadata.GetEnvironmentAsync(ct);

    [HttpGet("snapshots")]
    public Task<IReadOnlyList<MetadatoSnapshotDto>> Snapshots([FromQuery] string? tipo, CancellationToken ct) =>
        _metadata.ListSnapshotsAsync(tipo, ct);

    [HttpGet("snapshots/{snapshotId:guid}")]
    public async Task<ActionResult<MetadatoSnapshotDetailDto>> Snapshot(Guid snapshotId, CancellationToken ct)
    {
        var snapshot = await _metadata.GetSnapshotAsync(snapshotId, ct);
        return snapshot is null ? NotFound() : Ok(snapshot);
    }

    [HttpPost("snapshots")]
    [Authorize(Policy = "TiMetadataWrite")]
    public async Task<ActionResult<MetadatoSnapshotDetailDto>> SaveSnapshot(
        [FromBody] SaveSnapshotRequest request, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        Guid? userId = Guid.TryParse(userIdClaim, out var id) ? id : null;
        var snapshot = await _metadata.SaveSnapshotAsync(request, userId, ct);
        return CreatedAtAction(nameof(Snapshot), new { snapshotId = snapshot.Id }, snapshot);
    }
}

[ApiController]
[Route(ApiRoutes.AdminEntities)]
[Route(ApiRoutes.AdminEntitiesLegacy)]
public class GenericAdminController : ControllerBase
{
    private readonly IGenericAdminService _admin;

    public GenericAdminController(IGenericAdminService admin) => _admin = admin;

    [HttpGet("registry")]
    [HttpGet("")]
    [Authorize(Policy = "AdminEntitiesRead")]
    public ActionResult<IReadOnlyList<EntityRegistryDto>> Registry() => Ok(_admin.ListEntities());

    [HttpGet("{entityName}")]
    [Authorize(Policy = "AdminEntitiesRead")]
    public Task<EntityPageDto> List(
        string entityName,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default) =>
        _admin.ListRecordsAsync(entityName, page, pageSize, includeInactive, ct);

    [HttpGet("{entityName}/{id:guid}")]
    [Authorize(Policy = "AdminEntitiesRead")]
    public async Task<IActionResult> Get(string entityName, Guid id, CancellationToken ct)
    {
        var record = await _admin.GetRecordAsync(entityName, id, ct);
        return record is null ? NotFound() : Ok(record);
    }

    [HttpPost("{entityName}")]
    [Authorize(Policy = "AdminEntitiesWrite")]
    public async Task<ActionResult<object>> Create(string entityName, [FromBody] object payload, CancellationToken ct)
    {
        var id = await _admin.CreateRecordAsync(entityName, payload, ct);
        var created = await _admin.GetRecordAsync(entityName, id, ct);
        return CreatedAtAction(nameof(Get), new { entityName, id }, created);
    }

    [HttpPut("{entityName}/{id:guid}")]
    [Authorize(Policy = "AdminEntitiesWrite")]
    public async Task<IActionResult> Update(string entityName, Guid id, [FromBody] object payload, CancellationToken ct)
    {
        await _admin.UpdateRecordAsync(entityName, id, payload, ct);
        return NoContent();
    }

    [HttpDelete("{entityName}/{id:guid}")]
    [Authorize(Policy = "AdminEntitiesDelete")]
    public async Task<IActionResult> Delete(string entityName, Guid id, CancellationToken ct)
    {
        await _admin.DeleteRecordAsync(entityName, id, ct);
        return NoContent();
    }
}
