"use client";

import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card } from "@/components/ui/card";
import { PageContainer, PageHeader } from "@/components/ui/page-header";
import { absenceService } from "@/lib/services/absenceService";
import type { CalendarioAusencia } from "@/lib/types";

const MESES = ["Enero","Febrero","Marzo","Abril","Mayo","Junio","Julio","Agosto","Septiembre","Octubre","Noviembre","Diciembre"];

export default function CalendarioPage() {
  const [ausencias, setAusencias] = useState<CalendarioAusencia[]>([]);
  const now = new Date();
  const [mes, setMes] = useState(now.getMonth());
  const [anio, setAnio] = useState(now.getFullYear());

  useEffect(() => {
    const desde = new Date(anio, mes, 1);
    const hasta = new Date(anio, mes + 1, 0);
    absenceService.getCalendar(desde, hasta).then(setAusencias).catch(console.error);
  }, [mes, anio]);

  return (
    <PageContainer>
      <PageHeader
        title="Calendario de ausencias"
        action={
          <div className="flex items-center gap-2">
            <Button type="button" variant="secondary" className="min-w-11 px-3" onClick={() => { if (mes === 0) { setMes(11); setAnio(anio - 1); } else setMes(mes - 1); }}>←</Button>
            <span className="min-w-[8rem] text-center text-sm font-medium text-urrea-text">{MESES[mes]} {anio}</span>
            <Button type="button" variant="secondary" className="min-w-11 px-3" onClick={() => { if (mes === 11) { setMes(0); setAnio(anio + 1); } else setMes(mes + 1); }}>→</Button>
          </div>
        }
      />

      <Card>
        {ausencias.length === 0 ? (
          <p className="text-sm text-urrea-text-muted">No hay ausencias registradas este mes.</p>
        ) : (
          <ul className="space-y-3">
            {ausencias.map((a) => (
              <li key={a.solicitudId} className="flex flex-col gap-2 rounded-xl border border-urrea-border p-3 sm:flex-row sm:items-center sm:gap-4">
                <div className="flex min-w-0 flex-1 items-center gap-3">
                  <div className="h-3 w-3 shrink-0 rounded-full" style={{ backgroundColor: a.color }} />
                  <div className="min-w-0">
                    <p className="truncate font-medium text-urrea-text">{a.colaboradorNombre}</p>
                    <p className="text-sm text-urrea-text-muted">{a.tipoAusencia}</p>
                  </div>
                </div>
                <div className="flex flex-wrap items-center gap-2 sm:justify-end">
                  <span className="text-sm text-urrea-text-muted">
                    {new Date(a.fechaInicio).toLocaleDateString("es-MX")} – {new Date(a.fechaFin).toLocaleDateString("es-MX")}
                  </span>
                  <Badge estado={a.estado} />
                </div>
              </li>
            ))}
          </ul>
        )}
      </Card>
    </PageContainer>
  );
}
