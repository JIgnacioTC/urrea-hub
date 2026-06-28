"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { ApiError } from "@/lib/api";
import {
  dhOrgAdminService,
  type OrgCatalogDto,
  type OrgDepartamentoDto,
  type OrgSedeDto,
} from "@/lib/services/dhOrgAdminService";

const emptyForm = {
  codigo: "",
  nombre: "",
  descripcion: "",
  areaId: "",
  sedeId: "",
  isActive: true,
};

export function OrgDepartamentosView() {
  const [catalog, setCatalog] = useState<OrgCatalogDto | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setError(null);
    setCatalog(await dhOrgAdminService.getCatalog());
  }, []);

  useEffect(() => {
    load().catch((err) => setError(err instanceof Error ? err.message : "Error al cargar."));
  }, [load]);

  function startEdit(item: OrgDepartamentoDto) {
    setEditingId(item.id);
    setForm({
      codigo: item.codigo,
      nombre: item.nombre,
      descripcion: item.descripcion ?? "",
      areaId: item.areaId,
      sedeId: item.sedeId ?? "",
      isActive: item.isActive,
    });
  }

  function cancelEdit() {
    setEditingId(null);
    setForm(emptyForm);
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    if (!form.areaId) return;
    setSaving(true);
    setError(null);
    try {
      const dto = {
        codigo: form.codigo.trim(),
        nombre: form.nombre.trim(),
        descripcion: form.descripcion.trim() || undefined,
        areaId: form.areaId,
        sedeId: form.sedeId || undefined,
        isActive: form.isActive,
      };
      await dhOrgAdminService.upsertDepartamento(dto, editingId ?? undefined);
      cancelEdit();
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo guardar.");
    } finally {
      setSaving(false);
    }
  }

  const departamentos = catalog?.departamentos ?? [];
  const areas = catalog?.areas ?? [];
  const sedes = catalog?.sedes ?? [];

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Departamentos" subtitle="Departamentos vinculados a área y sede." />
      {error && <Alert variant="error">{error}</Alert>}

      <form onSubmit={onSubmit} className="rounded-xl border border-slate-200 bg-white p-4">
        <h2 className="mb-3 text-sm font-semibold">{editingId ? "Editar departamento" : "Nuevo departamento"}</h2>
        <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
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
          <select
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
            value={form.areaId}
            onChange={(e) => setForm({ ...form, areaId: e.target.value })}
            required
          >
            <option value="">Área…</option>
            {areas.map((a) => (
              <option key={a.id} value={a.id}>
                {a.nombre}
              </option>
            ))}
          </select>
          <select
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
            value={form.sedeId}
            onChange={(e) => setForm({ ...form, sedeId: e.target.value })}
          >
            <option value="">Sede (opcional)</option>
            {sedes.map((s: OrgSedeDto) => (
              <option key={s.id} value={s.id}>
                {s.nombre}
              </option>
            ))}
          </select>
          <input
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm sm:col-span-2"
            placeholder="Descripción"
            value={form.descripcion}
            onChange={(e) => setForm({ ...form, descripcion: e.target.value })}
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

      {departamentos.length === 0 ? (
        <EmptyState message="No hay departamentos." />
      ) : (
        <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white">
          <table className="min-w-full text-left text-sm">
            <thead>
              <tr className="border-b bg-slate-50 text-slate-500">
                <th className="px-4 py-3">Código</th>
                <th className="px-4 py-3">Nombre</th>
                <th className="px-4 py-3">Área</th>
                <th className="px-4 py-3">Sede</th>
                <th className="px-4 py-3">Estado</th>
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody>
              {departamentos.map((d) => (
                <tr key={d.id} className="border-b border-slate-100">
                  <td className="px-4 py-3 font-mono text-xs">{d.codigo}</td>
                  <td className="px-4 py-3">{d.nombre}</td>
                  <td className="px-4 py-3">{d.areaNombre}</td>
                  <td className="px-4 py-3">{d.sedeNombre ?? "—"}</td>
                  <td className="px-4 py-3">{d.isActive ? "Activo" : "Inactivo"}</td>
                  <td className="px-4 py-3">
                    <button type="button" className="text-sm text-urrea-primary hover:underline" onClick={() => startEdit(d)}>
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
