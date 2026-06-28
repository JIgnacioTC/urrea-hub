"use client";

import { useEffect, useMemo, useState } from "react";
import { PageContainer, PageHeader, Alert } from "@/components/ui/page-header";
import { TiExportButtons } from "@/components/ti/TiExportButtons";
import { ApiError } from "@/lib/api";
import { tiService, type TableSchema } from "@/lib/services/tiService";

export function TiSchemaView() {
  const [tables, setTables] = useState<TableSchema[]>([]);
  const [search, setSearch] = useState("");
  const [selected, setSelected] = useState<TableSchema | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    tiService.getSchema()
      .then((s) => {
        setTables(s.tables);
        setSelected(s.tables[0] ?? null);
      })
      .catch((err) => {
        if (err instanceof ApiError && err.status === 404) {
          setError("Reinicia la API: ./scripts/stop.sh && ./scripts/start.sh");
        } else {
          setError(err instanceof Error ? err.message : "Error al cargar schema.");
        }
      });
  }, []);

  const filtered = useMemo(() => {
    const q = search.trim().toLowerCase();
    if (!q) return tables;
    return tables.filter(
      (t) =>
        t.table.toLowerCase().includes(q) ||
        t.schema.toLowerCase().includes(q) ||
        t.entity.toLowerCase().includes(q),
    );
  }, [tables, search]);

  return (
    <PageContainer>
      <PageHeader title="Schemas de base de datos" subtitle="Introspección en vivo del modelo EF Core." />

      <div className="mb-4">
        <TiExportButtons kinds={["schema"]} variant="secondary" />
      </div>

      {error && <Alert variant="error">{error}</Alert>}

      <div className="mb-4">
        <input
          className="w-full max-w-md rounded-lg border border-slate-200 px-3 py-2 text-sm"
          placeholder="Buscar tabla, schema o entidad…"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </div>

      <div className="grid gap-4 lg:grid-cols-[320px_1fr]">
        <div className="max-h-[70vh] overflow-y-auto rounded-xl border border-slate-200 bg-white">
          {filtered.map((t) => (
            <button
              key={`${t.schema}.${t.table}`}
              type="button"
              onClick={() => setSelected(t)}
              className={`block w-full border-b border-slate-100 px-4 py-3 text-left text-sm hover:bg-slate-50 ${
                selected?.table === t.table && selected.schema === t.schema ? "bg-slate-100" : ""
              }`}
            >
              <p className="font-medium">{t.schema}.{t.table}</p>
              <p className="text-xs text-slate-500">{t.entity} · {t.columns.length} columnas</p>
            </button>
          ))}
        </div>

        {selected && (
          <div className="rounded-xl border border-slate-200 bg-white p-4">
            <h2 className="mb-3 text-lg font-semibold">{selected.schema}.{selected.table}</h2>
            <p className="mb-4 text-sm text-slate-500">Entidad: {selected.entity}</p>
            <div className="overflow-x-auto">
              <table className="min-w-full text-left text-sm">
                <thead>
                  <tr className="border-b text-slate-500">
                    <th className="py-2 pr-4">Columna</th>
                    <th className="py-2 pr-4">Tipo</th>
                    <th className="py-2 pr-4">PK</th>
                    <th className="py-2">Null</th>
                  </tr>
                </thead>
                <tbody>
                  {selected.columns.map((c) => (
                    <tr key={c.name} className="border-b border-slate-100">
                      <td className="py-2 pr-4 font-mono text-xs">{c.name}</td>
                      <td className="py-2 pr-4">{c.clrType}{c.maxLength ? `(${c.maxLength})` : ""}</td>
                      <td className="py-2 pr-4">{c.isKey ? "✓" : ""}</td>
                      <td className="py-2">{c.isNullable ? "Sí" : "No"}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
            {selected.foreignKeys.length > 0 && (
              <div className="mt-4">
                <h3 className="mb-2 font-medium">Foreign keys</h3>
                <ul className="space-y-1 text-sm text-slate-600">
                  {selected.foreignKeys.map((fk) => (
                    <li key={`${fk.column}-${fk.referencedTable}`}>
                      {fk.column} → {fk.referencedTable}.{fk.referencedColumn}
                    </li>
                  ))}
                </ul>
              </div>
            )}
          </div>
        )}
      </div>
    </PageContainer>
  );
}
