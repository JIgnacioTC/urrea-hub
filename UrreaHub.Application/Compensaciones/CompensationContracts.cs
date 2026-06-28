using UrreaHub.Application.Common;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.Compensaciones;

namespace UrreaHub.Application.Compensaciones;

public record CompensacionColaboradorDto(
    Guid ColaboradorId,
    string NumeroEmpleado,
    string NombreCompleto,
    string Puesto,
    string Departamento,
    string? CentroCosto,
    string? GrupoNomina,
    string? NivelSalarial,
    string? Jornada,
    string? Turno,
    bool Sindicalizado,
    string NivelVisibilidad,
    string Origen,
    DateTime? UltimaActualizacion,
    bool OverrideManual);

public record SolicitudAjusteDto(
    Guid Id,
    string ColaboradorNombre,
    string NumeroEmpleado,
    string TipoAjuste,
    string Estado,
    string ValorAnterior,
    string ValorNuevo,
    string Motivo,
    DateTime FechaSolicitud,
    decimal? MontoReferencia,
    IReadOnlyList<HistorialAjusteDto> Historial);

public record HistorialAjusteDto(string Accion, string? Detalle, DateTime Fecha, string? Usuario);

public record CrearSolicitudAjusteDto(
    Guid ColaboradorId,
    TipoAjusteCompensacion TipoAjuste,
    string ValorNuevo,
    string Motivo,
    decimal? MontoReferencia,
    bool RequiereFinanzas = false);

public record DecisionAjusteDto(string? Comentario);

public record TabuladorDto(Guid Id, string Codigo, string Nombre, string Moneda, IReadOnlyList<BandaSalarialDto> Bandas);

public record BandaSalarialDto(string Nivel, decimal Minimo, decimal Medio, decimal Maximo);

public record ConceptoCompensacionDto(Guid Id, string Codigo, string Nombre, string Tipo, bool ImpactaNomina);

public record CompensacionDashboardDto(
    int ColaboradoresConDatos,
    int SolicitudesPendientes,
    int SolicitudesAprobadas,
    int ListasNomina,
    IReadOnlyList<SolicitudAjusteResumenDto> Recientes);

public record SolicitudAjusteResumenDto(
    Guid Id,
    string ColaboradorNombre,
    string TipoAjuste,
    string Estado,
    DateTime FechaSolicitud);

public record MiCompensacionDto(
    IReadOnlyList<BeneficioActivoDto> BeneficiosActivos,
    IReadOnlyList<BeneficioDisponibleDto> BeneficiosDisponibles,
    IReadOnlyList<SolicitudBeneficioResumenDto> SolicitudesBeneficio,
    IReadOnlyList<SolicitudAjusteResumenDto> SolicitudesAjuste,
    bool MuestraDetalleCompensacion);

public record BeneficioActivoDto(Guid Id, string Nombre, string? Descripcion);
public record BeneficioDisponibleDto(Guid Id, string Codigo, string Nombre, string? Descripcion);
public record SolicitudBeneficioResumenDto(Guid Id, string BeneficioNombre, string Estado, DateTime Fecha);

public interface ICompensationService
{
    Task<IReadOnlyList<CompensacionColaboradorDto>> ListColaboradoresAsync(CancellationToken cancellationToken = default);
    Task<CompensacionColaboradorDto?> GetColaboradorAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
    Task<Result<SolicitudAjusteDto>> CreateAdjustmentRequestAsync(Guid solicitanteId, CrearSolicitudAjusteDto dto, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SolicitudAjusteDto>> ListAdjustmentRequestsAsync(EstadoAjusteCompensacion? estado, CancellationToken cancellationToken = default);
    Task<Result<SolicitudAjusteDto>> SubmitAdjustmentAsync(Guid solicitanteId, Guid id, CancellationToken cancellationToken = default);
    Task<Result<SolicitudAjusteDto>> ApproveAdjustmentAsync(Guid aprobadorId, Guid id, DecisionAjusteDto dto, bool isFinanzas, CancellationToken cancellationToken = default);
    Task<Result<SolicitudAjusteDto>> RejectAdjustmentAsync(Guid aprobadorId, Guid id, DecisionAjusteDto dto, CancellationToken cancellationToken = default);
    Task<Result<SolicitudAjusteDto>> ApplyAdjustmentAsync(Guid id, string performedBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TabuladorDto>> ListTabuladoresAsync(CancellationToken cancellationToken = default);
    Task<MiCompensacionDto> GetMyPackageAsync(Guid colaboradorId, CancellationToken cancellationToken = default);
    Task<CompensacionDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
}

public interface IBenefitsAdminService
{
    Task<IReadOnlyList<BeneficioDisponibleDto>> ListBenefitsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SolicitudBeneficioAdminDto>> ListBenefitRequestsAsync(EstadoSolicitud? estado, CancellationToken cancellationToken = default);
    Task<Result<SolicitudBeneficioAdminDto>> ApproveBenefitRequestAsync(Guid aprobadorId, Guid solicitudId, DecisionAjusteDto dto, CancellationToken cancellationToken = default);
    Task<Result<SolicitudBeneficioAdminDto>> RejectBenefitRequestAsync(Guid aprobadorId, Guid solicitudId, DecisionAjusteDto dto, CancellationToken cancellationToken = default);
    Task<Result<SolicitudBeneficioAdminDto>> CreateBenefitRequestAsync(Guid colaboradorId, Guid beneficioId, decimal? monto, string? justificacion, CancellationToken cancellationToken = default);
}

public record SolicitudBeneficioAdminDto(
    Guid Id,
    string ColaboradorNombre,
    string BeneficioNombre,
    decimal? Monto,
    string Estado,
    DateTime Fecha);
