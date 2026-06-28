import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";

const base = v1("/attendance/admin");

export interface AttendanceDashboard {
  presentesHoy: number;
  ausentesHoy: number;
  retardosHoy: number;
  salidasTempranas: number;
  horasExtra: number;
  correccionesPendientes: number;
  incidenciasNomina: number;
  comercialSinReporte: number;
  ultimoCorteGenerado?: string;
  incidenciasRecientes: IncidenciaAsistencia[];
}

export interface IncidenciaAsistencia {
  id: string;
  colaboradorId: string;
  colaboradorNombre: string;
  departamento?: string;
  fecha: string;
  tipo: string;
  estado: string;
  descripcion?: string;
  generaPrenomina: boolean;
}

export interface Turno {
  id: string;
  codigo: string;
  nombre: string;
  horaEntrada: string;
  horaSalida: string;
  minutosToleranciaEntrada: number;
  minutosComida: number;
  isActive: boolean;
}

export interface ReglasAsistencia {
  id: string;
  sedeId?: string;
  minutosToleranciaRetardo: number;
  minutosParaFalta: number;
  generaIncidenciaNominaRetardo: boolean;
  requiereValidacionLider: boolean;
  permitirRegistroMovil: boolean;
  requiereGeolocalizacion: boolean;
  radioMetrosSede: number;
}

export interface IncidenciaNominaAsistencia {
  id: string;
  numeroEmpleado: string;
  colaboradorNombre: string;
  periodo: string;
  tipoConcepto: string;
  cantidad: number;
  unidad: string;
  estado: string;
  fechaGeneracion: string;
  validadoPor?: string;
  nominaSyncAt?: string;
}

export const attendanceAdminService = {
  getDashboard: () => fetchApi<AttendanceDashboard>(`${base}/dashboard`),
  listRecords: (fecha?: string) =>
    fetchApi(`${base}/records${fecha ? `?fecha=${fecha}` : ""}`),
  listIncidents: (params?: Record<string, string>) => {
    const qs = params ? `?${new URLSearchParams(params).toString()}` : "";
    return fetchApi<IncidenciaAsistencia[]>(`${base}/incidents${qs}`);
  },
  generateIncidents: (fecha?: string) =>
    fetchApi<{ generadas: number }>(`${base}/incidents/generate${fecha ? `?fecha=${fecha}` : ""}`, { method: "POST" }),
  listPayrollIncidents: () => fetchApi<IncidenciaNominaAsistencia[]>(`${base}/payroll-incidents`),
  generatePayroll: (periodo: string) =>
    fetchApi<{ generadas: number }>(`${base}/payroll-incidents/generate`, {
      method: "POST",
      body: JSON.stringify({ periodo }),
    }),
  sendPayroll: () => fetchApi<void>(`${base}/payroll-incidents/send`, { method: "POST" }),
  listShifts: () => fetchApi<Turno[]>(`${base}/shifts`),
  upsertShift: (dto: object, id?: string) =>
    fetchApi<Turno>(id ? `${base}/shifts/${id}` : `${base}/shifts`, {
      method: id ? "PUT" : "POST",
      body: JSON.stringify(dto),
    }),
  getRules: () => fetchApi<ReglasAsistencia>(`${base}/rules`),
  updateRules: (dto: ReglasAsistencia) =>
    fetchApi<ReglasAsistencia>(`${base}/rules`, { method: "PUT", body: JSON.stringify(dto) }),
};
