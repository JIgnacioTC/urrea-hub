using UrreaHub.Domain.Common;

namespace UrreaHub.Application.Vacaciones;

public record PoliticaVacacionesDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    int DiasAnuales,
    int AntiguedadMinimaMeses,
    bool Acumulable,
    bool IsActive,
    int ColaboradoresAsignados);

public record UpsertPoliticaDto(
    string Nombre,
    string? Descripcion,
    int DiasAnuales,
    int AntiguedadMinimaMeses,
    bool Acumulable,
    bool IsActive = true);

public record UpsertTipoAusenciaDto(
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
    bool PermiteSolicitudEmpleado = true,
    bool IsActive = true,
    bool RequiereAprobacionJefe = true,
    bool RequiereAprobacionDH = false,
    bool RequiereAprobacionNominas = false);

public record CalendarioLaboralDto(
    Guid Id,
    string Nombre,
    int Anio,
    Guid? SedeId,
    string? SedeNombre,
    bool IsActive,
    IReadOnlyList<DiaInhabilDto> DiasInhabiles);

public record DiaInhabilDto(Guid Id, DateTime Fecha, string Descripcion, bool EsOficial);

public record UpsertCalendarioDto(string Nombre, int Anio, Guid? SedeId, bool IsActive = true);

public record UpsertDiaInhabilDto(DateTime Fecha, string Descripcion, bool EsOficial = true);

public record AdminBalanceDto(
    Guid ColaboradorId,
    string NumeroEmpleado,
    string NombreCompleto,
    int Anio,
    string PoliticaNombre,
    decimal DiasAsignados,
    decimal DiasUsados,
    decimal DiasComprometidos,
    decimal DiasDisponibles,
    DateTime? UltimaActualizacion);

public record AdjustBalanceDto(decimal DiasAsignados, string Motivo);

public record RecalculateBalanceDto(int? Anio);

public record AdminSolicitudDto(
    Guid Id,
    string NumeroEmpleado,
    string ColaboradorNombre,
    string Departamento,
    string? Area,
    string TipoAusencia,
    DateTime FechaInicio,
    DateTime FechaFin,
    decimal DiasSolicitados,
    EstadoSolicitud Estado,
    DateTime CreatedAt,
    string? AprobadorNombre);

public record IncidenciaNominaDto(
    Guid Id,
    Guid SolicitudId,
    string NumeroEmpleado,
    string ColaboradorNombre,
    string TipoIncidencia,
    DateTime FechaInicio,
    DateTime FechaFin,
    decimal Dias,
    string Estado,
    DateTime CreatedAt);

public interface IAbsenceAdminService
{
    Task<IReadOnlyList<PoliticaVacacionesDto>> ListPoliciesAsync(CancellationToken cancellationToken = default);
    Task<PoliticaVacacionesDto> UpsertPolicyAsync(Guid? id, UpsertPoliticaDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TipoAusenciaDto>> ListTypesAsync(CancellationToken cancellationToken = default);
    Task<TipoAusenciaDto> UpsertTypeAsync(Guid? id, UpsertTipoAusenciaDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CalendarioLaboralDto>> ListCalendarsAsync(int? anio, CancellationToken cancellationToken = default);
    Task<CalendarioLaboralDto> UpsertCalendarAsync(Guid? id, UpsertCalendarioDto dto, CancellationToken cancellationToken = default);
    Task<DiaInhabilDto> AddHolidayAsync(Guid calendarioId, UpsertDiaInhabilDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminBalanceDto>> ListBalancesAsync(int? anio, string? search, CancellationToken cancellationToken = default);
    Task AdjustBalanceAsync(Guid colaboradorId, AdjustBalanceDto dto, int? anio, string performedBy, CancellationToken cancellationToken = default);
    Task RecalculateBalancesAsync(RecalculateBalanceDto dto, string performedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminSolicitudDto>> ListRequestsAsync(
        EstadoSolicitud? estado,
        Guid? sedeId,
        Guid? tipoId,
        DateTime? desde,
        DateTime? hasta,
        string? search,
        CancellationToken cancellationToken = default);
    Task CancelAdministrativelyAsync(Guid solicitudId, string motivo, string performedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<IncidenciaNominaDto>> ListPayrollIncidentsAsync(CancellationToken cancellationToken = default);
    Task DeleteTypeAsync(Guid id, CancellationToken cancellationToken = default);
}
