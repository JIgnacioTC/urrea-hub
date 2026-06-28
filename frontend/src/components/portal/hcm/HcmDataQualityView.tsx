"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { DhEnterpriseHeader } from "@/components/dh/shared/page-frame";
import { DhCard } from "@/components/dh/shared/ui";
import { HcmEmployeeLink, HcmKpiStrip, formatNum } from "@/components/portal/hcm/shared";
import { employeeService, integrationService } from "@/lib/services/employeeService";
import type { HcmDataQualityReport } from "@/lib/types/hcm";

export function HcmDataQualityView() {
  const [report, setReport] = useState<HcmDataQualityReport | null>(null);
  const [syncing, setSyncing] = useState(false);

  const load = () => {
    employeeService.getDataQualityReport().then(setReport).catch(console.error);
  };

  useEffect(() => { load(); }, []);

  const handleNominaSync = async () => {
    setSyncing(true);
    try {
      await integrationService.runNominaSync();
      load();
    } finally {
      setSyncing(false);
    }
  };

  const s = report?.summary;

  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="analytics"
        title="Panel de calidad de datos"
        subtitle="Indicadores de integridad del directorio maestro. Identifica registros incompletos, conflictos de sync y pendientes SAP/CDM."
        breadcrumbs={[
          { label: "Portal", href: "/portal" },
          { label: "Núcleo HCM", href: "/portal/hcm" },
          { label: "Calidad de datos" },
        ]}
        action={
          <div className="flex gap-2">
            <Button type="button" variant="secondary" className="text-xs" disabled={syncing} onClick={handleNominaSync}>
              {syncing ? "Ejecutando…" : "Ejecutar sync nómina"}
            </Button>
            <Link href="/portal/hcm/personas">
              <Button type="button" variant="secondary" className="text-xs">Directorio</Button>
            </Link>
          </div>
        }
      />

      {s && (
        <HcmKpiStrip items={[
          { label: "Plantilla activa", value: formatNum(s.totalActive) },
          { label: "Sin jefe", value: String(s.withoutManager) },
          { label: "RFC faltante", value: String(s.missingRfc) },
          { label: "Pendientes sync", value: String(s.pendingSync) },
        ]} />
      )}

      <div className="mt-6 space-y-4">
        {report?.issues.map((issue) => (
          <DhCard key={issue.code}>
            <div className="flex flex-wrap items-start justify-between gap-3">
              <div>
                <h2 className="text-sm font-semibold text-urrea-text">{issue.label}</h2>
                <p className="mt-1 text-2xl font-semibold tabular-nums text-urrea-primary">{issue.count}</p>
              </div>
              <Link href={`/portal/hcm/personas?issue=${issue.code}`} className="text-xs font-medium text-urrea-secondary hover:underline">
                Ver en directorio →
              </Link>
            </div>
            {issue.samples.length > 0 && (
              <ul className="mt-4 space-y-2 border-t border-urrea-border/60 pt-4 text-sm">
                {issue.samples.map((sample) => (
                  <li key={sample.id} className="flex justify-between gap-4">
                    <HcmEmployeeLink id={sample.id} name={sample.fullName} number={sample.employeeNumber} />
                    <span className="text-xs text-urrea-text-muted">{sample.position}</span>
                  </li>
                ))}
              </ul>
            )}
          </DhCard>
        ))}
        {report && report.issues.length === 0 && (
          <DhCard>
            <p className="text-sm text-urrea-text">No se detectaron incidencias de calidad en la plantilla activa.</p>
          </DhCard>
        )}
      </div>
    </div>
  );
}
