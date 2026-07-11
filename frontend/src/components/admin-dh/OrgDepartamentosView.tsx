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
import { DhModal } from "@/components/dh/shared/ui";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";

const emptyForm = {
  codigo: "",
  nombre: "",
  descripcion: "",
  subareaId: "",
  sedeId: "",
  isActive: true,
};

export function OrgDepartamentosView() {
  const [catalog, setCatalog] = useState<OrgCatalogDto | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [viewingItem, setViewingItem] = useState<OrgDepartamentoDto | null>(null);
  const [isOpen, setIsOpen] = useState(false);
  const [confirmDeleteId, setConfirmDeleteId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setError(null);
    try {
      setCatalog(await dhOrgAdminService.getCatalog());
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al cargar.");
    }
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  function startCreate() {
    setEditingId(null);
    setViewingItem(null);
    setForm(emptyForm);
    setIsOpen(true);
  }

  function startEdit(item: OrgDepartamentoDto) {
    setEditingId(item.id);
    setViewingItem(null);
    setForm({
      codigo: item.codigo,
      nombre: item.nombre,
      descripcion: item.descripcion ?? "",
      subareaId: item.subareaId,
      sedeId: item.sedeId ?? "",
      isActive: item.isActive,
    });
    setIsOpen(true);
  }

  function startView(item: OrgDepartamentoDto) {
    setViewingItem(item);
    setEditingId(null);
    setIsOpen(true);
  }

  function closePopup() {
    setIsOpen(false);
    setEditingId(null);
    setViewingItem(null);
    setForm(emptyForm);
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    if (viewingItem) return;
    if (!form.subareaId) return;
    setSaving(true);
    setError(null);
    try {
      const dto = {
        codigo: form.codigo.trim(),
        nombre: form.nombre.trim(),
        descripcion: form.descripcion.trim() || undefined,
        subareaId: form.subareaId,
        sedeId: form.sedeId || undefined,
        isActive: form.isActive,
      };
      await dhOrgAdminService.upsertDepartamento(dto, editingId ?? undefined);
      closePopup();
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo guardar.");
    } finally {
      setSaving(false);
    }
  }

  async function onDeleteConfirm() {
    if (!confirmDeleteId) return;
    setError(null);
    setSaving(true);
    try {
      await dhOrgAdminService.deleteDepartamento(confirmDeleteId);
      setConfirmDeleteId(null);
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo eliminar.");
      setConfirmDeleteId(null);
    } finally {
      setSaving(false);
    }
  }

  const departamentos = catalog?.departamentos ?? [];
  const subareas = catalog?.subareas ?? [];
  const sedes = catalog?.sedes ?? [];

  const modalTitle = viewingItem
    ? "Detalle de Departamento"
    : editingId
    ? "Editar Departamento"
    : "Nuevo Departamento";

  return (
    <PageContainer className="max-w-6xl">
      <div className="flex items-center justify-between">
        <PageHeader title="Departamentos" subtitle="Departamentos vinculados a subárea y sede." />
        <Button onClick={startCreate}>Nuevo departamento</Button>
      </div>

      {error && <Alert variant="error" className="mb-4">{error}</Alert>}

      <DhModal open={isOpen} title={modalTitle} onClose={closePopup}>
        {viewingItem ? (
          <div className="space-y-4 text-sm">
            <div className="grid grid-cols-2 gap-4 border-b border-slate-100 pb-4">
              <div>
                <span className="block font-medium text-slate-500">Código</span>
                <span className="font-mono text-slate-900">{viewingItem.codigo}</span>
              </div>
              <div>
                <span className="block font-medium text-slate-500">Nombre</span>
                <span className="text-slate-900">{viewingItem.nombre}</span>
              </div>
              <div>
                <span className="block font-medium text-slate-500">Subárea</span>
                <span className="text-slate-900">{viewingItem.subareaNombre}</span>
              </div>
              <div>
                <span className="block font-medium text-slate-500">Área</span>
                <span className="text-slate-900">{viewingItem.areaNombre}</span>
              </div>
              <div>
                <span className="block font-medium text-slate-500">Sede</span>
                <span className="text-slate-900">{viewingItem.sedeNombre ?? "—"}</span>
              </div>
              <div>
                <span className="block font-medium text-slate-500">Estado</span>
                <span className={`inline-flex rounded-full px-2 text-xs font-semibold leading-5 ${viewingItem.isActive ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"}`}>
                  {viewingItem.isActive ? "Activo" : "Inactivo"}
                </span>
              </div>
              <div className="col-span-2">
                <span className="block font-medium text-slate-500">Descripción</span>
                <span className="text-slate-900">{viewingItem.descripcion ?? "—"}</span>
              </div>
            </div>

            <div className="flex justify-end gap-2 pt-2">
              <Button type="button" variant="secondary" onClick={closePopup}>
                Cerrar
              </Button>
              <Button type="button" onClick={() => startEdit(viewingItem)}>
                Editar
              </Button>
            </div>
          </div>
        ) : (
          <form onSubmit={onSubmit} className="space-y-4">
            <div className="grid gap-3 sm:grid-cols-2">
              <div className="flex flex-col gap-1">
                <label className="text-xs font-semibold text-slate-500">Código *</label>
                <input
                  className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                  placeholder="Código"
                  value={form.codigo}
                  onChange={(e) => setForm({ ...form, codigo: e.target.value })}
                  required
                />
              </div>
              <div className="flex flex-col gap-1">
                <label className="text-xs font-semibold text-slate-500">Nombre *</label>
                <input
                  className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                  placeholder="Nombre"
                  value={form.nombre}
                  onChange={(e) => setForm({ ...form, nombre: e.target.value })}
                  required
                />
              </div>
              <div className="flex flex-col gap-1">
                <label className="text-xs font-semibold text-slate-500">Subárea *</label>
                <select
                  className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                  value={form.subareaId}
                  onChange={(e) => setForm({ ...form, subareaId: e.target.value })}
                  required
                >
                  <option value="">Subárea…</option>
                  {subareas.map((s) => (
                    <option key={s.id} value={s.id}>
                      {s.nombre} ({s.areaNombre})
                    </option>
                  ))}
                </select>
              </div>
              <div className="flex flex-col gap-1">
                <label className="text-xs font-semibold text-slate-500">Sede (Opcional)</label>
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
              </div>
              <div className="col-span-2 flex flex-col gap-1">
                <label className="text-xs font-semibold text-slate-500">Descripción</label>
                <textarea
                  className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                  placeholder="Descripción (hasta 10,000 caracteres)"
                  value={form.descripcion}
                  onChange={(e) => setForm({ ...form, descripcion: e.target.value })}
                  rows={6}
                />
              </div>
            </div>

            <div className="mt-3 flex items-center justify-between border-t border-slate-100 pt-3">
              <label className="flex items-center gap-2 text-sm">
                <input
                  type="checkbox"
                  checked={form.isActive}
                  onChange={(e) => setForm({ ...form, isActive: e.target.checked })}
                />
                Activo
              </label>
              <div className="flex gap-2">
                <Button type="button" variant="secondary" onClick={closePopup}>
                  Cancelar
                </Button>
                <Button type="submit" disabled={saving}>
                  {saving ? "Guardando…" : editingId ? "Actualizar" : "Crear"}
                </Button>
              </div>
            </div>
          </form>
        )}
      </DhModal>

      {departamentos.length === 0 ? (
        <EmptyState message="No hay departamentos." />
      ) : (
        <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white">
          <table className="min-w-full text-left text-sm">
            <thead>
              <tr className="border-b bg-slate-50 text-slate-500">
                <th className="px-4 py-3">Código</th>
                <th className="px-4 py-3">Nombre</th>
                <th className="px-4 py-3">Subárea</th>
                <th className="px-4 py-3">Área</th>
                <th className="px-4 py-3">Sede</th>
                <th className="px-4 py-3">Estado</th>
                <th className="px-4 py-3 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {departamentos.map((d) => (
                <tr key={d.id} className="border-b border-slate-100 hover:bg-slate-50">
                  <td className="px-4 py-3 font-mono text-xs">{d.codigo}</td>
                  <td className="px-4 py-3 font-medium text-slate-900">{d.nombre}</td>
                  <td className="px-4 py-3 text-slate-600">{d.subareaNombre}</td>
                  <td className="px-4 py-3 text-slate-600">{d.areaNombre}</td>
                  <td className="px-4 py-3 text-slate-500">{d.sedeNombre ?? "—"}</td>
                  <td className="px-4 py-3">
                    <span className={`inline-flex rounded-full px-2 text-xs font-semibold leading-5 ${d.isActive ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"}`}>
                      {d.isActive ? "Activo" : "Inactivo"}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-right space-x-3">
                    <button type="button" className="text-xs text-slate-500 hover:text-slate-900 hover:underline" onClick={() => startView(d)}>
                      Ver
                    </button>
                    <button type="button" className="text-xs text-urrea-primary hover:underline" onClick={() => startEdit(d)}>
                      Editar
                    </button>
                    <button type="button" className="text-xs text-red-600 hover:underline" onClick={() => setConfirmDeleteId(d.id)}>
                      Eliminar
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <ConfirmDialog
        isOpen={confirmDeleteId !== null}
        title="Confirmar eliminación"
        message="¿Estás seguro de que deseas eliminar este departamento? Si está en uso, se desactivará lógicamente."
        confirmText="Eliminar"
        variant="danger"
        onConfirm={onDeleteConfirm}
        onCancel={() => setConfirmDeleteId(null)}
        loading={saving}
      />
    </PageContainer>
  );
}
