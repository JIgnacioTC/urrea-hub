"use client";

import Link from "next/link";
import { useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, StatCard } from "@/components/ui/card";
import { Alert, PageContainer, PageHeader } from "@/components/ui/page-header";
import { attendanceService, type AttendanceSummary } from "@/lib/services/attendanceService";

function fmtTime(iso?: string) {
  if (!iso) return "—";
  return new Date(iso).toLocaleTimeString("es-MX", { hour: "2-digit", minute: "2-digit" });
}

export function MiAsistenciaView() {
  const [summary, setSummary] = useState<AttendanceSummary | null>(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const load = useCallback(() => {
    attendanceService.getMySummary().then(setSummary).catch(console.error);
  }, []);

  useEffect(() => { load(); }, [load]);

  async function checkIn() {
    setError("");
    setLoading(true);
    try {
      await attendanceService.checkIn({ fuente: "AppMovil" });
      load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al registrar entrada");
    } finally {
      setLoading(false);
    }
  }

  async function checkOut() {
    setError("");
    setLoading(true);
    try {
      await attendanceService.checkOut({ fuente: "AppMovil" });
      load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al registrar salida");
    } finally {
      setLoading(false);
    }
  }

  const hoy = summary?.registroHoy;

  return (
    <PageContainer>
      <PageHeader
        title="Mi asistencia"
        subtitle="Registra entrada/salida y consulta tu historial."
        action={
          <Link href="/portal/asistencia/correccion" className="text-sm text-urrea-primary hover:underline">
            Solicitar corrección
          </Link>
        }
      />

      {error && <Alert variant="error">{error}</Alert>}

      <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard label="Retardos (mes)" value={String(summary?.retardosPeriodo ?? "—")} accentClass="text-amber-700" />
        <StatCard label="Ausencias (mes)" value={String(summary?.ausenciasPeriodo ?? "—")} accentClass="text-red-600" />
        <StatCard label="Correcciones pendientes" value={String(summary?.correccionesPendientes ?? "—")} accentClass="text-urrea-primary" />
        <StatCard label="Estado hoy" value={hoy?.estado ?? "Sin registro"} accentClass="text-emerald-700" />
      </div>

      <Card title="Registro de hoy">
        <div className="grid gap-4 sm:grid-cols-3">
          <div>
            <p className="text-xs text-urrea-text-muted">Entrada</p>
            <p className="text-2xl font-semibold tabular-nums">{fmtTime(hoy?.horaEntrada)}</p>
          </div>
          <div>
            <p className="text-xs text-urrea-text-muted">Salida</p>
            <p className="text-2xl font-semibold tabular-nums">{fmtTime(hoy?.horaSalida)}</p>
          </div>
          <div>
            <p className="text-xs text-urrea-text-muted">Fuente</p>
            <p className="text-sm">{hoy?.fuente ?? "—"}</p>
          </div>
        </div>
        <div className="mt-4 flex flex-wrap gap-2">
          <Button type="button" onClick={checkIn} disabled={loading || (!!hoy?.horaEntrada && !hoy?.horaSalida)}>
            Registrar entrada
          </Button>
          <Button type="button" variant="secondary" onClick={checkOut} disabled={loading || !hoy?.horaEntrada || !!hoy?.horaSalida}>
            Registrar salida
          </Button>
        </div>
      </Card>

      <Card title="Historial reciente">
        {!summary?.historialReciente.length ? (
          <p className="text-sm text-urrea-text-muted">Sin registros.</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full text-sm">
              <thead>
                <tr className="border-b text-urrea-text-muted">
                  <th className="py-2 pr-4 text-left">Fecha</th>
                  <th className="py-2 pr-4 text-left">Entrada</th>
                  <th className="py-2 pr-4 text-left">Salida</th>
                  <th className="py-2 text-left">Estado</th>
                </tr>
              </thead>
              <tbody>
                {summary.historialReciente.map((r) => (
                  <tr key={r.id} className="border-b border-urrea-border/60">
                    <td className="py-2 pr-4">{new Date(r.fecha).toLocaleDateString("es-MX")}</td>
                    <td className="py-2 pr-4">{fmtTime(r.horaEntrada)}</td>
                    <td className="py-2 pr-4">{fmtTime(r.horaSalida)}</td>
                    <td className="py-2">{r.estado}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </Card>
    </PageContainer>
  );
}
