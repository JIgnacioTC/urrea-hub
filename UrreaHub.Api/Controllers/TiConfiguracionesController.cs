using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrreaHub.Domain.Plataforma;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Api.Controllers;

[ApiController]
[Route("api/v1/ti/configuraciones")]
[Authorize]
public class TiConfiguracionesController : ControllerBase
{
    private readonly UrreaHubDbContext _context;

    public TiConfiguracionesController(UrreaHubDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetConfigs(CancellationToken ct)
    {
        var configs = await _context.ConfiguracionesGlobales.ToListAsync(ct);
        
        // Ensure default key exists so it's ready out-of-the-box
        var flowKey = "PowerAutomate_FlowPermisosUrl";
        if (!configs.Any(c => c.Clave == flowKey))
        {
            var newConfig = new ConfiguracionGlobal
            {
                Id = Guid.NewGuid(),
                Clave = flowKey,
                Valor = "",
                Descripcion = "URL de la API de Power Automate para el flujo de permisos y aprobaciones.",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _context.ConfiguracionesGlobales.Add(newConfig);
            await _context.SaveChangesAsync(ct);
            configs.Add(newConfig);
        }

        return Ok(configs);
    }

    [HttpPut("{clave}")]
    public async Task<IActionResult> UpdateConfig(string clave, [FromBody] UpdateConfigDto dto, CancellationToken ct)
    {
        var config = await _context.ConfiguracionesGlobales.FirstOrDefaultAsync(c => c.Clave == clave, ct);
        if (config == null)
        {
            config = new ConfiguracionGlobal
            {
                Id = Guid.NewGuid(),
                Clave = clave,
                Valor = dto.Valor ?? "",
                Descripcion = dto.Descripcion,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _context.ConfiguracionesGlobales.Add(config);
        }
        else
        {
            config.Valor = dto.Valor ?? "";
            if (dto.Descripcion != null)
            {
                config.Descripcion = dto.Descripcion;
            }
            config.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(ct);
        return Ok(config);
    }
}

public class UpdateConfigDto
{
    public string? Valor { get; set; }
    public string? Descripcion { get; set; }
}
