import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";

const base = v1("/recruitment");
const adminReq = v1("/requisitions/admin");

export interface RecruitmentDashboard {
  vacantesAbiertas: number;
  candidatosActivos: number;
  entrevistasProgramadas: number;
  ofertasEnviadas: number;
  ofertasAceptadas: number;
  requisicionesPorConvertir: number;
  vacantesRecientes: Vacante[];
}

export interface Vacante {
  id: string;
  codigo: string;
  titulo: string;
  estado: string;
  fechaPublicacion: string;
  requisicionFolio?: string;
  candidatosCount: number;
  etapaDominante?: string;
}

export interface Postulacion {
  id: string;
  vacanteId: string;
  vacanteTitulo: string;
  candidatoId: string;
  candidatoNombre: string;
  estado: string;
  fechaPostulacion: string;
  scorePromedio?: number;
  proximaEntrevista?: string;
}

export interface Oferta {
  id: string;
  postulacionId: string;
  candidatoNombre: string;
  vacanteTitulo: string;
  salarioOfrecido: number;
  moneda: string;
  fechaOferta: string;
  aceptada: boolean;
}

export const recruitmentAdminService = {
  getDashboard: () => fetchApi<RecruitmentDashboard>(`${base}/dashboard`),
  listVacancies: () => fetchApi<Vacante[]>(`${base}/vacancies`),
  listPipeline: (vacanteId?: string) =>
    fetchApi<Postulacion[]>(`${base}/pipeline${vacanteId ? `?vacanteId=${vacanteId}` : ""}`),
  changeStage: (id: string, estado: string) =>
    fetchApi<Postulacion>(`${base}/applications/${id}/stage`, { method: "POST", body: JSON.stringify({ estado }) }),
  createOffer: (postulacionId: string, salario: number) =>
    fetchApi<Oferta>(`${base}/applications/${postulacionId}/offers`, {
      method: "POST",
      body: JSON.stringify({ salarioOfrecido: salario, moneda: "MXN" }),
    }),
  listOffers: () => fetchApi<Oferta[]>(`${base}/offers`),
  acceptAndOnboard: (ofertaId: string) =>
    fetchApi<{ colaboradorId: string; planOnboardingId: string; numeroEmpleado: string }>(
      `${base}/offers/${ofertaId}/accept-and-onboard`, { method: "POST" }),
};

export const requisitionAdminService = {
  getDashboard: () => fetchApi<{ total: number; borradores: number; enAprobacion: number; aprobadas: number; convertidas: number; recientes: object[] }>(`${adminReq}/dashboard`),
  listAll: (estado?: string) => fetchApi(`${adminReq}/all${estado ? `?estado=${estado}` : ""}`),
  convertToVacancy: (id: string) =>
    fetchApi<{ vacanteId: string }>(`${adminReq}/${id}/convert-to-vacancy`, { method: "POST" }),
};
