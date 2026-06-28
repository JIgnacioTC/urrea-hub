"use client";

import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Alert, PageContainer, PageHeader } from "@/components/ui/page-header";
import { attendanceService, type CorreccionAsistencia } from "@/lib/services/attendanceService";

export function TeamAsistenciaPendientesView() {
  const [items, setItems] = useState<CorreccionAsistencia[]>([]);
  const [error, setError] = useState("");

  function load() {
    attendanceService.getPendingCorrections().then(setItems).catch(console.error);
  }

  useEffect(() => { load(); }, []);

  async function actuar(id: string, accion: "aprobar" | "rechazar") {
    setError("");
    try {
      if (accion === "aprobar") await attendanceService.approveCorrection(id);
      else await attendanceService.rejectCorrection(id);
      load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al procesar");
    }
  }

  return (
    <PageContainer>
      <PageHeader title="Validación de asistencia" subtitle="Correcciones pendientes de tu equipo." />
      {error && <Alert variant="error">{error}</Alert>}
      {items.length === 0 ? (
        <Card><p className="text-sm text-urrea-text-muted">No hay correcciones pendientes.</p></Card>
      ) : (
        items.map((c) => (
          <Card key={c.id}>
            <p className="font-semibold">{c.solicitanteNombre}</p>
            <p className="text-sm text-urrea-text-muted">{new Date(c.fecha).toLocaleDateString("es-MX")} · {c.tipoCorreccion}</p>
            <p className="mt-2 text-sm">Original: entrada {c.registroOriginalEntrada ?? "—"} · salida {c.registroOriginalSalida ?? "—"}</p>
            <p className="text-sm">Solicitado: entrada {c.horaEntradaSolicitada ? new Date(c.horaEntradaSolicitada).toLocaleTimeString("es-MX", { hour: "2-digit", minute: "2-digit" }) : "—"} · salida {c.horaSalidaSolicitada ? new Date(c.horaSalidaSolicitada).toLocaleTimeString("es-MX", { hour: "2-digit", minute: "2-digit" }) : "—"}</p>
            <p className="mt-2 text-sm italic">&quot;{c.motivo}&quot;</p>
            <div className="mt-3 flex gap-2">
              <Button type="button" onClick={() => actuar(c.id, "aprobar")}>Aprobar</Button>
              <Button type="button" variant="danger" onClick={() => actuar(c.id, "rechazar")}>Rechazar</Button>
            </div>
          </Card>
        ))
      )}
    </PageContainer>
  );
}

export function TeamAsistenciaView() {
  const [data, setData] = useState<Awaited<ReturnType<typeof attendanceService.getTeamSummary>> | null>(null);

  useEffect(() => {
    attendanceService.getTeamSummary().then(setData).catch(console.error);
  }, []);

  return (
    <PageContainer>
      <PageHeader title="Asistencia de mi equipo" subtitle="Vista operativa del día." />
      {data && (
        <>
          <div className="mb-4 grid gap-3 sm:grid-cols-2 lg:grid-cols-5">
            <Card><p className="text-xs text-urrea-text-muted">Presentes</p><p className="text-2xl font-semibold">{data.presentes}</p></Card>
            <Card><p className="text-xs text-urrea-text-muted">Ausentes</p><p className="text-2xl font-semibold">{data.ausentes}</p></Card>
            <Card><p className="text-xs text-urrea-text-muted">Retardos</p><p className="text-2xl font-semibold">{data.retardos}</p></Card>
            <Card><p className="text-xs text-urrea-text-muted">Salidas tempranas</p><p className="text-2xl font-semibold">{data.salidasTempranas}</p></Card>
            <Card><p className="text-xs text-urrea-text-muted">Correcciones</p><p className="text-2xl font-semibold">{data.correccionesPendientes}</p></Card>
          </div>
          <Card title="Registros de hoy">
            {data.registros.length === 0 ? (
              <p className="text-sm text-urrea-text-muted">Sin registros.</p>
            ) : (
              <ul className="space-y-2 text-sm">
                {data.registros.map((r) => (
                  <li key={r.id}>{r.estado} · {r.fuente} · {r.horaEntrada ? new Date(r.horaEntrada).toLocaleTimeString("es-MX") : "—"}</li>
                ))}
              </ul>
            )}
          </Card>
        </>
      )}
    </PageContainer>
  );
}
