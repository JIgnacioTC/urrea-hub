namespace UrreaHub.Domain.Compensaciones;

public enum TipoAjusteCompensacion
{
    CambioNivelSalarial,
    CambioGrupoNomina,
    CambioJornada,
    CambioTurno,
    CambioRelacionLaboral,
    CambioCentroCosto,
    Promocion,
    AjusteTabulador,
    AjusteExtraordinario,
}

public enum EstadoAjusteCompensacion
{
    Borrador,
    EnRevisionDh,
    EnRevisionFinanzas,
    Aprobado,
    Rechazado,
    AplicadoSap,
    SincronizadoCdm,
    Cancelado,
}
