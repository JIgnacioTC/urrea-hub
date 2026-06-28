using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrreaHub.Domain.Vacaciones;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Api.Controllers;

[Authorize(Policy = "RhAdmin")]
[Route("api/vacaciones/[controller]")]
public class SolicitudesAusenciaAdminController : ControllerBase
{
    private readonly UrreaHubDbContext _context;

    public SolicitudesAusenciaAdminController(UrreaHubDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SolicitudAusencia>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _context.SolicitudesAusencia.Where(s => s.IsActive).AsNoTracking().ToListAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SolicitudAusencia>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _context.SolicitudesAusencia.FindAsync(new object[] { id }, cancellationToken);
        return entity is null || !entity.IsActive ? NotFound() : Ok(entity);
    }
}
