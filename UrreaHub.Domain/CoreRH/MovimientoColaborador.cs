using UrreaHub.Domain.Common;

namespace UrreaHub.Domain.CoreRH;

public class MovimientoColaborador : BaseEntity
{
    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public string TipoMovimiento { get; set; } = string.Empty;
    public DateTime FechaEfectiva { get; set; }
    public string? ValorAnterior { get; set; }
    public string? ValorNuevo { get; set; }
    public string Origen { get; set; } = EmployeeExternalSource.Manual;
    public string? ReferenciaExterna { get; set; }
    public string CreadoPor { get; set; } = "system";
}
