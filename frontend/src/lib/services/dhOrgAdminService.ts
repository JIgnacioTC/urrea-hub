import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";

const base = v1("/rh/admin/org");

export interface OrgItemDto {
  id: string;
  codigo: string;
  nombre: string;
  descripcion?: string;
  isActive: boolean;
}

export interface OrgSubareaDto {
  id: string;
  codigo: string;
  nombre: string;
  descripcion?: string;
  areaId: string;
  areaNombre: string;
  isActive: boolean;
}

export interface OrgDepartamentoDto {
  id: string;
  codigo: string;
  nombre: string;
  descripcion?: string;
  subareaId: string;
  subareaNombre: string;
  areaId: string;
  areaNombre: string;
  sedeId?: string;
  sedeNombre?: string;
  isActive: boolean;
}

export interface OrgSedeDto {
  id: string;
  codigo: string;
  nombre: string;
  ciudad?: string;
  pais?: string;
  isActive: boolean;
}

export interface OrgPuestoDto {
  id: string;
  codigo: string;
  nombre: string;
  descripcion?: string;
  nivelJerarquico: number;
  gradoMercer?: number;
  impacto?: string;
  comunicacion?: string;
  innovacion?: string;
  educacionRequerida?: string;
  experienciaAnios?: number;
  presupuestoAnual?: number;
  personalCargoDirecto?: number;
  personalCargoIndirecto?: number;
  isActive: boolean;
}

export interface OrgCatalogDto {
  areas: OrgItemDto[];
  subareas: OrgSubareaDto[];
  departamentos: OrgDepartamentoDto[];
  puestos: OrgPuestoDto[];
  sedes: OrgSedeDto[];
  centrosCosto: OrgItemDto[];
}

export interface ManagerAssignmentDto {
  colaboradorId: string;
  numeroEmpleado: string;
  nombreCompleto: string;
  puesto: string;
  departamento: string;
  jefeDirectoId?: string;
  jefeDirectoNombre?: string;
  isManualOverride: boolean;
  syncStatus: string;
  externalSource?: string;
}

export const dhOrgAdminService = {
  getCatalog: () => fetchApi<OrgCatalogDto>(`${base}/catalogo`),
  upsertArea: (dto: { codigo: string; nombre: string; descripcion?: string; isActive?: boolean }, id?: string) =>
    fetchApi<OrgItemDto>(id ? `${base}/areas/${id}` : `${base}/areas`, {
      method: id ? "PUT" : "POST",
      body: JSON.stringify(dto),
    }),
  upsertSubarea: (dto: { codigo: string; nombre: string; descripcion?: string; areaId: string; isActive?: boolean }, id?: string) =>
    fetchApi<OrgSubareaDto>(id ? `${base}/subareas/${id}` : `${base}/subareas`, {
      method: id ? "PUT" : "POST",
      body: JSON.stringify(dto),
    }),
  upsertDepartamento: (
    dto: { codigo: string; nombre: string; descripcion?: string; subareaId: string; sedeId?: string; isActive?: boolean },
    id?: string,
  ) =>
    fetchApi<OrgDepartamentoDto>(id ? `${base}/departamentos/${id}` : `${base}/departamentos`, {
      method: id ? "PUT" : "POST",
      body: JSON.stringify(dto),
    }),
  upsertPuesto: (
    dto: {
      codigo: string;
      nombre: string;
      descripcion?: string;
      nivelJerarquico: number;
      gradoMercer?: number;
      impacto?: string;
      comunicacion?: string;
      innovacion?: string;
      educacionRequerida?: string;
      experienciaAnios?: number;
      presupuestoAnual?: number;
      personalCargoDirecto?: number;
      personalCargoIndirecto?: number;
      isActive?: boolean;
    },
    id?: string,
  ) =>
    fetchApi<OrgPuestoDto>(id ? `${base}/puestos/${id}` : `${base}/puestos`, {
      method: id ? "PUT" : "POST",
      body: JSON.stringify(dto),
    }),
  upsertSede: (
    dto: { codigo: string; nombre: string; ciudad?: string; pais?: string; isActive?: boolean },
    id?: string,
  ) =>
    fetchApi<OrgSedeDto>(id ? `${base}/sedes/${id}` : `${base}/sedes`, {
      method: id ? "PUT" : "POST",
      body: JSON.stringify(dto),
    }),
  upsertCentroCosto: (dto: { codigo: string; nombre: string; descripcion?: string; isActive?: boolean }, id?: string) =>
    fetchApi<OrgItemDto>(id ? `${base}/centros-costo/${id}` : `${base}/centros-costo`, {
      method: id ? "PUT" : "POST",
      body: JSON.stringify(dto),
    }),
  listManagers: (q?: string) => {
    const query = q ? `?q=${encodeURIComponent(q)}` : "";
    return fetchApi<ManagerAssignmentDto[]>(`${base}/jefes${query}`);
  },
  assignManager: (colaboradorId: string, dto: { jefeDirectoId?: string; motivo?: string; fechaEfectiva?: string }) =>
    fetchApi<void>(`${base}/jefes/${colaboradorId}`, { method: "PUT", body: JSON.stringify(dto) }),

  // Deletion calls
  deleteArea: (id: string) => fetchApi<void>(`${base}/areas/${id}`, { method: "DELETE" }),
  deleteSubarea: (id: string) => fetchApi<void>(`${base}/subareas/${id}`, { method: "DELETE" }),
  deleteDepartamento: (id: string) => fetchApi<void>(`${base}/departamentos/${id}`, { method: "DELETE" }),
  deletePuesto: (id: string) => fetchApi<void>(`${base}/puestos/${id}`, { method: "DELETE" }),
  deleteSede: (id: string) => fetchApi<void>(`${base}/sedes/${id}`, { method: "DELETE" }),
  deleteCentroCosto: (id: string) => fetchApi<void>(`${base}/centros-costo/${id}`, { method: "DELETE" }),
};
