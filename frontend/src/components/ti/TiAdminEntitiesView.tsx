"use client";

import { useEffect, useMemo, useState } from "react";
import Link from "next/link";
import { PageContainer, PageHeader } from "@/components/ui/page-header";
import { TiExportButtons } from "@/components/ti/TiExportButtons";
import { tiService, type EntityRegistry, type EntityPage } from "@/lib/services/tiService";

export function TiAdminEntitiesView() {
  const [entities, setEntities] = useState<EntityRegistry[]>([]);
  const [search, setSearch] = useState("");
  const [selected, setSelected] = useState<string | null>(null);
  const [page, setPage] = useState<EntityPage | null>(null);

  useEffect(() => {
    tiService.listEntities().then(setEntities).catch(console.error);
  }, []);

  const filtered = useMemo(() => {
    const q = search.trim().toLowerCase();
    if (!q) return entities;
    return entities.filter(
      (e) =>
        e.name.toLowerCase().includes(q) ||
        e.table.toLowerCase().includes(q) ||
        e.schema.toLowerCase().includes(q),
    );
  }, [entities, search]);

  useEffect(() => {
    if (!selected) return;
    tiService.listEntityRecords(selected, 1, 20).then(setPage).catch(console.error);
  }, [selected]);

  return (
    <PageContainer>
      <PageHeader
        title="Explorador de entidades"
        subtitle="CRUD genérico sobre todas las tablas del modelo (solo administradores)."
      />

      <div className="mb-4">
        <TiExportButtons kinds={["entities"]} variant="secondary" />
      </div>

      <input
        className="mb-4 w-full max-w-md rounded-lg border border-slate-200 px-3 py-2 text-sm"
        placeholder="Buscar entidad…"
        value={search}
        onChange={(e) => setSearch(e.target.value)}
      />

      <div className="grid gap-4 lg:grid-cols-[320px_1fr]">
        <div className="max-h-[70vh] overflow-y-auto rounded-xl border border-slate-200 bg-white">
          {filtered.map((e) => (
            <button
              key={e.name}
              type="button"
              onClick={() => setSelected(e.name)}
              className={`block w-full border-b border-slate-100 px-4 py-3 text-left text-sm hover:bg-slate-50 ${
                selected === e.name ? "bg-slate-100" : ""
              }`}
            >
              <p className="font-medium">{e.name}</p>
              <p className="text-xs text-slate-500">{e.schema}.{e.table}</p>
            </button>
          ))}
        </div>

        <div className="rounded-xl border border-slate-200 bg-white p-4">
          {!selected && <p className="text-sm text-slate-500">Selecciona una entidad.</p>}
          {selected && page && (
            <>
              <div className="mb-3 flex items-center justify-between">
                <h2 className="font-semibold">{selected}</h2>
                <span className="text-sm text-slate-500">{page.total} registros</span>
              </div>
              <div className="overflow-x-auto">
                <table className="min-w-full text-left text-xs">
                  <thead>
                    <tr className="border-b text-slate-500">
                      {Object.keys(page.items[0] ?? { id: "" }).slice(0, 6).map((k) => (
                        <th key={k} className="px-2 py-2">{k}</th>
                      ))}
                    </tr>
                  </thead>
                  <tbody>
                    {page.items.map((row) => (
                      <tr key={String(row.id)} className="border-b border-slate-100">
                        {Object.keys(page.items[0] ?? {}).slice(0, 6).map((k) => (
                          <td key={k} className="max-w-[160px] truncate px-2 py-2 font-mono">
                            {String(row[k] ?? "")}
                          </td>
                        ))}
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              <p className="mt-3 text-xs text-slate-500">
                API: <Link className="text-urrea-primary underline" href={`#`}>{`/api/v1/admin/entities/${selected}`}</Link>
              </p>
            </>
          )}
        </div>
      </div>
    </PageContainer>
  );
}
