"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { ApiError } from "@/lib/api";
import { dhOrgAdminService, type OrgItemDto } from "@/lib/services/dhOrgAdminService";

type OrgKind = "areas" | "puestos" | "centros-costo";

const CONFIG: Record<
  OrgKind,
  { title: string; subtitle: string; upsert: (dto: UpsertDto, id?: string) => Promise<OrgItemDto>; pick: (c: Awaited<ReturnType<typeof dhOrgAdminService.getCatalog>>) => OrgItemDto[] }
> = {
  areas: {
    title: "Áreas",
    subtitle: "Configura las áreas organizacionales del grupo.",
    upsert: dhOrgAdminService.upsertArea,
    pick: (c) => c.areas,
  },
  puestos: {
    title: "Puestos",
    subtitle: "Catálogo de puestos y niveles jerárquicos.",
    upsert: dhOrgAdminService.upsertPuesto,
    pick: (c) => c.puestos,
  },
  "centros-costo": {
    title: "Centros de costo",
    subtitle: "Centros de costo para nómina y reportes.",
    upsert: dhOrgAdminService.upsertCentroCosto,
    pick: (c) => c.centrosCosto,
  },
};

type UpsertDto = { codigo: string; nombre: string; descripcion?: string; isActive?: boolean };

const emptyForm: UpsertDto = { codigo: "", nombre: "", descripcion: "", isActive: true };

export function OrgItemsView({ kind }: { kind: OrgKind }) {
  const cfg = CONFIG[kind];
  const [items, setItems] = useState<OrgItemDto[]>([]);
  const [form, setForm] = useState(emptyForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setError(null);
    const catalog = await dhOrgAdminService.getCatalog();
    setItems(cfg.pick(catalog));
  }, [cfg]);

  useEffect(() => {
    load().catch((err) => setError(err instanceof Error ? err.message : "Error al cargar."));
  }, [load]);

  function startEdit(item: OrgItemDto) {
    setEditingId(item.id);
    setForm({
      codigo: item.codigo,
      nombre: item.nombre,
      descripcion: item.descripcion ?? "",
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
        descripcion: form.descripcion?.trim() || undefined,
        isActive: form.isActive ?? true,
      };
      await cfg.upsert(dto, editingId ?? undefined);
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
      <PageHeader title={cfg.title} subtitle={cfg.subtitle} />
      {error && <Alert variant="error">{error}</Alert>}

      <form onSubmit={onSubmit} className="rounded-xl border border-slate-200 bg-white p-4">
        <h2 className="mb-3 text-sm font-semibold">{editingId ? "Editar registro" : "Nuevo registro"}</h2>
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
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm sm:col-span-2"
            placeholder="Descripción"
            value={form.descripcion ?? ""}
            onChange={(e) => setForm({ ...form, descripcion: e.target.value })}
          />
        </div>
        <div className="mt-3 flex items-center gap-4">
          <label className="flex items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={form.isActive ?? true}
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

      {items.length === 0 ? (
        <EmptyState message="No hay registros." />
      ) : (
        <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white">
          <table className="min-w-full text-left text-sm">
            <thead>
              <tr className="border-b bg-slate-50 text-slate-500">
                <th className="px-4 py-3">Código</th>
                <th className="px-4 py-3">Nombre</th>
                <th className="px-4 py-3">Descripción</th>
                <th className="px-4 py-3">Estado</th>
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody>
              {items.map((item) => (
                <tr key={item.id} className="border-b border-slate-100">
                  <td className="px-4 py-3 font-mono text-xs">{item.codigo}</td>
                  <td className="px-4 py-3">{item.nombre}</td>
                  <td className="px-4 py-3 text-slate-500">{item.descripcion ?? "—"}</td>
                  <td className="px-4 py-3">{item.isActive ? "Activo" : "Inactivo"}</td>
                  <td className="px-4 py-3">
                    <button type="button" className="text-sm text-urrea-primary hover:underline" onClick={() => startEdit(item)}>
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
