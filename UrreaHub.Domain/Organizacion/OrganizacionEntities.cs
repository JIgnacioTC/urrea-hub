using UrreaHub.Domain.Common;
using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Domain.Organizacion;

public class Organigrama : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaVigencia { get; set; }
    public bool EsVigente { get; set; }

    public ICollection<PosicionOrganizacional> Posiciones { get; set; } = new List<PosicionOrganizacional>();
}

public class PosicionOrganizacional : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;

    public Guid OrganigramaId { get; set; }
    public Organigrama Organigrama { get; set; } = null!;

    public Guid? PosicionPadreId { get; set; }
    public PosicionOrganizacional? PosicionPadre { get; set; }
    public ICollection<PosicionOrganizacional> PosicionesHijas { get; set; } = new List<PosicionOrganizacional>();

    public Guid? ColaboradorId { get; set; }
    public Colaborador? Colaborador { get; set; }

    public ICollection<VacanteOrganizacional> Vacantes { get; set; } = new List<VacanteOrganizacional>();
}

public class VacanteOrganizacional : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public EstadoVacante Estado { get; set; } = EstadoVacante.Abierta;
    public DateTime FechaApertura { get; set; }
    public DateTime? FechaCierre { get; set; }

    public Guid PosicionId { get; set; }
    public PosicionOrganizacional Posicion { get; set; } = null!;
}

public class MovimientoOrganizacional : BaseEntity
{
    public TipoMovimientoOrganizacional Tipo { get; set; }
    public DateTime FechaMovimiento { get; set; }
    public string? Motivo { get; set; }
    public string? Observaciones { get; set; }

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public Guid? PosicionOrigenId { get; set; }
    public PosicionOrganizacional? PosicionOrigen { get; set; }

    public Guid? PosicionDestinoId { get; set; }
    public PosicionOrganizacional? PosicionDestino { get; set; }
}
