using UrreaHub.Domain.Common;

namespace UrreaHub.Domain.CoreRH;

public class Colaborador : BaseEntity
{
    public string NumeroEmpleado { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string ApellidoPaterno { get; set; } = string.Empty;
    public string? ApellidoMaterno { get; set; }
    public string? NombrePreferido { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaBaja { get; set; }
    public DateTime? NominaSyncAt { get; set; }

    public string? ExternalSource { get; set; }
    public string? ExternalEmployeeId { get; set; }
    public string? ExternalSystemCode { get; set; }
    public string SyncStatus { get; set; } = EmployeeSyncStatus.Pending;
    public string? SyncHash { get; set; }
    public bool IsManualOverride { get; set; }

    public CuentaAcceso? CuentaAcceso { get; set; }
    public ColaboradorDatosSensibles? DatosSensibles { get; set; }
    public ColaboradorDatosLaborales? DatosLaborales { get; set; }
    public ICollection<MovimientoColaborador> Movimientos { get; set; } = new List<MovimientoColaborador>();

    public Guid PuestoId { get; set; }
    public Puesto Puesto { get; set; } = null!;

    public Guid DepartamentoId { get; set; }
    public Departamento Departamento { get; set; } = null!;

    public Guid? SedeId { get; set; }
    public Sede? Sede { get; set; }

    public Guid? CentroCostoId { get; set; }
    public CentroCosto? CentroCosto { get; set; }

    public Guid RelacionLaboralId { get; set; }
    public RelacionLaboral RelacionLaboral { get; set; } = null!;

    public Guid? JefeDirectoId { get; set; }
    public Colaborador? JefeDirecto { get; set; }
    public ICollection<Colaborador> Subordinados { get; set; } = new List<Colaborador>();
}
