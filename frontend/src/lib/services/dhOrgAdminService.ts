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

export interface OrgDepartamentoDto {
  id: string;
  codigo: string;
  nombre: string;
  descripcion?: string;
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

export interface OrgCatalogDto {
  areas: OrgItemDto[];
  departamentos: OrgDepartamentoDto[];
  puestos: OrgItemDto[];
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
  upsertDepartamento: (
    dto: { codigo: string; nombre: string; descripcion?: string; areaId: string; sedeId?: string; isActive?: boolean },
    id?: string,
  ) =>
    fetchApi<OrgDepartamentoDto>(id ? `${base}/departamentos/${id}` : `${base}/departamentos`, {
      method: id ? "PUT" : "POST",
      body: JSON.stringify(dto),
    }),
  upsertPuesto: (dto: { codigo: string; nombre: string; descripcion?: string; isActive?: boolean }, id?: string) =>
    fetchApi<OrgItemDto>(id ? `${base}/puestos/${id}` : `${base}/puestos`, {
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
};
