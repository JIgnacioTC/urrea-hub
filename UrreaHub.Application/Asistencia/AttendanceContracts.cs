using UrreaHub.Application.Common;
using UrreaHub.Domain.Asistencia;

namespace UrreaHub.Application.Asistencia;

public record CheckInOutDto(
    FuenteRegistroAsistencia Fuente = FuenteRegistroAsistencia.AppMovil,
    decimal? Latitud = null,
    decimal? Longitud = null,
    string? Observaciones = null,
    string? ClienteComercial = null,
    string? UbicacionComercial = null);

public record RegistroAsistenciaDto(
    Guid Id,
    Guid ColaboradorId,
    DateTime Fecha,
    DateTime? HoraEntrada,
    DateTime? HoraSalida,
    string Fuente,
    string TipoRegistro,
    string Estado,
    decimal? HorasTrabajadas,
    string? Observaciones);

public record AttendanceSummaryDto(
    RegistroAsistenciaDto? RegistroHoy,
    int RetardosPeriodo,
    int AusenciasPeriodo,
    int CorreccionesPendientes,
    IReadOnlyList<RegistroAsistenciaDto> HistorialReciente,
    bool PuedenChecarRemotamente);

public record CrearCorreccionDto(
    DateTime Fecha,
    TipoCorreccionAsistencia TipoCorreccion,
    DateTime? HoraEntradaSolicitada,
    DateTime? HoraSalidaSolicitada,
    string Motivo,
    string? EvidenciaRef,
    Guid? RegistroId);

public record CorreccionDto(
    Guid Id,
    Guid IncidenciaId,
    string SolicitanteNombre,
    DateTime Fecha,
    string TipoCorreccion,
    DateTime? HoraEntradaSolicitada,
    DateTime? HoraSalidaSolicitada,
    string Motivo,
    string Estado,
    string? RegistroOriginalEntrada,
    string? RegistroOriginalSalida);

public record IncidenciaDto(
    Guid Id,
    Guid ColaboradorId,
    string ColaboradorNombre,
    string? Departamento,
    DateTime Fecha,
    string Tipo,
    string Estado,
    string? Descripcion,
    bool GeneraPrenomina,
    string? ValidacionLider);

public record TeamAttendanceSummaryDto(
    int Presentes,
    int Ausentes,
    int Retardos,
    int SalidasTempranas,
    int CorreccionesPendientes,
    IReadOnlyList<RegistroAsistenciaDto> Registros);

public record AttendanceDashboardDto(
    int PresentesHoy,
    int AusentesHoy,
    int RetardosHoy,
    int SalidasTempranas,
    int HorasExtra,
    int CorreccionesPendientes,
    int IncidenciasNomina,
    int ComercialSinReporte,
    DateTime? UltimoCorteGenerado,
    IReadOnlyList<IncidenciaDto> IncidenciasRecientes);

public record UpsertTurnoDto(
    string Codigo,
    string Nombre,
    TimeSpan HoraEntrada,
    TimeSpan HoraSalida,
    int MinutosToleranciaEntrada,
    int MinutosToleranciaSalida,
    int MinutosComida,
    bool AplicaLunes,
    bool AplicaMartes,
    bool AplicaMiercoles,
    bool AplicaJueves,
    bool AplicaViernes,
    bool AplicaSabado,
    bool AplicaDomingo,
    Guid? SedeId,
    Guid? AreaId,
    bool IsActive = true);

public record TurnoDto(
    Guid Id,
    string Codigo,
    string Nombre,
    TimeSpan HoraEntrada,
    TimeSpan HoraSalida,
    int MinutosToleranciaEntrada,
    int MinutosComida,
    bool IsActive);

public record AsignacionTurnoDto(Guid Id, Guid ColaboradorId, string ColaboradorNombre, Guid TurnoId, string TurnoNombre, DateTime FechaInicio, DateTime? FechaFin, string Origen);

public record UpsertAsignacionTurnoDto(Guid ColaboradorId, Guid TurnoId, DateTime FechaInicio, DateTime? FechaFin, string Origen = "Manual");

public record ReglasAsistenciaDto(
    Guid Id,
    Guid? SedeId,
    int MinutosToleranciaRetardo,
    int MinutosParaFalta,
    bool GeneraIncidenciaNominaRetardo,
    bool RequiereValidacionLider,
    bool PermitirRegistroMovil,
    bool RequiereGeolocalizacion,
    int RadioMetrosSede);

public record IncidenciaNominaAsistenciaDto(
    Guid Id,
    string NumeroEmpleado,
    string ColaboradorNombre,
    string Periodo,
    string TipoConcepto,
    decimal Cantidad,
    string Unidad,
    string Estado,
    DateTime FechaGeneracion,
    string? ValidadoPor,
    DateTime? NominaSyncAt);

public record DecisionCorreccionDto(string? Comentario);

public record SolicitudCambioHorarioDto(
    Guid Id,
    Guid ColaboradorId,
    string ColaboradorNombre,
    Guid TurnoActualId,
    string TurnoActualNombre,
    Guid TurnoSolicitadoId,
    string TurnoSolicitadoNombre,
    string Motivo,
    string Estado,
    string? ComentarioAprobador,
    Guid? AprobadorId,
    string? AprobadorNombre,
    DateTime? FechaDecision,
    DateTime CreatedAt);

public record CrearSolicitudCambioHorarioDto(
    Guid TurnoSolicitadoId,
    string Motivo);

public record DecisionCambioHorarioDto(
    string? Comentario);

public record ChecadorVerifyDto(
    string NumeroEmpleado,
    Guid SedeId);

public record ChecadorResultDto(
    bool Success,
    string? Error,
    string? Warning,
    string? EmpleadoNombre,
    string? NumeroEmpleado,
    string? TipoRegistro,
    DateTime? HoraRegistro,
    string? TurnoNombre,
    string? TurnoHorario);

public interface IAttendanceService
{
    Task<ChecadorResultDto> VerifyAndRegisterChecadorAsync(ChecadorVerifyDto dto, CancellationToken cancellationToken = default);
    Task<AttendanceSummaryDto> GetMySummaryAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistroAsistenciaDto>> GetMyRecordsAsync(Guid colaboradorId, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default);
    Task<Result<RegistroAsistenciaDto>> CheckInAsync(Guid colaboradorId, CheckInOutDto dto, CancellationToken cancellationToken = default);
    Task<Result<RegistroAsistenciaDto>> CheckOutAsync(Guid colaboradorId, CheckInOutDto dto, CancellationToken cancellationToken = default);
    Task<Result<CorreccionDto>> CreateCorrectionAsync(Guid colaboradorId, CrearCorreccionDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CorreccionDto>> GetMyCorrectionsAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CorreccionDto>> GetPendingCorrectionsAsync(Guid jefeId, CancellationToken cancellationToken = default);
    Task<Result<CorreccionDto>> ApproveCorrectionAsync(Guid aprobadorId, Guid correctionId, DecisionCorreccionDto dto, bool isRhAdmin, CancellationToken cancellationToken = default);
    Task<Result<CorreccionDto>> RejectCorrectionAsync(Guid aprobadorId, Guid correctionId, DecisionCorreccionDto dto, bool isRhAdmin, CancellationToken cancellationToken = default);
    Task<TeamAttendanceSummaryDto> GetTeamSummaryAsync(Guid jefeId, DateTime fecha, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IncidenciaDto>> GetTeamIncidentsAsync(Guid jefeId, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TurnoDto>> GetAvailableShiftsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AsignacionTurnoDto>> GetMyShiftHistoryAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
    Task<Result<SolicitudCambioHorarioDto>> CreateShiftChangeRequestAsync(Guid colaboradorId, CrearSolicitudCambioHorarioDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SolicitudCambioHorarioDto>> GetMyShiftChangeRequestsAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SolicitudCambioHorarioDto>> GetPendingShiftChangeRequestsAsync(Guid jefeId, CancellationToken cancellationToken = default);
    Task<Result<SolicitudCambioHorarioDto>> ApproveShiftChangeRequestAsync(Guid aprobadorId, Guid requestId, DecisionCambioHorarioDto dto, bool isRhAdmin, CancellationToken cancellationToken = default);
    Task<Result<SolicitudCambioHorarioDto>> RejectShiftChangeRequestAsync(Guid aprobadorId, Guid requestId, DecisionCambioHorarioDto dto, bool isRhAdmin, CancellationToken cancellationToken = default);
}

public interface IAttendanceAdminService
{
    Task<AttendanceDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistroAsistenciaDto>> ListRecordsAsync(DateTime? fecha, Guid? sedeId, Guid? departamentoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IncidenciaDto>> ListIncidentsAsync(EstadoIncidenciaAsistencia? estado, DateTime? desde, DateTime? hasta, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CorreccionDto>> ListCorrectionsAsync(EstadoCorreccionAsistencia? estado, CancellationToken cancellationToken = default);
    Task<int> GenerateDailyIncidentsAsync(DateTime? fecha, string performedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IncidenciaNominaAsistenciaDto>> ListPayrollIncidentsAsync(CancellationToken cancellationToken = default);
    Task<int> GeneratePayrollCutAsync(string periodo, string performedBy, CancellationToken cancellationToken = default);
    Task SendPayrollCutAsync(string performedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TurnoDto>> ListShiftsAsync(CancellationToken cancellationToken = default);
    Task<TurnoDto> UpsertShiftAsync(Guid? id, UpsertTurnoDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AsignacionTurnoDto>> ListShiftAssignmentsAsync(CancellationToken cancellationToken = default);
    Task<AsignacionTurnoDto> AssignShiftAsync(UpsertAsignacionTurnoDto dto, CancellationToken cancellationToken = default);
    Task<ReglasAsistenciaDto> GetRulesAsync(Guid? sedeId, CancellationToken cancellationToken = default);
    Task<ReglasAsistenciaDto> UpdateRulesAsync(ReglasAsistenciaDto dto, CancellationToken cancellationToken = default);
    Task<Result<RegistroAsistenciaDto>> CreateManualRecordAsync(Guid colaboradorId, DateTime fecha, DateTime? entrada, DateTime? salida, string performedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegistroAsistenciaDto>> ListCommercialRecordsAsync(DateTime? fecha, CancellationToken cancellationToken = default);
}
