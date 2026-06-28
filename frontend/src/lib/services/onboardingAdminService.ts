import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";
import type { PlanOnboarding, PlanResumen, TareaOnboarding } from "@/lib/services/onboardingService";

const base = v1("/onboarding/admin");

export interface OnboardingDashboard {
  planesActivos: number;
  planesCompletados: number;
  tareasPendientes: number;
  tareasVencidas: number;
  promedioAvance: number;
  planesRecientes: PlanResumen[];
}

export interface PlantillaOnboarding {
  codigo: string;
  nombre: string;
  descripcion: string;
  tareasCount: number;
  tareasPreview: string[];
}

export const onboardingAdminService = {
  getDashboard: () => fetchApi<OnboardingDashboard>(`${base}/dashboard`),
  listPlans: (estado?: string) =>
    fetchApi<PlanResumen[]>(`${base}/plans${estado ? `?estado=${estado}` : ""}`),
  getPlan: (id: string) => fetchApi<PlanOnboarding>(`${base}/plans/${id}`),
  listTemplates: () => fetchApi<PlantillaOnboarding[]>(`${base}/templates`),
  createPlan: (body: { colaboradorId: string; plantillaCodigo: string; fechaInicio: string; nombre?: string }) =>
    fetchApi<PlanOnboarding>(`${base}/plans`, { method: "POST", body: JSON.stringify(body) }),
};

export type { PlanOnboarding, PlanResumen, TareaOnboarding };
