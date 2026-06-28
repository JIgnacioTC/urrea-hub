import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";

export interface CatalogItem {
  id: string;
  codigo: string;
  nombre: string;
  descripcion?: string;
}

export interface CatalogoEstado {
  id: string;
  codigo: string;
  nombre: string;
  pais: string;
}

export interface CatalogoMunicipio {
  id: string;
  codigo: string;
  nombre: string;
  estadoId: string;
  estadoNombre: string;
}

export interface AreaConSubareas {
  id: string;
  codigo: string;
  nombre: string;
  descripcion?: string;
  subareas: CatalogItem[];
}

export interface MinimumCatalog {
  key: string;
  nombre: string;
  endpoint: string;
  count: number;
  cumpleMinimo: boolean;
}

function withEstadoId(estadoId?: string) {
  return estadoId ? `?stateId=${estadoId}` : "";
}

export const catalogService = {
  getMinimumManifest: () => fetchApi<MinimumCatalog[]>(v1("/catalogs/minimum")),
  getAreas: () => fetchApi<AreaConSubareas[]>(v1("/catalogs/areas")),
  getCargos: () => fetchApi<CatalogItem[]>(v1("/catalogs/positions")),
  getEstados: () => fetchApi<CatalogoEstado[]>(v1("/catalogs/states")),
  getMunicipios: (estadoId?: string) =>
    fetchApi<CatalogoMunicipio[]>(`${v1("/catalogs/municipalities")}${withEstadoId(estadoId)}`),
  getRazonesTermino: () => fetchApi<CatalogItem[]>(v1("/catalogs/termination-reasons")),
  getEstadosCiviles: () => fetchApi<CatalogItem[]>(v1("/catalogs/marital-statuses")),
  getRegistrosPatronales: () => fetchApi<CatalogItem[]>(v1("/catalogs/employer-registrations")),
  getCentrosCosto: () => fetchApi<CatalogItem[]>(v1("/catalogs/cost-centers")),
  getJerarquias: () => fetchApi<CatalogItem[]>(v1("/catalogs/hierarchies")),
  getPoliticasVacaciones: () => fetchApi<CatalogItem[]>(v1("/catalogs/vacation-policies")),
};
