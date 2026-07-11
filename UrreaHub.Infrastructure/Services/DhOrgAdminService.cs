using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.CoreRH;
using UrreaHub.Application.Notificaciones;
using UrreaHub.Domain.CoreRH;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class DhOrgAdminService : IDhOrgAdminService
{
    private readonly UrreaHubDbContext _context;
    private readonly IAuditService _audit;

    public DhOrgAdminService(UrreaHubDbContext context, IAuditService audit)
    {
        _context = context;
        _audit = audit;
    }

    public async Task<OrgCatalogDto> GetCatalogAsync(CancellationToken cancellationToken = default)
    {
        var areas = await _context.Areas.AsNoTracking()
            .Where(a => a.IsActive)
            .OrderBy(a => a.Codigo)
            .Select(a => new OrgItemDto(a.Id, a.Codigo, a.Nombre, a.Descripcion, a.IsActive))
            .ToListAsync(cancellationToken);

        var subareas = await _context.Subareas.AsNoTracking()
            .Include(s => s.Area)
            .Where(s => s.IsActive)
            .OrderBy(s => s.Codigo)
            .Select(s => new OrgSubareaDto(s.Id, s.Codigo, s.Nombre, s.Descripcion, s.AreaId, s.Area.Nombre, s.IsActive))
            .ToListAsync(cancellationToken);

        var departamentos = await _context.Departamentos.AsNoTracking()
            .Include(d => d.Subarea).ThenInclude(s => s.Area)
            .Include(d => d.Sede)
            .Where(d => d.IsActive)
            .OrderBy(d => d.Codigo)
            .Select(d => new OrgDepartamentoDto(
                d.Id, d.Codigo, d.Nombre, d.Descripcion, d.SubareaId, d.Subarea.Nombre, d.Subarea.AreaId, d.Subarea.Area.Nombre,
                d.SedeId, d.Sede != null ? d.Sede.Nombre : null, d.IsActive))
            .ToListAsync(cancellationToken);

        var puestos = await _context.Puestos.AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Codigo)
            .Select(p => new OrgPuestoDto(
                p.Id, p.Codigo, p.Nombre, p.Descripcion, p.NivelJerarquico,
                p.GradoMercer, p.Impacto, p.Comunicacion, p.Innovacion, p.EducacionRequerida,
                p.ExperienciaAnios, p.PresupuestoAnual, p.PersonalCargoDirecto, p.PersonalCargoIndirecto, p.IsActive))
            .ToListAsync(cancellationToken);

        var sedes = await _context.Sedes.AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Codigo)
            .Select(s => new OrgSedeDto(s.Id, s.Codigo, s.Nombre, s.Ciudad, s.Pais, s.IsActive))
            .ToListAsync(cancellationToken);

        var centros = await _context.CentrosCosto.AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Codigo)
            .Select(c => new OrgItemDto(c.Id, c.Codigo, c.Nombre, c.Descripcion, c.IsActive))
            .ToListAsync(cancellationToken);

        return new OrgCatalogDto(areas, subareas, departamentos, puestos, sedes, centros);
    }

    public async Task<OrgItemDto> UpsertAreaAsync(Guid? id, UpsertOrgItemDto dto, string performedBy, CancellationToken cancellationToken = default)
    {
        Area entity;
        if (id.HasValue)
        {
            entity = await _context.Areas.FirstOrDefaultAsync(a => a.Id == id.Value, cancellationToken)
                ?? throw new InvalidOperationException("Área no encontrada.");
            entity.Codigo = dto.Codigo.Trim();
            entity.Nombre = dto.Nombre.Trim();
            entity.Descripcion = dto.Descripcion;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            entity = new Area
            {
                Id = Guid.NewGuid(),
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
            };
            _context.Areas.Add(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Organizacion", id.HasValue ? "Update" : "Create", "Area", entity.Id, performedBy, entity.Nombre, cancellationToken);
        return new OrgItemDto(entity.Id, entity.Codigo, entity.Nombre, entity.Descripcion, entity.IsActive);
    }

    public async Task<OrgPuestoDto> UpsertPuestoAsync(Guid? id, UpsertPuestoDto dto, string performedBy, CancellationToken cancellationToken = default)
    {
        Puesto entity;
        if (id.HasValue)
        {
            entity = await _context.Puestos.FirstOrDefaultAsync(p => p.Id == id.Value, cancellationToken)
                ?? throw new InvalidOperationException("Puesto no encontrado.");
            entity.Codigo = dto.Codigo.Trim();
            entity.Nombre = dto.Nombre.Trim();
            entity.Descripcion = dto.Descripcion;
            entity.NivelJerarquico = dto.NivelJerarquico;
            entity.GradoMercer = dto.GradoMercer;
            entity.Impacto = dto.Impacto;
            entity.Comunicacion = dto.Comunicacion;
            entity.Innovacion = dto.Innovacion;
            entity.EducacionRequerida = dto.EducacionRequerida;
            entity.ExperienciaAnios = dto.ExperienciaAnios;
            entity.PresupuestoAnual = dto.PresupuestoAnual;
            entity.PersonalCargoDirecto = dto.PersonalCargoDirecto;
            entity.PersonalCargoIndirecto = dto.PersonalCargoIndirecto;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            entity = new Puesto
            {
                Id = Guid.NewGuid(),
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion,
                NivelJerarquico = dto.NivelJerarquico,
                GradoMercer = dto.GradoMercer,
                Impacto = dto.Impacto,
                Comunicacion = dto.Comunicacion,
                Innovacion = dto.Innovacion,
                EducacionRequerida = dto.EducacionRequerida,
                ExperienciaAnios = dto.ExperienciaAnios,
                PresupuestoAnual = dto.PresupuestoAnual,
                PersonalCargoDirecto = dto.PersonalCargoDirecto,
                PersonalCargoIndirecto = dto.PersonalCargoIndirecto,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
            };
            _context.Puestos.Add(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Organizacion", id.HasValue ? "Update" : "Create", "Puesto", entity.Id, performedBy, entity.Nombre, cancellationToken);
        
        var catalog = await GetCatalogAsync(cancellationToken);
        return catalog.Puestos.First(p => p.Id == entity.Id);
    }

    public async Task<OrgItemDto> UpsertCentroCostoAsync(Guid? id, UpsertOrgItemDto dto, string performedBy, CancellationToken cancellationToken = default)
    {
        CentroCosto entity;
        if (id.HasValue)
        {
            entity = await _context.CentrosCosto.FirstOrDefaultAsync(c => c.Id == id.Value, cancellationToken)
                ?? throw new InvalidOperationException("Centro de costo no encontrado.");
            entity.Codigo = dto.Codigo.Trim();
            entity.Nombre = dto.Nombre.Trim();
            entity.Descripcion = dto.Descripcion;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            entity = new CentroCosto
            {
                Id = Guid.NewGuid(),
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
            };
            _context.CentrosCosto.Add(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Organizacion", id.HasValue ? "Update" : "Create", "CentroCosto", entity.Id, performedBy, entity.Nombre, cancellationToken);
        return new OrgItemDto(entity.Id, entity.Codigo, entity.Nombre, entity.Descripcion, entity.IsActive);
    }

    public async Task<OrgSubareaDto> UpsertSubareaAsync(
        Guid? id,
        UpsertSubareaDto dto,
        string performedBy,
        CancellationToken cancellationToken = default)
    {
        Subarea entity;
        if (id.HasValue)
        {
            entity = await _context.Subareas.FirstOrDefaultAsync(s => s.Id == id.Value, cancellationToken)
                ?? throw new InvalidOperationException("Subárea no encontrada.");
            entity.Codigo = dto.Codigo.Trim();
            entity.Nombre = dto.Nombre.Trim();
            entity.Descripcion = dto.Descripcion;
            entity.AreaId = dto.AreaId;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            entity = new Subarea
            {
                Id = Guid.NewGuid(),
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion,
                AreaId = dto.AreaId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
            };
            _context.Subareas.Add(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Organizacion", id.HasValue ? "Update" : "Create", "Subarea", entity.Id, performedBy, entity.Nombre, cancellationToken);

        var catalog = await GetCatalogAsync(cancellationToken);
        return catalog.Subareas.First(s => s.Id == entity.Id);
    }

    public async Task<OrgDepartamentoDto> UpsertDepartamentoAsync(
        Guid? id,
        UpsertDepartamentoDto dto,
        string performedBy,
        CancellationToken cancellationToken = default)
    {
        Departamento entity;
        if (id.HasValue)
        {
            entity = await _context.Departamentos.FirstOrDefaultAsync(d => d.Id == id.Value, cancellationToken)
                ?? throw new InvalidOperationException("Departamento no encontrado.");
            entity.Codigo = dto.Codigo.Trim();
            entity.Nombre = dto.Nombre.Trim();
            entity.Descripcion = dto.Descripcion;
            entity.SubareaId = dto.SubareaId;
            entity.SedeId = dto.SedeId;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            entity = new Departamento
            {
                Id = Guid.NewGuid(),
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion,
                SubareaId = dto.SubareaId,
                SedeId = dto.SedeId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
            };
            _context.Departamentos.Add(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Organizacion", id.HasValue ? "Update" : "Create", "Departamento", entity.Id, performedBy, entity.Nombre, cancellationToken);

        var catalog = await GetCatalogAsync(cancellationToken);
        return catalog.Departamentos.First(d => d.Id == entity.Id);
    }

    public async Task<OrgSedeDto> UpsertSedeAsync(Guid? id, UpsertSedeDto dto, string performedBy, CancellationToken cancellationToken = default)
    {
        Sede entity;
        if (id.HasValue)
        {
            entity = await _context.Sedes.FirstOrDefaultAsync(s => s.Id == id.Value, cancellationToken)
                ?? throw new InvalidOperationException("Sede no encontrada.");
            entity.Codigo = dto.Codigo.Trim();
            entity.Nombre = dto.Nombre.Trim();
            entity.Ciudad = dto.Ciudad;
            entity.Pais = dto.Pais;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            entity = new Sede
            {
                Id = Guid.NewGuid(),
                Codigo = dto.Codigo.Trim(),
                Nombre = dto.Nombre.Trim(),
                Ciudad = dto.Ciudad,
                Pais = dto.Pais,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
            };
            _context.Sedes.Add(entity);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Organizacion", id.HasValue ? "Update" : "Create", "Sede", entity.Id, performedBy, entity.Nombre, cancellationToken);
        return new OrgSedeDto(entity.Id, entity.Codigo, entity.Nombre, entity.Ciudad, entity.Pais, entity.IsActive);
    }

    public async Task<IReadOnlyList<ManagerAssignmentDto>> ListManagerAssignmentsAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Colaboradores.AsNoTracking()
            .Include(c => c.Puesto)
            .Include(c => c.Departamento)
            .Include(c => c.JefeDirecto)
            .Where(c => c.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(c =>
                c.NumeroEmpleado.ToLower().Contains(term) ||
                c.Nombre.ToLower().Contains(term) ||
                c.ApellidoPaterno.ToLower().Contains(term));
        }

        return await query
            .OrderBy(c => c.ApellidoPaterno).ThenBy(c => c.Nombre)
            .Take(100)
            .Select(c => new ManagerAssignmentDto(
                c.Id,
                c.NumeroEmpleado,
                c.Nombre + " " + c.ApellidoPaterno,
                c.Puesto.Nombre,
                c.Departamento.Nombre,
                c.JefeDirectoId,
                c.JefeDirecto != null ? c.JefeDirecto.Nombre + " " + c.JefeDirecto.ApellidoPaterno : null,
                c.IsManualOverride,
                c.SyncStatus,
                c.ExternalSource))
            .ToListAsync(cancellationToken);
    }

    public async Task AssignManagerAsync(
        Guid colaboradorId,
        AssignManagerDto dto,
        string performedBy,
        CancellationToken cancellationToken = default)
    {
        var colaborador = await _context.Colaboradores
            .FirstOrDefaultAsync(c => c.Id == colaboradorId && c.IsActive, cancellationToken)
            ?? throw new InvalidOperationException("Colaborador no encontrado.");

        if (dto.JefeDirectoId.HasValue && dto.JefeDirectoId.Value == colaboradorId)
            throw new InvalidOperationException("Un colaborador no puede ser su propio jefe.");

        if (dto.JefeDirectoId.HasValue)
        {
            var jefeExists = await _context.Colaboradores
                .AnyAsync(c => c.Id == dto.JefeDirectoId.Value && c.IsActive, cancellationToken);
            if (!jefeExists)
                throw new InvalidOperationException("Jefe directo no encontrado.");
        }

        var anterior = colaborador.JefeDirectoId?.ToString();
        colaborador.JefeDirectoId = dto.JefeDirectoId;
        colaborador.IsManualOverride = true;
        colaborador.SyncStatus = EmployeeSyncStatus.ManualOverride;
        colaborador.UpdatedAt = DateTime.UtcNow;

        _context.MovimientosColaborador.Add(new MovimientoColaborador
        {
            Id = Guid.NewGuid(),
            ColaboradorId = colaboradorId,
            TipoMovimiento = "CambioJefe",
            FechaEfectiva = dto.FechaEfectiva?.Date ?? DateTime.UtcNow.Date,
            ValorAnterior = anterior,
            ValorNuevo = colaborador.JefeDirectoId?.ToString(),
            Origen = EmployeeExternalSource.Manual,
            CreadoPor = performedBy,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        });

        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Organizacion", "AssignManager", "Colaborador", colaboradorId, performedBy,
            dto.Motivo ?? "Asignación manual de jefe directo", cancellationToken);
    }

    public async Task DeleteAreaAsync(Guid id, string performedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Areas.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Área no encontrada.");
        var inUse = await _context.Subareas.AnyAsync(x => x.AreaId == id && x.IsActive, cancellationToken);
        if (inUse)
        {
            entity.IsActive = false;
        }
        else
        {
            _context.Areas.Remove(entity);
        }
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Organizacion", "Delete", "Area", id, performedBy, entity.Nombre, cancellationToken);
    }

    public async Task DeleteSubareaAsync(Guid id, string performedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Subareas.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Subárea no encontrada.");
        var inUse = await _context.Departamentos.AnyAsync(x => x.SubareaId == id && x.IsActive, cancellationToken);
        if (inUse)
        {
            entity.IsActive = false;
        }
        else
        {
            _context.Subareas.Remove(entity);
        }
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Organizacion", "Delete", "Subarea", id, performedBy, entity.Nombre, cancellationToken);
    }

    public async Task DeleteDepartamentoAsync(Guid id, string performedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Departamentos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Departamento no encontrado.");
        var inUse = await _context.Colaboradores.AnyAsync(x => x.DepartamentoId == id && x.IsActive, cancellationToken);
        if (inUse)
        {
            entity.IsActive = false;
        }
        else
        {
            _context.Departamentos.Remove(entity);
        }
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Organizacion", "Delete", "Departamento", id, performedBy, entity.Nombre, cancellationToken);
    }

    public async Task DeletePuestoAsync(Guid id, string performedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Puestos.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Puesto no encontrado.");
        var inUse = await _context.Colaboradores.AnyAsync(x => x.PuestoId == id && x.IsActive, cancellationToken);
        if (inUse)
        {
            entity.IsActive = false;
        }
        else
        {
            _context.Puestos.Remove(entity);
        }
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Organizacion", "Delete", "Puesto", id, performedBy, entity.Nombre, cancellationToken);
    }

    public async Task DeleteSedeAsync(Guid id, string performedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Sedes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Sede no encontrada.");
        var inUse = await _context.Departamentos.AnyAsync(x => x.SedeId == id && x.IsActive, cancellationToken) ||
                    await _context.Colaboradores.AnyAsync(x => x.SedeId == id && x.IsActive, cancellationToken);
        if (inUse)
        {
            entity.IsActive = false;
        }
        else
        {
            _context.Sedes.Remove(entity);
        }
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Organizacion", "Delete", "Sede", id, performedBy, entity.Nombre, cancellationToken);
    }

    public async Task DeleteCentroCostoAsync(Guid id, string performedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _context.CentrosCosto.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Centro de costo no encontrado.");
        var inUse = await _context.Colaboradores.AnyAsync(x => x.CentroCostoId == id && x.IsActive, cancellationToken);
        if (inUse)
        {
            entity.IsActive = false;
        }
        else
        {
            _context.CentrosCosto.Remove(entity);
        }
        await _context.SaveChangesAsync(cancellationToken);
        await _audit.LogEventoAsync("Organizacion", "Delete", "CentroCosto", id, performedBy, entity.Nombre, cancellationToken);
    }
}
