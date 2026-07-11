"use client";

import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card } from "@/components/ui/card";
import { Textarea } from "@/components/ui/input";
import { Alert, PageContainer, PageHeader } from "@/components/ui/page-header";
import { AprobacionStepper } from "@/components/vacaciones/AprobacionStepper";
import { absenceService } from "@/lib/services/absenceService";
import type { PendingApproval } from "@/lib/types";

const NIVEL_LABELS: Record<string, string> = {
  Jefe: "Nivel: Jefe directo",
  DH: "Nivel: Desarrollo Humano",
  Nominas: "Nivel: Nóminas",
};

export default function AprobacionesPage() {
  const [pendientes, setPendientes] = useState<PendingApproval[]>([]);
  const [comentario, setComentario] = useState<Record<string, string>>({});
  const [error, setError] = useState("");

  function load() {
    absenceService.getPendingApprovals().then(setPendientes).catch(console.error);
  }

  useEffect(() => { load(); }, []);

  async function actuar(id: string, accion: "aprobar" | "rechazar") {
    setError("");
    try {
      if (accion === "aprobar") {
        await absenceService.approveRequest(id, comentario[id] ?? null);
      } else {
        await absenceService.rejectRequest(id, comentario[id] ?? null);
      }
      load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al procesar");
    }
  }

  return (
    <PageContainer>
      <PageHeader title="Aprobaciones pendientes" subtitle="Solicitudes de tu equipo" />
      {error && <Alert variant="error">{error}</Alert>}

      {pendientes.length === 0 ? (
        <Card><p className="text-sm text-urrea-text-muted">No hay solicitudes pendientes.</p></Card>
      ) : (
        pendientes.map((s) => (
          <Card key={s.id}>
            <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
              <div>
                <p className="font-semibold text-urrea-text">{s.colaboradorNombre}</p>
                <p className="text-sm text-urrea-text-muted">
                  {s.tipoAusenciaNombre} · {s.diasSolicitados} días{s.puesto ? ` · ${s.puesto}` : ""}
                </p>
                <p className="mt-1 text-sm text-urrea-text-muted">
                  {new Date(s.fechaInicio).toLocaleDateString("es-MX")} – {new Date(s.fechaFin).toLocaleDateString("es-MX")}
                </p>
                {s.comentario && <p className="mt-2 text-sm italic text-urrea-text-muted">&quot;{s.comentario}&quot;</p>}
              </div>
              <div className="flex flex-col items-end gap-1">
                <Badge estado={s.estado} />
                {s.nivelActual && (
                  <span className="text-[11px] font-medium text-urrea-text-muted">{NIVEL_LABELS[s.nivelActual] ?? s.nivelActual}</span>
                )}
              </div>
            </div>
            {s.pasosAprobacion && s.pasosAprobacion.length > 1 && (
              <div className="mt-3">
                <AprobacionStepper pasos={s.pasosAprobacion} compact />
              </div>
            )}
            <Textarea
              placeholder="Comentario (opcional)"
              value={comentario[s.id] ?? ""}
              onChange={(e) => setComentario({ ...comentario, [s.id]: e.target.value })}
              rows={2}
              className="mt-4"
            />
            <div className="mt-3 flex flex-col gap-2 sm:flex-row">
              <Button type="button" onClick={() => actuar(s.id, "aprobar")} className="flex-1 sm:flex-none">Aprobar</Button>
              <Button type="button" variant="danger" onClick={() => actuar(s.id, "rechazar")} className="flex-1 sm:flex-none">Rechazar</Button>
            </div>
          </Card>
        ))
      )}
    </PageContainer>
  );
}
