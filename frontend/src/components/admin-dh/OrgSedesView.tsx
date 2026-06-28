"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { ApiError } from "@/lib/api";
import { dhOrgAdminService, type OrgSedeDto } from "@/lib/services/dhOrgAdminService";

const emptyForm = { codigo: "", nombre: "", ciudad: "", pais: "", isActive: true };

export function OrgSedesView() {
  const [sedes, setSedes] = useState<OrgSedeDto[]>([]);
  const [form, setForm] = useState(emptyForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setError(null);
    const catalog = await dhOrgAdminService.getCatalog();
    setSedes(catalog.sedes);
  }, []);

  useEffect(() => {
    load().catch((err) => setError(err instanceof Error ? err.message : "Error al cargar."));
  }, [load]);

  function startEdit(item: OrgSedeDto) {
    setEditingId(item.id);
    setForm({
      codigo: item.codigo,
      nombre: item.nombre,
      ciudad: item.ciudad ?? "",
      pais: item.pais ?? "",
      isActive: item.isActive,
    });
  }

  function cancelEdit() {
    setEditingId(null);
    setForm(emptyForm);
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setSaving(true);
    setError(null);
    try {
      const dto = {
        codigo: form.codigo.trim(),
        nombre: form.nombre.trim(),
        ciudad: form.ciudad.trim() || undefined,
        pais: form.pais.trim() || undefined,
        isActive: form.isActive,
      };
      await dhOrgAdminService.upsertSede(dto, editingId ?? undefined);
      cancelEdit();
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo guardar.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Sedes" subtitle="Ubicaciones físicas y centros de trabajo." />
      {error && <Alert variant="error">{error}</Alert>}

      <form onSubmit={onSubmit} className="rounded-xl border border-slate-200 bg-white p-4">
        <h2 className="mb-3 text-sm font-semibold">{editingId ? "Editar sede" : "Nueva sede"}</h2>
        <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
          <input
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
            placeholder="Código"
            value={form.codigo}
            onChange={(e) => setForm({ ...form, codigo: e.target.value })}
            required
          />
          <input
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
            placeholder="Nombre"
            value={form.nombre}
            onChange={(e) => setForm({ ...form, nombre: e.target.value })}
            required
          />
          <input
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
            placeholder="Ciudad"
            value={form.ciudad}
            onChange={(e) => setForm({ ...form, ciudad: e.target.value })}
          />
          <input
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
            placeholder="País"
            value={form.pais}
            onChange={(e) => setForm({ ...form, pais: e.target.value })}
          />
        </div>
        <div className="mt-3 flex items-center gap-4">
          <label className="flex items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={form.isActive}
              onChange={(e) => setForm({ ...form, isActive: e.target.checked })}
            />
            Activo
          </label>
          <Button type="submit" disabled={saving}>
            {saving ? "Guardando…" : editingId ? "Actualizar" : "Crear"}
          </Button>
          {editingId && (
            <Button type="button" variant="secondary" onClick={cancelEdit}>
              Cancelar
            </Button>
          )}
        </div>
      </form>

      {sedes.length === 0 ? (
        <EmptyState message="No hay sedes." />
      ) : (
        <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white">
          <table className="min-w-full text-left text-sm">
            <thead>
              <tr className="border-b bg-slate-50 text-slate-500">
                <th className="px-4 py-3">Código</th>
                <th className="px-4 py-3">Nombre</th>
                <th className="px-4 py-3">Ciudad</th>
                <th className="px-4 py-3">País</th>
                <th className="px-4 py-3">Estado</th>
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody>
              {sedes.map((s) => (
                <tr key={s.id} className="border-b border-slate-100">
                  <td className="px-4 py-3 font-mono text-xs">{s.codigo}</td>
                  <td className="px-4 py-3">{s.nombre}</td>
                  <td className="px-4 py-3">{s.ciudad ?? "—"}</td>
                  <td className="px-4 py-3">{s.pais ?? "—"}</td>
                  <td className="px-4 py-3">{s.isActive ? "Activo" : "Inactivo"}</td>
                  <td className="px-4 py-3">
                    <button type="button" className="text-sm text-urrea-primary hover:underline" onClick={() => startEdit(s)}>
                      Editar
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </PageContainer>
  );
}
