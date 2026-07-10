"use client";

import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card } from "@/components/ui/card";
import { Textarea } from "@/components/ui/input";
import { Alert, PageContainer, PageHeader } from "@/components/ui/page-header";
import {
  attendanceService,
  type SolicitudCambioHorario,
} from "@/lib/services/attendanceService";

export function AprobacionesHorariosView() {
  const [requests, setRequests] = useState<SolicitudCambioHorario[]>([]);
  const [comentarios, setComentarios] = useState<Record<string, string>>({});
  const [loadingMap, setLoadingMap] = useState<Record<string, boolean>>({});
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const loadRequests = async () => {
    try {
      const data = await attendanceService.getPendingShiftRequests();
      setRequests(data);
    } catch (err) {
      console.error("Error al cargar las solicitudes pendientes:", err);
    }
  };

  useEffect(() => {
    loadRequests();
  }, []);

  const handleDecision = async (id: string, action: "approve" | "reject") => {
    setLoadingMap((prev) => ({ ...prev, [id]: true }));
    setError("");
    setSuccess("");

    try {
      const comment = comentarios[id] || "";
      if (action === "approve") {
        await attendanceService.approveShiftRequest(id, comment);
        setSuccess("Solicitud aprobada con éxito. El horario del colaborador ha sido actualizado.");
      } else {
        await attendanceService.rejectShiftRequest(id, comment);
        setSuccess("Solicitud rechazada con éxito.");
      }
      
      // Clean comment
      setComentarios((prev) => {
        const next = { ...prev };
        delete next[id];
        return next;
      });

      await loadRequests();
    } catch (err: any) {
      setError(err.message || "Error al procesar la solicitud.");
    } finally {
      setLoadingMap((prev) => ({ ...prev, [id]: false }));
    }
  };

  return (
    <PageContainer>
      <PageHeader
        title="Aprobar Cambios de Horario"
        subtitle="Revisa y gestiona las solicitudes de cambio de turno de tu equipo de colaboradores."
      />

      {error && <Alert variant="error">{error}</Alert>}
      {success && <Alert variant="success">{success}</Alert>}

      <div className="mt-4 space-y-4">
        {requests.length === 0 ? (
          <Card>
            <p className="text-sm text-urrea-text-muted">No hay solicitudes de cambio de horario pendientes por revisar.</p>
          </Card>
        ) : (
          requests.map((r) => (
            <Card key={r.id}>
              <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
                <div>
                  <p className="font-bold text-urrea-text text-base">{r.colaboradorNombre}</p>
                  <div className="mt-2 space-y-1">
                    <p className="text-sm text-urrea-text-muted">
                      <span className="font-semibold text-urrea-text">Horario Actual:</span> {r.turnoActualNombre}
                    </p>
                    <p className="text-sm text-urrea-text-muted">
                      <span className="font-semibold text-urrea-text">Horario Solicitado:</span> <span className="text-indigo-600 font-bold">{r.turnoSolicitadoNombre}</span>
                    </p>
                    <p className="text-xs text-urrea-text-muted">
                      Solicitado el {new Date(r.createdAt).toLocaleString("es-MX")}
                    </p>
                  </div>
                  
                  <div className="mt-3 text-sm text-urrea-text-muted bg-slate-50 p-3 rounded-xl border border-urrea-border/60">
                    <span className="font-bold text-urrea-text block text-xs mb-1">Motivo justificado:</span>
                    &quot;{r.motivo}&quot;
                  </div>
                </div>

                <div className="shrink-0">
                  <span className="px-2.5 py-1 text-xs rounded-full bg-amber-50 text-amber-700 border border-amber-200 font-medium">
                    {r.estado}
                  </span>
                </div>
              </div>

              <div className="mt-4">
                <Textarea
                  placeholder="Escribe un comentario o retroalimentación (opcional)..."
                  value={comentarios[r.id] ?? ""}
                  onChange={(e) => setComentarios({ ...comentarios, [r.id]: e.target.value })}
                  rows={2}
                />
              </div>

              <div className="mt-3 flex flex-col gap-2 sm:flex-row">
                <Button
                  type="button"
                  onClick={() => handleDecision(r.id, "approve")}
                  disabled={loadingMap[r.id]}
                  className="flex-1 sm:flex-none"
                >
                  {loadingMap[r.id] ? "Procesando..." : "Aprobar Cambio"}
                </Button>
                <Button
                  type="button"
                  variant="danger"
                  onClick={() => handleDecision(r.id, "reject")}
                  disabled={loadingMap[r.id]}
                  className="flex-1 sm:flex-none"
                >
                  {loadingMap[r.id] ? "Procesando..." : "Rechazar Solicitud"}
                </Button>
              </div>
            </Card>
          ))
        )}
      </div>
    </PageContainer>
  );
}
