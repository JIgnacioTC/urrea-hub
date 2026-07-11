using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrreaHub.Application.Asistencia;
using UrreaHub.Application.Common;
using UrreaHub.Application.Vacaciones;
using Microsoft.EntityFrameworkCore;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Api.Controllers;

[ApiController]
[Route("api/v1/permisos")]
[Authorize]
public class PermisosController : ControllerBase
{
    private readonly ISolicitudAusenciaService _solicitudesService;
    private readonly IAsistenciaImportService _importService;
    private readonly ICurrentUser _currentUser;
    private readonly UrreaHubDbContext _context;

    public PermisosController(
        ISolicitudAusenciaService solicitudesService,
        IAsistenciaImportService importService,
        ICurrentUser currentUser,
        UrreaHubDbContext context)
    {
        _solicitudesService = solicitudesService;
        _importService = importService;
        _currentUser = currentUser;
        _context = context;
    }

    [HttpGet("mis-casos")]
    public async Task<IActionResult> GetMisCasos(CancellationToken cancellationToken)
    {
        var result = await _solicitudesService.GetMisSolicitudesAsync(_currentUser.ColaboradorId, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = "RhAdmin")]
    [HttpPost("importar-asistencias")]
    public async Task<IActionResult> ImportarAsistencias(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { Error = "Archivo no proporcionado" });

        using var stream = file.OpenReadStream();
        var result = await _importService.ImportarExcelAsistenciasAsync(stream, _currentUser.ColaboradorId.ToString(), cancellationToken);
        
        if (!result.Success)
            return BadRequest(new { Error = result.Error });

        return Ok(result.Data);
    }

    [Authorize(Policy = "RhAdmin")]
    [HttpPut("configuracion/{tipoId}")]
    public async Task<IActionResult> ConfigurarPermiso(Guid tipoId, [FromBody] ConfigPermisoDto dto, CancellationToken cancellationToken)
    {
        var tipo = await _context.TiposAusencia.FirstOrDefaultAsync(t => t.Id == tipoId, cancellationToken);
        if (tipo == null) return NotFound();

        tipo.WebhookUrl = dto.WebhookUrl;
        tipo.AreaDestinoId = dto.AreaDestinoId;
        tipo.NotificarTeams = dto.NotificarTeams;
        tipo.NotificarCorreo = dto.NotificarCorreo;
        tipo.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new { Message = "Configuración actualizada" });
    }
}

public class ConfigPermisoDto
{
    public string? WebhookUrl { get; set; }
    public Guid? AreaDestinoId { get; set; }
    public bool NotificarTeams { get; set; }
    public bool NotificarCorreo { get; set; }
}
