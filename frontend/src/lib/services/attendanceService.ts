import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";

export interface RegistroAsistencia {
  id: string;
  colaboradorId: string;
  fecha: string;
  horaEntrada?: string;
  horaSalida?: string;
  fuente: string;
  tipoRegistro: string;
  estado: string;
  horasTrabajadas?: number;
  observaciones?: string;
}

export interface AttendanceSummary {
  registroHoy?: RegistroAsistencia;
  retardosPeriodo: number;
  ausenciasPeriodo: number;
  correccionesPendientes: number;
  historialReciente: RegistroAsistencia[];
}

export interface CorreccionAsistencia {
  id: string;
  incidenciaId: string;
  solicitanteNombre: string;
  fecha: string;
  tipoCorreccion: string;
  horaEntradaSolicitada?: string;
  horaSalidaSolicitada?: string;
  motivo: string;
  estado: string;
  registroOriginalEntrada?: string;
  registroOriginalSalida?: string;
}

export const attendanceService = {
  getMySummary: () => fetchApi<AttendanceSummary>(v1("/attendance/my-summary")),
  getMyRecords: (desde?: string, hasta?: string) => {
    const params = new URLSearchParams();
    if (desde) params.set("desde", desde);
    if (hasta) params.set("hasta", hasta);
    const qs = params.toString();
    return fetchApi<RegistroAsistencia[]>(`${v1("/attendance/my-records")}${qs ? `?${qs}` : ""}`);
  },
  checkIn: (body?: object) =>
    fetchApi<RegistroAsistencia>(v1("/attendance/check-in"), { method: "POST", body: JSON.stringify(body ?? { fuente: "AppMovil" }) }),
  checkOut: (body?: object) =>
    fetchApi<RegistroAsistencia>(v1("/attendance/check-out"), { method: "POST", body: JSON.stringify(body ?? { fuente: "AppMovil" }) }),
  createCorrection: (body: object) =>
    fetchApi<CorreccionAsistencia>(v1("/attendance/corrections"), { method: "POST", body: JSON.stringify(body) }),
  getMyCorrections: () => fetchApi<CorreccionAsistencia[]>(v1("/attendance/corrections/my")),
  getTeamSummary: (fecha?: string) =>
    fetchApi<{ presentes: number; ausentes: number; retardos: number; salidasTempranas: number; correccionesPendientes: number; registros: RegistroAsistencia[] }>(
      `${v1("/attendance/team/summary")}${fecha ? `?fecha=${fecha}` : ""}`,
    ),
  getPendingCorrections: () => fetchApi<CorreccionAsistencia[]>(v1("/attendance/team/corrections/pending")),
  approveCorrection: (id: string, comentario?: string) =>
    fetchApi<CorreccionAsistencia>(v1(`/attendance/corrections/${id}/approve`), {
      method: "POST",
      body: JSON.stringify({ comentario: comentario ?? null }),
    }),
  rejectCorrection: (id: string, comentario?: string) =>
    fetchApi<CorreccionAsistencia>(v1(`/attendance/corrections/${id}/reject`), {
      method: "POST",
      body: JSON.stringify({ comentario: comentario ?? null }),
    }),
};
