import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";

const base = v1("/compensation");
const adminBase = v1("/compensation/admin");

export interface CompensacionColaborador {
  colaboradorId: string;
  numeroEmpleado: string;
  nombreCompleto: string;
  puesto: string;
  departamento: string;
  centroCosto?: string;
  grupoNomina?: string;
  nivelSalarial?: string;
  jornada?: string;
  turno?: string;
  sindicalizado: boolean;
  nivelVisibilidad: string;
  origen: string;
  ultimaActualizacion?: string;
  overrideManual: boolean;
}

export interface HistorialAjuste {
  accion: string;
  detalle?: string;
  fecha: string;
  usuario?: string;
}

export interface SolicitudAjuste {
  id: string;
  colaboradorNombre: string;
  numeroEmpleado: string;
  tipoAjuste: string;
  estado: string;
  valorAnterior: string;
  valorNuevo: string;
  motivo: string;
  fechaSolicitud: string;
  montoReferencia?: number;
  historial: HistorialAjuste[];
}

export interface SolicitudAjusteResumen {
  id: string;
  colaboradorNombre: string;
  tipoAjuste: string;
  estado: string;
  fechaSolicitud: string;
}

export interface Tabulador {
  id: string;
  codigo: string;
  nombre: string;
  moneda: string;
  bandas: { nivel: string; minimo: number; medio: number; maximo: number }[];
}

export interface CompensacionDashboard {
  colaboradoresConDatos: number;
  solicitudesPendientes: number;
  solicitudesAprobadas: number;
  listasNomina: number;
  recientes: SolicitudAjusteResumen[];
}

export interface BeneficioDisponible {
  id: string;
  codigo: string;
  nombre: string;
  descripcion?: string;
}

export interface BeneficioActivo {
  id: string;
  nombre: string;
  descripcion?: string;
}

export interface SolicitudBeneficioResumen {
  id: string;
  beneficioNombre: string;
  estado: string;
  fecha: string;
}

export interface MiCompensacion {
  beneficiosActivos: BeneficioActivo[];
  beneficiosDisponibles: BeneficioDisponible[];
  solicitudesBeneficio: SolicitudBeneficioResumen[];
  solicitudesAjuste: SolicitudAjusteResumen[];
  muestraDetalleCompensacion: boolean;
}

export const compensationService = {
  getMyPackage: () => fetchApi<MiCompensacion>(`${base}/my-package`),
  createBenefitRequest: (body: { beneficioId: string; monto?: number; justificacion?: string }) =>
    fetchApi(`${base}/benefit-requests`, { method: "POST", body: JSON.stringify(body) }),
};

export const compensationAdminService = {
  getDashboard: () => fetchApi<CompensacionDashboard>(`${adminBase}/dashboard`),
  listColaboradores: () => fetchApi<CompensacionColaborador[]>(`${adminBase}/colaboradores`),
  listTabuladores: () => fetchApi<Tabulador[]>(`${adminBase}/tabuladores`),
  listAdjustments: (estado?: string) =>
    fetchApi<SolicitudAjuste[]>(`${adminBase}/adjustments${estado ? `?estado=${estado}` : ""}`),
  createAdjustment: (body: {
    colaboradorId: string;
    tipoAjuste: string;
    valorNuevo: string;
    motivo: string;
    montoReferencia?: number;
    requiereFinanzas?: boolean;
  }) => fetchApi<SolicitudAjuste>(`${adminBase}/adjustments`, { method: "POST", body: JSON.stringify(body) }),
  submitAdjustment: (id: string) =>
    fetchApi<SolicitudAjuste>(`${adminBase}/adjustments/${id}/submit`, { method: "POST" }),
  approveAdjustment: (id: string, comentario?: string) =>
    fetchApi<SolicitudAjuste>(`${adminBase}/adjustments/${id}/approve`, {
      method: "POST",
      body: JSON.stringify({ comentario }),
    }),
  rejectAdjustment: (id: string, comentario?: string) =>
    fetchApi<SolicitudAjuste>(`${adminBase}/adjustments/${id}/reject`, {
      method: "POST",
      body: JSON.stringify({ comentario }),
    }),
  applyAdjustment: (id: string) =>
    fetchApi<SolicitudAjuste>(`${adminBase}/adjustments/${id}/apply`, { method: "POST" }),
};

export const benefitsAdminService = {
  listBenefits: () => fetchApi<BeneficioDisponible[]>(v1("/benefits/admin/benefits")),
  listRequests: (estado?: string) =>
    fetchApi<SolicitudBeneficioAdmin[]>(v1(`/benefits/admin/requests${estado ? `?estado=${estado}` : ""}`)),
  approve: (id: string, comentario?: string) =>
    fetchApi<SolicitudBeneficioAdmin>(v1(`/benefits/admin/requests/${id}/approve`), {
      method: "POST",
      body: JSON.stringify({ comentario }),
    }),
  reject: (id: string, comentario?: string) =>
    fetchApi<SolicitudBeneficioAdmin>(v1(`/benefits/admin/requests/${id}/reject`), {
      method: "POST",
      body: JSON.stringify({ comentario }),
    }),
};

export interface SolicitudBeneficioAdmin {
  id: string;
  colaboradorNombre: string;
  beneficioNombre: string;
  monto?: number;
  estado: string;
  fecha: string;
}
