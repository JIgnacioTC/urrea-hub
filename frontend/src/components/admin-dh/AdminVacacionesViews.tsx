"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { absenceAdminService, type AdminBalance, type AdminSolicitud, type CalendarioLaboral, type IncidenciaNomina, type PoliticaVacaciones } from "@/lib/services/absenceAdminService";
import type { TipoAusencia } from "@/lib/types";
import { DhModal } from "@/components/dh/shared/ui";
import { ConfirmDialog } from "@/components/ui/confirm-dialog";
import { ApiError } from "@/lib/api";

export function AdminSolicitudesView() {
  const [rows, setRows] = useState<AdminSolicitud[]>([]);
  const [estado, setEstado] = useState("");
  const [search, setSearch] = useState("");
  const [error, setError] = useState("");

  const load = useCallback(async () => {
    const params: Record<string, string> = {};
    if (estado) params.estado = estado;
    if (search.trim()) params.q = search.trim();
    setRows(await absenceAdminService.listRequests(params));
  }, [estado, search]);

  useEffect(() => { load().catch(console.error); }, [load]);

  async function cancelar(id: string) {
    const motivo = prompt("Motivo de cancelación administrativa:");
    if (!motivo?.trim()) return;
    try {
      await absenceAdminService.cancelRequest(id, motivo);
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error");
    }
  }

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Solicitudes de ausencia" subtitle="Vista administrativa con filtros y cancelación con motivo." />
      {error && <Alert variant="error">{error}</Alert>}
      <div className="flex flex-wrap gap-3">
        <input className="rounded-lg border px-3 py-2 text-sm" placeholder="Buscar colaborador…" value={search} onChange={(e) => setSearch(e.target.value)} />
        <select className="rounded-lg border px-3 py-2 text-sm" value={estado} onChange={(e) => setEstado(e.target.value)}>
          <option value="">Todos los estados</option>
          <option value="Pendiente">Pendiente</option>
          <option value="Aprobada">Aprobada</option>
          <option value="Rechazada">Rechazada</option>
          <option value="Cancelada">Cancelada</option>
        </select>
      </div>
      {rows.length === 0 ? <EmptyState message="Sin solicitudes." /> : (
        <div className="overflow-x-auto rounded-xl border bg-white">
          <table className="min-w-full text-left text-sm">
            <thead><tr className="border-b bg-slate-50 text-slate-500">
              <th className="px-3 py-2">Colaborador</th><th className="px-3 py-2">Tipo</th><th className="px-3 py-2">Fechas</th><th className="px-3 py-2">Días</th><th className="px-3 py-2">Estado</th><th className="px-3 py-2" />
            </tr></thead>
            <tbody>
              {rows.map((r) => (
                <tr key={r.id} className="border-b">
                  <td className="px-3 py-2"><p className="font-medium">{r.colaboradorNombre}</p><p className="text-xs text-slate-500">{r.numeroEmpleado}</p></td>
                  <td className="px-3 py-2">{r.tipoAusencia}</td>
                  <td className="px-3 py-2">{new Date(r.fechaInicio).toLocaleDateString("es-MX")} – {new Date(r.fechaFin).toLocaleDateString("es-MX")}</td>
                  <td className="px-3 py-2">{r.diasSolicitados}</td>
                  <td className="px-3 py-2"><Badge estado={r.estado} /></td>
                  <td className="px-3 py-2">{r.estado !== "Cancelada" && r.estado !== "Rechazada" && (
                    <button type="button" className="text-xs text-red-600 hover:underline" onClick={() => cancelar(r.id)}>Cancelar</button>
                  )}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </PageContainer>
  );
}

export function AdminSaldosView() {
  const [rows, setRows] = useState<AdminBalance[]>([]);
  const [anio, setAnio] = useState(new Date().getFullYear());
  const [selected, setSelected] = useState<AdminBalance | null>(null);
  const [dias, setDias] = useState("");
  const [motivo, setMotivo] = useState("");
  const [msg, setMsg] = useState("");

  const load = useCallback(() => absenceAdminService.listBalances(anio).then(setRows), [anio]);
  useEffect(() => { load().catch(console.error); }, [load]);

  async function ajustar() {
    if (!selected || !motivo.trim()) return;
    await absenceAdminService.adjustBalance(selected.colaboradorId, { diasAsignados: Number(dias), motivo }, anio);
    setMsg("Saldo ajustado.");
    setSelected(null);
    await load();
  }

  async function recalcular() {
    await absenceAdminService.recalculateBalances(anio);
    setMsg("Saldos recalculados.");
    await load();
  }

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Saldos de vacaciones" subtitle="Consulta, recalcula y ajusta con motivo obligatorio." action={<Button type="button" variant="secondary" onClick={recalcular}>Recalcular todos</Button>} />
      {msg && <Alert variant="success">{msg}</Alert>}
      <select className="rounded-lg border px-3 py-2 text-sm" value={anio} onChange={(e) => setAnio(Number(e.target.value))}>
        {[anio - 1, anio, anio + 1].map((y) => <option key={y} value={y}>{y}</option>)}
      </select>
      <div className="overflow-x-auto rounded-xl border bg-white">
        <table className="min-w-full text-left text-sm">
          <thead><tr className="border-b bg-slate-50 text-slate-500">
            <th className="px-3 py-2">Colaborador</th><th className="px-3 py-2">Política</th><th className="px-3 py-2">Asignados</th><th className="px-3 py-2">Usados</th><th className="px-3 py-2">Comprometidos</th><th className="px-3 py-2">Disponible</th><th className="px-3 py-2" />
          </tr></thead>
          <tbody>
            {rows.map((r) => (
              <tr key={r.colaboradorId} className="border-b">
                <td className="px-3 py-2">{r.nombreCompleto}</td>
                <td className="px-3 py-2">{r.politicaNombre}</td>
                <td className="px-3 py-2">{r.diasAsignados}</td>
                <td className="px-3 py-2">{r.diasUsados}</td>
                <td className="px-3 py-2">{r.diasComprometidos}</td>
                <td className="px-3 py-2 font-medium">{r.diasDisponibles}</td>
                <td className="px-3 py-2"><button type="button" className="text-urrea-primary hover:underline" onClick={() => { setSelected(r); setDias(String(r.diasAsignados)); }}>Ajustar</button></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      {selected && (
        <div className="rounded-xl border bg-white p-4">
          <h3 className="font-semibold">Ajustar {selected.nombreCompleto}</h3>
          <div className="mt-3 grid gap-3 sm:grid-cols-2">
            <input className="rounded-lg border px-3 py-2 text-sm" type="number" step="0.5" value={dias} onChange={(e) => setDias(e.target.value)} />
            <input className="rounded-lg border px-3 py-2 text-sm" placeholder="Motivo obligatorio" value={motivo} onChange={(e) => setMotivo(e.target.value)} />
          </div>
          <Button type="button" className="mt-3" onClick={ajustar}>Guardar ajuste</Button>
        </div>
      )}
    </PageContainer>
  );
}

const emptyPolitica = { nombre: "", descripcion: "", diasAnuales: 12, antiguedadMinimaMeses: 0, acumulable: false, isActive: true };

export function AdminPoliticasView() {
  const [rows, setRows] = useState<PoliticaVacaciones[]>([]);
  const [form, setForm] = useState(emptyPolitica);
  const [editingId, setEditingId] = useState<string | null>(null);

  const load = () => absenceAdminService.listPolicies().then(setRows);
  useEffect(() => { load().catch(console.error); }, []);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    await absenceAdminService.upsertPolicy(form, editingId ?? undefined);
    setForm(emptyPolitica);
    setEditingId(null);
    await load();
  }

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Políticas de vacaciones" subtitle="Configura días anuales, antigüedad y acumulación." />
      <form onSubmit={onSubmit} className="rounded-xl border bg-white p-4">
        <div className="grid gap-3 sm:grid-cols-3">
          <input className="rounded-lg border px-3 py-2 text-sm" placeholder="Nombre" value={form.nombre} onChange={(e) => setForm({ ...form, nombre: e.target.value })} required />
          <input className="rounded-lg border px-3 py-2 text-sm" type="number" placeholder="Días anuales" value={form.diasAnuales} onChange={(e) => setForm({ ...form, diasAnuales: Number(e.target.value) })} />
          <input className="rounded-lg border px-3 py-2 text-sm" type="number" placeholder="Antigüedad mínima (meses)" value={form.antiguedadMinimaMeses} onChange={(e) => setForm({ ...form, antiguedadMinimaMeses: Number(e.target.value) })} />
        </div>
        <label className="mt-3 flex items-center gap-2 text-sm"><input type="checkbox" checked={form.acumulable} onChange={(e) => setForm({ ...form, acumulable: e.target.checked })} /> Acumulable</label>
        <Button type="submit" className="mt-3">{editingId ? "Actualizar" : "Crear política"}</Button>
      </form>
      <div className="mt-4 overflow-x-auto rounded-xl border bg-white">
        <table className="min-w-full text-sm">
          <thead><tr className="border-b bg-slate-50"><th className="px-3 py-2">Nombre</th><th className="px-3 py-2">Días</th><th className="px-3 py-2">Antigüedad</th><th className="px-3 py-2">Colaboradores</th><th className="px-3 py-2" /></tr></thead>
          <tbody>
            {rows.map((p) => (
              <tr key={p.id} className="border-b">
                <td className="px-3 py-2">{p.nombre}</td>
                <td className="px-3 py-2">{p.diasAnuales}</td>
                <td className="px-3 py-2">{p.antiguedadMinimaMeses} meses</td>
                <td className="px-3 py-2">{p.colaboradoresAsignados}</td>
                <td className="px-3 py-2"><button type="button" className="text-urrea-primary hover:underline" onClick={() => { setEditingId(p.id); setForm({ nombre: p.nombre, descripcion: p.descripcion ?? "", diasAnuales: p.diasAnuales, antiguedadMinimaMeses: p.antiguedadMinimaMeses, acumulable: p.acumulable, isActive: p.isActive }); }}>Editar</button></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </PageContainer>
  );
}

const emptyTypeForm = {
  codigo: "",
  nombre: "",
  descuentaSaldo: false,
  requiereAprobacion: true,
  requiereAprobacionJefe: true,
  requiereAprobacionDH: false,
  requiereAprobacionNominas: false,
  color: "#3b82f6",
  categoria: "PermisoDiaCompleto",
  esParcial: false,
  permiteMultiDia: true,
  diasMaximosAnuales: "",
  diasMaximosEvento: "",
  requiereComprobante: false,
  remunerado: true,
  baseLegalLft: "",
  descripcion: "",
  icono: "folder",
  orden: 0,
  permiteSolicitudEmpleado: true,
  isActive: true,
};

export function AdminTiposAusenciaView() {
  const [rows, setRows] = useState<TipoAusencia[]>([]);
  const [form, setForm] = useState(emptyTypeForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [viewingItem, setViewingItem] = useState<TipoAusencia | null>(null);
  const [isOpen, setIsOpen] = useState(false);
  const [confirmDeleteId, setConfirmDeleteId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setError(null);
    try {
      setRows(await absenceAdminService.listTypes());
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al cargar tipos de ausencia.");
    }
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  function startCreate() {
    setEditingId(null);
    setViewingItem(null);
    setForm(emptyTypeForm);
    setIsOpen(true);
  }

  function startEdit(item: TipoAusencia) {
    setEditingId(item.id);
    setViewingItem(null);
    setForm({
      codigo: item.codigo,
      nombre: item.nombre,
      descuentaSaldo: item.descuentaSaldo,
      requiereAprobacion: item.requiereAprobacion,
      requiereAprobacionJefe: item.requiereAprobacionJefe ?? true,
      requiereAprobacionDH: item.requiereAprobacionDH ?? false,
      requiereAprobacionNominas: item.requiereAprobacionNominas ?? false,
      color: item.color ?? "#3b82f6",
      categoria: item.categoria ?? "PermisoDiaCompleto",
      esParcial: item.esParcial ?? false,
      permiteMultiDia: item.permiteMultiDia ?? true,
      diasMaximosAnuales: item.diasMaximosAnuales !== undefined && item.diasMaximosAnuales !== null ? String(item.diasMaximosAnuales) : "",
      diasMaximosEvento: item.diasMaximosEvento !== undefined && item.diasMaximosEvento !== null ? String(item.diasMaximosEvento) : "",
      requiereComprobante: item.requiereComprobante ?? false,
      remunerado: item.remunerado ?? true,
      baseLegalLft: item.baseLegalLft ?? "",
      descripcion: item.descripcion ?? "",
      icono: item.icono ?? "folder",
      orden: item.orden ?? 0,
      permiteSolicitudEmpleado: item.permiteSolicitudEmpleado ?? true,
      isActive: true,
    });
    setIsOpen(true);
  }

  function startView(item: TipoAusencia) {
    setViewingItem(item);
    setEditingId(null);
    setIsOpen(true);
  }

  function closePopup() {
    setIsOpen(false);
    setEditingId(null);
    setViewingItem(null);
    setForm(emptyTypeForm);
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    if (viewingItem) return;
    setSaving(true);
    setError(null);
    try {
      const dto = {
        codigo: form.codigo.trim().toUpperCase(),
        nombre: form.nombre.trim(),
        descuentaSaldo: form.descuentaSaldo,
        requiereAprobacion: form.requiereAprobacion,
        requiereAprobacionJefe: form.requiereAprobacionJefe,
        requiereAprobacionDH: form.requiereAprobacionDH,
        requiereAprobacionNominas: form.requiereAprobacionNominas,
        color: form.color || undefined,
        categoria: form.categoria,
        esParcial: form.esParcial,
        permiteMultiDia: form.permiteMultiDia,
        diasMaximosAnuales: form.diasMaximosAnuales ? Number(form.diasMaximosAnuales) : null,
        diasMaximosEvento: form.diasMaximosEvento ? Number(form.diasMaximosEvento) : null,
        requiereComprobante: form.requiereComprobante,
        remunerado: form.remunerado,
        baseLegalLft: form.baseLegalLft.trim() || undefined,
        descripcion: form.descripcion.trim() || undefined,
        icono: form.icono,
        orden: Number(form.orden),
        permiteSolicitudEmpleado: form.permiteSolicitudEmpleado,
        isActive: true,
      };
      await absenceAdminService.upsertType(dto, editingId ?? undefined);
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
      await absenceAdminService.deleteType(confirmDeleteId);
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
    ? "Detalle de Tipo de Ausencia"
    : editingId
    ? "Editar Tipo de Ausencia"
    : "Nuevo Tipo de Ausencia";

  return (
    <PageContainer className="max-w-6xl">
      <div className="flex items-center justify-between">
        <PageHeader title="Tipos de ausencia" subtitle="Catálogo operativo conectado al flujo de solicitudes." />
        <Button onClick={startCreate}>Nuevo tipo</Button>
      </div>

      {error && <Alert variant="error" className="mb-4">{error}</Alert>}

      <DhModal open={isOpen} title={modalTitle} onClose={closePopup}>
        {viewingItem ? (
          <div className="space-y-4 text-sm max-h-[75vh] overflow-y-auto pr-2">
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
                <span className="block font-medium text-slate-500">Categoría</span>
                <span className="text-slate-900">{viewingItem.categoria}</span>
              </div>
              <div>
                <span className="block font-medium text-slate-500">Color distintivo</span>
                <div className="flex items-center gap-2">
                  <div className="h-4 w-4 rounded-full border border-slate-200" style={{ backgroundColor: viewingItem.color ?? "#fff" }} />
                  <span className="font-mono text-xs">{viewingItem.color ?? "—"}</span>
                </div>
              </div>
            </div>

            <div className="grid grid-cols-2 gap-2 border-b border-slate-100 pb-4">
              <div className="flex items-center gap-2">
                <span className={`h-2.5 w-2.5 rounded-full ${viewingItem.permiteSolicitudEmpleado ? "bg-green-500" : "bg-red-500"}`} />
                <span>Solicitable por empleados: <strong className="text-slate-900">{viewingItem.permiteSolicitudEmpleado ? "Sí" : "No"}</strong></span>
              </div>
              <div className="flex items-center gap-2">
                <span className={`h-2.5 w-2.5 rounded-full ${viewingItem.descuentaSaldo ? "bg-green-500" : "bg-slate-300"}`} />
                <span>Descuenta saldo: <strong className="text-slate-900">{viewingItem.descuentaSaldo ? "Sí" : "No"}</strong></span>
              </div>
              <div className="flex items-center gap-2">
                <span className={`h-2.5 w-2.5 rounded-full ${viewingItem.requiereAprobacion ? "bg-green-500" : "bg-red-500"}`} />
                <span>Requiere aprobación: <strong className="text-slate-900">{viewingItem.requiereAprobacion ? "Sí" : "No"}</strong></span>
              </div>
              <div className="flex items-center gap-2">
                <span className={`h-2.5 w-2.5 rounded-full ${viewingItem.requiereComprobante ? "bg-green-500" : "bg-slate-300"}`} />
                <span>Requiere comprobante: <strong className="text-slate-900">{viewingItem.requiereComprobante ? "Sí" : "No"}</strong></span>
              </div>
              <div className="flex items-center gap-2">
                <span className={`h-2.5 w-2.5 rounded-full ${viewingItem.remunerado ? "bg-green-500" : "bg-red-500"}`} />
                <span>Remunerado (con goce): <strong className="text-slate-900">{viewingItem.remunerado ? "Sí" : "No"}</strong></span>
              </div>
              <div className="flex items-center gap-2">
                <span className={`h-2.5 w-2.5 rounded-full ${viewingItem.esParcial ? "bg-green-500" : "bg-slate-300"}`} />
                <span>Permiso parcial (por horas): <strong className="text-slate-900">{viewingItem.esParcial ? "Sí" : "No"}</strong></span>
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4 border-b border-slate-100 pb-4">
              <div>
                <span className="block font-medium text-slate-500">Días máximos por año</span>
                <span className="font-semibold text-slate-900">{viewingItem.diasMaximosAnuales ?? "Sin límite"}</span>
              </div>
              <div>
                <span className="block font-medium text-slate-500">Días máximos por solicitud</span>
                <span className="font-semibold text-slate-900">{viewingItem.diasMaximosEvento ?? "Sin límite"}</span>
              </div>
            </div>

            <div className="space-y-2 pb-2">
              <div>
                <span className="block font-medium text-slate-500">Descripción</span>
                <p className="text-slate-700 bg-slate-50 p-2.5 rounded-lg border border-slate-100 whitespace-pre-wrap">{viewingItem.descripcion ?? "Sin descripción."}</p>
              </div>
              <div>
                <span className="block font-medium text-slate-500">Base legal (LFT)</span>
                <p className="text-slate-700 bg-slate-50 p-2.5 rounded-lg border border-slate-100 whitespace-pre-wrap">{viewingItem.baseLegalLft ?? "—"}</p>
              </div>
            </div>

            <div className="flex justify-end gap-2 pt-2 border-t border-slate-100">
              <Button type="button" variant="secondary" onClick={closePopup}>
                Cerrar
              </Button>
              <Button type="button" onClick={() => startEdit(viewingItem)}>
                Editar
              </Button>
            </div>
          </div>
        ) : (
          <form onSubmit={onSubmit} className="space-y-4 max-h-[75vh] overflow-y-auto pr-2">
            <div className="grid gap-3 sm:grid-cols-2">
              <div className="flex flex-col gap-1">
                <label className="text-xs font-semibold text-slate-500">Código *</label>
                <input
                  className="rounded-lg border border-slate-200 px-3 py-2 text-sm uppercase"
                  placeholder="Ej. VAC, LMAT, PER_MED"
                  value={form.codigo}
                  onChange={(e) => setForm({ ...form, codigo: e.target.value })}
                  required
                />
              </div>
              <div className="flex flex-col gap-1">
                <label className="text-xs font-semibold text-slate-500">Nombre *</label>
                <input
                  className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                  placeholder="Nombre del tipo de ausencia"
                  value={form.nombre}
                  onChange={(e) => setForm({ ...form, nombre: e.target.value })}
                  required
                />
              </div>
              <div className="flex flex-col gap-1">
                <label className="text-xs font-semibold text-slate-500">Categoría *</label>
                <select
                  className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                  value={form.categoria}
                  onChange={(e) => setForm({ ...form, categoria: e.target.value })}
                  required
                >
                  <option value="Vacacion">Vacaciones</option>
                  <option value="PermisoDiaCompleto">Permiso de Día Completo</option>
                  <option value="PermisoParcial">Permiso Parcial (Horas)</option>
                  <option value="LicenciaLegal">Licencia Legal (Maternidad, Paternidad, Incapacidad)</option>
                </select>
              </div>
              <div className="flex gap-2">
                <div className="flex flex-1 flex-col gap-1">
                  <label className="text-xs font-semibold text-slate-500">Color distintivo</label>
                  <input
                    type="color"
                    className="h-10 w-full rounded-lg border border-slate-200 p-1"
                    value={form.color}
                    onChange={(e) => setForm({ ...form, color: e.target.value })}
                  />
                </div>
                <div className="flex flex-col gap-1">
                  <label className="text-xs font-semibold text-slate-500">Orden visual</label>
                  <input
                    type="number"
                    className="w-20 rounded-lg border border-slate-200 px-3 py-2 text-sm"
                    value={form.orden}
                    onChange={(e) => setForm({ ...form, orden: Number(e.target.value) })}
                    required
                  />
                </div>
              </div>
            </div>

            <div className="border-t border-slate-100 pt-3">
              <h4 className="mb-2 font-semibold text-sm text-slate-800">Condiciones y Reglas</h4>
              <div className="grid gap-2 sm:grid-cols-2">
                <label className="flex items-center gap-2 rounded-lg border border-slate-150 p-2.5 text-sm hover:bg-slate-50">
                  <input
                    type="checkbox"
                    checked={form.permiteSolicitudEmpleado}
                    onChange={(e) => setForm({ ...form, permiteSolicitudEmpleado: e.target.checked })}
                  />
                  <div>
                    <span className="block font-medium">Permitir a los empleados</span>
                    <span className="text-xs text-slate-400">Visible en autoservicio</span>
                  </div>
                </label>

                <label className="flex items-center gap-2 rounded-lg border border-slate-150 p-2.5 text-sm hover:bg-slate-50">
                  <input
                    type="checkbox"
                    checked={form.descuentaSaldo}
                    onChange={(e) => setForm({ ...form, descuentaSaldo: e.target.checked })}
                  />
                  <div>
                    <span className="block font-medium">Descuenta saldo</span>
                    <span className="text-xs text-slate-400">Resta días de vacaciones</span>
                  </div>
                </label>

                <label className="flex items-center gap-2 rounded-lg border border-slate-150 p-2.5 text-sm hover:bg-slate-50">
                  <input
                    type="checkbox"
                    checked={form.requiereAprobacion}
                    onChange={(e) => setForm({ ...form, requiereAprobacion: e.target.checked })}
                  />
                  <div>
                    <span className="block font-medium">Requiere aprobación</span>
                    <span className="text-xs text-slate-400">Activa el flujo de aprobación por niveles</span>
                  </div>
                </label>

                {form.requiereAprobacion && (
                  <div className="col-span-full rounded-lg border border-slate-150 bg-slate-50/60 p-3">
                    <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-slate-500">Niveles de aprobación</p>
                    <div className="grid grid-cols-1 gap-2 sm:grid-cols-3">
                      <label className="flex items-center gap-2 rounded-lg border border-slate-150 bg-white p-2.5 text-sm hover:bg-slate-50">
                        <input
                          type="checkbox"
                          checked={form.requiereAprobacionJefe}
                          onChange={(e) => setForm({ ...form, requiereAprobacionJefe: e.target.checked })}
                        />
                        <span className="font-medium">Jefe directo</span>
                      </label>
                      <label className="flex items-center gap-2 rounded-lg border border-slate-150 bg-white p-2.5 text-sm hover:bg-slate-50">
                        <input
                          type="checkbox"
                          checked={form.requiereAprobacionDH}
                          onChange={(e) => setForm({ ...form, requiereAprobacionDH: e.target.checked })}
                        />
                        <span className="font-medium">Desarrollo Humano</span>
                      </label>
                      <label className="flex items-center gap-2 rounded-lg border border-slate-150 bg-white p-2.5 text-sm hover:bg-slate-50">
                        <input
                          type="checkbox"
                          checked={form.requiereAprobacionNominas}
                          onChange={(e) => setForm({ ...form, requiereAprobacionNominas: e.target.checked })}
                        />
                        <span className="font-medium">Nóminas</span>
                      </label>
                    </div>
                    <p className="mt-2 text-xs text-slate-400">
                      La solicitud avanza en este orden. Si no marcas ningún nivel, se usa Jefe directo por defecto.
                    </p>
                  </div>
                )}

                <label className="flex items-center gap-2 rounded-lg border border-slate-150 p-2.5 text-sm hover:bg-slate-50">
                  <input
                    type="checkbox"
                    checked={form.requiereComprobante}
                    onChange={(e) => setForm({ ...form, requiereComprobante: e.target.checked })}
                  />
                  <div>
                    <span className="block font-medium">Requiere comprobante</span>
                    <span className="text-xs text-slate-400">Obliga a subir justificante</span>
                  </div>
                </label>

                <label className="flex items-center gap-2 rounded-lg border border-slate-150 p-2.5 text-sm hover:bg-slate-50">
                  <input
                    type="checkbox"
                    checked={form.remunerado}
                    onChange={(e) => setForm({ ...form, remunerado: e.target.checked })}
                  />
                  <div>
                    <span className="block font-medium">Remunerado</span>
                    <span className="text-xs text-slate-400">Con goce de sueldo</span>
                  </div>
                </label>

                <label className="flex items-center gap-2 rounded-lg border border-slate-150 p-2.5 text-sm hover:bg-slate-50">
                  <input
                    type="checkbox"
                    checked={form.esParcial}
                    onChange={(e) => setForm({ ...form, esParcial: e.target.checked })}
                  />
                  <div>
                    <span className="block font-medium">Permiso parcial</span>
                    <span className="text-xs text-slate-400">Solicitado por horas</span>
                  </div>
                </label>
              </div>
            </div>

            <div className="grid gap-3 sm:grid-cols-2 border-t border-slate-100 pt-3">
              <div className="flex flex-col gap-1">
                <label className="text-xs font-semibold text-slate-500">Días máximos por año</label>
                <input
                  type="number"
                  step="0.5"
                  className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                  placeholder="Sin límite"
                  value={form.diasMaximosAnuales}
                  onChange={(e) => setForm({ ...form, diasMaximosAnuales: e.target.value })}
                />
              </div>
              <div className="flex flex-col gap-1">
                <label className="text-xs font-semibold text-slate-500">Días máximos por solicitud</label>
                <input
                  type="number"
                  step="0.5"
                  className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                  placeholder="Sin límite"
                  value={form.diasMaximosEvento}
                  onChange={(e) => setForm({ ...form, diasMaximosEvento: e.target.value })}
                />
              </div>
            </div>

            <div className="flex flex-col gap-1 border-t border-slate-100 pt-3">
              <label className="text-xs font-semibold text-slate-500">Descripción completa</label>
              <textarea
                className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                placeholder="Políticas de uso de este tipo de ausencia (visible para el colaborador)"
                value={form.descripcion}
                onChange={(e) => setForm({ ...form, descripcion: e.target.value })}
                rows={4}
              />
            </div>

            <div className="flex flex-col gap-1">
              <label className="text-xs font-semibold text-slate-500">Base legal (LFT / Artículos)</label>
              <textarea
                className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                placeholder="Referencia a la Ley Federal del Trabajo"
                value={form.baseLegalLft}
                onChange={(e) => setForm({ ...form, baseLegalLft: e.target.value })}
                rows={2}
              />
            </div>

            <div className="mt-3 flex items-center justify-end gap-2 border-t border-slate-100 pt-3">
              <Button type="button" variant="secondary" onClick={closePopup}>
                Cancelar
              </Button>
              <Button type="submit" disabled={saving}>
                {saving ? "Guardando…" : editingId ? "Actualizar" : "Crear"}
              </Button>
            </div>
          </form>
        )}
      </DhModal>

      {rows.length === 0 ? (
        <EmptyState message="No hay tipos de ausencias configuradas." />
      ) : (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3 mt-4">
          {rows.map((t) => (
            <div key={t.id} className="relative rounded-2xl border border-slate-200 bg-white p-5 shadow-sm hover:shadow-md transition-all flex flex-col justify-between">
              <div>
                <div className="flex items-center justify-between mb-3">
                  <div className="flex items-center gap-2">
                    <span className="h-3 w-3 rounded-full border border-slate-100 shadow-sm" style={{ backgroundColor: t.color ?? "#ccc" }} />
                    <span className="font-mono text-xs font-bold text-slate-500 bg-slate-100 px-2 py-0.5 rounded">{t.codigo}</span>
                  </div>
                  <span className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${t.permiteSolicitudEmpleado ? "bg-green-50 text-green-700 border border-green-150" : "bg-red-50 text-red-700 border border-red-150"}`}>
                    {t.permiteSolicitudEmpleado ? "Solicitable" : "Restringido"}
                  </span>
                </div>
                <h3 className="font-bold text-slate-900 text-base mb-1">{t.nombre}</h3>
                <p className="text-xs text-slate-500 line-clamp-2 mb-4">{t.descripcion ?? "Sin descripción."}</p>

                <div className="grid grid-cols-2 gap-y-2 gap-x-4 text-xs border-t border-slate-100 pt-3 mb-4">
                  <div className="flex items-center gap-1.5 text-slate-600">
                    <span className={`h-1.5 w-1.5 rounded-full ${t.requiereAprobacion ? "bg-blue-500" : "bg-slate-300"}`} />
                    <span>Req. Aprobación</span>
                  </div>
                  <div className="flex items-center gap-1.5 text-slate-600">
                    <span className={`h-1.5 w-1.5 rounded-full ${t.descuentaSaldo ? "bg-amber-500" : "bg-slate-300"}`} />
                    <span>Descuenta Saldo</span>
                  </div>
                  <div className="flex items-center gap-1.5 text-slate-600">
                    <span className={`h-1.5 w-1.5 rounded-full ${t.requiereComprobante ? "bg-indigo-500" : "bg-slate-300"}`} />
                    <span>Req. Comprobante</span>
                  </div>
                  <div className="flex items-center gap-1.5 text-slate-600">
                    <span className={`h-1.5 w-1.5 rounded-full ${t.remunerado ? "bg-emerald-500" : "bg-slate-300"}`} />
                    <span>Con goce de sueldo</span>
                  </div>
                </div>
              </div>

              <div className="flex items-center justify-end gap-3 border-t border-slate-100 pt-3 text-xs">
                <button type="button" className="font-medium text-slate-500 hover:text-slate-900 hover:underline" onClick={() => startView(t)}>
                  Ver detalles
                </button>
                <button type="button" className="font-medium text-urrea-primary hover:underline" onClick={() => startEdit(t)}>
                  Editar
                </button>
                <button type="button" className="font-medium text-red-600 hover:underline" onClick={() => setConfirmDeleteId(t.id)}>
                  Eliminar
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      <ConfirmDialog
        isOpen={confirmDeleteId !== null}
        title="Eliminar Tipo de Ausencia"
        message="¿Estás seguro de que deseas desactivar este tipo de ausencia del sistema?"
        confirmText="Eliminar"
        variant="danger"
        onConfirm={onDeleteConfirm}
        onCancel={() => setConfirmDeleteId(null)}
        loading={saving}
      />
    </PageContainer>
  );
}

export function AdminCalendariosView() {
  const [rows, setRows] = useState<CalendarioLaboral[]>([]);
  const [anio, setAnio] = useState(new Date().getFullYear());
  useEffect(() => { absenceAdminService.listCalendars(anio).then(setRows).catch(console.error); }, [anio]);

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Calendarios laborales" subtitle="Calendarios por año y sede con días inhábiles." />
      <select className="rounded-lg border px-3 py-2 text-sm" value={anio} onChange={(e) => setAnio(Number(e.target.value))}>
        {[anio - 1, anio, anio + 1].map((y) => <option key={y} value={y}>{y}</option>)}
      </select>
      {rows.map((c) => (
        <div key={c.id} className="mt-4 rounded-xl border bg-white p-4">
          <h3 className="font-semibold">{c.nombre} ({c.anio}) {c.sedeNombre ? `· ${c.sedeNombre}` : "· Global"}</h3>
          <ul className="mt-2 text-sm text-slate-600">
            {c.diasInhabiles.map((d) => (
              <li key={d.id}>{new Date(d.fecha).toLocaleDateString("es-MX")} — {d.descripcion}</li>
            ))}
          </ul>
        </div>
      ))}
    </PageContainer>
  );
}

export function AdminNominaView() {
  const [rows, setRows] = useState<IncidenciaNomina[]>([]);
  useEffect(() => { absenceAdminService.listPayrollIncidents().then(setRows).catch(console.error); }, []);

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Incidencias para nómina" subtitle="Salida preparada al aprobar solicitudes de ausencia." />
      {rows.length === 0 ? <EmptyState message="Sin incidencias generadas." /> : (
        <div className="overflow-x-auto rounded-xl border bg-white">
          <table className="min-w-full text-sm">
            <thead><tr className="border-b bg-slate-50"><th className="px-3 py-2">Empleado</th><th className="px-3 py-2">Tipo</th><th className="px-3 py-2">Periodo</th><th className="px-3 py-2">Días</th><th className="px-3 py-2">Estado</th></tr></thead>
            <tbody>
              {rows.map((r) => (
                <tr key={r.id} className="border-b">
                  <td className="px-3 py-2">{r.colaboradorNombre} ({r.numeroEmpleado})</td>
                  <td className="px-3 py-2">{r.tipoIncidencia}</td>
                  <td className="px-3 py-2">{new Date(r.fechaInicio).toLocaleDateString("es-MX")} – {new Date(r.fechaFin).toLocaleDateString("es-MX")}</td>
                  <td className="px-3 py-2">{r.dias}</td>
                  <td className="px-3 py-2">{r.estado}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </PageContainer>
  );
}
