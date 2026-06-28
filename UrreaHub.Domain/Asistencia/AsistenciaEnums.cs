namespace UrreaHub.Domain.Asistencia;

public enum FuenteRegistroAsistencia
{
    Biometrico,
    AppMovil,
    Manual,
    Comercial
}

public enum EstadoRegistroAsistencia
{
    Completo,
    EntradaSinSalida,
    SalidaSinEntrada,
    Manual,
    ConIncidencia,
    Validado,
    Cancelado
}

public enum TipoIncidenciaAsistencia
{
    Retardo,
    AusenciaInjustificada,
    SalidaTemprana,
    EntradaOmitida,
    SalidaOmitida,
    HoraExtra,
    TrabajoCampo,
    RegistroManual,
    ErrorBiometrico,
    PermisoRelacionado
}

public enum EstadoIncidenciaAsistencia
{
    Detectada,
    PendienteValidacion,
    Justificada,
    NoJustificada,
    Rechazada,
    AplicadaNomina,
    Cancelada
}

public enum EstadoCorreccionAsistencia
{
    Solicitada,
    EnRevision,
    Aprobada,
    Rechazada,
    Cancelada
}

public enum EstadoIncidenciaNominaAsistencia
{
    Pendiente,
    ListaParaEnvio,
    Enviada,
    Aplicada,
    Error,
    Cancelada
}

public enum TipoCorreccionAsistencia
{
    EntradaOmitida,
    SalidaOmitida,
    ErrorBiometrico,
    VisitaCliente,
    TrabajoCampo,
    ErrorGeolocalizacion,
    CambioHorario,
    Otro
}
