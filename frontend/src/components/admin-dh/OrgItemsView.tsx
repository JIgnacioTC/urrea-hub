"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { ApiError } from "@/lib/api";
import { dhOrgAdminService } from "@/lib/services/dhOrgAdminService";
import { DhModal } from "@/components/dh/shared/ui";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";

type OrgKind = "areas" | "subareas" | "puestos" | "centros-costo";

const CONFIG: Record<
  OrgKind,
  { title: string; subtitle: string; upsert: (dto: any, id?: string) => Promise<any>; pick: (c: Awaited<ReturnType<typeof dhOrgAdminService.getCatalog>>) => any[] }
> = {
  areas: {
    title: "Áreas",
    subtitle: "Configura las áreas organizacionales del grupo.",
    upsert: dhOrgAdminService.upsertArea,
    pick: (c) => c.areas,
  },
  subareas: {
    title: "Subáreas",
    subtitle: "Configura las subáreas organizacionales vinculadas a un área.",
    upsert: dhOrgAdminService.upsertSubarea,
    pick: (c) => c.subareas,
  },
  puestos: {
    title: "Puestos",
    subtitle: "Catálogo de puestos y niveles jerárquicos con evaluación Mercer.",
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

const DELETE_METHODS: Record<OrgKind, (id: string) => Promise<any>> = {
  areas: dhOrgAdminService.deleteArea,
  subareas: dhOrgAdminService.deleteSubarea,
  puestos: dhOrgAdminService.deletePuesto,
  "centros-costo": dhOrgAdminService.deleteCentroCosto,
};

const emptyForm = {
  codigo: "",
  nombre: "",
  descripcion: "",
  areaId: "",
  nivelJerarquico: 3,
  gradoMercer: "",
  impacto: "",
  comunicacion: "",
  innovacion: "",
  educacionRequerida: "",
  experienciaAnios: "",
  presupuestoAnual: "",
  personalCargoDirecto: "",
  personalCargoIndirecto: "",
  isActive: true,
};

export function OrgItemsView({ kind }: { kind: OrgKind }) {
  const cfg = CONFIG[kind];
  const [items, setItems] = useState<any[]>([]);
  const [areasList, setAreasList] = useState<any[]>([]);
  const [form, setForm] = useState(emptyForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [viewingItem, setViewingItem] = useState<any | null>(null);
  const [isOpen, setIsOpen] = useState(false);
  const [confirmDeleteId, setConfirmDeleteId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setError(null);
    try {
      const catalog = await dhOrgAdminService.getCatalog();
      setItems(cfg.pick(catalog));
      setAreasList(catalog.areas);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al cargar catálogo.");
    }
  }, [cfg]);

  useEffect(() => {
    load();
  }, [load]);

  function startCreate() {
    setEditingId(null);
    setViewingItem(null);
    setForm(emptyForm);
    setIsOpen(true);
  }

  function startEdit(item: any) {
    setEditingId(item.id);
    setViewingItem(null);
    setForm({
      codigo: item.codigo,
      nombre: item.nombre,
      descripcion: item.descripcion ?? "",
      areaId: item.areaId ?? "",
      nivelJerarquico: item.nivelJerarquico ?? 3,
      gradoMercer: item.gradoMercer !== undefined && item.gradoMercer !== null ? String(item.gradoMercer) : "",
      impacto: item.impacto ?? "",
      comunicacion: item.comunicacion ?? "",
      innovacion: item.innovacion ?? "",
      educacionRequerida: item.educacionRequerida ?? "",
      experienciaAnios: item.experienciaAnios !== undefined && item.experienciaAnios !== null ? String(item.experienciaAnios) : "",
      presupuestoAnual: item.presupuestoAnual !== undefined && item.presupuestoAnual !== null ? String(item.presupuestoAnual) : "",
      personalCargoDirecto: item.personalCargoDirecto !== undefined && item.personalCargoDirecto !== null ? String(item.personalCargoDirecto) : "",
      personalCargoIndirecto: item.personalCargoIndirecto !== undefined && item.personalCargoIndirecto !== null ? String(item.personalCargoIndirecto) : "",
      isActive: item.isActive,
    });
    setIsOpen(true);
  }

  function startView(item: any) {
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
        descripcion: form.descripcion?.trim() || undefined,
        areaId: kind === "subareas" ? form.areaId : undefined,
        nivelJerarquico: kind === "puestos" ? Number(form.nivelJerarquico) : undefined,
        gradoMercer: kind === "puestos" && form.gradoMercer ? Number(form.gradoMercer) : undefined,
        impacto: kind === "puestos" && form.impacto ? form.impacto : undefined,
        comunicacion: kind === "puestos" && form.comunicacion ? form.comunicacion : undefined,
        innovacion: kind === "puestos" && form.innovacion ? form.innovacion : undefined,
        educacionRequerida: kind === "puestos" && form.educacionRequerida ? form.educacionRequerida : undefined,
        experienciaAnios: kind === "puestos" && form.experienciaAnios ? Number(form.experienciaAnios) : undefined,
        presupuestoAnual: kind === "puestos" && form.presupuestoAnual ? Number(form.presupuestoAnual) : undefined,
        personalCargoDirecto: kind === "puestos" && form.personalCargoDirecto ? Number(form.personalCargoDirecto) : undefined,
        personalCargoIndirecto: kind === "puestos" && form.personalCargoIndirecto ? Number(form.personalCargoIndirecto) : undefined,
        isActive: form.isActive ?? true,
      };
      await cfg.upsert(dto, editingId ?? undefined);
      closePopup();
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo guardar el registro.");
    } finally {
      setSaving(false);
    }
  }

  async function onDeleteConfirm() {
    if (!confirmDeleteId) return;
    setError(null);
    setSaving(true);
    try {
      await DELETE_METHODS[kind](confirmDeleteId);
      setConfirmDeleteId(null);
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo eliminar el registro.");
      setConfirmDeleteId(null);
    } finally {
      setSaving(false);
    }
  }

  const modalTitle = viewingItem
    ? `Detalle de ${cfg.title.slice(0, -1)}`
    : editingId
    ? `Editar ${cfg.title.slice(0, -1)}`
    : `Nuevo ${cfg.title.slice(0, -1)}`;

  return (
    <PageContainer className="max-w-6xl">
      <div className="flex items-center justify-between">
        <PageHeader title={cfg.title} subtitle={cfg.subtitle} />
        <Button onClick={startCreate}>Nuevo registro</Button>
      </div>

      {error && <Alert variant="error" className="mb-4">{error}</Alert>}

      {/* Popup Form Modal */}
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
              {kind === "subareas" && (
                <div className="col-span-2">
                  <span className="block font-medium text-slate-500">Área correspondiente</span>
                  <span className="text-slate-900">{viewingItem.areaNombre}</span>
                </div>
              )}
              <div className="col-span-2">
                <span className="block font-medium text-slate-500">Descripción</span>
                <span className="text-slate-900">{viewingItem.descripcion ?? "—"}</span>
              </div>
              <div>
                <span className="block font-medium text-slate-500">Estado</span>
                <span className={`inline-flex rounded-full px-2 text-xs font-semibold leading-5 ${viewingItem.isActive ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"}`}>
                  {viewingItem.isActive ? "Activo" : "Inactivo"}
                </span>
              </div>
            </div>

            {kind === "puestos" && (
              <div>
                <h4 className="mb-2 font-semibold text-urrea-primary">Parámetros Mercer</h4>
                <div className="grid grid-cols-2 gap-4 bg-slate-50 p-3 rounded-lg border border-slate-100">
                  <div>
                    <span className="block text-xs font-medium text-slate-500">Nivel Jerárquico</span>
                    <span className="text-slate-900 font-semibold">{viewingItem.nivelJerarquico}</span>
                  </div>
                  <div>
                    <span className="block text-xs font-medium text-slate-500">Grado Mercer (IPE)</span>
                    <span className="text-slate-900 font-semibold">{viewingItem.gradoMercer ?? "—"}</span>
                  </div>
                  <div>
                    <span className="block text-xs font-medium text-slate-500">Impacto Organizacional</span>
                    <span className="text-slate-900">{viewingItem.impacto ?? "—"}</span>
                  </div>
                  <div>
                    <span className="block text-xs font-medium text-slate-500">Comunicación / Influencia</span>
                    <span className="text-slate-900">{viewingItem.comunicacion ?? "—"}</span>
                  </div>
                  <div>
                    <span className="block text-xs font-medium text-slate-500">Innovación / Resolución</span>
                    <span className="text-slate-900">{viewingItem.innovacion ?? "—"}</span>
                  </div>
                  <div>
                    <span className="block text-xs font-medium text-slate-500">Educación Mínima</span>
                    <span className="text-slate-900">{viewingItem.educacionRequerida ?? "—"}</span>
                  </div>
                  <div>
                    <span className="block text-xs font-medium text-slate-500">Experiencia Requerida</span>
                    <span className="text-slate-900">{viewingItem.experienciaAnios ? `${viewingItem.experienciaAnios} años` : "—"}</span>
                  </div>
                  <div>
                    <span className="block text-xs font-medium text-slate-500">Presupuesto Anual</span>
                    <span className="text-slate-900">{viewingItem.presupuestoAnual ? `$${Number(viewingItem.presupuestoAnual).toLocaleString("es-MX")}` : "—"}</span>
                  </div>
                  <div>
                    <span className="block text-xs font-medium text-slate-500">Reportes Directos</span>
                    <span className="text-slate-900">{viewingItem.personalCargoDirecto ?? 0}</span>
                  </div>
                  <div>
                    <span className="block text-xs font-medium text-slate-500">Reportes Indirectos</span>
                    <span className="text-slate-900">{viewingItem.personalCargoIndirecto ?? 0}</span>
                  </div>
                </div>
              </div>
            )}

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

              {kind === "subareas" && (
                <div className="col-span-2 flex flex-col gap-1">
                  <label className="text-xs font-semibold text-slate-500">Área *</label>
                  <select
                    className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                    value={form.areaId}
                    onChange={(e) => setForm({ ...form, areaId: e.target.value })}
                    required
                  >
                    <option value="">Seleccione área…</option>
                    {areasList.map((a) => (
                      <option key={a.id} value={a.id}>
                        {a.nombre}
                      </option>
                    ))}
                  </select>
                </div>
              )}

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

            {kind === "puestos" && (
              <div className="border-t border-slate-100 pt-3">
                <h4 className="mb-2 font-semibold text-sm text-urrea-primary">Parámetros Mercer</h4>
                <div className="grid gap-3 sm:grid-cols-2">
                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold text-slate-500">Nivel Jerárquico *</label>
                    <input
                      type="number"
                      className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                      placeholder="Ej. 3"
                      value={form.nivelJerarquico}
                      onChange={(e) => setForm({ ...form, nivelJerarquico: Number(e.target.value) })}
                      required
                    />
                  </div>
                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold text-slate-500">Grado Mercer (IPE)</label>
                    <input
                      type="number"
                      className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                      placeholder="Ej. 12"
                      value={form.gradoMercer}
                      onChange={(e) => setForm({ ...form, gradoMercer: e.target.value })}
                    />
                  </div>
                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold text-slate-500">Impacto</label>
                    <select
                      className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                      value={form.impacto}
                      onChange={(e) => setForm({ ...form, impacto: e.target.value })}
                    >
                      <option value="">Seleccione impacto…</option>
                      <option value="Soporte">Soporte</option>
                      <option value="Profesional">Profesional</option>
                      <option value="Gestión">Gestión</option>
                      <option value="Dirección">Dirección</option>
                    </select>
                  </div>
                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold text-slate-500">Comunicación</label>
                    <select
                      className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                      value={form.comunicacion}
                      onChange={(e) => setForm({ ...form, comunicacion: e.target.value })}
                    >
                      <option value="">Seleccione comunicación…</option>
                      <option value="Intercambio">Intercambio de información</option>
                      <option value="Influencia">Influencia</option>
                      <option value="Negociación">Negociación</option>
                      <option value="Estratégica">Estratégica / Corporativa</option>
                    </select>
                  </div>
                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold text-slate-500">Innovación</label>
                    <select
                      className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                      value={form.innovacion}
                      onChange={(e) => setForm({ ...form, innovacion: e.target.value })}
                    >
                      <option value="">Seleccione innovación…</option>
                      <option value="Rutinaria">Rutinaria / Operativa</option>
                      <option value="Adaptable">Adaptable / Mejora</option>
                      <option value="Compleja">Compleja / Rediseño</option>
                      <option value="Científica">Científica / Estratégica</option>
                    </select>
                  </div>
                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold text-slate-500">Educación Requerida</label>
                    <select
                      className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                      value={form.educacionRequerida}
                      onChange={(e) => setForm({ ...form, educacionRequerida: e.target.value })}
                    >
                      <option value="">Seleccione educación…</option>
                      <option value="Secundaria">Secundaria</option>
                      <option value="Carrera Técnica">Carrera Técnica</option>
                      <option value="Licenciatura">Licenciatura</option>
                      <option value="Maestría">Maestría</option>
                      <option value="Doctorado">Doctorado</option>
                    </select>
                  </div>
                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold text-slate-500">Experiencia (Años)</label>
                    <input
                      type="number"
                      className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                      placeholder="Años"
                      value={form.experienciaAnios}
                      onChange={(e) => setForm({ ...form, experienciaAnios: e.target.value })}
                    />
                  </div>
                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold text-slate-500">Presupuesto Anual (MXN)</label>
                    <input
                      type="number"
                      className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                      placeholder="Monto anual"
                      value={form.presupuestoAnual}
                      onChange={(e) => setForm({ ...form, presupuestoAnual: e.target.value })}
                    />
                  </div>
                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold text-slate-500">Reportes Directos</label>
                    <input
                      type="number"
                      className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                      placeholder="Reportes directos"
                      value={form.personalCargoDirecto}
                      onChange={(e) => setForm({ ...form, personalCargoDirecto: e.target.value })}
                    />
                  </div>
                  <div className="flex flex-col gap-1">
                    <label className="text-xs font-semibold text-slate-500">Reportes Indirectos</label>
                    <input
                      type="number"
                      className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                      placeholder="Reportes indirectos"
                      value={form.personalCargoIndirecto}
                      onChange={(e) => setForm({ ...form, personalCargoIndirecto: e.target.value })}
                    />
                  </div>
                </div>
              </div>
            )}

            <div className="mt-3 flex items-center justify-between border-t border-slate-100 pt-3">
              <label className="flex items-center gap-2 text-sm">
                <input
                  type="checkbox"
                  checked={form.isActive ?? true}
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

      {items.length === 0 ? (
        <EmptyState message="No hay registros." />
      ) : (
        <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white">
          <table className="min-w-full text-left text-sm">
            <thead>
              <tr className="border-b bg-slate-50 text-slate-500">
                <th className="px-4 py-3">Código</th>
                <th className="px-4 py-3">Nombre</th>
                {kind === "subareas" && <th className="px-4 py-3">Área</th>}
                {kind === "puestos" && <th className="px-4 py-3">Grado Mercer</th>}
                <th className="px-4 py-3">Descripción</th>
                <th className="px-4 py-3">Estado</th>
                <th className="px-4 py-3 text-right">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {items.map((item) => (
                <tr key={item.id} className="border-b border-slate-100 hover:bg-slate-50">
                  <td className="px-4 py-3 font-mono text-xs">{item.codigo}</td>
                  <td className="px-4 py-3 font-medium text-slate-900">{item.nombre}</td>
                  {kind === "subareas" && <td className="px-4 py-3 text-slate-600">{item.areaNombre}</td>}
                  {kind === "puestos" && <td className="px-4 py-3 font-semibold text-slate-700">{item.gradoMercer ?? "—"}</td>}
                  <td className="px-4 py-3 text-slate-500 truncate max-w-[200px]">{item.descripcion ?? "—"}</td>
                  <td className="px-4 py-3">
                    <span className={`inline-flex rounded-full px-2 text-xs font-semibold leading-5 ${item.isActive ? "bg-green-100 text-green-800" : "bg-red-100 text-red-800"}`}>
                      {item.isActive ? "Activo" : "Inactivo"}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-right space-x-3">
                    <button type="button" className="text-xs text-slate-500 hover:text-slate-900 hover:underline" onClick={() => startView(item)}>
                      Ver
                    </button>
                    <button type="button" className="text-xs text-urrea-primary hover:underline" onClick={() => startEdit(item)}>
                      Editar
                    </button>
                    <button type="button" className="text-xs text-red-600 hover:underline" onClick={() => setConfirmDeleteId(item.id)}>
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
        message="¿Estás seguro de que deseas eliminar este registro? Si está en uso, se desactivará lógicamente."
        confirmText="Eliminar"
        variant="danger"
        onConfirm={onDeleteConfirm}
        onCancel={() => setConfirmDeleteId(null)}
        loading={saving}
      />
    </PageContainer>
  );
}
