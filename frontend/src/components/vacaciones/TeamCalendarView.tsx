"use client";

import { useEffect, useMemo, useState } from "react";
import { Card } from "@/components/ui/card";
import { PageContainer, PageHeader } from "@/components/ui/page-header";
import { absenceService } from "@/lib/services/absenceService";
import type { TeamCalendar } from "@/lib/types";

function monthRange(offset = 0) {
  const now = new Date();
  const start = new Date(now.getFullYear(), now.getMonth() + offset, 1);
  const end = new Date(now.getFullYear(), now.getMonth() + offset + 1, 0);
  return { start, end };
}

export function TeamCalendarView() {
  const [data, setData] = useState<TeamCalendar | null>(null);
  const range = useMemo(() => monthRange(), []);

  useEffect(() => {
    absenceService.getTeamCalendar(range.start, range.end).then(setData).catch(console.error);
  }, [range.end, range.start]);

  return (
    <PageContainer>
      <PageHeader title="Calendario de equipo" subtitle="Ausencias aprobadas, pendientes y días inhábiles." />

      <Card title={`${range.start.toLocaleDateString("es-MX", { month: "long", year: "numeric" })}`}>
        {data?.diasInhabiles.length ? (
          <p className="mb-3 text-sm text-urrea-text-muted">
            Días inhábiles: {data.diasInhabiles.map((d) => new Date(d.fecha).toLocaleDateString("es-MX")).join(", ")}
          </p>
        ) : null}

        {!data?.ausencias.length ? (
          <p className="text-sm text-urrea-text-muted">Sin ausencias en el periodo.</p>
        ) : (
          <ul className="space-y-2">
            {data.ausencias.map((a) => (
              <li key={a.solicitudId} className="flex items-center gap-3 rounded-lg border border-urrea-border p-3 text-sm">
                <span className="h-3 w-3 rounded-full" style={{ backgroundColor: a.color }} />
                <div>
                  <p className="font-medium">{a.colaboradorNombre}</p>
                  <p className="text-urrea-text-muted">
                    {a.tipoAusencia} · {new Date(a.fechaInicio).toLocaleDateString("es-MX")} – {new Date(a.fechaFin).toLocaleDateString("es-MX")} · {a.estado}
                  </p>
                </div>
              </li>
            ))}
          </ul>
        )}
      </Card>
    </PageContainer>
  );
}
