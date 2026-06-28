import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";

export interface ResponsableOnboarding {
  colaboradorId: string;
  nombre: string;
  rol: string;
}

export interface TareaOnboarding {
  id: string;
  titulo: string;
  descripcion?: string;
  orden: number;
  completada: boolean;
  fechaCompromiso?: string;
  vencida: boolean;
  responsables: ResponsableOnboarding[];
  evidenciasCount: number;
}

export interface ChecklistItem {
  id: string;
  item: string;
  completado: boolean;
  fechaCompletado?: string;
}

export interface PlanOnboarding {
  id: string;
  colaboradorId: string;
  colaboradorNombre: string;
  puesto?: string;
  departamento?: string;
  nombre: string;
  descripcion?: string;
  fechaInicio: string;
  fechaFin?: string;
  estado: string;
  avancePorcentaje: number;
  tareasPendientes: number;
  tareasVencidas: number;
  tareas: TareaOnboarding[];
  checklist: ChecklistItem[];
}

export interface OnboardingSummary {
  planActivo?: PlanOnboarding;
  tareasPendientes: number;
  tareasCompletadas: number;
  tareasVencidas: number;
}

export interface PlanResumen {
  id: string;
  colaboradorNombre: string;
  nombre: string;
  estado: string;
  avancePorcentaje: number;
  fechaInicio: string;
  tareasVencidas: number;
}

export const onboardingService = {
  getMySummary: () => fetchApi<OnboardingSummary>(v1("/onboarding/my-summary")),
  getMyPlan: () => fetchApi<PlanOnboarding | null>(v1("/onboarding/my-plan")),
  completeTask: (taskId: string, body?: { comentario?: string; evidenciaNombre?: string }) =>
    fetchApi<TareaOnboarding>(v1(`/onboarding/tasks/${taskId}/complete`), {
      method: "POST",
      body: JSON.stringify(body ?? {}),
    }),
  getTeamPlans: () => fetchApi<PlanResumen[]>(v1("/onboarding/team/plans")),
  getTeamPendingTasks: () => fetchApi<TareaOnboarding[]>(v1("/onboarding/team/tasks/pending")),
};
