"use client";

import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, StatCard } from "@/components/ui/card";
import { Input, Textarea, Select } from "@/components/ui/input";
import { Alert } from "@/components/ui/page-header";
import {
  attendanceService,
  type AsignacionTurno,
  type Turno,
  type SolicitudCambioHorario,
} from "@/lib/services/attendanceService";

function fmtShiftTime(t?: string) {
  if (!t) return "—";
  const parts = t.split(":");
  return `${parts[0]}:${parts[1]}`;
}

export function HorariosTabContent() {
  const [shifts, setShifts] = useState<AsignacionTurno[]>([]);
  const [availableShifts, setAvailableShifts] = useState<Turno[]>([]);
  const [requests, setRequests] = useState<SolicitudCambioHorario[]>([]);
  const [activeTab, setActiveTab] = useState<"activo" | "cambiar" | "solicitudes">("activo");

  // Form State
  const [selectedShiftId, setSelectedShiftId] = useState("");
  const [reason, setReason] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [successMsg, setSuccessMsg] = useState("");

  const loadData = async () => {
    try {
      const [myShifts, allAvailable, myRequests] = await Promise.all([
        attendanceService.getMyShifts(),
        attendanceService.getAvailableShifts(),
        attendanceService.getMyShiftRequests(),
      ]);
      setShifts(myShifts);
      setAvailableShifts(allAvailable);
      setRequests(myRequests);
    } catch (err) {
      console.error("Error al cargar los horarios:", err);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const activeAssignment = shifts.find((s) => !s.fechaFin);
  const activeShiftInfo = availableShifts.find((t) => t.id === activeAssignment?.turnoId);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedShiftId) {
      setError("Por favor selecciona el turno que deseas solicitar.");
      return;
    }
    if (!reason.trim()) {
      setError("Por favor ingresa el motivo del cambio.");
      return;
    }

    setLoading(true);
    setError("");
    setSuccessMsg("");

    try {
      await attendanceService.createShiftRequest({
        turnoSolicitadoId: selectedShiftId,
        motivo: reason,
      });
      setSuccessMsg("¡Tu solicitud de cambio de horario ha sido enviada con éxito!");
      setReason("");
      setSelectedShiftId("");
      await loadData();
      setActiveTab("solicitudes");
    } catch (err: any) {
      setError(err.message || "Error al crear la solicitud.");
    } finally {
      setLoading(false);
    }
  };

  const getStatusBadge = (estado: string) => {
    switch (estado) {
      case "Pendiente":
        return <span className="px-2.5 py-1 text-xs rounded-full bg-amber-50 text-amber-700 border border-amber-200 font-medium">Pendiente</span>;
      case "Aprobado":
        return <span className="px-2.5 py-1 text-xs rounded-full bg-emerald-50 text-emerald-700 border border-emerald-200 font-medium">Aprobado</span>;
      case "Rechazado":
        return <span className="px-2.5 py-1 text-xs rounded-full bg-rose-50 text-rose-700 border border-rose-200 font-medium">Rechazado</span>;
      default:
        return <span className="px-2.5 py-1 text-xs rounded-full bg-slate-50 text-slate-700 border border-slate-200 font-medium">{estado}</span>;
    }
  };

  return (
    <div className="space-y-6">
      {error && <Alert variant="error">{error}</Alert>}
      {successMsg && <Alert variant="success">{successMsg}</Alert>}

      <div className="mb-6 border-b border-urrea-border/60">
        <nav className="flex space-x-6" aria-label="Tabs">
          <button
            onClick={() => setActiveTab("activo")}
            className={`pb-4 text-sm font-semibold border-b-2 transition ${
              activeTab === "activo"
                ? "border-urrea-primary text-urrea-primary"
                : "border-transparent text-urrea-text-muted hover:text-urrea-text"
            }`}
          >
            Horario Activo
          </button>
          <button
            onClick={() => setActiveTab("cambiar")}
            className={`pb-4 text-sm font-semibold border-b-2 transition ${
              activeTab === "cambiar"
                ? "border-urrea-primary text-urrea-primary"
                : "border-transparent text-urrea-text-muted hover:text-urrea-text"
            }`}
          >
            Solicitar Cambio
          </button>
          <button
            onClick={() => setActiveTab("solicitudes")}
            className={`pb-4 text-sm font-semibold border-b-2 transition ${
              activeTab === "solicitudes"
                ? "border-urrea-primary text-urrea-primary"
                : "border-transparent text-urrea-text-muted hover:text-urrea-text"
            }`}
          >
            Mis Solicitudes ({requests.length})
          </button>
        </nav>
      </div>

      {activeTab === "activo" && (
        <div className="space-y-6">
          <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
            <StatCard
              label="Turno Actual"
              value={activeAssignment?.turnoNombre ?? "Sin asignar"}
              accentClass="text-urrea-primary"
            />
            <StatCard
              label="Horas Laborales"
              value={
                activeShiftInfo
                  ? `${fmtShiftTime(activeShiftInfo.horaEntrada)} – ${fmtShiftTime(activeShiftInfo.horaSalida)}`
                  : "—"
              }
              accentClass="text-emerald-700"
            />
            <StatCard
              label="Tolerancia Entrada"
              value={activeShiftInfo ? `${activeShiftInfo.minutosToleranciaEntrada} minutos` : "—"}
              accentClass="text-amber-700"
            />
          </div>

          <Card title="Detalles del Horario">
            {activeShiftInfo ? (
              <div className="grid gap-6 md:grid-cols-2">
                <div className="space-y-3">
                  <div>
                    <h4 className="text-xs text-urrea-text-muted font-bold uppercase tracking-wider">Código del turno</h4>
                    <p className="text-sm font-semibold text-urrea-text mt-0.5">{activeShiftInfo.codigo}</p>
                  </div>
                  <div>
                    <h4 className="text-xs text-urrea-text-muted font-bold uppercase tracking-wider">Nombre</h4>
                    <p className="text-sm font-semibold text-urrea-text mt-0.5">{activeShiftInfo.nombre}</p>
                  </div>
                  <div>
                    <h4 className="text-xs text-urrea-text-muted font-bold uppercase tracking-wider">Horario de Comida</h4>
                    <p className="text-sm font-semibold text-urrea-text mt-0.5">{activeShiftInfo.minutosComida} minutos</p>
                  </div>
                </div>

                <div className="space-y-3">
                  <div>
                    <h4 className="text-xs text-urrea-text-muted font-bold uppercase tracking-wider">Asignación activa desde</h4>
                    <p className="text-sm font-semibold text-urrea-text mt-0.5">
                      {activeAssignment ? new Date(activeAssignment.fechaInicio).toLocaleDateString("es-MX") : "—"}
                    </p>
                  </div>
                  <div>
                    <h4 className="text-xs text-urrea-text-muted font-bold uppercase tracking-wider">Método de origen</h4>
                    <p className="text-sm font-semibold text-urrea-text mt-0.5">{activeAssignment?.origen ?? "Sistema"}</p>
                  </div>
                </div>
              </div>
            ) : (
              <p className="text-sm text-urrea-text-muted">No se encontraron detalles para tu turno activo.</p>
            )}
          </Card>

          <Card title="Historial de Asignaciones">
            {shifts.length === 0 ? (
              <p className="text-sm text-urrea-text-muted">Sin historial registrado.</p>
            ) : (
              <div className="overflow-x-auto">
                <table className="min-w-full text-sm text-left">
                  <thead>
                    <tr className="border-b text-urrea-text-muted font-bold">
                      <th className="py-2.5 pr-4">Turno</th>
                      <th className="py-2.5 pr-4">Fecha Inicio</th>
                      <th className="py-2.5 pr-4">Fecha Fin</th>
                      <th className="py-2.5">Origen</th>
                    </tr>
                  </thead>
                  <tbody>
                    {shifts.map((s) => (
                      <tr key={s.id} className="border-b border-urrea-border/60 hover:bg-slate-50/50">
                        <td className="py-3 pr-4 font-semibold text-urrea-text">{s.turnoNombre}</td>
                        <td className="py-3 pr-4 text-urrea-text-muted">{new Date(s.fechaInicio).toLocaleDateString("es-MX")}</td>
                        <td className="py-3 pr-4 text-urrea-text-muted">
                          {s.fechaFin ? new Date(s.fechaFin).toLocaleDateString("es-MX") : "Activo"}
                        </td>
                        <td className="py-3 text-urrea-text-muted">{s.origen}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </Card>
        </div>
      )}

      {activeTab === "cambiar" && (
        <form onSubmit={handleSubmit} className="max-w-xl">
          <Card title="Solicitud de cambio de turno">
            <div className="space-y-4">
              <div>
                <label className="block text-xs font-bold text-urrea-text uppercase tracking-wider mb-2">Turno Solicitado</label>
                <select
                  value={selectedShiftId}
                  onChange={(e) => setSelectedShiftId(e.target.value)}
                  className="w-full rounded-xl border border-urrea-border/80 bg-white px-3 py-2 text-sm shadow-sm transition hover:border-urrea-border focus:border-urrea-primary focus:outline-none"
                >
                  <option value="">Selecciona un turno...</option>
                  {availableShifts
                    .filter((t) => t.id !== activeAssignment?.turnoId)
                    .map((t) => (
                      <option key={t.id} value={t.id}>
                        {t.nombre} ({fmtShiftTime(t.horaEntrada)} - {fmtShiftTime(t.horaSalida)})
                      </option>
                    ))}
                </select>
              </div>

              <div>
                <label className="block text-xs font-bold text-urrea-text uppercase tracking-wider mb-2">Motivo del Cambio</label>
                <textarea
                  value={reason}
                  onChange={(e) => setReason(e.target.value)}
                  rows={4}
                  placeholder="Por favor explica brevemente la razón por la cual estás solicitando el cambio..."
                  className="w-full rounded-xl border border-urrea-border/80 bg-white px-3 py-2 text-sm shadow-sm transition hover:border-urrea-border focus:border-urrea-primary focus:outline-none"
                />
              </div>

              <Button type="submit" disabled={loading} className="w-full sm:w-auto">
                {loading ? "Enviando..." : "Enviar Solicitud"}
              </Button>
            </div>
          </Card>
        </form>
      )}

      {activeTab === "solicitudes" && (
        <Card title="Mis solicitudes de cambio de horario">
          {requests.length === 0 ? (
            <p className="text-sm text-urrea-text-muted">No has enviado ninguna solicitud de cambio.</p>
          ) : (
            <div className="space-y-4">
              {requests.map((r) => (
                <div key={r.id} className="border border-urrea-border/60 rounded-xl p-4 shadow-sm hover:shadow transition bg-white">
                  <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                    <div>
                      <p className="text-sm font-semibold text-urrea-text">
                        De <span className="underline">{r.turnoActualNombre}</span> a <span className="underline font-bold">{r.turnoSolicitadoNombre}</span>
                      </p>
                      <p className="text-xs text-urrea-text-muted mt-1">
                        Solicitado el {new Date(r.createdAt).toLocaleString("es-MX")}
                      </p>
                      <p className="mt-2 text-sm text-urrea-text-muted bg-slate-50 p-2.5 rounded-lg border border-urrea-border/40">
                        <span className="font-bold text-urrea-text block text-xs mb-0.5">Motivo del colaborador:</span>
                        &quot;{r.motivo}&quot;
                      </p>
                      {r.comentarioAprobador && (
                        <p className="mt-2 text-sm text-urrea-text-muted bg-indigo-50/50 p-2.5 rounded-lg border border-indigo-100">
                          <span className="font-bold text-indigo-950 block text-xs mb-0.5">Respuesta de {r.aprobadorNombre}:</span>
                          &quot;{r.comentarioAprobador}&quot;
                        </p>
                      )}
                    </div>
                    <div className="shrink-0">
                      {getStatusBadge(r.estado)}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </Card>
      )}
    </div>
  );
}
