"use client";

import { useEffect, useState } from "react";
import { Badge } from "@/components/ui/badge";
import { Card } from "@/components/ui/card";
import { EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { rhService } from "@/lib/services/teamService";
import type { ReporteAusencia } from "@/lib/types";

export default function RhPermisosSolicitudesPage() {
  const [reporte, setReporte] = useState<ReporteAusencia[]>([]);

  useEffect(() => {
    rhService.getAbsenceReport(new URLSearchParams({ format: "json" }))
      .then((data) => setReporte(data.filter((r) => r.tipoAusencia !== "Vacaciones")))
      .catch(console.error);
  }, []);

  return (
    <PageContainer>
      <PageHeader title="Solicitudes de permisos" subtitle="Todas las ausencias excepto vacaciones" />

      <Card>
        {reporte.length === 0 ? (
          <EmptyState message="No hay solicitudes de permisos registradas." />
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full min-w-[640px] text-left text-sm">
              <thead>
                <tr className="border-b border-urrea-border text-urrea-text-muted">
                  <th className="pb-2 pr-4 font-medium">Colaborador</th>
                  <th className="pb-2 pr-4 font-medium">Tipo</th>
                  <th className="pb-2 pr-4 font-medium">Periodo</th>
                  <th className="pb-2 pr-4 font-medium">Días</th>
                  <th className="pb-2 font-medium">Estado</th>
                </tr>
              </thead>
              <tbody>
                {reporte.map((r, i) => (
                  <tr key={`${r.numeroEmpleado}-${r.fechaInicio}-${i}`} className="border-b border-urrea-border/50">
                    <td className="py-3 pr-4">
                      <p className="font-medium text-urrea-text">{r.colaborador}</p>
                      <p className="text-xs text-urrea-text-muted">{r.numeroEmpleado}</p>
                    </td>
                    <td className="py-3 pr-4">{r.tipoAusencia}</td>
                    <td className="py-3 pr-4 text-urrea-text-muted">
                      {new Date(r.fechaInicio).toLocaleDateString("es-MX")} –{" "}
                      {new Date(r.fechaFin).toLocaleDateString("es-MX")}
                    </td>
                    <td className="py-3 pr-4">{r.dias}</td>
                    <td className="py-3"><Badge estado={r.estado} /></td>
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
