using UrreaHub.Application.Common;
using UrreaHub.Domain.Common;

namespace UrreaHub.Application.Reclutamiento;

public record VacanteDto(
    Guid Id,
    string Codigo,
    string Titulo,
    string Estado,
    DateTime FechaPublicacion,
    DateTime? FechaCierre,
    Guid? RequisicionId,
    string? RequisicionFolio,
    int CandidatosCount,
    string? EtapaDominante);

public record CandidatoDto(
    Guid Id,
    string NombreCompleto,
    string Email,
    string? Telefono,
    int PostulacionesCount);

public record PostulacionDto(
    Guid Id,
    Guid VacanteId,
    string VacanteTitulo,
    Guid CandidatoId,
    string CandidatoNombre,
    string Estado,
    DateTime FechaPostulacion,
    decimal? ScorePromedio,
    DateTime? ProximaEntrevista,
    string? Notas);

public record EntrevistaDto(Guid Id, DateTime FechaHora, string Tipo, string? Ubicacion, string? Notas);

public record OfertaDto(
    Guid Id,
    Guid PostulacionId,
    string CandidatoNombre,
    string VacanteTitulo,
    decimal SalarioOfrecido,
    string Moneda,
    DateTime FechaOferta,
    bool Aceptada,
    DateTime? FechaRespuesta);

public record RecruitmentDashboardDto(
    int VacantesAbiertas,
    int CandidatosActivos,
    int EntrevistasProgramadas,
    int OfertasEnviadas,
    int OfertasAceptadas,
    int RequisicionesPorConvertir,
    IReadOnlyList<VacanteDto> VacantesRecientes);

public record CrearCandidatoDto(string Nombre, string ApellidoPaterno, string? ApellidoMaterno, string Email, string? Telefono, Guid VacanteId);

public record CrearEntrevistaDto(DateTime FechaHora, string Tipo, string? Ubicacion, string? Notas);

public record CrearOfertaDto(decimal SalarioOfrecido, string Moneda);

public record CambiarEtapaDto(EstadoPostulacion Estado, string? Notas);

public record EnviarOnboardingResultDto(Guid ColaboradorId, Guid PlanOnboardingId, string NumeroEmpleado);

public interface IRecruitmentService
{
    Task<RecruitmentDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VacanteDto>> ListVacanciesAsync(EstadoVacante? estado, CancellationToken cancellationToken = default);
    Task<VacanteDto?> GetVacancyAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PostulacionDto>> ListPipelineAsync(Guid? vacanteId, CancellationToken cancellationToken = default);
    Task<CandidatoDto?> GetCandidateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PostulacionDto>> CreateCandidateAndApplyAsync(CrearCandidatoDto dto, CancellationToken cancellationToken = default);
    Task<Result<PostulacionDto>> ChangeStageAsync(Guid postulacionId, CambiarEtapaDto dto, CancellationToken cancellationToken = default);
    Task<Result<EntrevistaDto>> ScheduleInterviewAsync(Guid postulacionId, CrearEntrevistaDto dto, CancellationToken cancellationToken = default);
    Task<Result<OfertaDto>> CreateOfferAsync(Guid postulacionId, CrearOfertaDto dto, CancellationToken cancellationToken = default);
    Task<Result<EnviarOnboardingResultDto>> AcceptOfferAndSendToOnboardingAsync(Guid ofertaId, string performedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OfertaDto>> ListOffersAsync(CancellationToken cancellationToken = default);
}
