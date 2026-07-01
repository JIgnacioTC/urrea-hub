using UrreaHub.Application.Common;

namespace UrreaHub.Application.Asistencia;

public record ImportResultDto(
    int TotalFilas,
    int InasistenciasDetectadas,
    int InasistenciasJustificadas,
    int CasosPorRevisar,
    List<string> Errores
);

public interface IAsistenciaImportService
{
    Task<Result<ImportResultDto>> ImportarExcelAsistenciasAsync(Stream excelStream, string performedBy, CancellationToken cancellationToken = default);
}
