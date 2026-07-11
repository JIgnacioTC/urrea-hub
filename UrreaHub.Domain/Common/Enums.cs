namespace UrreaHub.Domain.Common;

public enum EstadoSolicitud
{
    Borrador,
    Pendiente,
    Aprobada,
    Rechazada,
    Cancelada,
    SolicitaAjuste,
    AplicadaNomina,
    ErrorNomina,
    ConvertidaVacante,
}

public enum NivelAprobacionAusencia
{
    Jefe,
    DH,
    Nominas,
}

public enum NivelConfidencialidad
{
    Publico,
    Interno,
    Restringido,
    Confidencial
}

public enum TipoMovimientoOrganizacional
{
    Alta,
    Baja,
    Promocion,
    Transferencia,
    Reorganizacion
}

public enum EstadoVacante
{
    Abierta,
    EnProceso,
    Cerrada,
    Cancelada
}

public enum EstadoPostulacion
{
    Recibida,
    EnRevision,
    Entrevista,
    Oferta,
    Contratado,
    Rechazado
}

public enum EstadoIntegracion
{
    Pendiente,
    Exitosa,
    Fallida
}

public enum RolUsuario
{
    Colaborador,
    Jefe,
    RhAdmin
}

public enum CategoriaPermiso
{
    Vacacion,
    PermisoDiaCompleto,
    PermisoParcial,
    LicenciaLegal
}

public enum EstadoPlanAccion
{
    Pendiente,
    EnProgreso,
    Completado,
    Cancelado
}

public enum CategoriaDocumentoCorporativo
{
    Reglamentos,
    Politicas,
    Etica,
    Prevision,
    Prestaciones,
    Puntualidad
}

public enum TipoDiaFestivo
{
    Oficial,
    Empresa
}

public enum TipoPublicacionPortal
{
    Announcement,
    Recognition,
    Event,
    General
}
