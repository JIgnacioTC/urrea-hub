using System.ComponentModel.DataAnnotations.Schema;
using UrreaHub.Domain.Common;
using UrreaHub.Domain.CoreRH;

namespace UrreaHub.Domain.Vacaciones;

public class PoliticaVacaciones : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int DiasAnuales { get; set; }
    public int AntiguedadMinimaMeses { get; set; }
    public bool Acumulable { get; set; }
}

public class TipoAusencia : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public bool DescuentaSaldo { get; set; }
    public bool RequiereAprobacion { get; set; }
    public bool RequiereAprobacionJefe { get; set; } = true;
    public bool RequiereAprobacionDH { get; set; }
    public bool RequiereAprobacionNominas { get; set; }
    public string? Color { get; set; }
    public CategoriaPermiso Categoria { get; set; } = CategoriaPermiso.PermisoDiaCompleto;
    public bool EsParcial { get; set; }
    public bool PermiteMultiDia { get; set; } = true;
    public decimal? DiasMaximosAnuales { get; set; }
    public decimal? DiasMaximosEvento { get; set; }
    public bool RequiereComprobante { get; set; }
    public bool Remunerado { get; set; } = true;
    public string? BaseLegalLft { get; set; }
    public string? Descripcion { get; set; }
    public string? Icono { get; set; }
    public int Orden { get; set; }
    public int? IdLegacy { get; set; }
    public string? WebhookUrl { get; set; }
    public Guid? AreaDestinoId { get; set; }
    public Area? AreaDestino { get; set; }
    public bool PermiteSolicitudEmpleado { get; set; } = true;
    public bool NotificarTeams { get; set; }
    public bool NotificarCorreo { get; set; }

    public ICollection<SolicitudAusencia> Solicitudes { get; set; } = new List<SolicitudAusencia>();
}

public class SaldoVacaciones : BaseEntity
{
    public int Anio { get; set; }
    public decimal DiasAsignados { get; set; }
    public decimal DiasUsados { get; set; }

    [NotMapped]
    public decimal DiasPendientes => DiasAsignados - DiasUsados;

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public Guid PoliticaId { get; set; }
    public PoliticaVacaciones Politica { get; set; } = null!;
}

public class CalendarioLaboral : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public int Anio { get; set; }
    public Guid? SedeId { get; set; }
    public Sede? Sede { get; set; }

    public ICollection<DiaInhabil> DiasInhabiles { get; set; } = new List<DiaInhabil>();
}

public class DiaInhabil : BaseEntity
{
    public DateTime Fecha { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public bool EsOficial { get; set; }

    public Guid CalendarioId { get; set; }
    public CalendarioLaboral Calendario { get; set; } = null!;
}

public class SolicitudAusencia : BaseEntity
{
    public string Folio { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal DiasSolicitados { get; set; }
    public string? Comentario { get; set; }
    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;
    public bool EsDiaCompleto { get; set; } = true;
    public TimeSpan? HoraInicio { get; set; }
    public TimeSpan? HoraFin { get; set; }

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public Guid TipoAusenciaId { get; set; }
    public TipoAusencia TipoAusencia { get; set; } = null!;

    public ICollection<AprobacionAusencia> Aprobaciones { get; set; } = new List<AprobacionAusencia>();
}

public class AprobacionAusencia : BaseEntity
{
    public int Orden { get; set; }
    public NivelAprobacionAusencia Nivel { get; set; }
    public EstadoSolicitud Decision { get; set; } = EstadoSolicitud.Pendiente;
    public string? Comentario { get; set; }
    public DateTime? FechaDecision { get; set; }

    public Guid SolicitudId { get; set; }
    public SolicitudAusencia Solicitud { get; set; } = null!;

    public Guid? AprobadorId { get; set; }
    public Colaborador? Aprobador { get; set; }
}

public class AjusteSaldo : BaseEntity
{
    public Guid SaldoId { get; set; }
    public SaldoVacaciones Saldo { get; set; } = null!;

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public decimal DiasAnteriores { get; set; }
    public decimal DiasNuevos { get; set; }
    public decimal Delta { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string RealizadoPor { get; set; } = string.Empty;
}

public class IncidenciaNomina : BaseEntity
{
    public Guid SolicitudId { get; set; }
    public SolicitudAusencia Solicitud { get; set; } = null!;

    public Guid ColaboradorId { get; set; }
    public Colaborador Colaborador { get; set; } = null!;

    public string NumeroEmpleado { get; set; } = string.Empty;
    public string TipoIncidencia { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal Dias { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public string? PayloadJson { get; set; }
    public DateTime? EnviadaAt { get; set; }
}
