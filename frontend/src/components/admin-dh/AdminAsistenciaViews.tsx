"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, StatCard } from "@/components/ui/card";
import { Input, Select, Textarea } from "@/components/ui/input";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import {
  attendanceAdminService,
  type AttendanceDashboard,
  type IncidenciaAsistencia,
  type IncidenciaNominaAsistencia,
  type ReglasAsistencia,
  type Turno,
} from "@/lib/services/attendanceAdminService";
import type { RegistroAsistencia } from "@/lib/services/attendanceService";
import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";

function fmtTime(iso?: string) {
  if (!iso) return "—";
  return new Date(iso).toLocaleTimeString("es-MX", { hour: "2-digit", minute: "2-digit" });
}

function fmtShiftTime(t?: string) {
  if (!t) return "—";
  const parts = t.split(":");
  return `${parts[0]}:${parts[1]}`;
}

export function AdminAsistenciaDashboardView() {
  const [dash, setDash] = useState<AttendanceDashboard | null>(null);
  const [records, setRecords] = useState<RegistroAsistencia[]>([]);
  const [msg, setMsg] = useState("");
  const [error, setError] = useState("");

  const load = useCallback(async () => {
    const [d, r] = await Promise.all([
      attendanceAdminService.getDashboard(),
      attendanceAdminService.listRecords() as Promise<RegistroAsistencia[]>,
    ]);
    setDash(d);
    setRecords(r);
  }, []);

  useEffect(() => { load().catch(console.error); }, [load]);

  async function generarIncidencias() {
    setError("");
    try {
      const res = await attendanceAdminService.generateIncidents();
      setMsg(`Incidencias generadas: ${res.generadas}`);
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error");
    }
  }

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader
        title="Monitor de asistencia"
        subtitle="Tablero operativo DH — registros, incidencias y prenómina."
        action={
          <Button type="button" variant="secondary" onClick={generarIncidencias}>
            Generar incidencias del día
          </Button>
        }
      />
      {msg && <Alert variant="success">{msg}</Alert>}
      {error && <Alert variant="error">{error}</Alert>}

      {dash && (
        <>
          <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
            <StatCard label="Presentes hoy" value={String(dash.presentesHoy)} accentClass="text-emerald-700" />
            <StatCard label="Ausentes hoy" value={String(dash.ausentesHoy)} accentClass="text-red-600" />
            <StatCard label="Retardos hoy" value={String(dash.retardosHoy)} accentClass="text-amber-700" />
            <StatCard label="Correcciones pendientes" value={String(dash.correccionesPendientes)} accentClass="text-urrea-primary" />
            <StatCard label="Salidas tempranas" value={String(dash.salidasTempranas)} accentClass="text-amber-600" />
            <StatCard label="Horas extra" value={String(dash.horasExtra)} accentClass="text-blue-700" />
            <StatCard label="Incidencias nómina" value={String(dash.incidenciasNomina)} accentClass="text-purple-700" />
            <StatCard label="Comercial sin reporte" value={String(dash.comercialSinReporte)} accentClass="text-slate-600" />
          </div>

          {dash.ultimoCorteGenerado && (
            <p className="text-sm text-urrea-text-muted">
              Último corte: {new Date(dash.ultimoCorteGenerado).toLocaleString("es-MX")}
            </p>
          )}

          <Card title="Registros de hoy">
            {records.length === 0 ? (
              <EmptyState message="Sin registros para hoy." />
            ) : (
              <div className="overflow-x-auto">
                <table className="min-w-full text-sm">
                  <thead>
                    <tr className="border-b text-urrea-text-muted">
                      <th className="py-2 pr-4 text-left">Fecha</th>
                      <th className="py-2 pr-4 text-left">Entrada</th>
                      <th className="py-2 pr-4 text-left">Salida</th>
                      <th className="py-2 pr-4 text-left">Fuente</th>
                      <th className="py-2 text-left">Estado</th>
                    </tr>
                  </thead>
                  <tbody>
                    {records.map((r) => (
                      <tr key={r.id} className="border-b border-urrea-border/60">
                        <td className="py-2 pr-4">{new Date(r.fecha).toLocaleDateString("es-MX")}</td>
                        <td className="py-2 pr-4">{fmtTime(r.horaEntrada)}</td>
                        <td className="py-2 pr-4">{fmtTime(r.horaSalida)}</td>
                        <td className="py-2 pr-4">{r.fuente}</td>
                        <td className="py-2">{r.estado}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </Card>

          {dash.incidenciasRecientes.length > 0 && (
            <Card title="Incidencias recientes">
              <ul className="space-y-2 text-sm">
                {dash.incidenciasRecientes.map((i) => (
                  <li key={i.id}>
                    {i.colaboradorNombre} · {i.tipo} · {i.estado}
                  </li>
                ))}
              </ul>
            </Card>
          )}
        </>
      )}
    </PageContainer>
  );
}

export function AdminIncidenciasView() {
  const [rows, setRows] = useState<IncidenciaAsistencia[]>([]);
  const [estado, setEstado] = useState("");

  const load = useCallback(() => {
    const params: Record<string, string> = {};
    if (estado) params.estado = estado;
    return attendanceAdminService.listIncidents(params).then(setRows);
  }, [estado]);

  useEffect(() => { load().catch(console.error); }, [load]);

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Incidencias de asistencia" subtitle="Retardos, ausencias, salidas tempranas y más." />
      <select className="rounded-lg border px-3 py-2 text-sm" value={estado} onChange={(e) => setEstado(e.target.value)}>
        <option value="">Todos los estados</option>
        <option value="Detectada">Detectada</option>
        <option value="PendienteValidacion">Pendiente validación</option>
        <option value="Justificada">Justificada</option>
        <option value="NoJustificada">No justificada</option>
        <option value="Rechazada">Rechazada</option>
        <option value="AplicadaNomina">Aplicada nómina</option>
      </select>
      {rows.length === 0 ? (
        <EmptyState message="Sin incidencias." />
      ) : (
        <div className="overflow-x-auto rounded-xl border bg-white">
          <table className="min-w-full text-left text-sm">
            <thead>
              <tr className="border-b bg-slate-50 text-slate-500">
                <th className="px-3 py-2">Colaborador</th>
                <th className="px-3 py-2">Área</th>
                <th className="px-3 py-2">Fecha</th>
                <th className="px-3 py-2">Tipo</th>
                <th className="px-3 py-2">Estado</th>
                <th className="px-3 py-2">Nómina</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((r) => (
                <tr key={r.id} className="border-b">
                  <td className="px-3 py-2 font-medium">{r.colaboradorNombre}</td>
                  <td className="px-3 py-2">{r.departamento ?? "—"}</td>
                  <td className="px-3 py-2">{new Date(r.fecha).toLocaleDateString("es-MX")}</td>
                  <td className="px-3 py-2">{r.tipo}</td>
                  <td className="px-3 py-2"><Badge estado={r.estado} /></td>
                  <td className="px-3 py-2">{r.generaPrenomina ? "Sí" : "No"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </PageContainer>
  );
}

export function AdminTurnosView() {
  const [rows, setRows] = useState<Turno[]>([]);
  const [msg, setMsg] = useState("");

  const load = useCallback(() => attendanceAdminService.listShifts().then(setRows), []);

  useEffect(() => { load().catch(console.error); }, [load]);

  async function crear() {
    await attendanceAdminService.upsertShift({
      codigo: "TURNO-NUEVO",
      nombre: "Turno nuevo",
      horaEntrada: "08:30:00",
      horaSalida: "18:00:00",
      minutosToleranciaEntrada: 10,
      minutosToleranciaSalida: 5,
      minutosComida: 60,
      aplicaLunes: true,
      aplicaMartes: true,
      aplicaMiercoles: true,
      aplicaJueves: true,
      aplicaViernes: true,
      aplicaSabado: false,
      aplicaDomingo: false,
      isActive: true,
    });
    setMsg("Turno creado.");
    await load();
  }

  return (
    <PageContainer className="max-w-4xl">
      <PageHeader title="Configuración de turnos" subtitle="Horarios, tolerancias y días laborales." action={<Button type="button" onClick={crear}>Nuevo turno</Button>} />
      {msg && <Alert variant="success">{msg}</Alert>}
      {rows.length === 0 ? (
        <EmptyState message="Sin turnos configurados." />
      ) : (
        <div className="space-y-3">
          {rows.map((t) => (
            <Card key={t.id}>
              <p className="font-semibold">{t.nombre} <span className="text-sm font-normal text-urrea-text-muted">({t.codigo})</span></p>
              <p className="text-sm">Entrada {fmtShiftTime(t.horaEntrada)} · Salida {fmtShiftTime(t.horaSalida)}</p>
              <p className="text-sm text-urrea-text-muted">Tolerancia entrada: {t.minutosToleranciaEntrada} min · Comida: {t.minutosComida} min</p>
              <p className="text-xs">{t.isActive ? "Activo" : "Inactivo"}</p>
            </Card>
          ))}
        </div>
      )}
    </PageContainer>
  );
}

export function AdminReglasView() {
  const [rules, setRules] = useState<ReglasAsistencia | null>(null);
  const [msg, setMsg] = useState("");

  useEffect(() => {
    attendanceAdminService.getRules().then(setRules).catch(console.error);
  }, []);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    if (!rules) return;
    const updated = await attendanceAdminService.updateRules(rules);
    setRules(updated);
    setMsg("Reglas actualizadas.");
  }

  if (!rules) return <PageContainer><p className="text-sm text-urrea-text-muted">Cargando…</p></PageContainer>;

  return (
    <PageContainer className="max-w-2xl">
      <PageHeader title="Reglas de asistencia" subtitle="Tolerancias, geolocalización y validación de líder." />
      {msg && <Alert variant="success">{msg}</Alert>}
      <form onSubmit={onSubmit}>
        <Card title="Parámetros globales">
          <div className="grid gap-4 sm:grid-cols-2">
            <Input label="Minutos tolerancia retardo" type="number" value={rules.minutosToleranciaRetardo} onChange={(e) => setRules({ ...rules, minutosToleranciaRetardo: Number(e.target.value) })} />
            <Input label="Minutos para falta" type="number" value={rules.minutosParaFalta} onChange={(e) => setRules({ ...rules, minutosParaFalta: Number(e.target.value) })} />
            <Input label="Radio metros sede" type="number" value={rules.radioMetrosSede} onChange={(e) => setRules({ ...rules, radioMetrosSede: Number(e.target.value) })} />
            <Select label="Registro móvil" value={rules.permitirRegistroMovil ? "1" : "0"} onChange={(e) => setRules({ ...rules, permitirRegistroMovil: e.target.value === "1" })}>
              <option value="1">Permitido</option>
              <option value="0">No permitido</option>
            </Select>
            <Select label="Requiere geolocalización" value={rules.requiereGeolocalizacion ? "1" : "0"} onChange={(e) => setRules({ ...rules, requiereGeolocalizacion: e.target.value === "1" })}>
              <option value="1">Sí</option>
              <option value="0">No</option>
            </Select>
            <Select label="Validación líder" value={rules.requiereValidacionLider ? "1" : "0"} onChange={(e) => setRules({ ...rules, requiereValidacionLider: e.target.value === "1" })}>
              <option value="1">Requerida</option>
              <option value="0">No requerida</option>
            </Select>
          </div>
          <Button type="submit" className="mt-4">Guardar reglas</Button>
        </Card>
      </form>
    </PageContainer>
  );
}

export function AdminNominaAsistenciaView() {
  const [rows, setRows] = useState<IncidenciaNominaAsistencia[]>([]);
  const [periodo, setPeriodo] = useState(new Date().toISOString().slice(0, 7));
  const [msg, setMsg] = useState("");
  const [error, setError] = useState("");

  const load = useCallback(() => attendanceAdminService.listPayrollIncidents().then(setRows), []);

  useEffect(() => { load().catch(console.error); }, [load]);

  async function generar() {
    setError("");
    try {
      const res = await attendanceAdminService.generatePayroll(periodo);
      setMsg(`Corte generado: ${res.generadas} incidencias.`);
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error");
    }
  }

  async function enviar() {
    setError("");
    try {
      await attendanceAdminService.sendPayroll();
      setMsg("Enviado a nómina.");
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error");
    }
  }

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader
        title="Reporte para nómina"
        subtitle="Corte de incidencias de asistencia listas para prenómina."
        action={
          <div className="flex gap-2">
            <Button type="button" variant="secondary" onClick={generar}>Generar corte</Button>
            <Button type="button" onClick={enviar}>Enviar a nómina</Button>
          </div>
        }
      />
      <Input label="Periodo (YYYY-MM)" value={periodo} onChange={(e) => setPeriodo(e.target.value)} className="max-w-xs" />
      {msg && <Alert variant="success">{msg}</Alert>}
      {error && <Alert variant="error">{error}</Alert>}
      {rows.length === 0 ? (
        <EmptyState message="Sin incidencias de nómina." />
      ) : (
        <div className="overflow-x-auto rounded-xl border bg-white">
          <table className="min-w-full text-left text-sm">
            <thead>
              <tr className="border-b bg-slate-50 text-slate-500">
                <th className="px-3 py-2">Colaborador</th>
                <th className="px-3 py-2">Periodo</th>
                <th className="px-3 py-2">Concepto</th>
                <th className="px-3 py-2">Cantidad</th>
                <th className="px-3 py-2">Estado</th>
                <th className="px-3 py-2">Sync nómina</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((r) => (
                <tr key={r.id} className="border-b">
                  <td className="px-3 py-2"><p className="font-medium">{r.colaboradorNombre}</p><p className="text-xs text-slate-500">{r.numeroEmpleado}</p></td>
                  <td className="px-3 py-2">{r.periodo}</td>
                  <td className="px-3 py-2">{r.tipoConcepto}</td>
                  <td className="px-3 py-2">{r.cantidad} {r.unidad}</td>
                  <td className="px-3 py-2"><Badge estado={r.estado} /></td>
                  <td className="px-3 py-2">{r.nominaSyncAt ? new Date(r.nominaSyncAt).toLocaleString("es-MX") : "—"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </PageContainer>
  );
}

export function AdminComercialView() {
  const [rows, setRows] = useState<RegistroAsistencia[]>([]);
  const [cliente, setCliente] = useState("");
  const [ubicacion, setUbicacion] = useState("");
  const [comentario, setComentario] = useState("");
  const [msg, setMsg] = useState("");
  const [error, setError] = useState("");

  const load = useCallback(() =>
    fetchApi<RegistroAsistencia[]>(`${v1("/attendance/commercial/records")}`).then(setRows), []);

  useEffect(() => { load().catch(console.error); }, [load]);

  async function checkIn() {
    setError("");
    try {
      await fetchApi(v1("/attendance/commercial/check-in"), {
        method: "POST",
        body: JSON.stringify({ clienteComercial: cliente, ubicacionComercial: ubicacion, observaciones: comentario }),
      });
      setMsg("Check-in comercial registrado.");
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error");
    }
  }

  async function checkOut() {
    setError("");
    try {
      await fetchApi(v1("/attendance/commercial/check-out"), {
        method: "POST",
        body: JSON.stringify({ observaciones: comentario }),
      });
      setMsg("Check-out comercial registrado.");
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error");
    }
  }

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Asistencia fuerza de ventas" subtitle="Registro por visita, ruta y check-in con cliente." />
      {msg && <Alert variant="success">{msg}</Alert>}
      {error && <Alert variant="error">{error}</Alert>}
      <Card title="Nuevo registro comercial">
        <div className="grid gap-4 sm:grid-cols-2">
          <Input label="Cliente" value={cliente} onChange={(e) => setCliente(e.target.value)} />
          <Input label="Ubicación" value={ubicacion} onChange={(e) => setUbicacion(e.target.value)} />
          <Textarea label="Comentario" value={comentario} onChange={(e) => setComentario(e.target.value)} rows={2} className="sm:col-span-2" />
        </div>
        <div className="mt-3 flex gap-2">
          <Button type="button" onClick={checkIn}>Check-in visita</Button>
          <Button type="button" variant="secondary" onClick={checkOut}>Check-out visita</Button>
        </div>
      </Card>
      <Card title="Registros comerciales">
        {rows.length === 0 ? (
          <EmptyState message="Sin registros comerciales hoy." />
        ) : (
          <ul className="space-y-2 text-sm">
            {rows.map((r) => (
              <li key={r.id}>
                {new Date(r.fecha).toLocaleDateString("es-MX")} · {fmtTime(r.horaEntrada)} – {fmtTime(r.horaSalida)} · {r.estado}
                {r.observaciones && <span className="text-urrea-text-muted"> — {r.observaciones}</span>}
              </li>
            ))}
          </ul>
        )}
      </Card>
    </PageContainer>
  );
}
