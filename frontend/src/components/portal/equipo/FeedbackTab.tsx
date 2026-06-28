"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input, Select, Textarea } from "@/components/ui/input";
import { Alert, EmptyState } from "@/components/ui/page-header";
import { teamService } from "@/lib/services/teamService";
import type { EquipoMiembro, FeedbackEquipo } from "@/lib/types";
import { cn } from "@/lib/utils";

const TIPO_COLORS: Record<string, string> = {
  Reconocimiento: "bg-emerald-100 text-emerald-800",
  Constructivo: "bg-sky-100 text-sky-800",
  Positivo: "bg-teal-100 text-teal-800",
  Mejora: "bg-urrea-accent-sand/40 text-urrea-primary",
};

export function FeedbackTab({
  miembros,
  feedbacks,
  onRefresh,
}: {
  miembros: EquipoMiembro[];
  feedbacks: FeedbackEquipo[];
  onRefresh: () => void;
}) {
  const [showForm, setShowForm] = useState(false);
  const [colaboradorId, setColaboradorId] = useState("");
  const [tipo, setTipo] = useState("Constructivo");
  const [comentario, setComentario] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      await teamService.createFeedback({ colaboradorId, tipo, comentario });
      setShowForm(false);
      setComentario("");
      setColaboradorId("");
      onRefresh();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al enviar feedback");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <p className="text-sm text-urrea-text-muted">
          Registra reconocimientos y retroalimentación constructiva para el desarrollo de tu equipo.
        </p>
        <Button type="button" onClick={() => setShowForm((v) => !v)} className="w-full sm:w-auto">
          {showForm ? "Cancelar" : "Dar feedback"}
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
            <Select label="Tipo" value={tipo} onChange={(e) => setTipo(e.target.value)}>
              <option value="Reconocimiento">Reconocimiento</option>
              <option value="Constructivo">Constructivo</option>
              <option value="Positivo">Positivo</option>
              <option value="Mejora">Mejora</option>
            </Select>
            <Textarea label="Comentario" value={comentario} onChange={(e) => setComentario(e.target.value)} rows={4} className="sm:col-span-2" required />
          </div>
          {error && <Alert variant="error" className="mt-3">{error}</Alert>}
          <Button type="submit" disabled={loading || !colaboradorId || !comentario} className="mt-4">
            Enviar feedback
          </Button>
        </form>
      )}

      {feedbacks.length === 0 ? (
        <EmptyState message="Aún no has registrado feedback para tu equipo." />
      ) : (
        <ul className="space-y-3">
          {feedbacks.map((f) => (
            <li key={f.id} className="rounded-2xl border border-urrea-border/80 bg-urrea-bg p-4">
              <div className="flex flex-wrap items-center gap-2">
                <p className="font-semibold text-urrea-text">{f.colaboradorNombre}</p>
                <span className={cn("rounded-full px-2 py-0.5 text-[10px] font-semibold uppercase", TIPO_COLORS[f.tipo] ?? TIPO_COLORS.Constructivo)}>
                  {f.tipo}
                </span>
              </div>
              <p className="mt-2 text-sm leading-relaxed text-urrea-text">{f.comentario}</p>
              <p className="mt-2 text-xs text-urrea-text-muted">
                {f.autorNombre} · {new Date(f.fecha).toLocaleDateString("es-MX", { day: "numeric", month: "short", year: "numeric" })}
              </p>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
