"use client";

import { useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { PageContainer, PageHeader } from "@/components/ui/page-header";
import { tiService, type MetadatoSnapshot, type MetadatoSnapshotDetail } from "@/lib/services/tiService";

export function TiSnapshotsView() {
  const [snapshots, setSnapshots] = useState<MetadatoSnapshot[]>([]);
  const [selected, setSelected] = useState<MetadatoSnapshotDetail | null>(null);
  const [msg, setMsg] = useState("");
  const [form, setForm] = useState({
    etiqueta: "",
    origen: "Dev",
    versionTag: "",
    notas: "",
  });

  const refresh = useCallback(async () => {
    setSnapshots(await tiService.listSnapshots());
  }, []);

  useEffect(() => {
    refresh().catch(console.error);
  }, [refresh]);

  async function saveSnapshot() {
    if (!form.etiqueta.trim()) return;
    const snapshot = await tiService.saveSnapshot({
      tipo: "Combined",
      origen: form.origen,
      etiqueta: form.etiqueta,
      versionTag: form.versionTag || undefined,
      notas: form.notas || undefined,
      includeLiveSchema: true,
      includeLiveApis: true,
      includeEnvironment: true,
    });
    setMsg(`Snapshot guardado: ${snapshot.etiqueta}`);
    setForm({ etiqueta: "", origen: "Dev", versionTag: "", notas: "" });
    await refresh();
  }

  return (
    <PageContainer>
      <PageHeader
        title="Snapshots dev / producción"
        subtitle="Guarda el estado de schema, APIs y ambiente para comparar entre entornos."
      />

      <div className="mb-6 rounded-xl border border-slate-200 bg-white p-4">
        <h2 className="mb-3 font-semibold">Capturar snapshot actual</h2>
        <div className="grid gap-3 sm:grid-cols-2">
          <input
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
            placeholder="Etiqueta (ej. dev-2026-06-24)"
            value={form.etiqueta}
            onChange={(e) => setForm((f) => ({ ...f, etiqueta: e.target.value }))}
          />
          <select
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
            value={form.origen}
            onChange={(e) => setForm((f) => ({ ...f, origen: e.target.value }))}
          >
            <option value="Dev">Dev</option>
            <option value="Staging">Staging</option>
            <option value="Produccion">Producción</option>
            <option value="Manual">Manual</option>
          </select>
          <input
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
            placeholder="Version tag (opcional)"
            value={form.versionTag}
            onChange={(e) => setForm((f) => ({ ...f, versionTag: e.target.value }))}
          />
          <input
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
            placeholder="Notas (opcional)"
            value={form.notas}
            onChange={(e) => setForm((f) => ({ ...f, notas: e.target.value }))}
          />
        </div>
        <Button className="mt-3" onClick={() => saveSnapshot().catch(console.error)}>Guardar snapshot</Button>
        {msg && <p className="mt-2 text-sm text-emerald-700">{msg}</p>}
      </div>

      <div className="grid gap-4 lg:grid-cols-[360px_1fr]">
        <div className="rounded-xl border border-slate-200 bg-white">
          {snapshots.map((s) => (
            <button
              key={s.id}
              type="button"
              onClick={() => tiService.getSnapshot(s.id).then(setSelected).catch(console.error)}
              className="block w-full border-b border-slate-100 px-4 py-3 text-left text-sm hover:bg-slate-50"
            >
              <p className="font-medium">{s.etiqueta}</p>
              <p className="text-xs text-slate-500">{s.origen} · {s.tipo} · {new Date(s.createdAt).toLocaleString("es-MX")}</p>
            </button>
          ))}
          {snapshots.length === 0 && <p className="p-4 text-sm text-slate-500">Sin snapshots aún.</p>}
        </div>

        {selected && (
          <div className="rounded-xl border border-slate-200 bg-white p-4">
            <h2 className="mb-2 font-semibold">{selected.etiqueta}</h2>
            <p className="mb-3 text-sm text-slate-500">Migración: {selected.migracionId ?? "—"}</p>
            <pre className="max-h-[60vh] overflow-auto rounded-lg bg-slate-900 p-4 text-xs text-slate-100">
              {JSON.stringify(JSON.parse(selected.contenidoJson), null, 2)}
            </pre>
          </div>
        )}
      </div>
    </PageContainer>
  );
}
