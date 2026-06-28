"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { Button } from "@/components/ui/button";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { ApiError } from "@/lib/api";
import { dhOrgAdminService, type ManagerAssignmentDto } from "@/lib/services/dhOrgAdminService";

export function OrgJefesView() {
  const [search, setSearch] = useState("");
  const [rows, setRows] = useState<ManagerAssignmentDto[]>([]);
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [jefeId, setJefeId] = useState("");
  const [motivo, setMotivo] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setRows(await dhOrgAdminService.listManagers(search.trim() || undefined));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al cargar.");
    } finally {
      setLoading(false);
    }
  }, [search]);

  useEffect(() => {
    const t = setTimeout(() => {
      load().catch(console.error);
    }, 300);
    return () => clearTimeout(t);
  }, [load]);

  const selected = useMemo(
    () => rows.find((r) => r.colaboradorId === selectedId) ?? null,
    [rows, selectedId],
  );

  const managerOptions = useMemo(
    () => rows.filter((r) => r.colaboradorId !== selectedId),
    [rows, selectedId],
  );

  useEffect(() => {
    if (selected) {
      setJefeId(selected.jefeDirectoId ?? "");
      setMotivo("");
    }
  }, [selected]);

  async function save() {
    if (!selected) return;
    setSaving(true);
    setError(null);
    setSuccess(null);
    try {
      await dhOrgAdminService.assignManager(selected.colaboradorId, {
        jefeDirectoId: jefeId || undefined,
        motivo: motivo.trim() || undefined,
      });
      setSuccess("Jefe directo actualizado.");
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo asignar el jefe.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader
        title="Asignación de jefes"
        subtitle="Administra la relación jefe directo. Los datos de integración pueden sobrescribirse en sincronización."
      />

      {error && <Alert variant="error">{error}</Alert>}
      {success && <Alert variant="success">{success}</Alert>}

      <input
        className="w-full max-w-md rounded-lg border border-slate-200 px-3 py-2 text-sm"
        placeholder="Buscar colaborador…"
        value={search}
        onChange={(e) => setSearch(e.target.value)}
      />

      <div className="grid gap-4 lg:grid-cols-[1fr_360px]">
        <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white">
          {loading && <p className="p-4 text-sm text-slate-500">Cargando…</p>}
          {!loading && rows.length === 0 && <EmptyState message="Sin colaboradores." />}
          {!loading && rows.length > 0 && (
            <table className="min-w-full text-left text-sm">
              <thead>
                <tr className="border-b bg-slate-50 text-slate-500">
                  <th className="px-4 py-3">Colaborador</th>
                  <th className="px-4 py-3">Puesto</th>
                  <th className="px-4 py-3">Departamento</th>
                  <th className="px-4 py-3">Jefe actual</th>
                  <th className="px-4 py-3">Origen</th>
                </tr>
              </thead>
              <tbody>
                {rows.map((r) => (
                  <tr
                    key={r.colaboradorId}
                    className={`cursor-pointer border-b border-slate-100 hover:bg-slate-50 ${
                      selectedId === r.colaboradorId ? "bg-slate-100" : ""
                    }`}
                    onClick={() => setSelectedId(r.colaboradorId)}
                  >
                    <td className="px-4 py-3">
                      <p className="font-medium">{r.nombreCompleto}</p>
                      <p className="text-xs text-slate-500">{r.numeroEmpleado}</p>
                    </td>
                    <td className="px-4 py-3">{r.puesto}</td>
                    <td className="px-4 py-3">{r.departamento}</td>
                    <td className="px-4 py-3">{r.jefeDirectoNombre ?? "—"}</td>
                    <td className="px-4 py-3 text-xs">{r.externalSource ?? r.syncStatus}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        <div className="rounded-xl border border-slate-200 bg-white p-4">
          {!selected && <EmptyState message="Selecciona un colaborador." />}
          {selected && (
            <div className="space-y-4">
              <div>
                <h2 className="font-semibold">{selected.nombreCompleto}</h2>
                <p className="text-sm text-slate-500">
                  {selected.puesto} · {selected.departamento}
                </p>
              </div>

              {!selected.isManualOverride && selected.externalSource && (
                <Alert variant="info">
                  Este dato proviene de integración ({selected.externalSource}). Un cambio manual puede ser
                  sobrescrito en la próxima sincronización.
                </Alert>
              )}

              <div>
                <label className="mb-1 block text-xs text-slate-500">Jefe directo</label>
                <select
                  className="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm"
                  value={jefeId}
                  onChange={(e) => setJefeId(e.target.value)}
                >
                  <option value="">Sin jefe</option>
                  {managerOptions.map((m) => (
                    <option key={m.colaboradorId} value={m.colaboradorId}>
                      {m.nombreCompleto} ({m.numeroEmpleado})
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="mb-1 block text-xs text-slate-500">Motivo del cambio</label>
                <textarea
                  className="w-full rounded-lg border border-slate-200 px-3 py-2 text-sm"
                  rows={3}
                  value={motivo}
                  onChange={(e) => setMotivo(e.target.value)}
                  placeholder="Opcional"
                />
              </div>

              <Button type="button" onClick={save} disabled={saving}>
                {saving ? "Guardando…" : "Guardar jefe directo"}
              </Button>
            </div>
          )}
        </div>
      </div>
    </PageContainer>
  );
}
