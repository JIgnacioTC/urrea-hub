import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";

const base = v1("/ti/admin/security");

export interface RolDto {
  id: string;
  codigo: string;
  nombre: string;
  descripcion?: string;
  isActive: boolean;
  permisoCount: number;
  colaboradorCount: number;
}

export interface PermisoDto {
  id: string;
  codigo: string;
  modulo: string;
  nombre: string;
  descripcion?: string;
  isActive: boolean;
}

export interface RolePermissionRowDto {
  permisoId: string;
  codigo: string;
  modulo: string;
  nombre: string;
  byRoleCodigo: Record<string, boolean>;
}

export interface RolePermissionMatrixDto {
  roles: RolDto[];
  permisos: PermisoDto[];
  rows: RolePermissionRowDto[];
}

export interface ColaboradorAccessSummaryDto {
  id: string;
  numeroEmpleado: string;
  nombreCompleto: string;
  puesto: string;
  departamento: string;
  roles: string[];
}

export interface ColaboradorAccessDetailDto {
  id: string;
  numeroEmpleado: string;
  nombreCompleto: string;
  puesto: string;
  departamento: string;
  area?: string;
  roles: RolDto[];
  permisosEfectivos: string[];
}

export const securityAdminService = {
  listRoles: () => fetchApi<RolDto[]>(`${base}/roles`),
  createRole: (dto: { codigo: string; nombre: string; descripcion?: string; isActive?: boolean }) =>
    fetchApi<RolDto>(`${base}/roles`, { method: "POST", body: JSON.stringify(dto) }),
  updateRole: (id: string, dto: { codigo: string; nombre: string; descripcion?: string; isActive?: boolean }) =>
    fetchApi<RolDto>(`${base}/roles/${id}`, { method: "PUT", body: JSON.stringify(dto) }),
  listPermissions: (modulo?: string) => {
    const q = modulo ? `?modulo=${encodeURIComponent(modulo)}` : "";
    return fetchApi<PermisoDto[]>(`${base}/permisos${q}`);
  },
  getMatrix: () => fetchApi<RolePermissionMatrixDto>(`${base}/matriz`),
  updateRolePermissions: (rolId: string, permisoIds: string[]) =>
    fetchApi<void>(`${base}/roles/${rolId}/permisos`, {
      method: "PUT",
      body: JSON.stringify({ permisoIds }),
    }),
  searchColaboradores: (q?: string) => {
    const query = q ? `?q=${encodeURIComponent(q)}` : "";
    return fetchApi<ColaboradorAccessSummaryDto[]>(`${base}/colaboradores${query}`);
  },
  getColaboradorAccess: (id: string) =>
    fetchApi<ColaboradorAccessDetailDto>(`${base}/colaboradores/${id}/acceso`),
  assignRole: (colaboradorId: string, rolId: string) =>
    fetchApi<void>(`${base}/colaboradores/${colaboradorId}/roles/${rolId}`, { method: "POST" }),
  removeRole: (colaboradorId: string, rolId: string) =>
    fetchApi<void>(`${base}/colaboradores/${colaboradorId}/roles/${rolId}`, { method: "DELETE" }),
};
