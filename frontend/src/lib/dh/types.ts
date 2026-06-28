export type DhRole = "admin_dh" | "key_user" | "lider" | "colaborador" | "ti";

export type ColaboradorEstatus = "activo" | "baja" | "reingreso" | "pendiente";

export interface ColaboradorHcm {
  id: string;
  numeroEmpleado: string;
  nombreLegal: string;
  nombrePreferido: string;
  rfc: string;
  curp: string;
  nss: string;
  email: string;
  telefono: string;
  puesto: string;
  area: string;
  centroCosto: string;
  jefeId: string | null;
  jefeNombre: string;
  estatus: ColaboradorEstatus;
  antiguedadAnios: number;
  ubicacion: string;
  tipoContrato: string;
  jornada: string;
  salarioMensual: number;
  fechaIngreso: string;
  domicilio: string;
  confidencial?: boolean;
}

export interface Vacante {
  id: string;
  titulo: string;
  area: string;
  puesto: string;
  ubicacion: string;
  vacantes: number;
  estatus: "abierta" | "pausada" | "cerrada";
  fechaApertura: string;
  candidatosCount: number;
}

export interface Candidato {
  id: string;
  vacanteId: string;
  nombre: string;
  email: string;
  etapa: "nuevo" | "filtrado" | "entrevista" | "oferta" | "aceptado" | "rechazado";
  score: number;
  altaSap: "pendiente" | "completada" | "na";
  fechaPostulacion: string;
}

export interface OnboardingProceso {
  id: string;
  colaboradorNombre: string;
  puesto: string;
  area: string;
  fechaInicio: string;
  avance: number;
  tareas: OnboardingTarea[];
}

export interface OnboardingTarea {
  id: string;
  nombre: string;
  responsable: string;
  fechaLimite: string;
  estado: "pendiente" | "en_proceso" | "completado" | "vencido";
}

export interface DocumentoExpediente {
  id: string;
  colaboradorId: string;
  tipo: string;
  nombre: string;
  version: string;
  estado: "pendiente_carga" | "pendiente_firma" | "firmado" | "vencido" | "rechazado";
  vigencia?: string;
  confidencial: boolean;
}

export interface SolicitudAusenciaDh {
  id: string;
  colaboradorId: string;
  colaboradorNombre: string;
  tipo: string;
  fechaInicio: string;
  fechaFin: string;
  dias: number;
  estado: "pendiente" | "aprobada" | "rechazada" | "ajuste";
  comentario?: string;
}

export interface IncidenciaAsistencia {
  id: string;
  colaboradorId: string;
  colaboradorNombre: string;
  fecha: string;
  tipo: "retardo" | "ausencia" | "hora_extra" | "correccion" | "salida_temprano";
  minutos?: number;
  origen: "biometrico" | "app_movil" | "manual";
  estado: "detectada" | "validada" | "correccion_pendiente" | "reportada_nomina";
  area: string;
}

export interface CursoLms {
  id: string;
  nombre: string;
  categoria: string;
  duracionHoras: number;
  modalidad: string;
  estatus: "asignado" | "en_progreso" | "completado" | "vencido";
  fechaLimite: string;
  calificacion?: number;
  obligatorio: boolean;
  colaboradorId?: string;
}

export interface ObjetivoOkr {
  id: string;
  colaboradorId: string;
  colaboradorNombre: string;
  objetivo: string;
  kpi: string;
  avance: number;
  periodo: string;
  estado: "en_curso" | "completado" | "en_riesgo";
  alineacion: string;
}

export interface EncuestaDh {
  id: string;
  titulo: string;
  tipo: "clima" | "pulse" | "onboarding" | "salida" | "libre";
  audiencia: string;
  anonima: boolean;
  tasaRespuesta: number;
  estatus: "borrador" | "activa" | "cerrada";
  fechaCierre: string;
}

export interface TicketServicio {
  id: string;
  folio: string;
  categoria: string;
  asunto: string;
  solicitante: string;
  responsable: string;
  slaHoras: number;
  estado: "nuevo" | "asignado" | "en_proceso" | "en_espera" | "resuelto" | "cerrado";
  prioridad: "baja" | "media" | "alta";
  createdAt: string;
}

export interface DenunciaCaso {
  id: string;
  token: string;
  tipo: "acoso" | "fraude" | "seguridad" | "conflicto_interes" | "conducta" | "otro";
  estado: "recibido" | "triage" | "investigacion" | "requiere_info" | "resuelto" | "cerrado";
  fecha: string;
  responsable?: string;
  confidencial: true;
}

export interface LogIntegracion {
  id: string;
  proceso: string;
  origen: string;
  destino: string;
  registrosOk: number;
  registrosError: number;
  estado: "exitoso" | "parcial" | "fallido";
  fecha: string;
  mensaje?: string;
}

export interface EventoTimeline {
  id: string;
  modulo: string;
  descripcion: string;
  usuario: string;
  fecha: string;
  tipo: "info" | "alerta" | "critico" | "integracion";
}

export interface FeedPublicacion {
  id: string;
  autor: string;
  area: string;
  contenido: string;
  tipo: "anuncio" | "reconocimiento" | "evento";
  reacciones: number;
  comentarios: number;
  fecha: string;
}

export interface BeneficioSolicitud {
  id: string;
  colaboradorNombre: string;
  beneficio: string;
  estatus: "solicitado" | "aprobado" | "rechazado" | "en_proceso";
  fecha: string;
}

export const DH_ROLE_LABELS: Record<DhRole, string> = {
  admin_dh: "Administrador DH",
  key_user: "Key User de módulo",
  lider: "Líder / Jefe directo",
  colaborador: "Colaborador",
  ti: "TI / Integraciones",
};
