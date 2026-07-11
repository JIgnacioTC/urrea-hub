using UrreaHub.Application.Common;
using UrreaHub.Domain.Common;

namespace UrreaHub.Application.Vacaciones;

public record CrearSolicitudDto(
    Guid TipoAusenciaId,
    DateTime FechaInicio,
    DateTime FechaFin,
    string? Comentario,
    bool Enviar = false,
    bool EsDiaCompleto = true,
    string? HoraInicio = null,
    string? HoraFin = null);

public record TipoAusenciaDto(
    Guid Id,
    string Codigo,
    string Nombre,
    bool DescuentaSaldo,
    bool RequiereAprobacion,
    string? Color,
    CategoriaPermiso Categoria,
    bool EsParcial,
    bool PermiteMultiDia,
    decimal? DiasMaximosAnuales,
    decimal? DiasMaximosEvento,
    bool RequiereComprobante,
    bool Remunerado,
    string? BaseLegalLft,
    string? Descripcion,
    string? Icono,
    int Orden,
    bool PermiteSolicitudEmpleado,
    bool RequiereAprobacionJefe,
    bool RequiereAprobacionDH,
    bool RequiereAprobacionNominas);

public record PasoAprobacionDto(
    int Orden,
    NivelAprobacionAusencia Nivel,
    string NivelLabel,
    EstadoSolicitud Estado,
    string? AprobadorNombre,
    string? Comentario,
    DateTime? FechaDecision);

public record ResumenTipoPermisoDto(
    Guid TipoAusenciaId,
    string Codigo,
    string Nombre,
    decimal DiasUsadosAnio,
    decimal? DiasMaximosAnuales,
    decimal? DiasMaximosEvento,
    decimal? DiasDisponibles);

public record SolicitudAusenciaDto(
    Guid Id,
    Guid ColaboradorId,
    string ColaboradorNombre,
    Guid TipoAusenciaId,
    string TipoAusenciaNombre,
    string TipoAusenciaCodigo,
    DateTime FechaInicio,
    DateTime FechaFin,
    decimal DiasSolicitados,
    string? Comentario,
    EstadoSolicitud Estado,
    DateTime CreatedAt,
    bool EsDiaCompleto,
    string? HoraInicio,
    string? HoraFin,
    IReadOnlyList<PasoAprobacionDto> PasosAprobacion);

public record SaldoVacacionesDto(
    int Anio,
    decimal DiasAsignados,
    decimal DiasUsados,
    decimal DiasComprometidos,
    decimal DiasDisponibles)
{
    public decimal DiasPendientes => DiasDisponibles;
}

public record CalculateDaysDto(DateTime FechaInicio, DateTime FechaFin, Guid? TipoAusenciaId);

public record CalculateDaysResultDto(
    decimal DiasHabiles,
    decimal? SaldoDisponible,
    decimal? SaldoPosterior,
    bool ExcedeSaldo,
    bool TieneTraslape);

public record PendingApprovalDto(
    Guid Id,
    Guid ColaboradorId,
    string ColaboradorNombre,
    string Puesto,
    string Departamento,
    Guid TipoAusenciaId,
    string TipoAusenciaNombre,
    string TipoAusenciaCodigo,
    DateTime FechaInicio,
    DateTime FechaFin,
    decimal DiasSolicitados,
    string? Comentario,
    EstadoSolicitud Estado,
    DateTime CreatedAt,
    decimal? SaldoDisponible,
    decimal? SaldoPosterior,
    IReadOnlyList<string> TraslapesEquipo,
    NivelAprobacionAusencia NivelActual,
    IReadOnlyList<PasoAprobacionDto> PasosAprobacion);

public record TeamCalendarDto(
    DateTime Desde,
    DateTime Hasta,
    IReadOnlyList<CalendarioAusenciaDto> Ausencias,
    IReadOnlyList<DiaInhabilCalendarioDto> DiasInhabiles);

public record DiaInhabilCalendarioDto(DateTime Fecha, string Descripcion, bool EsOficial);

public record AprobacionRequestDto(string? Comentario);

public record CalendarioAusenciaDto(
    Guid SolicitudId,
    Guid ColaboradorId,
    string ColaboradorNombre,
    string TipoAusencia,
    string Color,
    DateTime FechaInicio,
    DateTime FechaFin,
    EstadoSolicitud Estado);

public record RhDashboardDto(
    int SolicitudesPendientes,
    int AprobadasMes,
    int ColaboradoresActivos,
    int SolicitudesRechazadasMes);

public record ReporteAusenciaDto(
    string NumeroEmpleado,
    string Colaborador,
    string TipoAusencia,
    DateTime FechaInicio,
    DateTime FechaFin,
    decimal Dias,
    string Estado,
    string? Aprobador);

public interface ISaldoVacacionesService
{
    Task<SaldoVacacionesDto?> GetMiSaldoAsync(Guid colaboradorId, int? anio, CancellationToken cancellationToken = default);
    Task EnsureSaldoAnualAsync(Guid colaboradorId, int anio, CancellationToken cancellationToken = default);
    Task<decimal> GetDiasComprometidosAsync(Guid colaboradorId, int anio, Guid? excludeSolicitudId = null, CancellationToken cancellationToken = default);
}

public interface ISolicitudAusenciaService
{
    Task<Result<SolicitudAusenciaDto>> CrearAsync(Guid colaboradorId, CrearSolicitudDto dto, CancellationToken cancellationToken = default);
    Task<Result<SolicitudAusenciaDto>> EnviarAsync(Guid colaboradorId, Guid solicitudId, CancellationToken cancellationToken = default);
    Task<Result<SolicitudAusenciaDto>> CancelarAsync(Guid colaboradorId, Guid solicitudId, CancellationToken cancellationToken = default);
    Task<Result<SolicitudAusenciaDto>> AprobarAsync(Guid aprobadorId, Guid solicitudId, AprobacionRequestDto dto, bool isRhAdmin, bool isNominaAdmin, CancellationToken cancellationToken = default);
    Task<Result<SolicitudAusenciaDto>> RechazarAsync(Guid aprobadorId, Guid solicitudId, AprobacionRequestDto dto, bool isRhAdmin, bool isNominaAdmin, CancellationToken cancellationToken = default);
    Task<SolicitudAusenciaDto?> GetByIdAsync(Guid solicitudId, Guid? viewerColaboradorId, bool isRhAdmin, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SolicitudAusenciaDto>> GetMisSolicitudesAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PendingApprovalDto>> GetPendientesAprobacionAsync(Guid aprobadorId, bool isRhAdmin, bool isNominaAdmin, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CalendarioAusenciaDto>> GetCalendarioAusenciasAsync(Guid colaboradorId, DateTime desde, DateTime hasta, CancellationToken cancellationToken = default);
    Task<TeamCalendarDto> GetTeamCalendarAsync(Guid colaboradorId, DateTime desde, DateTime hasta, CancellationToken cancellationToken = default);
    Task<CalculateDaysResultDto> PreviewDaysAsync(Guid colaboradorId, CalculateDaysDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReporteAusenciaDto>> GetReporteAusenciasAsync(DateTime? desde, DateTime? hasta, Guid? sedeId, Guid? tipoId, EstadoSolicitud? estado, CancellationToken cancellationToken = default);
    Task<RhDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<decimal> CalcularDiasHabilesAsync(Guid colaboradorId, DateTime inicio, DateTime fin, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TipoAusenciaDto>> GetTiposPermisoAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TipoAusenciaDto>> GetAllTiposAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ResumenTipoPermisoDto>> GetResumenPermisosAsync(Guid colaboradorId, int? anio, CancellationToken cancellationToken = default);
}

public interface ICalendarioLaboralService
{
    Task<IReadOnlySet<DateTime>> GetDiasInhabilesAsync(Guid? sedeId, int anio, CancellationToken cancellationToken = default);
}
