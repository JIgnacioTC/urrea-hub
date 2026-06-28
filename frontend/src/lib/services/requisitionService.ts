import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";

export interface RequisicionResumen {
  id: string;
  folio: string;
  titulo: string;
  departamento?: string;
  vacantesSolicitadas: number;
  estado: string;
  solicitanteNombre: string;
  aprobadorActual?: string;
  fechaSolicitud: string;
}

export interface Requisicion extends RequisicionResumen {
  motivo?: string;
  impactoNegocio?: string;
  descripcionPuesto?: string;
  montoEstimado?: number;
  moneda?: string;
  historial: { accion: string; detalle?: string; fecha: string; usuario?: string }[];
}

export const requisitionService = {
  list: () => fetchApi<RequisicionResumen[]>(v1("/requisitions")),
  get: (id: string) => fetchApi<Requisicion>(v1(`/requisitions/${id}`)),
  create: (body: object) => fetchApi<Requisicion>(v1("/requisitions"), { method: "POST", body: JSON.stringify(body) }),
  update: (id: string, body: object) => fetchApi<Requisicion>(v1(`/requisitions/${id}`), { method: "PUT", body: JSON.stringify(body) }),
  submit: (id: string) => fetchApi<Requisicion>(v1(`/requisitions/${id}/submit`), { method: "POST" }),
  pendingApprovals: () => fetchApi<RequisicionResumen[]>(v1("/requisitions/pending-approvals")),
  approve: (id: string, comentario?: string) =>
    fetchApi<Requisicion>(v1(`/requisitions/${id}/approve`), { method: "POST", body: JSON.stringify({ comentario }) }),
  reject: (id: string, comentario?: string) =>
    fetchApi<Requisicion>(v1(`/requisitions/${id}/reject`), { method: "POST", body: JSON.stringify({ comentario }) }),
};
