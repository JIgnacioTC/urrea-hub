using Microsoft.EntityFrameworkCore;
using UrreaHub.Application.Common;
using UrreaHub.Application.CoreRH;
using UrreaHub.Application.Equipo;
using UrreaHub.Domain.Capacitacion;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.Desempeno;
using UrreaHub.Infrastructure.Persistence;

namespace UrreaHub.Infrastructure.Services;

public class EquipoService : IEquipoService
{
    private readonly UrreaHubDbContext _context;
    private readonly IColaboradorService _colaboradorService;

    public EquipoService(UrreaHubDbContext context, IColaboradorService colaboradorService)
    {
        _context = context;
        _colaboradorService = colaboradorService;
    }

    public async Task<IReadOnlyList<EquipoMiembroDto>> GetMiEquipoAsync(Guid jefeId, CancellationToken cancellationToken = default)
    {
        return await _context.Colaboradores.AsNoTracking()
            .Include(c => c.Puesto)
            .Include(c => c.Departamento)
            .Where(c => c.IsActive && c.JefeDirectoId == jefeId)
            .OrderBy(c => c.ApellidoPaterno).ThenBy(c => c.Nombre)
            .Select(c => new EquipoMiembroDto(
                c.Id, c.NumeroEmpleado, c.Nombre + " " + c.ApellidoPaterno,
                c.Puesto.Nombre, c.Departamento.Nombre, c.Email, c.FechaIngreso))
            .ToListAsync(cancellationToken);
    }

    public async Task<ColaboradorPerfilDto?> GetFichaSubordinadoAsync(Guid jefeId, Guid colaboradorId, CancellationToken cancellationToken = default)
    {
        var esSubordinado = await _context.Colaboradores.AsNoTracking()
            .AnyAsync(c => c.Id == colaboradorId && c.JefeDirectoId == jefeId && c.IsActive, cancellationToken);
        if (!esSubordinado) return null;
        return await _colaboradorService.GetPerfilAsync(colaboradorId, cancellationToken);
    }

    public async Task<IReadOnlyList<PlanAccionDto>> GetPlanesAccionAsync(Guid jefeId, CancellationToken cancellationToken = default)
    {
        var subordinados = await GetSubordinadoIdsAsync(jefeId, cancellationToken);
        return await _context.PlanesAccion.AsNoTracking()
            .Include(p => p.Colaborador)
            .Where(p => p.IsActive && subordinados.Contains(p.ColaboradorId))
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PlanAccionDto(
                p.Id, p.ColaboradorId,
                p.Colaborador.Nombre + " " + p.Colaborador.ApellidoPaterno,
                p.Titulo, p.Descripcion, p.FechaInicio, p.FechaFin,
                p.Estado, p.Avance, p.Prioridad, p.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<PlanAccionDto>> CrearPlanAccionAsync(Guid jefeId, CrearPlanAccionDto dto, CancellationToken cancellationToken = default)
    {
        if (!await EsSubordinadoAsync(jefeId, dto.ColaboradorId, cancellationToken))
            return Result<PlanAccionDto>.Fail("Colaborador no pertenece a tu equipo.");

        if (string.IsNullOrWhiteSpace(dto.Titulo))
            return Result<PlanAccionDto>.Fail("El título es obligatorio.");

        if (dto.FechaFin < dto.FechaInicio)
            return Result<PlanAccionDto>.Fail("La fecha fin debe ser posterior a la fecha inicio.");

        var plan = new PlanAccion
        {
            Id = Guid.NewGuid(),
            ColaboradorId = dto.ColaboradorId,
            CreadoPorId = jefeId,
            Titulo = dto.Titulo.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim(),
            FechaInicio = dto.FechaInicio.Date,
            FechaFin = dto.FechaFin.Date,
            Prioridad = dto.Prioridad,
            Estado = EstadoPlanAccion.Pendiente,
            Avance = 0,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };

        _context.PlanesAccion.Add(plan);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<PlanAccionDto>.Ok(await GetPlanDtoAsync(plan.Id, cancellationToken));
    }

    public async Task<IReadOnlyList<FeedbackEquipoDto>> GetFeedbackAsync(Guid jefeId, CancellationToken cancellationToken = default)
    {
        var subordinados = await GetSubordinadoIdsAsync(jefeId, cancellationToken);
        return await _context.FeedbacksEquipo.AsNoTracking()
            .Include(f => f.Colaborador)
            .Include(f => f.Autor)
            .Where(f => f.IsActive && subordinados.Contains(f.ColaboradorId))
            .OrderByDescending(f => f.Fecha)
            .Select(f => new FeedbackEquipoDto(
                f.Id, f.ColaboradorId,
                f.Colaborador.Nombre + " " + f.Colaborador.ApellidoPaterno,
                f.Tipo, f.Comentario, f.Fecha,
                f.Autor.Nombre + " " + f.Autor.ApellidoPaterno))
            .ToListAsync(cancellationToken);
    }

    public async Task<Result<FeedbackEquipoDto>> CrearFeedbackAsync(Guid jefeId, CrearFeedbackEquipoDto dto, CancellationToken cancellationToken = default)
    {
        if (!await EsSubordinadoAsync(jefeId, dto.ColaboradorId, cancellationToken))
            return Result<FeedbackEquipoDto>.Fail("Colaborador no pertenece a tu equipo.");

        if (string.IsNullOrWhiteSpace(dto.Comentario))
            return Result<FeedbackEquipoDto>.Fail("El comentario es obligatorio.");

        var feedback = new FeedbackEquipo
        {
            Id = Guid.NewGuid(),
            ColaboradorId = dto.ColaboradorId,
            AutorId = jefeId,
            Tipo = string.IsNullOrWhiteSpace(dto.Tipo) ? "Constructivo" : dto.Tipo,
            Comentario = dto.Comentario.Trim(),
            Fecha = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };

        _context.FeedbacksEquipo.Add(feedback);
        await _context.SaveChangesAsync(cancellationToken);

        var saved = await _context.FeedbacksEquipo.AsNoTracking()
            .Include(f => f.Colaborador)
            .Include(f => f.Autor)
            .FirstAsync(f => f.Id == feedback.Id, cancellationToken);

        return Result<FeedbackEquipoDto>.Ok(new FeedbackEquipoDto(
            saved.Id, saved.ColaboradorId,
            saved.Colaborador.Nombre + " " + saved.Colaborador.ApellidoPaterno,
            saved.Tipo, saved.Comentario, saved.Fecha,
            saved.Autor.Nombre + " " + saved.Autor.ApellidoPaterno));
    }

    public async Task<IReadOnlyList<CapacitacionEquipoDto>> GetCapacitacionesEquipoAsync(Guid jefeId, CancellationToken cancellationToken = default)
    {
        var subordinados = await GetSubordinadoIdsAsync(jefeId, cancellationToken);
        return await _context.InscripcionesCurso.AsNoTracking()
            .Include(i => i.Colaborador)
            .Include(i => i.Curso)
            .Include(i => i.Evaluacion)
            .Where(i => i.IsActive && subordinados.Contains(i.ColaboradorId))
            .OrderByDescending(i => i.FechaInscripcion)
            .Select(i => new CapacitacionEquipoDto(
                i.Id, i.ColaboradorId,
                i.Colaborador.Nombre + " " + i.Colaborador.ApellidoPaterno,
                i.Colaborador.NumeroEmpleado,
                i.Curso.Codigo, i.Curso.Nombre, i.Curso.Modalidad, i.Curso.DuracionHoras,
                i.FechaInscripcion, i.FechaCompletado, i.Estado,
                i.Evaluacion != null ? i.Evaluacion.Puntuacion : null,
                i.Evaluacion != null ? i.Evaluacion.Aprobado : null))
            .ToListAsync(cancellationToken);
    }

    private async Task<List<Guid>> GetSubordinadoIdsAsync(Guid jefeId, CancellationToken cancellationToken)
        => await _context.Colaboradores.AsNoTracking()
            .Where(c => c.IsActive && c.JefeDirectoId == jefeId)
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

    private async Task<bool> EsSubordinadoAsync(Guid jefeId, Guid colaboradorId, CancellationToken cancellationToken)
        => await _context.Colaboradores.AsNoTracking()
            .AnyAsync(c => c.Id == colaboradorId && c.JefeDirectoId == jefeId && c.IsActive, cancellationToken);

    private async Task<PlanAccionDto> GetPlanDtoAsync(Guid id, CancellationToken cancellationToken)
    {
        var p = await _context.PlanesAccion.AsNoTracking()
            .Include(x => x.Colaborador)
            .FirstAsync(x => x.Id == id, cancellationToken);
        return MapPlan(p);
    }

    private static PlanAccionDto MapPlan(PlanAccion p) => new(
        p.Id, p.ColaboradorId,
        p.Colaborador.Nombre + " " + p.Colaborador.ApellidoPaterno,
        p.Titulo, p.Descripcion, p.FechaInicio, p.FechaFin,
        p.Estado, p.Avance, p.Prioridad, p.CreatedAt);
}
