"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { ApiError } from "@/lib/api";
import { dhOrgAdminService, type OrgSedeDto } from "@/lib/services/dhOrgAdminService";
import { DhModal } from "@/components/dh/shared/ui";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";

const emptyForm = { codigo: "", nombre: "", ciudad: "", pais: "", isActive: true };

export function OrgSedesView() {
  const [sedes, setSedes] = useState<OrgSedeDto[]>([]);
  const [form, setForm] = useState(emptyForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [viewingItem, setViewingItem] = useState<OrgSedeDto | null>(null);
  const [isOpen, setIsOpen] = useState(false);
  const [confirmDeleteId, setConfirmDeleteId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setError(null);
    try {
      const catalog = await dhOrgAdminService.getCatalog();
      setSedes(catalog.sedes);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al cargar sedes.");
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

  function startEdit(item: OrgSedeDto) {
    setEditingId(item.id);
    setViewingItem(null);
    setForm({
      codigo: item.codigo,
      nombre: item.nombre,
      ciudad: item.ciudad ?? "",
      pais: item.pais ?? "",
      isActive: item.isActive,
    });
    setIsOpen(true);
  }

  function startView(item: OrgSedeDto) {
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
      await dhOrgAdminService.deleteSede(confirmDeleteId);
      setConfirmDeleteId(null);
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo eliminar.");
      setConfirmDeleteId(null);
    } finally {
      setSaving(false);
    }
  }

  const modalTitle = viewingItem
    ? "Detalle de Sede"
    : editingId
    ? "Editar Sede"
    : "Nueva Sede";

  return (
    <PageContainer className="max-w-6xl">
      <div className="flex items-center justify-between">
        <PageHeader title="Sedes" subtitle="Ubicaciones físicas y centros de trabajo." />
        <Button onClick={startCreate}>Nueva sede</Button>
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
                <span className="block font-medium text-slate-500">Ciudad</span>
                <span className="text-slate-900">{viewingItem.ciudad ?? "—"}</span>
              </div>
              <div>
                <span className="block font-medium text-slate-500">País</span>
                <span className="text-slate-900">{viewingItem.pais ?? "—"}</span>
              </div>
              <div>
                <span className="block font-medium text-slate-500">Estado</span>
                <span className={`inline-flex rounded-full px-2 text-xs font-semibold leading-5 ${viewingItem.isActive ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"}`}>
                  {viewingItem.isActive ? "Activo" : "Inactivo"}
                </span>
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
                <label className="text-xs font-semibold text-slate-500">Ciudad</label>
                <input
                  className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                  placeholder="Ciudad"
                  value={form.ciudad}
                  onChange={(e) => setForm({ ...form, ciudad: e.target.value })}
                />
              </div>
              <div className="flex flex-col gap-1">
                <label className="text-xs font-semibold text-slate-500">País</label>
                <input
                  className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                  placeholder="País"
                  value={form.pais}
                  onChange={(e) => setForm({ ...form, pais: e.target.value })}
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
                <th className="px-4 py-3 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {sedes.map((s) => (
                <tr key={s.id} className="border-b border-slate-100 hover:bg-slate-50">
                  <td className="px-4 py-3 font-mono text-xs">{s.codigo}</td>
                  <td className="px-4 py-3 font-medium text-slate-900">{s.nombre}</td>
                  <td className="px-4 py-3 text-slate-600">{s.ciudad ?? "—"}</td>
                  <td className="px-4 py-3 text-slate-600">{s.pais ?? "—"}</td>
                  <td className="px-4 py-3">
                    <span className={`inline-flex rounded-full px-2 text-xs font-semibold leading-5 ${s.isActive ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"}`}>
                      {s.isActive ? "Activo" : "Inactivo"}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-right space-x-3">
                    <button type="button" className="text-xs text-slate-500 hover:text-slate-900 hover:underline" onClick={() => startView(s)}>
                      Ver
                    </button>
                    <button type="button" className="text-xs text-urrea-primary hover:underline" onClick={() => startEdit(s)}>
                      Editar
                    </button>
                    <button type="button" className="text-xs text-red-600 hover:underline" onClick={() => setConfirmDeleteId(s.id)}>
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
        message="¿Estás seguro de que deseas eliminar esta sede? Si está en uso, se desactivará lógicamente."
        confirmText="Eliminar"
        variant="danger"
        onConfirm={onDeleteConfirm}
        onCancel={() => setConfirmDeleteId(null)}
        loading={saving}
      />
    </PageContainer>
  );
}
