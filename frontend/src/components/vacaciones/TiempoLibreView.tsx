"use client";

import Link from "next/link";
import { useSearchParams } from "next/navigation";
import { FormEvent, useEffect, useMemo, useState } from "react";
import { DhIcon } from "@/components/dh/shared/icons";
import { AprobacionStepper } from "@/components/vacaciones/AprobacionStepper";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, StatCard } from "@/components/ui/card";
import { Input, Select, Textarea } from "@/components/ui/input";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { absenceTypeIcon, isVacationType } from "@/lib/absence-icons";
import { absenceService } from "@/lib/services/absenceService";
import type {
  CalculateDaysResult,
  ResumenTipoPermiso,
  SaldoVacaciones,
  SolicitudAusencia,
  TipoAusencia,
} from "@/lib/types";
import { cn } from "@/lib/utils";

function formatPeriodo(s: SolicitudAusencia) {
  const fecha = new Date(s.fechaInicio).toLocaleDateString("es-MX", { day: "numeric", month: "short", year: "numeric" });
  if (s.esDiaCompleto === false || s.horaInicio || s.horaFin) {
    const hora = s.tipoAusenciaCodigo === "ENTRADA_TARDE" ? s.horaInicio : s.horaFin;
    return `${fecha}${hora ? ` · ${hora}` : ""}`;
  }
  if (s.fechaInicio === s.fechaFin) return fecha;
  const fin = new Date(s.fechaFin).toLocaleDateString("es-MX", { day: "numeric", month: "short", year: "numeric" });
  return `${fecha} – ${fin}`;
}

type HistorialFilter = "todos" | "vacaciones" | "permisos";

export function TiempoLibreView() {
  const searchParams = useSearchParams();
  const defaultTab = searchParams.get("solicitar") != null ? "solicitar" : "solicitar";

  const [tab, setTab] = useState(defaultTab);
  const [tipos, setTipos] = useState<TipoAusencia[]>([]);
  const [resumen, setResumen] = useState<ResumenTipoPermiso[]>([]);
  const [saldo, setSaldo] = useState<SaldoVacaciones | null>(null);
  const [solicitudes, setSolicitudes] = useState<SolicitudAusencia[]>([]);
  const [tipoId, setTipoId] = useState("");
  const [fechaInicio, setFechaInicio] = useState("");
  const [fechaFin, setFechaFin] = useState("");
  const [horaInicio, setHoraInicio] = useState("");
  const [horaFin, setHoraFin] = useState("");
  const [comentario, setComentario] = useState("");
  const [preview, setPreview] = useState<CalculateDaysResult | null>(null);
  const [diasPreview, setDiasPreview] = useState<number | null>(null);
  const [historialFilter, setHistorialFilter] = useState<HistorialFilter>("todos");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);

  const tipo = useMemo(() => tipos.find((t) => t.id === tipoId) ?? null, [tipos, tipoId]);
  const vacTipos = useMemo(() => tipos.filter((t) => isVacationType(t.codigo)), [tipos]);
  const permTipos = useMemo(() => tipos.filter((t) => !isVacationType(t.codigo)), [tipos]);

  const historial = useMemo(() => {
    const list = solicitudes;
    if (historialFilter === "vacaciones") return list.filter((s) => isVacationType(s.tipoAusenciaCodigo));
    if (historialFilter === "permisos") return list.filter((s) => !isVacationType(s.tipoAusenciaCodigo));
    return list;
  }, [solicitudes, historialFilter]);

  const pendientes = solicitudes.filter((s) => s.estado === "Pendiente");

  function loadData() {
    absenceService.getRequestTypes().then(setTipos).catch(console.error);
    absenceService.getPermissionSummary().then(setResumen).catch(console.error);
    absenceService.getMyBalance().then(setSaldo).catch(console.error);
    absenceService.getMyRequests().then(setSolicitudes).catch(console.error);
  }

  useEffect(() => { loadData(); }, []);

  useEffect(() => {
    const pre = searchParams.get("tipo");
    if (!pre || tipos.length === 0) return;
    if (pre === "vac") {
      const vac = tipos.find((t) => t.codigo === "VAC");
      if (vac) setTipoId(vac.id);
      setTab("solicitar");
    } else if (pre === "permiso" && permTipos[0]) {
      setTipoId(permTipos[0].id);
      setTab("solicitar");
    }
  }, [searchParams, tipos, permTipos]);

  useEffect(() => {
    if (!tipo) { setPreview(null); setDiasPreview(null); return; }
    if (tipo.esParcial && fechaInicio) {
      setDiasPreview(0.5);
      setPreview(null);
      return;
    }
    if (fechaInicio && (fechaFin || !tipo.permiteMultiDia)) {
      const fin = tipo.permiteMultiDia ? (fechaFin || fechaInicio) : fechaInicio;
      absenceService.calculateDays(fechaInicio, fin, tipo.id)
        .then((r) => { setPreview(r); setDiasPreview(r.diasHabiles); })
        .catch(() => { setPreview(null); setDiasPreview(null); });
      return;
    }
    setPreview(null);
    setDiasPreview(null);
  }, [fechaInicio, fechaFin, tipo]);

  useEffect(() => {
    if (tipo?.esParcial && fechaInicio) setFechaFin(fechaInicio);
  }, [fechaInicio, tipo]);

  function resetForm() {
    setTipoId("");
    setFechaInicio("");
    setFechaFin("");
    setHoraInicio("");
    setHoraFin("");
    setComentario("");
    setPreview(null);
    setDiasPreview(null);
    setError("");
  }

  async function submit(e: FormEvent) {
    e.preventDefault();
    if (!tipo) return;
    setError("");
    setSuccess("");
    setLoading(true);
    try {
      const fin = tipo.esParcial || !tipo.permiteMultiDia ? fechaInicio : (fechaFin || fechaInicio);
      await absenceService.createRequest({
        tipoAusenciaId: tipo.id,
        fechaInicio,
        fechaFin: fin,
        comentario: comentario || null,
        enviar: true,
        esDiaCompleto: !tipo.esParcial,
        horaInicio: horaInicio || null,
        horaFin: horaFin || null,
      });
      setSuccess("Solicitud enviada. Tu jefe la revisará pronto.");
      resetForm();
      loadData();
      setTab("historial");
    } catch (err) {
      setError(err instanceof Error ? err.message : "No se pudo enviar la solicitud");
    } finally {
      setLoading(false);
    }
  }

  async function cancelar(id: string) {
    try {
      await absenceService.cancelRequest(id);
      loadData();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al cancelar");
    }
  }

  const resumenTipo = tipo ? resumen.find((r) => r.codigo === tipo.codigo) : undefined;

  return (
    <PageContainer className="max-w-3xl animate-fade-up">
      <PageHeader
        title="Vacaciones y permisos"
        subtitle="Un solo lugar para solicitar tiempo libre y consultar tu historial."
        infoTitle="¿Vacaciones o permiso?"
        infoContent="Las vacaciones descuentan tu saldo anual de días. Los permisos son ausencias reguladas por la LFT (incapacidad, matrimonio, etc.) y pueden tener reglas distintas de goce de sueldo. Selecciona el tipo, indica fechas y envía."
      />

      <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
        <StatCard label="Disponibles" value={String(saldo?.diasDisponibles ?? saldo?.diasPendientes ?? "—")} accentClass="text-emerald-700" />
        <StatCard label="Asignados" value={String(saldo?.diasAsignados ?? "—")} accentClass="text-urrea-primary" />
        <StatCard label="Usados" value={String(saldo?.diasUsados ?? "—")} accentClass="text-urrea-secondary" />
        <StatCard label="Pendientes" value={String(pendientes.length)} accentClass="text-amber-700" />
      </div>

      <Tabs defaultValue="solicitar" value={tab} onValueChange={setTab}>
        <TabsList className="mb-4 w-full">
          <TabsTrigger value="solicitar" className="flex-1">Solicitar</TabsTrigger>
          <TabsTrigger value="historial" className="flex-1">Historial</TabsTrigger>
        </TabsList>

        <TabsContent value="solicitar">
          <form onSubmit={submit}>
            <Card title="Nueva solicitud" description="Completa el formulario y envía para aprobación.">
              <div className="space-y-4">
                <Select
                  label="Tipo de ausencia"
                  value={tipoId}
                  onChange={(e) => {
                    setTipoId(e.target.value);
                    setFechaInicio("");
                    setFechaFin("");
                    setError("");
                    setSuccess("");
                  }}
                  required
                >
                  <option value="">Selecciona vacaciones o permiso…</option>
                  {vacTipos.length > 0 && (
                    <optgroup label="Vacaciones">
                      {vacTipos.map((t) => (
                        <option key={t.id} value={t.id}>{t.nombre}</option>
                      ))}
                    </optgroup>
                  )}
                  {permTipos.length > 0 && (
                    <optgroup label="Permisos legales (LFT)">
                      {permTipos.map((t) => (
                        <option key={t.id} value={t.id}>{t.nombre}</option>
                      ))}
                    </optgroup>
                  )}
                </Select>

                {tipo && (
                  <div className="rounded-xl border border-urrea-border/60 bg-urrea-bg-soft/40 p-4">
                    <div className="flex items-start gap-3">
                      <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-white text-urrea-primary ring-1 ring-urrea-border/50">
                        <DhIcon name={absenceTypeIcon(tipo.codigo)} className="h-5 w-5" />
                      </span>
                      <div className="min-w-0 flex-1">
                        <p className="font-medium text-urrea-text">{tipo.nombre}</p>
                        {tipo.descripcion && <p className="mt-1 text-sm text-urrea-text-muted">{tipo.descripcion}</p>}
                        <div className="mt-2 flex flex-wrap gap-2">
                          {tipo.descuentaSaldo && (
                            <span className="rounded-md bg-urrea-primary/8 px-2 py-0.5 text-[11px] font-medium text-urrea-primary">Descuenta saldo</span>
                          )}
                          {tipo.remunerado === false && (
                            <span className="rounded-md bg-amber-100 px-2 py-0.5 text-[11px] font-medium text-amber-800">Sin goce de sueldo</span>
                          )}
                          {tipo.requiereComprobante && (
                            <span className="rounded-md bg-urrea-bg-soft px-2 py-0.5 text-[11px] font-medium text-urrea-text-muted">Requiere comprobante</span>
                          )}
                        </div>
                        {tipo.baseLegalLft && (
                          <p className="mt-2 text-xs text-urrea-secondary">{tipo.baseLegalLft}</p>
                        )}
                        {resumenTipo?.diasDisponibles != null && (
                          <p className="mt-1 text-xs font-medium text-urrea-primary">
                            Disponible este año: {resumenTipo.diasDisponibles} día(s)
                          </p>
                        )}
                      </div>
                    </div>
                  </div>
                )}

                <div className="grid gap-4 sm:grid-cols-2">
                  <Input
                    label={tipo?.esParcial ? "Fecha" : "Fecha inicio"}
                    type="date"
                    value={fechaInicio}
                    onChange={(e) => setFechaInicio(e.target.value)}
                    required
                    disabled={!tipo}
                  />
                  {tipo && !tipo.esParcial && tipo.permiteMultiDia !== false && (
                    <Input
                      label="Fecha fin"
                      type="date"
                      value={fechaFin}
                      onChange={(e) => setFechaFin(e.target.value)}
                      min={fechaInicio}
                      required
                    />
                  )}
                  {tipo?.codigo === "ENTRADA_TARDE" && (
                    <Input
                      label="Hora estimada de entrada"
                      type="time"
                      value={horaInicio}
                      onChange={(e) => setHoraInicio(e.target.value)}
                    />
                  )}
                  {tipo?.codigo === "SALIDA_TEMPRANO" && (
                    <Input
                      label="Hora de salida"
                      type="time"
                      value={horaFin}
                      onChange={(e) => setHoraFin(e.target.value)}
                    />
                  )}
                  <Textarea
                    label={tipo?.requiereComprobante ? "Motivo / referencia" : "Comentario (opcional)"}
                    value={comentario}
                    onChange={(e) => setComentario(e.target.value)}
                    rows={3}
                    className="sm:col-span-2"
                    placeholder="Agrega contexto para tu jefe o RH…"
                    disabled={!tipo}
                  />
                </div>

                {diasPreview != null && (
                  <div className="grid gap-3 rounded-xl border border-urrea-secondary/20 bg-urrea-secondary/5 p-4 sm:grid-cols-3">
                    <div>
                      <p className="text-xs font-medium uppercase tracking-wide text-urrea-text-muted">Días hábiles</p>
                      <p className="text-2xl font-semibold tabular-nums text-urrea-secondary">{diasPreview}</p>
                    </div>
                    {preview?.saldoPosterior != null && (
                      <div>
                        <p className="text-xs font-medium uppercase tracking-wide text-urrea-text-muted">Saldo después</p>
                        <p className="text-2xl font-semibold tabular-nums text-urrea-primary">{preview.saldoPosterior}</p>
                      </div>
                    )}
                    {preview?.excedeSaldo && (
                      <div className="sm:col-span-3">
                        <Alert variant="error">Excedes tu saldo disponible de vacaciones.</Alert>
                      </div>
                    )}
                    {preview?.tieneTraslape && (
                      <div className="sm:col-span-3">
                        <Alert variant="info">Hay traslape con otra solicitud activa en esas fechas.</Alert>
                      </div>
                    )}
                  </div>
                )}

                {error && <Alert variant="error">{error}</Alert>}
                {success && <Alert variant="success">{success}</Alert>}

                <div className="flex flex-col gap-2 sm:flex-row">
                  <Button
                    type="submit"
                    disabled={loading || !tipo || !fechaInicio || preview?.excedeSaldo}
                    className="w-full sm:w-auto"
                  >
                    {loading ? "Enviando…" : "Enviar solicitud"}
                  </Button>
                  {tipoId && (
                    <Button type="button" variant="ghost" onClick={resetForm} className="w-full sm:w-auto">
                      Limpiar
                    </Button>
                  )}
                </div>
              </div>
            </Card>
          </form>

          <p className="mt-4 text-center text-sm text-urrea-text-muted">
            <Link href="/portal/vacaciones/calendario" className="font-medium text-urrea-secondary hover:underline">
              Ver calendario personal
            </Link>
          </p>
        </TabsContent>

        <TabsContent value="historial">
          <Card title="Mis solicitudes">
            <div className="mb-4 flex flex-wrap gap-2">
              {(["todos", "vacaciones", "permisos"] as const).map((f) => (
                <button
                  key={f}
                  type="button"
                  onClick={() => setHistorialFilter(f)}
                  className={cn(
                    "rounded-full px-3 py-1.5 text-xs font-medium transition",
                    historialFilter === f
                      ? "bg-urrea-primary text-white"
                      : "bg-urrea-bg-soft text-urrea-text-muted hover:text-urrea-text",
                  )}
                >
                  {f === "todos" ? "Todos" : f === "vacaciones" ? "Vacaciones" : "Permisos"}
                </button>
              ))}
            </div>

            {historial.length === 0 ? (
              <EmptyState message="Sin solicitudes en esta categoría." />
            ) : (
              <ul className="space-y-3">
                {historial.map((s) => (
                  <li key={s.id} className="rounded-xl border border-urrea-border/70 bg-white p-4">
                    <div className="flex items-start gap-3">
                      <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-urrea-bg-soft text-urrea-primary">
                        <DhIcon name={absenceTypeIcon(s.tipoAusenciaCodigo)} className="h-4 w-4" />
                      </span>
                      <div className="min-w-0 flex-1">
                        <div className="flex flex-wrap items-center justify-between gap-2">
                          <p className="font-medium text-urrea-text">{s.tipoAusenciaNombre}</p>
                          <Badge estado={s.estado} />
                        </div>
                        <p className="mt-1 text-sm text-urrea-text-muted">
                          {formatPeriodo(s)} · {s.diasSolicitados} día{s.diasSolicitados === 1 ? "" : "s"}
                        </p>
                        {s.pasosAprobacion && s.pasosAprobacion.length > 0 && (
                          <div className="mt-3">
                            <AprobacionStepper pasos={s.pasosAprobacion} compact />
                          </div>
                        )}
                        {s.comentario && (
                          <p className="mt-2 text-xs text-urrea-text-muted">{s.comentario}</p>
                        )}
                        {s.estado === "Pendiente" && (
                          <button
                            type="button"
                            onClick={() => cancelar(s.id)}
                            className="mt-2 text-xs font-medium text-red-600 hover:underline"
                          >
                            Cancelar solicitud
                          </button>
                        )}
                      </div>
                    </div>
                  </li>
                ))}
              </ul>
            )}
          </Card>
        </TabsContent>
      </Tabs>
    </PageContainer>
  );
}
