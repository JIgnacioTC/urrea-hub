import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";
import type {
  CalculateDaysResult,
  CalendarioAusencia,
  PendingApproval,
  ResumenTipoPermiso,
  SaldoVacaciones,
  SolicitudAusencia,
  TeamCalendar,
  TipoAusencia,
} from "@/lib/types";

export const absenceService = {
  getVacationTypes: () => fetchApi<TipoAusencia[]>(v1("/absence/types")),

  /** Todos los tipos para crear solicitud (sin duplicar vacaciones + permisos). */
  getRequestTypes: async () => {
    const [all, perm] = await Promise.all([
      fetchApi<TipoAusencia[]>(v1("/absence/types")),
      fetchApi<TipoAusencia[]>(v1("/absence/permission-types")),
    ]);
    const byId = new Map<string, TipoAusencia>();
    for (const t of [...all, ...perm]) {
      if (!byId.has(t.id)) byId.set(t.id, t);
    }
    return [...byId.values()].sort((a, b) => (a.orden ?? 0) - (b.orden ?? 0));
  },

  getPermissionTypes: () => fetchApi<TipoAusencia[]>(v1("/absence/permission-types")),

  getPermissionSummary: (anio?: number) =>
    fetchApi<ResumenTipoPermiso[]>(
      `${v1("/absence/permission-types/summary")}${anio ? `?anio=${anio}` : ""}`,
    ),

  getMyRequests: () => fetchApi<SolicitudAusencia[]>(v1("/absence/my-requests")),

  getRequest: (id: string) => fetchApi<SolicitudAusencia>(v1(`/absence/requests/${id}`)),

  getMyBalance: (year?: number) =>
    fetchApi<SaldoVacaciones>(`${v1("/absence/my-balance")}${year ? `?year=${year}` : ""}`),

  getPendingApprovals: () => fetchApi<PendingApproval[]>(v1("/absence/pending-approvals")),

  getTeamCalendar: (desde: Date, hasta: Date) =>
    fetchApi<TeamCalendar>(
      `${v1("/absence/team-calendar")}?desde=${desde.toISOString()}&hasta=${hasta.toISOString()}`,
    ),

  getCalendar: (desde: Date, hasta: Date) =>
    fetchApi<CalendarioAusencia[]>(
      `${v1("/absence/calendario/ausencias")}?desde=${desde.toISOString()}&hasta=${hasta.toISOString()}`,
    ),

  calculateDays: (fechaInicio: string, fechaFin: string, tipoAusenciaId?: string) =>
    fetchApi<CalculateDaysResult>(v1("/absence/requests/calculate-days"), {
      method: "POST",
      body: JSON.stringify({ fechaInicio, fechaFin, tipoAusenciaId: tipoAusenciaId ?? null }),
    }),

  createRequest: (body: unknown) =>
    fetchApi<SolicitudAusencia>(v1("/absence/requests"), {
      method: "POST",
      body: JSON.stringify(body),
    }),

  submitRequest: (id: string) =>
    fetchApi<SolicitudAusencia>(v1(`/absence/requests/${id}/submit`), { method: "POST" }),

  cancelRequest: (id: string) =>
    fetchApi(v1(`/absence/requests/${id}/cancel`), { method: "POST" }),

  approveRequest: (id: string, comentario?: string | null) =>
    fetchApi(v1(`/absence/requests/${id}/approve`), {
      method: "POST",
      body: JSON.stringify({ comentario: comentario ?? null }),
    }),

  rejectRequest: (id: string, comentario?: string | null) =>
    fetchApi(v1(`/absence/requests/${id}/reject`), {
      method: "POST",
      body: JSON.stringify({ comentario: comentario ?? null }),
    }),
};
