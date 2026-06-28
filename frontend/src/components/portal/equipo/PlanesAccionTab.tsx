"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input, Select, Textarea } from "@/components/ui/input";
import { Alert, EmptyState } from "@/components/ui/page-header";
import { teamService } from "@/lib/services/teamService";
import { PLAN_ESTADO_COLORS, PLAN_ESTADO_LABELS, type EquipoMiembro, type PlanAccion } from "@/lib/types";
import { cn } from "@/lib/utils";

export function PlanesAccionTab({
  miembros,
  planes,
  onRefresh,
}: {
  miembros: EquipoMiembro[];
  planes: PlanAccion[];
  onRefresh: () => void;
}) {
  const [showForm, setShowForm] = useState(false);
  const [colaboradorId, setColaboradorId] = useState("");
  const [titulo, setTitulo] = useState("");
  const [descripcion, setDescripcion] = useState("");
  const [fechaInicio, setFechaInicio] = useState("");
  const [fechaFin, setFechaFin] = useState("");
  const [prioridad, setPrioridad] = useState("Media");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      await teamService.createActionPlan({ colaboradorId, titulo, descripcion: descripcion || null, fechaInicio, fechaFin, prioridad });
      setShowForm(false);
      setTitulo("");
      setDescripcion("");
      setFechaInicio("");
      setFechaFin("");
      setColaboradorId("");
      onRefresh();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al crear plan");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <p className="text-sm text-urrea-text-muted">
          Define planes de acción con objetivos, fechas y seguimiento para tu equipo.
        </p>
        <Button type="button" onClick={() => setShowForm((v) => !v)} className="w-full sm:w-auto">
          {showForm ? "Cancelar" : "Nuevo plan de acción"}
        </Button>
      </div>

      {showForm && (
        <form onSubmit={submit} className="rounded-2xl border border-urrea-border/80 bg-urrea-bg-soft/30 p-4">
          <div className="grid gap-4 sm:grid-cols-2">
            <Select label="Colaborador" value={colaboradorId} onChange={(e) => setColaboradorId(e.target.value)} className="sm:col-span-2">
              <option value="">Seleccionar...</option>
              {miembros.map((m) => (
                <option key={m.id} value={m.id}>{m.nombreCompleto}</option>
              ))}
            </Select>
            <Input label="Título" value={titulo} onChange={(e) => setTitulo(e.target.value)} className="sm:col-span-2" required />
            <Input label="Fecha inicio" type="date" value={fechaInicio} onChange={(e) => setFechaInicio(e.target.value)} required />
            <Input label="Fecha fin" type="date" value={fechaFin} onChange={(e) => setFechaFin(e.target.value)} min={fechaInicio} required />
            <Select label="Prioridad" value={prioridad} onChange={(e) => setPrioridad(e.target.value)}>
              <option value="Alta">Alta</option>
              <option value="Media">Media</option>
              <option value="Baja">Baja</option>
            </Select>
            <Textarea label="Descripción" value={descripcion} onChange={(e) => setDescripcion(e.target.value)} rows={3} className="sm:col-span-2" />
          </div>
          {error && <Alert variant="error" className="mt-3">{error}</Alert>}
          <Button type="submit" disabled={loading || !colaboradorId || !titulo} className="mt-4">
            Crear plan
          </Button>
        </form>
      )}

      {planes.length === 0 ? (
        <EmptyState message="No hay planes de acción registrados." />
      ) : (
        <div className="space-y-3">
          {planes.map((p) => (
            <div key={p.id} className="rounded-2xl border border-urrea-border/80 bg-urrea-bg p-4">
              <div className="flex flex-wrap items-start justify-between gap-2">
                <div>
                  <p className="font-semibold text-urrea-text">{p.titulo}</p>
                  <p className="text-sm text-urrea-text-muted">{p.colaboradorNombre}</p>
                </div>
                <span className={cn("rounded-full px-2.5 py-0.5 text-xs font-medium", PLAN_ESTADO_COLORS[p.estado] ?? PLAN_ESTADO_COLORS.Pendiente)}>
                  {PLAN_ESTADO_LABELS[p.estado] ?? p.estado}
                </span>
              </div>
              {p.descripcion && <p className="mt-2 text-sm text-urrea-text-muted">{p.descripcion}</p>}
              <div className="mt-3 flex flex-wrap gap-3 text-xs text-urrea-text-muted">
                <span>{new Date(p.fechaInicio).toLocaleDateString("es-MX")} – {new Date(p.fechaFin).toLocaleDateString("es-MX")}</span>
                <span>Prioridad: {p.prioridad}</span>
              </div>
              <div className="mt-3">
                <div className="mb-1 flex justify-between text-xs">
                  <span className="text-urrea-text-muted">Avance</span>
                  <span className="font-semibold text-urrea-primary">{p.avance}%</span>
                </div>
                <div className="h-2 overflow-hidden rounded-full bg-urrea-chrome/30">
                  <div className="h-full rounded-full bg-urrea-secondary transition-all" style={{ width: `${p.avance}%` }} />
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
