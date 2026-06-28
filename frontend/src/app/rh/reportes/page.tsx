"use client";

import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { PageContainer, PageHeader } from "@/components/ui/page-header";
import { rhService } from "@/lib/services/teamService";
import type { ReporteAusencia } from "@/lib/types";

export default function RhReportesPage() {
  const [reporte, setReporte] = useState<ReporteAusencia[]>([]);
  const [desde, setDesde] = useState("");
  const [hasta, setHasta] = useState("");

  function load() {
    const params = new URLSearchParams();
    if (desde) params.set("desde", desde);
    if (hasta) params.set("hasta", hasta);
    rhService.getAbsenceReport(params)
      .then(setReporte)
      .catch(console.error);
  }

  useEffect(() => { load(); }, []);

  async function exportCsv() {
    const params = new URLSearchParams({ format: "csv" });
    if (desde) params.set("desde", desde);
    if (hasta) params.set("hasta", hasta);
    const blob = await rhService.downloadAbsenceReportCsv(params);
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = "ausencias.csv";
    a.click();
    URL.revokeObjectURL(url);
  }

  return (
    <PageContainer>
      <PageHeader
        title="Reporte de ausencias"
        action={
          <Button type="button" onClick={exportCsv} className="w-full sm:w-auto">
            Exportar CSV
          </Button>
        }
      />

      <div className="flex flex-col gap-3 sm:flex-row sm:flex-wrap sm:items-end">
        <Input label="Desde" type="date" value={desde} onChange={(e) => setDesde(e.target.value)} className="min-w-[140px] flex-1" />
        <Input label="Hasta" type="date" value={hasta} onChange={(e) => setHasta(e.target.value)} className="min-w-[140px] flex-1" />
        <Button type="button" variant="secondary" onClick={load} className="w-full sm:w-auto">Filtrar</Button>
      </div>

      <Card>
        <ul className="space-y-3 lg:hidden">
          {reporte.map((r, i) => (
            <li key={i} className="rounded-xl border border-urrea-border p-3 text-sm">
              <p className="font-medium text-urrea-text">{r.colaborador}</p>
              <p className="text-urrea-text-muted">{r.tipoAusencia}</p>
              <p className="mt-1 text-urrea-text-muted">
                {new Date(r.fechaInicio).toLocaleDateString("es-MX")} – {new Date(r.fechaFin).toLocaleDateString("es-MX")} · {r.dias} d
              </p>
              <div className="mt-2"><Badge estado={r.estado} /></div>
            </li>
          ))}
        </ul>
        <div className="hidden overflow-x-auto lg:block">
          <table className="w-full min-w-[640px] text-left text-sm">
            <thead>
              <tr className="border-b border-urrea-border text-urrea-text-muted">
                <th className="pb-2 pr-3 font-medium">Empleado</th>
                <th className="pb-2 pr-3 font-medium">Tipo</th>
                <th className="pb-2 pr-3 font-medium">Periodo</th>
                <th className="pb-2 pr-3 font-medium">Días</th>
                <th className="pb-2 font-medium">Estado</th>
              </tr>
            </thead>
            <tbody>
              {reporte.map((r, i) => (
                <tr key={i} className="border-b border-urrea-border/60">
                  <td className="py-2 pr-3">{r.colaborador}</td>
                  <td className="py-2 pr-3">{r.tipoAusencia}</td>
                  <td className="py-2 pr-3 text-urrea-text-muted">
                    {new Date(r.fechaInicio).toLocaleDateString("es-MX")} – {new Date(r.fechaFin).toLocaleDateString("es-MX")}
                  </td>
                  <td className="py-2 pr-3">{r.dias}</td>
                  <td className="py-2"><Badge estado={r.estado} /></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </Card>
    </PageContainer>
  );
}
