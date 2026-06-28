"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import { ButtonLink } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, StatCard } from "@/components/ui/card";
import { Alert, PageContainer, PageHeader } from "@/components/ui/page-header";
import { absenceService } from "@/lib/services/absenceService";
import type { SaldoVacaciones, SolicitudAusencia } from "@/lib/types";

export function VacacionesHubView() {
  const [saldo, setSaldo] = useState<SaldoVacaciones | null>(null);
  const [solicitudes, setSolicitudes] = useState<SolicitudAusencia[]>([]);
  const [error, setError] = useState("");

  function load() {
    absenceService.getMyBalance().then(setSaldo).catch(console.error);
    absenceService.getMyRequests().then(setSolicitudes).catch(console.error);
  }

  useEffect(() => { load(); }, []);

  const vacaciones = useMemo(
    () => solicitudes.filter((s) => s.tipoAusenciaCodigo === "VAC"),
    [solicitudes],
  );

  const pendientes = vacaciones.filter((s) => s.estado === "Pendiente");
  const proximas = vacaciones.filter(
    (s) => s.estado === "Aprobada" && new Date(s.fechaInicio) >= new Date(),
  );

  async function cancelar(id: string) {
    try {
      await absenceService.cancelRequest(id);
      load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al cancelar");
    }
  }

  return (
    <PageContainer>
      <PageHeader
        title="Mis vacaciones y permisos"
        subtitle="Consulta saldo, crea solicitudes y revisa tu historial."
        action={
          <ButtonLink href="/portal/vacaciones/nueva" className="w-full sm:w-auto">
            Nueva solicitud
          </ButtonLink>
        }
      />

      {error && <Alert variant="error">{error}</Alert>}

      <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard label="Disponible" value={String(saldo?.diasDisponibles ?? saldo?.diasPendientes ?? "—")} accentClass="text-emerald-700" />
        <StatCard label="Asignados" value={String(saldo?.diasAsignados ?? "—")} accentClass="text-urrea-primary" />
        <StatCard label="Usados" value={String(saldo?.diasUsados ?? "—")} accentClass="text-urrea-secondary" />
        <StatCard label="Pendientes aprobación" value={String(saldo?.diasComprometidos ?? pendientes.reduce((a, s) => a + s.diasSolicitados, 0))} accentClass="text-amber-700" />
      </div>

      <div className="grid gap-4 lg:grid-cols-2">
        <Card title="Próximas ausencias">
          {proximas.length === 0 ? (
            <p className="text-sm text-urrea-text-muted">No tienes vacaciones aprobadas próximas.</p>
          ) : (
            <ul className="space-y-2">
              {proximas.slice(0, 5).map((s) => (
                <li key={s.id} className="text-sm">
                  {new Date(s.fechaInicio).toLocaleDateString("es-MX")} – {new Date(s.fechaFin).toLocaleDateString("es-MX")} · {s.diasSolicitados} días
                </li>
              ))}
            </ul>
          )}
        </Card>

        <Card title="Enlaces">
          <div className="flex flex-col gap-2 text-sm">
            <Link href="/portal/vacaciones/calendario" className="text-urrea-primary hover:underline">Calendario personal</Link>
            <Link href="/portal/permisos" className="text-urrea-primary hover:underline">Permisos y licencias LFT</Link>
            <Link href="/portal/equipo/vacaciones/pendientes" className="text-urrea-primary hover:underline">Aprobaciones de equipo</Link>
          </div>
        </Card>
      </div>

      <Card title="Historial de solicitudes">
        {vacaciones.length === 0 ? (
          <p className="text-sm text-urrea-text-muted">Sin solicitudes registradas.</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full text-left text-sm">
              <thead>
                <tr className="border-b text-urrea-text-muted">
                  <th className="px-2 py-2">Periodo</th>
                  <th className="px-2 py-2">Días</th>
                  <th className="px-2 py-2">Estado</th>
                  <th className="px-2 py-2" />
                </tr>
              </thead>
              <tbody>
                {vacaciones.map((s) => (
                  <tr key={s.id} className="border-b border-urrea-border/60">
                    <td className="px-2 py-2">
                      {new Date(s.fechaInicio).toLocaleDateString("es-MX")} – {new Date(s.fechaFin).toLocaleDateString("es-MX")}
                    </td>
                    <td className="px-2 py-2">{s.diasSolicitados}</td>
                    <td className="px-2 py-2"><Badge estado={s.estado} /></td>
                    <td className="px-2 py-2">
                      {s.estado === "Pendiente" && (
                        <button type="button" onClick={() => cancelar(s.id)} className="text-red-600 hover:underline">
                          Cancelar
                        </button>
                      )}
                    </td>
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
