using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Common;
using UrreaHub.Domain.Vacaciones;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Api.Controllers;

[Authorize(Policy = "RhAdmin")]
[Route("api/vacaciones/admin/[controller]")]
public class PoliticasVacacionesController : CrudControllerBase<PoliticaVacaciones>
{
    public PoliticasVacacionesController(UrreaHubDbContext context) : base(context) { }
}

[Authorize(Policy = "RhAdmin")]
[Route("api/vacaciones/admin/[controller]")]
public class SaldosVacacionesController : CrudControllerBase<SaldoVacaciones>
{
    public SaldosVacacionesController(UrreaHubDbContext context) : base(context) { }
}

[Authorize(Policy = "RhAdmin")]
[Route("api/vacaciones/admin/[controller]")]
public class CalendariosLaboralesController : CrudControllerBase<CalendarioLaboral>
{
    public CalendariosLaboralesController(UrreaHubDbContext context) : base(context) { }
}

[Authorize(Policy = "RhAdmin")]
[Route("api/vacaciones/admin/[controller]")]
public class DiasInhabilesController : CrudControllerBase<DiaInhabil>
{
    public DiasInhabilesController(UrreaHubDbContext context) : base(context) { }
}

[Authorize(Policy = "RhAdmin")]
[Route("api/core-rh/rh/[controller]")]
public class ColaboradoresRhController : ControllerBase
{
    private readonly UrreaHubDbContext _context;

    public ColaboradoresRhController(UrreaHubDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<PagedResult<object>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Colaboradores.AsNoTracking()
            .Include(c => c.Puesto)
            .Include(c => c.Departamento)
            .Include(c => c.DatosSensibles)
            .Where(c => c.IsActive);

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(c => c.ApellidoPaterno)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(c => new
            {
                c.Id, c.NumeroEmpleado, c.Nombre, c.ApellidoPaterno, c.Email,
                Rfc = c.DatosSensibles != null ? c.DatosSensibles.Rfc : null,
                c.FechaIngreso, c.FechaBaja, c.NominaSyncAt,
                Puesto = c.Puesto.Nombre, Departamento = c.Departamento.Nombre,
                c.PuestoId, c.DepartamentoId, c.SedeId, c.CentroCostoId, c.RelacionLaboralId, c.JefeDirectoId
            })
            .ToListAsync(cancellationToken);

        return Ok(new PagedResult<object> { Items = items, Total = total, Page = page, PageSize = pageSize });
    }
}
