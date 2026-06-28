"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { absenceAdminService, type AdminBalance, type AdminSolicitud, type CalendarioLaboral, type IncidenciaNomina, type PoliticaVacaciones } from "@/lib/services/absenceAdminService";
import type { TipoAusencia } from "@/lib/types";

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

export function AdminTiposAusenciaView() {
  const [rows, setRows] = useState<TipoAusencia[]>([]);
  useEffect(() => { absenceAdminService.listTypes().then(setRows).catch(console.error); }, []);

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Tipos de ausencia" subtitle="Catálogo operativo conectado al flujo de solicitudes." />
      <div className="overflow-x-auto rounded-xl border bg-white">
        <table className="min-w-full text-xs">
          <thead><tr className="border-b bg-slate-50">
            <th className="px-2 py-2">Código</th><th className="px-2 py-2">Nombre</th><th className="px-2 py-2">Saldo</th><th className="px-2 py-2">Aprob.</th><th className="px-2 py-2">Comp.</th><th className="px-2 py-2">Parcial</th><th className="px-2 py-2">Máx/año</th>
          </tr></thead>
          <tbody>
            {rows.map((t) => (
              <tr key={t.id} className="border-b">
                <td className="px-2 py-2 font-mono">{t.codigo}</td>
                <td className="px-2 py-2">{t.nombre}</td>
                <td className="px-2 py-2">{t.descuentaSaldo ? "Sí" : "—"}</td>
                <td className="px-2 py-2">{t.requiereAprobacion ? "Sí" : "—"}</td>
                <td className="px-2 py-2">{t.requiereComprobante ? "Sí" : "—"}</td>
                <td className="px-2 py-2">{t.esParcial ? "Sí" : "—"}</td>
                <td className="px-2 py-2">{t.diasMaximosAnuales ?? t.diasMaximosEvento ?? "—"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
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
