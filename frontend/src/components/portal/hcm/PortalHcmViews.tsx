"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { DhEnterpriseHeader } from "@/components/dh/shared/page-frame";
import { DhCard } from "@/components/dh/shared/ui";
import { HcmKpiStrip, formatNum } from "@/components/portal/hcm/shared";
import { employeeService } from "@/lib/services/employeeService";
import type { HcmDashboard, HcmDataQuality } from "@/lib/types/hcm";
import type { OrganigramaNodo } from "@/lib/types";
import { HcmEmployeeListView } from "@/components/portal/hcm/HcmEmployeeList";
import { HcmEmployeeDetailView } from "@/components/portal/hcm/HcmEmployeeDetail";
import { HcmDataQualityView } from "@/components/portal/hcm/HcmDataQualityView";
import { cn } from "@/lib/utils";

export { HcmEmployeeListView, HcmEmployeeDetailView, HcmDataQualityView };

export function PortalHcmDashboardView() {
  const [dashboard, setDashboard] = useState<HcmDashboard | null>(null);
  const [quality, setQuality] = useState<HcmDataQuality | null>(null);

  useEffect(() => {
    employeeService.getDashboard().then(setDashboard).catch(console.error);
    employeeService.getDataQuality().then(setQuality).catch(console.error);
  }, []);

  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="dashboard"
        title="Centro HCM"
        subtitle="Núcleo de gestión de capital humano — directorio maestro, calidad de datos e integración SAP/CDM."
        breadcrumbs={[{ label: "Portal", href: "/portal" }, { label: "Núcleo HCM" }]}
        action={
          <div className="flex gap-2">
            <Link href="/portal/hcm/calidad-datos"><Button type="button" variant="secondary" className="text-xs">Calidad de datos</Button></Link>
            <Link href="/portal/hcm/personas"><Button type="button" variant="secondary" className="text-xs">Gestión de personas</Button></Link>
          </div>
        }
      />

      {dashboard && (
        <HcmKpiStrip items={[
          { label: "Plantilla activa", value: formatNum(dashboard.activeEmployees) },
          { label: "Centros de costo", value: String(dashboard.costCenters) },
          { label: "Unidades org.", value: String(dashboard.organizationUnits) },
          { label: "Sedes", value: String(dashboard.locations) },
        ]} />
      )}

      {quality && (
        <div className="mt-6 grid gap-4 lg:grid-cols-2">
          <DhCard>
            <h2 className="text-sm font-semibold">Calidad de datos</h2>
            <ul className="mt-3 space-y-1 text-sm text-urrea-text-muted">
              <li>Sin jefe: {quality.withoutManager}</li>
              <li>RFC faltante: {quality.missingRfc}</li>
              <li>Pendientes sync: {quality.pendingSync}</li>
            </ul>
            <Link href="/portal/hcm/calidad-datos" className="mt-3 inline-block text-xs font-medium text-urrea-primary hover:underline">Ver panel completo →</Link>
          </DhCard>
          <DhCard>
            <h2 className="text-sm font-semibold">Accesos</h2>
            <div className="mt-3 grid gap-2">
              <Link href="/portal/hcm/personas" className="border border-urrea-border px-3 py-2 text-sm text-urrea-primary hover:bg-slate-50">Directorio de personas</Link>
              <Link href="/portal/hcm/organigrama" className="border border-urrea-border px-3 py-2 text-sm text-urrea-primary hover:bg-slate-50">Organigrama</Link>
            </div>
          </DhCard>
        </div>
      )}
    </div>
  );
}

function OrganigramaNode({ node, depth = 0 }: { node: OrganigramaNodo; depth?: number }) {
  return (
    <div className={cn(depth > 0 && "ml-4 border-l border-urrea-border pl-4")}>
      <div className="mb-2 flex flex-wrap items-center gap-2 py-1">
        <Link href={`/portal/hcm/personas/${node.id}`} className="text-sm font-medium text-urrea-primary hover:underline">{node.nombreCompleto}</Link>
        <span className="text-xs text-urrea-text-muted">{node.puesto}</span>
      </div>
      {node.subordinados.map((child) => <OrganigramaNode key={child.id} node={child} depth={depth + 1} />)}
    </div>
  );
}

export function PortalHcmOrganigramaView() {
  const [arbol, setArbol] = useState<OrganigramaNodo | null>(null);
  useEffect(() => { employeeService.getOrganigrama().then(setArbol).catch(console.error); }, []);

  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="chart"
        title="Organigrama corporativo"
        subtitle="Estructura jerárquica en tiempo real desde Core RH."
        breadcrumbs={[{ label: "Portal", href: "/portal" }, { label: "Núcleo HCM", href: "/portal/hcm" }, { label: "Organigrama" }]}
      />
      <DhCard>{!arbol ? <p className="text-sm text-urrea-text-muted">Cargando…</p> : <OrganigramaNode node={arbol} />}</DhCard>
    </div>
  );
}

export function PortalHcmListView() {
  return <HcmEmployeeListView />;
}

export function PortalHcmDetailView({ id }: { id: string }) {
  return <HcmEmployeeDetailView id={id} />;
}
