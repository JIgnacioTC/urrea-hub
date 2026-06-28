import { fetchApi, fetchApiBlob } from "@/lib/api";
import { v1 } from "@/lib/api/v1";
import type {
  CapacitacionEquipo,
  ColaboradorPerfil,
  ColaboradorResumen,
  EquipoMiembro,
  FeedbackEquipo,
  PlanAccion,
  PagedResult,
  ReporteAusencia,
  RhDashboard,
} from "@/lib/types";

export const teamService = {
  getMembers: () => fetchApi<EquipoMiembro[]>(v1("/team/employees")),

  getMember: (id: string) => fetchApi<ColaboradorPerfil>(v1(`/team/employees/${id}`)),

  getActionPlans: () => fetchApi<PlanAccion[]>(v1("/team/action-plans")),

  createActionPlan: (body: unknown) =>
    fetchApi<PlanAccion>(v1("/team/action-plans"), {
      method: "POST",
      body: JSON.stringify(body),
    }),

  getFeedback: () => fetchApi<FeedbackEquipo[]>(v1("/team/feedback")),

  createFeedback: (body: unknown) =>
    fetchApi<FeedbackEquipo>(v1("/team/feedback"), {
      method: "POST",
      body: JSON.stringify(body),
    }),

  getTraining: () => fetchApi<CapacitacionEquipo[]>(v1("/team/capacitaciones")),
};

export const rhService = {
  getDashboard: () => fetchApi<RhDashboard>(v1("/rh/dashboard")),

  syncPayroll: () =>
    fetchApi<{ sincronizados: number }>(v1("/rh/payroll/sync"), { method: "POST" }),

  getEmployees: (page = 1, pageSize = 50, search?: string) => {
    const q = search ? `&search=${encodeURIComponent(search)}` : "";
    return fetchApi<PagedResult<ColaboradorResumen>>(
      `${v1("/rh/colaboradores")}?page=${page}&pageSize=${pageSize}${q}`,
    );
  },

  getAbsenceReport: (params: URLSearchParams) =>
    fetchApi<ReporteAusencia[]>(`${v1("/rh/reportes/ausencias")}?${params.toString()}`),

  downloadAbsenceReportCsv: (params: URLSearchParams) =>
    fetchApiBlob(`${v1("/rh/reportes/ausencias")}?${params.toString()}`),
};
