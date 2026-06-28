import type { ApiCatalog, DatabaseSchema, EntityRegistry, EnvironmentInfo } from "@/lib/services/tiService";

function stamp() {
  return new Date().toISOString().slice(0, 10);
}

function header(title: string, env?: EnvironmentInfo | null) {
  const lines = [
    "=".repeat(72),
    title,
    `Generado: ${new Date().toLocaleString("es-MX")}`,
  ];
  if (env) {
    lines.push(`Ambiente: ${env.environmentName}`);
    lines.push(`Base de datos: ${env.databaseName}`);
    lines.push(`Migración: ${env.migrationId}`);
  }
  lines.push("=".repeat(72), "");
  return lines.join("\n");
}

export function formatSchemaTxt(schema: DatabaseSchema, env?: EnvironmentInfo | null): string {
  const lines: string[] = [header("URREA Hub — Schema de base de datos", env)];
  lines.push(`Total tablas: ${schema.tableCount}`);
  lines.push(`Migración modelo: ${schema.migrationId}`);
  lines.push("");

  for (const table of schema.tables) {
    lines.push("-".repeat(72));
    lines.push(`TABLA: ${table.schema}.${table.table}`);
    lines.push(`Entidad: ${table.entity}`);
    lines.push("");
    lines.push("Columnas:");
    for (const col of table.columns) {
      const flags = [
        col.isKey ? "PK" : null,
        col.isNullable ? "NULL" : "NOT NULL",
        col.maxLength ? `max ${col.maxLength}` : null,
      ].filter(Boolean).join(", ");
      lines.push(`  - ${col.name} (${col.clrType}) [${flags}]`);
    }
    if (table.foreignKeys.length > 0) {
      lines.push("");
      lines.push("Foreign keys:");
      for (const fk of table.foreignKeys) {
        lines.push(`  - ${fk.column} -> ${fk.referencedTable}.${fk.referencedColumn}`);
      }
    }
    lines.push("");
  }

  return lines.join("\n");
}

export function formatApisTxt(catalog: ApiCatalog, env?: EnvironmentInfo | null): string {
  const lines: string[] = [header("URREA Hub — Catálogo de APIs", env)];
  lines.push(`Total endpoints: ${catalog.endpointCount}`);
  lines.push("");

  const grouped = new Map<string, typeof catalog.endpoints>();
  for (const ep of catalog.endpoints) {
    const list = grouped.get(ep.controller) ?? [];
    list.push(ep);
    grouped.set(ep.controller, list);
  }

  for (const [controller, eps] of [...grouped.entries()].sort(([a], [b]) => a.localeCompare(b))) {
    lines.push("-".repeat(72));
    lines.push(`CONTROLADOR: ${controller}`);
    lines.push("");
    for (const ep of eps) {
      const auth = ep.authPolicies.length > 0 ? ep.authPolicies.join(", ") : "Authenticated";
      lines.push(`  ${ep.method.padEnd(7)} ${ep.route}`);
      lines.push(`           Action: ${ep.action} | Auth: ${auth}`);
    }
    lines.push("");
  }

  return lines.join("\n");
}

export function formatEntitiesTxt(entities: EntityRegistry[], env?: EnvironmentInfo | null): string {
  const lines: string[] = [header("URREA Hub — Registro de entidades administrables", env)];
  lines.push(`Total entidades: ${entities.length}`);
  lines.push("");
  lines.push(`${"Entidad".padEnd(36)} ${"Schema".padEnd(16)} ${"Tabla".padEnd(28)} Cols  Ruta admin`);
  lines.push("-".repeat(120));

  for (const e of entities) {
    lines.push(
      `${e.name.padEnd(36)} ${e.schema.padEnd(16)} ${e.table.padEnd(28)} ${String(e.columnCount).padStart(4)}  ${e.adminRoute}`,
    );
  }

  lines.push("");
  return lines.join("\n");
}

export function downloadTxt(filename: string, content: string) {
  const blob = new Blob([content], { type: "text/plain;charset=utf-8" });
  const url = URL.createObjectURL(blob);
  const anchor = document.createElement("a");
  anchor.href = url;
  anchor.download = filename;
  anchor.click();
  URL.revokeObjectURL(url);
}

export function schemaFilename() {
  return `urrea-schema-${stamp()}.txt`;
}

export function apisFilename() {
  return `urrea-apis-${stamp()}.txt`;
}

export function entitiesFilename() {
  return `urrea-entidades-${stamp()}.txt`;
}
