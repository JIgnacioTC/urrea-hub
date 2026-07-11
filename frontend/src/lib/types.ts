export interface LoginResponse {
  token: string;
  colaboradorId: string;
  nombreCompleto: string;
  numeroEmpleado: string;
  roles: string[];
  debeCambiarPassword: boolean;
}

export interface SaldoVacaciones {
  anio: number;
  diasAsignados: number;
  diasUsados: number;
  diasComprometidos: number;
  diasDisponibles: number;
  /** Alias de diasDisponibles (compatibilidad) */
  diasPendientes: number;
}

export interface CalculateDaysResult {
  diasHabiles: number;
  saldoDisponible?: number;
  saldoPosterior?: number;
  excedeSaldo: boolean;
  tieneTraslape: boolean;
}

export interface PendingApproval extends SolicitudAusencia {
  puesto: string;
  departamento: string;
  saldoDisponible?: number;
  saldoPosterior?: number;
  traslapesEquipo: string[];
}

export interface TeamCalendar {
  desde: string;
  hasta: string;
  ausencias: CalendarioAusencia[];
  diasInhabiles: { fecha: string; descripcion: string; esOficial: boolean }[];
}

export interface TipoAusencia {
  id: string;
  codigo: string;
  nombre: string;
  descuentaSaldo: boolean;
  requiereAprobacion: boolean;
  color?: string;
  categoria?: string;
  esParcial?: boolean;
  permiteMultiDia?: boolean;
  diasMaximosAnuales?: number | null;
  diasMaximosEvento?: number | null;
  requiereComprobante?: boolean;
  remunerado?: boolean;
  baseLegalLft?: string;
  descripcion?: string;
  icono?: string;
  orden?: number;
  permiteSolicitudEmpleado?: boolean;
}

export interface ResumenTipoPermiso {
  tipoAusenciaId: string;
  codigo: string;
  nombre: string;
  diasUsadosAnio: number;
  diasMaximosAnuales?: number | null;
  diasMaximosEvento?: number | null;
  diasDisponibles?: number | null;
}

export interface SolicitudAusencia {
  id: string;
  colaboradorId: string;
  colaboradorNombre: string;
  tipoAusenciaId: string;
  tipoAusenciaNombre: string;
  tipoAusenciaCodigo: string;
  fechaInicio: string;
  fechaFin: string;
  diasSolicitados: number;
  comentario?: string;
  estado: string;
  createdAt: string;
  esDiaCompleto?: boolean;
  horaInicio?: string | null;
  horaFin?: string | null;
}

export interface ColaboradorPerfil {
  id: string;
  numeroEmpleado: string;
  nombre: string;
  apellidoPaterno: string;
  apellidoMaterno?: string;
  email: string;
  rfc?: string;
  telefono?: string;
  fechaIngreso: string;
  fechaBaja?: string;
  nominaSyncAt?: string;
  puesto: string;
  departamento: string;
  sede?: string;
  jefeDirecto?: string;
  subordinados: Array<{
    id: string;
    numeroEmpleado: string;
    nombreCompleto: string;
    puesto: string;
  }>;
}

export interface CalendarioAusencia {
  solicitudId: string;
  colaboradorId: string;
  colaboradorNombre: string;
  tipoAusencia: string;
  color: string;
  fechaInicio: string;
  fechaFin: string;
  estado: string;
}

export interface RhDashboard {
  solicitudesPendientes: number;
  aprobadasMes: number;
  colaboradoresActivos: number;
  solicitudesRechazadasMes: number;
}

export interface PagedResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface ColaboradorResumen {
  id: string;
  numeroEmpleado: string;
  nombreCompleto: string;
  puesto: string;
}

export interface HcmDashboard {
  colaboradoresActivos: number;
  centrosCosto: number;
  unidadesOrganizativas: number;
  sedes: number;
  ultimaSyncNomina?: string;
}

export interface ColaboradorHcmList {
  id: string;
  numeroEmpleado: string;
  nombreCompleto: string;
  puesto: string;
  departamento: string;
  area?: string;
  centroCosto?: string;
  jefeNombre?: string;
  estatus: string;
  sede?: string;
  antiguedadAnios: number;
}

export interface ColaboradorHcmDetail {
  id: string;
  numeroEmpleado: string;
  nombre: string;
  apellidoPaterno: string;
  apellidoMaterno?: string;
  email: string;
  rfc?: string;
  telefono?: string;
  fechaIngreso: string;
  fechaBaja?: string;
  nominaSyncAt?: string;
  puesto: string;
  departamento: string;
  area?: string;
  sede?: string;
  centroCosto?: string;
  relacionLaboral: string;
  jefeDirecto?: string;
  estatus: string;
  antiguedadAnios: number;
  subordinados: ColaboradorResumen[];
}

export interface HcmCatalogos {
  sedes: Array<{ id: string; nombre: string }>;
  departamentos: Array<{ id: string; nombre: string }>;
}

export interface OrganigramaNodo {
  id: string;
  numeroEmpleado: string;
  nombreCompleto: string;
  puesto: string;
  departamento: string;
  subordinados: OrganigramaNodo[];
}

export interface ReporteAusencia {
  numeroEmpleado: string;
  colaborador: string;
  tipoAusencia: string;
  fechaInicio: string;
  fechaFin: string;
  dias: number;
  estado: string;
  aprobador?: string;
}

export const ESTADO_LABELS: Record<string, string> = {
  Borrador: "Borrador",
  Pendiente: "Pendiente",
  Aprobada: "Aprobada",
  Rechazada: "Rechazada",
  Cancelada: "Cancelada",
  SolicitaAjuste: "Solicita ajuste",
  AplicadaNomina: "Aplicada en nómina",
  ErrorNomina: "Error en nómina",
};

export const ESTADO_COLORS: Record<string, string> = {
  Borrador: "bg-urrea-bg-soft text-urrea-text-muted",
  Pendiente: "bg-urrea-accent-sand/30 text-urrea-primary",
  Aprobada: "bg-emerald-100 text-emerald-800",
  Rechazada: "bg-red-100 text-red-800",
  Cancelada: "bg-urrea-chrome/40 text-urrea-text-muted",
  SolicitaAjuste: "bg-amber-100 text-amber-800",
  AplicadaNomina: "bg-indigo-100 text-indigo-800",
  ErrorNomina: "bg-red-100 text-red-800",
};

export interface EquipoMiembro {
  id: string;
  numeroEmpleado: string;
  nombreCompleto: string;
  puesto: string;
  departamento: string;
  email: string;
  fechaIngreso: string;
}

export interface PlanAccion {
  id: string;
  colaboradorId: string;
  colaboradorNombre: string;
  titulo: string;
  descripcion?: string;
  fechaInicio: string;
  fechaFin: string;
  estado: string;
  avance: number;
  prioridad: string;
  createdAt: string;
}

export interface FeedbackEquipo {
  id: string;
  colaboradorId: string;
  colaboradorNombre: string;
  tipo: string;
  comentario: string;
  fecha: string;
  autorNombre: string;
}

export interface CapacitacionEquipo {
  inscripcionId: string;
  colaboradorId: string;
  colaboradorNombre: string;
  numeroEmpleado: string;
  cursoCodigo: string;
  cursoNombre: string;
  modalidad?: string;
  duracionHoras: number;
  fechaInscripcion: string;
  fechaCompletado?: string | null;
  estado: string;
  puntuacion?: number | null;
  aprobado?: boolean | null;
}

export const PLAN_ESTADO_LABELS: Record<string, string> = {
  Pendiente: "Pendiente",
  EnProgreso: "En progreso",
  Completado: "Completado",
  Cancelado: "Cancelado",
};

export const PLAN_ESTADO_COLORS: Record<string, string> = {
  Pendiente: "bg-urrea-accent-sand/30 text-urrea-primary",
  EnProgreso: "bg-sky-100 text-sky-800",
  Completado: "bg-emerald-100 text-emerald-800",
  Cancelado: "bg-urrea-chrome/40 text-urrea-text-muted",
};

export interface FeedPost {
  id: string;
  authorName: string;
  authorRole: string;
  authorInitials: string;
  department: string;
  content: string;
  imageGradient?: string;
  likes: number;
  comments: number;
  shares: number;
  createdAt: string;
  type: "announcement" | "recognition" | "event" | "general";
}

export interface DocumentoCorporativo {
  id: string;
  codigo: string;
  categoria: string;
  titulo: string;
  descripcion: string;
  icono?: string;
  actualizado: string;
  paginas?: number;
  urlDocumento?: string;
}

export interface DiaFestivo {
  fecha: string;
  nombre: string;
  tipo: string;
}

export interface ConvenioProveedor {
  id: string;
  codigo: string;
  proveedor: string;
  categoria: string;
  descuento: string;
  descripcion: string;
  icono?: string;
  vigencia: string;
  codigoPromocional?: string;
}

export interface ProductoTienda {
  id: string;
  codigo: string;
  nombre: string;
  categoria: string;
  puntos: number;
  stock: number;
  icono?: string;
  gradiente?: string;
  destacado: boolean;
}

export interface BeneficiosCatalogo {
  categoriaLabels: Record<string, string>;
  documentos: DocumentoCorporativo[];
  festivos: DiaFestivo[];
  convenios: ConvenioProveedor[];
  productos: ProductoTienda[];
}

export interface ModuloPortal {
  codigoModulo: string;
  titulo: string;
  subtitulo?: string;
  descripcion?: string;
  icono?: string;
  publicado: boolean;
}

export interface IntegracionEstado {
  id: string;
  nombre: string;
  sistemaExterno: string;
  endpoint?: string;
  estado: string;
  ultimaEjecucion?: string;
}

export interface PortalAdminEstado {
  databaseOk: boolean;
  entorno: string;
  connectionInfo: string;
  publicaciones: number;
  documentos: number;
  convenios: number;
  productos: number;
  modulos: number;
  colaboradoresActivos: number;
  integraciones: IntegracionEstado[];
}
