import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";
import type { TipoAusencia } from "@/lib/types";

const base = v1("/absence/admin");

export interface PoliticaVacaciones {
  id: string;
  nombre: string;
  descripcion?: string;
  diasAnuales: number;
  antiguedadMinimaMeses: number;
  acumulable: boolean;
  isActive: boolean;
  colaboradoresAsignados: number;
}

export interface AdminBalance {
  colaboradorId: string;
  numeroEmpleado: string;
  nombreCompleto: string;
  anio: number;
  politicaNombre: string;
  diasAsignados: number;
  diasUsados: number;
  diasComprometidos: number;
  diasDisponibles: number;
  ultimaActualizacion?: string;
}

export interface AdminSolicitud {
  id: string;
  numeroEmpleado: string;
  colaboradorNombre: string;
  departamento: string;
  area?: string;
  tipoAusencia: string;
  fechaInicio: string;
  fechaFin: string;
  diasSolicitados: number;
  estado: string;
  createdAt: string;
  aprobadorNombre?: string;
}

export interface CalendarioLaboral {
  id: string;
  nombre: string;
  anio: number;
  sedeId?: string;
  sedeNombre?: string;
  isActive: boolean;
  diasInhabiles: { id: string; fecha: string; descripcion: string; esOficial: boolean }[];
}

export interface IncidenciaNomina {
  id: string;
  solicitudId: string;
  numeroEmpleado: string;
  colaboradorNombre: string;
  tipoIncidencia: string;
  fechaInicio: string;
  fechaFin: string;
  dias: number;
  estado: string;
  createdAt: string;
}

export const absenceAdminService = {
  listPolicies: () => fetchApi<PoliticaVacaciones[]>(`${base}/policies`),
  upsertPolicy: (dto: object, id?: string) =>
    fetchApi<PoliticaVacaciones>(id ? `${base}/policies/${id}` : `${base}/policies`, {
      method: id ? "PUT" : "POST",
      body: JSON.stringify(dto),
    }),
  listTypes: () => fetchApi<TipoAusencia[]>(`${base}/types`),
  upsertType: (dto: object, id?: string) =>
    fetchApi<TipoAusencia>(id ? `${base}/types/${id}` : `${base}/types`, {
      method: id ? "PUT" : "POST",
      body: JSON.stringify(dto),
    }),
  listCalendars: (anio?: number) =>
    fetchApi<CalendarioLaboral[]>(`${base}/calendars${anio ? `?anio=${anio}` : ""}`),
  upsertCalendar: (dto: object, id?: string) =>
    fetchApi<CalendarioLaboral>(id ? `${base}/calendars/${id}` : `${base}/calendars`, {
      method: id ? "PUT" : "POST",
      body: JSON.stringify(dto),
    }),
  addHoliday: (calendarioId: string, dto: { fecha: string; descripcion: string; esOficial?: boolean }) =>
    fetchApi(`${base}/calendars/${calendarioId}/holidays`, { method: "POST", body: JSON.stringify(dto) }),
  listBalances: (anio?: number, q?: string) => {
    const params = new URLSearchParams();
    if (anio) params.set("anio", String(anio));
    if (q) params.set("q", q);
    const qs = params.toString();
    return fetchApi<AdminBalance[]>(`${base}/balances${qs ? `?${qs}` : ""}`);
  },
  adjustBalance: (colaboradorId: string, dto: { diasAsignados: number; motivo: string }, anio?: number) =>
    fetchApi<void>(`${base}/balances/${colaboradorId}/adjust${anio ? `?anio=${anio}` : ""}`, {
      method: "POST",
      body: JSON.stringify(dto),
    }),
  recalculateBalances: (anio?: number) =>
    fetchApi<void>(`${base}/balances/recalculate`, {
      method: "POST",
      body: JSON.stringify({ anio }),
    }),
  listRequests: (params?: Record<string, string>) => {
    const qs = params ? `?${new URLSearchParams(params).toString()}` : "";
    return fetchApi<AdminSolicitud[]>(`${base}/requests${qs}`);
  },
  cancelRequest: (id: string, motivo: string) =>
    fetchApi<void>(`${base}/requests/${id}/cancel`, {
      method: "POST",
      body: JSON.stringify({ comentario: motivo }),
    }),
  listPayrollIncidents: () => fetchApi<IncidenciaNomina[]>(`${base}/payroll-incidents`),
  deleteType: (id: string) => fetchApi<void>(`${base}/types/${id}`, { method: "DELETE" }),
};
