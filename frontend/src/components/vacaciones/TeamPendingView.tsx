"use client";

import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card } from "@/components/ui/card";
import { Textarea } from "@/components/ui/input";
import { Alert, PageContainer, PageHeader } from "@/components/ui/page-header";
import { absenceService } from "@/lib/services/absenceService";
import type { PendingApproval } from "@/lib/types";

export function TeamPendingView() {
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
      if (accion === "aprobar") await absenceService.approveRequest(id, comentario[id] ?? null);
      else await absenceService.rejectRequest(id, comentario[id] ?? null);
      load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al procesar");
    }
  }

  return (
    <PageContainer>
      <PageHeader title="Solicitudes pendientes del equipo" subtitle="Revisa saldo, traslapes y aprueba con contexto." />
      {error && <Alert variant="error">{error}</Alert>}

      {pendientes.length === 0 ? (
        <Card><p className="text-sm text-urrea-text-muted">No hay solicitudes pendientes.</p></Card>
      ) : (
        pendientes.map((s) => (
          <Card key={s.id}>
            <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
              <div>
                <p className="font-semibold">{s.colaboradorNombre}</p>
                <p className="text-sm text-urrea-text-muted">{s.puesto} · {s.departamento}</p>
                <p className="text-sm">{s.tipoAusenciaNombre} · {s.diasSolicitados} días</p>
                <p className="text-sm text-urrea-text-muted">
                  {new Date(s.fechaInicio).toLocaleDateString("es-MX")} – {new Date(s.fechaFin).toLocaleDateString("es-MX")}
                </p>
                {s.saldoDisponible != null && (
                  <p className="mt-1 text-sm">Saldo: {s.saldoDisponible} → {s.saldoPosterior} días</p>
                )}
                {s.traslapesEquipo.length > 0 && (
                  <Alert variant="info" className="mt-2">
                    Traslapes en equipo: {s.traslapesEquipo.join(", ")}
                  </Alert>
                )}
              </div>
              <Badge estado={s.estado} />
            </div>
            <Textarea
              placeholder="Comentario (opcional)"
              value={comentario[s.id] ?? ""}
              onChange={(e) => setComentario({ ...comentario, [s.id]: e.target.value })}
              rows={2}
              className="mt-4"
            />
            <div className="mt-3 flex gap-2">
              <Button type="button" onClick={() => actuar(s.id, "aprobar")}>Aprobar</Button>
              <Button type="button" variant="danger" onClick={() => actuar(s.id, "rechazar")}>Rechazar</Button>
            </div>
          </Card>
        ))
      )}
    </PageContainer>
  );
}
