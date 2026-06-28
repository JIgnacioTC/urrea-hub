import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";
import type { OrganigramaNodo, PagedResult } from "@/lib/types";
import type {
  EmployeeAuditLog,
  EmployeeDetail,
  EmployeeDocument,
  EmployeeListItem,
  EmployeeModuleLink,
  EmployeeMovement,
  EmployeeOrganization,
  EmployeeUpdatePayload,
  EmployeeVacationSummary,
  HcmCatalogs,
  HcmDashboard,
  HcmDataQuality,
  HcmDataQualityReport,
} from "@/lib/types/hcm";

export interface EmployeeListParams {
  page?: number;
  pageSize?: number;
  search?: string;
  departmentId?: string;
  locationId?: string;
  positionId?: string;
  costCenterId?: string;
  contractTypeId?: string;
  status?: string;
  externalSource?: string;
  syncStatus?: string;
}

function buildQuery(params: Record<string, string | number | undefined>) {
  const q = new URLSearchParams();
  for (const [key, value] of Object.entries(params)) {
    if (value !== undefined && value !== "") q.set(key, String(value));
  }
  const s = q.toString();
  return s ? `?${s}` : "";
}

export const employeeService = {
  getDashboard: () => fetchApi<HcmDashboard>(v1("/hcm/dashboard")),

  getCatalogs: () => fetchApi<HcmCatalogs>(v1("/hcm/catalogs")),

  getEmployees: (params: EmployeeListParams = {}) =>
    fetchApi<PagedResult<EmployeeListItem>>(
      `${v1("/hcm/employees")}${buildQuery({
        page: params.page ?? 1,
        pageSize: params.pageSize ?? 50,
        search: params.search,
        departmentId: params.departmentId,
        locationId: params.locationId,
        positionId: params.positionId,
        costCenterId: params.costCenterId,
        contractTypeId: params.contractTypeId,
        status: params.status,
        externalSource: params.externalSource,
        syncStatus: params.syncStatus,
      })}`,
    ),

  getEmployee: (id: string) => fetchApi<EmployeeDetail>(v1(`/hcm/employees/${id}`)),

  updateEmployee: (id: string, payload: EmployeeUpdatePayload) =>
    fetchApi<EmployeeDetail>(v1(`/hcm/employees/${id}`), {
      method: "PUT",
      body: JSON.stringify(payload),
    }),

  getOrganization: (id: string) =>
    fetchApi<EmployeeOrganization>(v1(`/hcm/employees/${id}/organization`)),

  getMovements: (id: string) =>
    fetchApi<EmployeeMovement[]>(v1(`/hcm/employees/${id}/movements`)),

  getAuditLog: (id: string) =>
    fetchApi<EmployeeAuditLog[]>(v1(`/hcm/employees/${id}/audit-log`)),

  getVacationSummary: (id: string, year?: number) =>
    fetchApi<EmployeeVacationSummary>(
      `${v1(`/hcm/employees/${id}/vacation-summary`)}${year ? `?year=${year}` : ""}`,
    ),

  getDocuments: (id: string) =>
    fetchApi<EmployeeDocument[]>(v1(`/hcm/employees/${id}/documents`)),

  getModuleLinks: (id: string) =>
    fetchApi<EmployeeModuleLink[]>(v1(`/hcm/employees/${id}/module-links`)),

  getDataQuality: () => fetchApi<HcmDataQuality>(v1("/hcm/data-quality")),

  getDataQualityReport: () =>
    fetchApi<HcmDataQualityReport>(v1("/hcm/data-quality/report")),

  getOrganigrama: () => fetchApi<OrganigramaNodo>(v1("/hcm/org-chart")),

  syncEmployee: (id: string) =>
    fetchApi<{ synced: boolean }>(v1(`/hcm/employees/${id}/sync`), { method: "POST" }),
};

export const integrationService = {
  getSyncRuns: () =>
    fetchApi<Array<{ id: string; name: string; externalSystem: string; status: string; lastRunAt?: string }>>(
      v1("/integrations/sync-runs"),
    ),

  runNominaSync: () =>
    fetchApi<{ recordsSynced: number }>(v1("/integrations/payroll/sync"), { method: "POST" }),
};
