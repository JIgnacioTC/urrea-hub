using UrreaHub.Application.Common;
using UrreaHub.Application.CoreRH;
using UrreaHub.Domain.Common;

namespace UrreaHub.Application.Equipo;

public record EquipoMiembroDto(
    Guid Id,
    string NumeroEmpleado,
    string NombreCompleto,
    string Puesto,
    string Departamento,
    string Email,
    DateTime FechaIngreso);

public record CrearPlanAccionDto(
    Guid ColaboradorId,
    string Titulo,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime FechaFin,
    string Prioridad = "Media");

public record PlanAccionDto(
    Guid Id,
    Guid ColaboradorId,
    string ColaboradorNombre,
    string Titulo,
    string? Descripcion,
    DateTime FechaInicio,
    DateTime FechaFin,
    EstadoPlanAccion Estado,
    decimal Avance,
    string Prioridad,
    DateTime CreatedAt);

public record CrearFeedbackEquipoDto(
    Guid ColaboradorId,
    string Tipo,
    string Comentario);

public record FeedbackEquipoDto(
    Guid Id,
    Guid ColaboradorId,
    string ColaboradorNombre,
    string Tipo,
    string Comentario,
    DateTime Fecha,
    string AutorNombre);

public record CapacitacionEquipoDto(
    Guid InscripcionId,
    Guid ColaboradorId,
    string ColaboradorNombre,
    string NumeroEmpleado,
    string CursoCodigo,
    string CursoNombre,
    string? Modalidad,
    int DuracionHoras,
    DateTime FechaInscripcion,
    DateTime? FechaCompletado,
    EstadoSolicitud Estado,
    decimal? Puntuacion,
    bool? Aprobado);

public interface IEquipoService
{
    Task<IReadOnlyList<EquipoMiembroDto>> GetMiEquipoAsync(Guid jefeId, CancellationToken cancellationToken = default);
    Task<ColaboradorPerfilDto?> GetFichaSubordinadoAsync(Guid jefeId, Guid colaboradorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PlanAccionDto>> GetPlanesAccionAsync(Guid jefeId, CancellationToken cancellationToken = default);
    Task<Result<PlanAccionDto>> CrearPlanAccionAsync(Guid jefeId, CrearPlanAccionDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeedbackEquipoDto>> GetFeedbackAsync(Guid jefeId, CancellationToken cancellationToken = default);
    Task<Result<FeedbackEquipoDto>> CrearFeedbackAsync(Guid jefeId, CrearFeedbackEquipoDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CapacitacionEquipoDto>> GetCapacitacionesEquipoAsync(Guid jefeId, CancellationToken cancellationToken = default);
}
