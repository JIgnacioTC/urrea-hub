using UrreaHub.Application.Common;
using UrreaHub.Domain.Common;

namespace UrreaHub.Application.Requisiciones;

public record RequisicionDto(
    Guid Id,
    string Folio,
    string Titulo,
    int VacantesSolicitadas,
    string Estado,
    DateTime FechaSolicitud,
    string SolicitanteNombre,
    string? Departamento,
    string? AprobadorActual,
    decimal? MontoEstimado,
    string? Moneda,
    string? Motivo,
    string? ImpactoNegocio,
    string? DescripcionPuesto,
    IReadOnlyList<HistorialRequisicionDto> Historial);

public record HistorialRequisicionDto(string Accion, string? Detalle, DateTime Fecha, string? Usuario);

public record RequisicionResumenDto(
    Guid Id,
    string Folio,
    string Titulo,
    string? Departamento,
    int VacantesSolicitadas,
    string Estado,
    string SolicitanteNombre,
    string? AprobadorActual,
    DateTime FechaSolicitud);

public record CrearRequisicionDto(
    string Titulo,
    int VacantesSolicitadas,
    Guid? DepartamentoId,
    string Motivo,
    string? ImpactoNegocio,
    string? AlternativasConsideradas,
    string DescripcionPuesto,
    string? ExperienciaRequerida,
    string? EducacionRequerida,
    string? CompetenciasRequeridas,
    decimal MontoAutorizado,
    string Moneda,
    string? CentroCostoCodigo);

public record ActualizarRequisicionDto(
    string Titulo,
    int VacantesSolicitadas,
    Guid? DepartamentoId,
    string Motivo,
    string? ImpactoNegocio,
    string? AlternativasConsideradas,
    string DescripcionPuesto,
    string? ExperienciaRequerida,
    string? EducacionRequerida,
    string? CompetenciasRequeridas,
    decimal MontoAutorizado,
    string Moneda,
    string? CentroCostoCodigo);

public record DecisionRequisicionDto(string? Comentario);

public record RequisicionDashboardDto(
    int Total,
    int Borradores,
    int EnAprobacion,
    int Aprobadas,
    int Convertidas,
    IReadOnlyList<RequisicionResumenDto> Recientes);

public interface IRequisitionService
{
    Task<IReadOnlyList<RequisicionResumenDto>> ListMyAsync(Guid solicitanteId, CancellationToken cancellationToken = default);
    Task<RequisicionDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<RequisicionDto>> CreateAsync(Guid solicitanteId, CrearRequisicionDto dto, CancellationToken cancellationToken = default);
    Task<Result<RequisicionDto>> UpdateAsync(Guid solicitanteId, Guid id, ActualizarRequisicionDto dto, CancellationToken cancellationToken = default);
    Task<Result<RequisicionDto>> SubmitAsync(Guid solicitanteId, Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RequisicionResumenDto>> ListPendingApprovalsAsync(Guid aprobadorId, CancellationToken cancellationToken = default);
    Task<Result<RequisicionDto>> ApproveAsync(Guid aprobadorId, Guid id, DecisionRequisicionDto dto, CancellationToken cancellationToken = default);
    Task<Result<RequisicionDto>> RejectAsync(Guid aprobadorId, Guid id, DecisionRequisicionDto dto, CancellationToken cancellationToken = default);
    Task<Result<RequisicionDto>> RequestChangesAsync(Guid aprobadorId, Guid id, DecisionRequisicionDto dto, CancellationToken cancellationToken = default);
}

public interface IRequisitionAdminService
{
    Task<RequisicionDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RequisicionResumenDto>> ListAllAsync(EstadoSolicitud? estado, Guid? departamentoId, CancellationToken cancellationToken = default);
    Task<Result<Guid>> ConvertToVacancyAsync(Guid requisicionId, string performedBy, CancellationToken cancellationToken = default);
}
