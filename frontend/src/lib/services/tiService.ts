import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";

export interface ColumnSchema {
  name: string;
  clrType: string;
  isKey: boolean;
  isNullable: boolean;
  maxLength?: number;
}

export interface TableSchema {
  schema: string;
  table: string;
  entity: string;
  columns: ColumnSchema[];
  foreignKeys: { column: string; referencedTable: string; referencedColumn: string }[];
}

export interface DatabaseSchema {
  database: string;
  migrationId: string;
  tableCount: number;
  tables: TableSchema[];
}

export interface ApiEndpoint {
  method: string;
  route: string;
  controller: string;
  action: string;
  authPolicies: string[];
}

export interface ApiCatalog {
  endpointCount: number;
  endpoints: ApiEndpoint[];
}

export interface EnvironmentInfo {
  environmentName: string;
  applicationVersion: string;
  migrationId: string;
  databaseName: string;
  serverTimeUtc: string;
  roles: string[];
}

export interface MetadatoSnapshot {
  id: string;
  tipo: string;
  origen: string;
  etiqueta: string;
  versionTag?: string;
  notas?: string;
  migracionId?: string;
  createdAt: string;
}

export interface MetadatoSnapshotDetail extends MetadatoSnapshot {
  contenidoJson: string;
}

export interface EntityRegistry {
  name: string;
  schema: string;
  table: string;
  columnCount: number;
  adminRoute: string;
}

export interface EntityPage {
  entity: string;
  total: number;
  page: number;
  pageSize: number;
  items: Record<string, unknown>[];
}

export const tiService = {
  getSchema: () => fetchApi<DatabaseSchema>(v1("/ti/metadata/schema")),
  getApis: () => fetchApi<ApiCatalog>(v1("/ti/metadata/apis")),
  getEnvironment: () => fetchApi<EnvironmentInfo>(v1("/ti/metadata/environment")),
  listSnapshots: (tipo?: string) =>
    fetchApi<MetadatoSnapshot[]>(`${v1("/ti/metadata/snapshots")}${tipo ? `?tipo=${tipo}` : ""}`),
  getSnapshot: (id: string) => fetchApi<MetadatoSnapshotDetail>(v1(`/ti/metadata/snapshots/${id}`)),
  saveSnapshot: (body: {
    tipo: string;
    origen: string;
    etiqueta: string;
    versionTag?: string;
    notas?: string;
    includeLiveSchema: boolean;
    includeLiveApis: boolean;
    includeEnvironment: boolean;
  }) =>
    fetchApi<MetadatoSnapshotDetail>(v1("/ti/metadata/snapshots"), {
      method: "POST",
      body: JSON.stringify(body),
    }),
  listEntities: () => fetchApi<EntityRegistry[]>(v1("/admin/entities/registry")),
  listEntityRecords: (entity: string, page = 1, pageSize = 25, includeInactive = false) =>
    fetchApi<EntityPage>(
      `${v1(`/admin/entities/${entity}`)}?page=${page}&pageSize=${pageSize}&includeInactive=${includeInactive}`,
    ),
  deleteEntityRecord: (entity: string, id: string) =>
    fetchApi(v1(`/admin/entities/${entity}/${id}`), { method: "DELETE" }),
};
