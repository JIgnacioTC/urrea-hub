using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrreaHub.Domain.Common;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Api.Controllers;

[ApiController]
[Authorize(Policy = "RhAdmin")]
[Route("api/[controller]")]
public abstract class CrudControllerBase<TEntity> : ControllerBase where TEntity : BaseEntity
{
    protected readonly UrreaHubDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    protected CrudControllerBase(UrreaHubDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<TEntity>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await DbSet.Where(e => e.IsActive).AsNoTracking().ToListAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public virtual async Task<ActionResult<TEntity>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await DbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null || !entity.IsActive)
            return NotFound();

        return Ok(entity);
    }

    [HttpPost]
    public virtual async Task<ActionResult<TEntity>> Create(TEntity entity, CancellationToken cancellationToken)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        DbSet.Add(entity);
        await Context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:guid}")]
    public virtual async Task<IActionResult> Update(Guid id, TEntity entity, CancellationToken cancellationToken)
    {
        if (id != entity.Id)
            return BadRequest();

        var existing = await DbSet.FindAsync(new object[] { id }, cancellationToken);
        if (existing is null)
            return NotFound();

        entity.UpdatedAt = DateTime.UtcNow;
        Context.Entry(existing).CurrentValues.SetValues(entity);
        await Context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public virtual async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var entity = await DbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity is null)
            return NotFound();

        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await Context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
