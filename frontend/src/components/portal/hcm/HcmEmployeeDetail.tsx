"use client";

import Link from "next/link";
import { useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { DhEnterpriseHeader } from "@/components/dh/shared/page-frame";
import { DhBadge, DhCard, estatusTone } from "@/components/dh/shared/ui";
import {
  HcmAvatar,
  HcmFieldGrid,
  HcmKpiStrip,
  HcmModulePanel,
  HcmTimeline,
} from "@/components/portal/hcm/shared";
import { employeeService } from "@/lib/services/employeeService";
import type {
  EmployeeAuditLog,
  EmployeeDetail,
  EmployeeDocument,
  EmployeeModuleLink,
  EmployeeMovement,
  EmployeeOrganization,
  EmployeeVacationSummary,
} from "@/lib/types/hcm";
import { cn } from "@/lib/utils";

const TABS = [
  "Resumen",
  "Información personal",
  "Información laboral",
  "Organización",
  "Documentos",
  "Movimientos",
  "Vacaciones",
  "Asistencia",
  "Capacitación",
  "Desempeño",
  "Beneficios",
  "Bitácora",
  "Integración",
] as const;

type Tab = (typeof TABS)[number];

export function HcmEmployeeDetailView({ id }: { id: string }) {
  const [col, setCol] = useState<EmployeeDetail | null>(null);
  const [tab, setTab] = useState<Tab>("Resumen");
  const [org, setOrg] = useState<EmployeeOrganization | null>(null);
  const [movements, setMovements] = useState<EmployeeMovement[]>([]);
  const [audit, setAudit] = useState<EmployeeAuditLog[]>([]);
  const [vacation, setVacation] = useState<EmployeeVacationSummary | null>(null);
  const [documents, setDocuments] = useState<EmployeeDocument[]>([]);
  const [modules, setModules] = useState<EmployeeModuleLink[]>([]);
  const [syncing, setSyncing] = useState(false);

  const loadCore = useCallback(() => {
    employeeService.getEmployee(id).then(setCol).catch(console.error);
  }, [id]);

  useEffect(() => { loadCore(); }, [loadCore]);

  useEffect(() => {
    if (tab === "Organización" || tab === "Resumen") {
      employeeService.getOrganization(id).then(setOrg).catch(console.error);
    }
    if (tab === "Movimientos" || tab === "Resumen") {
      employeeService.getMovements(id).then(setMovements).catch(console.error);
    }
    if (tab === "Bitácora") {
      employeeService.getAuditLog(id).then(setAudit).catch(console.error);
    }
    if (tab === "Vacaciones" || tab === "Resumen") {
      employeeService.getVacationSummary(id).then(setVacation).catch(() => setVacation(null));
    }
    if (tab === "Documentos") {
      employeeService.getDocuments(id).then(setDocuments).catch(console.error);
    }
    if (tab === "Resumen" || ["Asistencia", "Capacitación", "Desempeño", "Beneficios"].includes(tab)) {
      employeeService.getModuleLinks(id).then(setModules).catch(console.error);
    }
  }, [id, tab]);

  const handleSync = async () => {
    setSyncing(true);
    try {
      await employeeService.syncEmployee(id);
      loadCore();
    } catch (e) {
      console.error(e);
    } finally {
      setSyncing(false);
    }
  };

  if (!col) {
    return <p className="py-16 text-center text-sm text-urrea-text-muted">Cargando expediente ejecutivo…</p>;
  }

  const nombreCompleto = `${col.legalFirstName} ${col.legalLastName}`;
  const mod = (key: string) => modules.find((m) => m.module === key);

  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="users"
        title={nombreCompleto}
        subtitle={`${col.position} · ${col.department} · No. ${col.employeeNumber}`}
        breadcrumbs={[
          { label: "Portal", href: "/portal" },
          { label: "Núcleo HCM", href: "/portal/hcm" },
          { label: "Personas", href: "/portal/hcm/personas" },
          { label: col.preferredName ?? nombreCompleto },
        ]}
        action={
          <div className="flex gap-2">
            <Button type="button" variant="secondary" className="text-xs" disabled={syncing || col.isManualOverride} onClick={handleSync}>
              {syncing ? "Sincronizando…" : "Forzar resincronización"}
            </Button>
            <Link href="/portal/hcm/personas" className="self-center text-sm font-medium text-urrea-secondary hover:underline">← Directorio</Link>
          </div>
        }
      />

      <div className="mb-5 flex flex-wrap items-center gap-3 border border-urrea-border/80 bg-slate-50 px-4 py-3">
        <HcmAvatar name={nombreCompleto} />
        <DhBadge tone={estatusTone(col.status)}>{col.status}</DhBadge>
        <DhBadge tone={col.syncStatus === "Synced" ? "success" : "warning"}>{col.syncStatus}</DhBadge>
        {col.isManualOverride && <DhBadge tone="warning">Override manual</DhBadge>}
        <span className="text-xs text-urrea-text-muted">Antigüedad: {col.tenureYears} años</span>
        <span className="text-xs text-urrea-text-muted">Contrato: {col.contractType}</span>
        <span className="text-xs text-urrea-text-muted">Origen: {col.externalSource ?? "Manual"}</span>
      </div>

      <div className="mb-0 flex gap-0 overflow-x-auto border-b border-urrea-border bg-white">
        {TABS.map((t) => (
          <button
            key={t}
            type="button"
            onClick={() => setTab(t)}
            className={cn(
              "shrink-0 border-b-2 px-3 py-3 text-[11px] font-semibold uppercase tracking-wide transition",
              tab === t ? "border-urrea-primary text-urrea-primary" : "border-transparent text-urrea-text-muted hover:text-urrea-text",
            )}
          >
            {t}
          </button>
        ))}
      </div>

      <DhCard className="border-t-0">
        {tab === "Resumen" && (
          <div className="space-y-6">
            <HcmKpiStrip items={[
              { label: "Antigüedad", value: `${col.tenureYears} años` },
              { label: "Subordinados", value: String(org?.directReportsCount ?? col.directReports.length) },
              { label: "Días vacaciones", value: vacation ? `${vacation.daysPending} pendientes` : "—" },
              { label: "Última sync", value: col.lastSyncAt ? new Date(col.lastSyncAt).toLocaleDateString("es-MX") : "—" },
            ]} />
            <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
              {modules.map((m) => (
                <HcmModulePanel key={m.module} label={m.label} available={m.moduleAvailable} message={m.statusMessage} recordCount={m.recordCount} />
              ))}
            </div>
            {movements.length > 0 && (
              <div>
                <h3 className="mb-3 text-sm font-semibold text-urrea-text">Últimos movimientos</h3>
                <HcmTimeline items={movements.slice(0, 3).map((m) => ({
                  date: m.effectiveDate,
                  title: m.movementType,
                  subtitle: m.newValue ?? m.previousValue,
                  meta: `${m.source} · ${m.createdBy}`,
                }))} />
              </div>
            )}
          </div>
        )}

        {tab === "Información personal" && (
          <HcmFieldGrid fields={[
            ["Nombre legal", col.legalFirstName],
            ["Apellido paterno", col.legalLastName],
            ["Apellido materno", col.legalMiddleName ?? "—"],
            ["Nombre preferido", col.preferredName ?? "—"],
            ["RFC", col.personal?.rfc ?? "—"],
            ["CURP", col.personal?.curp ?? "—"],
            ["NSS", col.personal?.nss ?? "—"],
            ["Fecha nacimiento", col.personal?.birthDate ? new Date(col.personal.birthDate).toLocaleDateString("es-MX") : "—"],
            ["Correo corporativo", col.workEmail],
            ["Teléfono", col.phone ?? "—"],
            ["Domicilio", col.personal?.address ?? "—"],
          ]} />
        )}

        {tab === "Información laboral" && (
          <HcmFieldGrid fields={[
            ["Puesto", col.position],
            ["Departamento", col.department],
            ["Área / dirección", col.area ?? "—"],
            ["Centro de costo", col.costCenter ?? "—"],
            ["Jefe inmediato", col.managerName ?? "—"],
            ["Relación laboral", col.contractType],
            ["Sede / ubicación", col.location ?? "—"],
            ["Fecha de ingreso", new Date(col.hireDate).toLocaleDateString("es-MX", { dateStyle: "long" })],
            ["Jornada", col.employment?.workSchedule ?? "—"],
            ["Turno", col.employment?.shift ?? "—"],
            ["Grupo nómina", col.employment?.payrollGroup ?? "—"],
            ["Visibilidad compensación", col.employment?.compensationVisibility ?? "Restricted"],
          ]} />
        )}

        {tab === "Organización" && org && (
          <div className="space-y-4">
            <HcmFieldGrid fields={[
              ["Jefe inmediato", org.managerName ?? "Sin asignar"],
              ["No. empleado jefe", org.managerEmployeeNumber ?? "—"],
              ["Puesto", org.position],
              ["Departamento", org.department],
              ["Área", org.area ?? "—"],
              ["Centro de costo", org.costCenter ?? "—"],
              ["Ubicación", org.location ?? "—"],
              ["Reportes directos", String(org.directReportsCount)],
            ]} />
            {col.directReports.length > 0 && (
              <table className="w-full text-left text-sm">
                <thead className="bg-slate-50 text-[11px] uppercase tracking-wide text-urrea-text-muted">
                  <tr><th className="px-3 py-2">Colaborador</th><th className="px-3 py-2">Puesto</th></tr>
                </thead>
                <tbody>
                  {col.directReports.map((s) => (
                    <tr key={s.id} className="border-t border-urrea-border/40">
                      <td className="px-3 py-2.5"><Link href={`/portal/hcm/personas/${s.id}`} className="font-medium text-urrea-primary hover:underline">{s.fullName}</Link></td>
                      <td className="px-3 py-2.5">{s.position}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        )}

        {tab === "Documentos" && (
          documents.length === 0 ? (
            <p className="text-sm text-urrea-text-muted">Expediente documental vacío. Los documentos se gestionan en Fase 7 — Expediente digital.</p>
          ) : (
            <table className="w-full text-left text-sm">
              <thead className="bg-slate-50 text-[11px] uppercase tracking-wide text-urrea-text-muted">
                <tr><th className="px-3 py-2">Documento</th><th className="px-3 py-2">Tipo</th><th className="px-3 py-2">Versión</th><th className="px-3 py-2">Estado</th></tr>
              </thead>
              <tbody>
                {documents.map((d) => (
                  <tr key={d.id} className="border-t border-urrea-border/40">
                    <td className="px-3 py-2.5 font-medium">{d.name}</td>
                    <td className="px-3 py-2.5 text-urrea-text-muted">{d.documentType}</td>
                    <td className="px-3 py-2.5 font-mono text-xs">v{d.version}</td>
                    <td className="px-3 py-2.5"><DhBadge tone={d.status === "Vigente" ? "success" : "danger"}>{d.status}</DhBadge></td>
                  </tr>
                ))}
              </tbody>
            </table>
          )
        )}

        {tab === "Movimientos" && (
          <HcmTimeline items={movements.map((m) => ({
            date: m.effectiveDate,
            title: m.movementType,
            subtitle: [m.previousValue, m.newValue].filter(Boolean).join(" → ") || undefined,
            meta: `${m.source} · ${m.createdBy}${m.externalReference ? ` · Ref: ${m.externalReference}` : ""}`,
          }))} />
        )}

        {tab === "Vacaciones" && (
          vacation ? (
            <HcmFieldGrid fields={[
              ["Año", String(vacation.year)],
              ["Días asignados", String(vacation.daysAssigned)],
              ["Días usados", String(vacation.daysUsed)],
              ["Días pendientes", String(vacation.daysPending)],
              ["Solicitudes del año", String(vacation.requestsThisYear)],
              ["Pendientes de aprobación", String(vacation.pendingApproval)],
            ]} />
          ) : (
            <p className="text-sm text-urrea-text-muted">Sin saldo de vacaciones registrado para el año actual.</p>
          )
        )}

        {tab === "Asistencia" && mod("attendance") && (
          <HcmModulePanel label={mod("attendance")!.label} available={mod("attendance")!.moduleAvailable} message={mod("attendance")!.statusMessage} recordCount={mod("attendance")!.recordCount} />
        )}

        {tab === "Capacitación" && mod("training") && (
          <HcmModulePanel label={mod("training")!.label} available={mod("training")!.moduleAvailable} message={mod("training")!.statusMessage} recordCount={mod("training")!.recordCount} />
        )}

        {tab === "Desempeño" && mod("performance") && (
          <HcmModulePanel label={mod("performance")!.label} available={mod("performance")!.moduleAvailable} message={mod("performance")!.statusMessage} recordCount={mod("performance")!.recordCount} />
        )}

        {tab === "Beneficios" && mod("benefits") && (
          <HcmModulePanel label={mod("benefits")!.label} available={mod("benefits")!.moduleAvailable} message={mod("benefits")!.statusMessage} recordCount={mod("benefits")!.recordCount} />
        )}

        {tab === "Bitácora" && (
          <HcmTimeline items={audit.map((a) => ({
            date: a.occurredAt,
            title: `${a.module} · ${a.action}`,
            subtitle: a.detail,
            meta: a.user ?? "system",
          }))} />
        )}

        {tab === "Integración" && (
          <HcmFieldGrid fields={[
            ["Origen de datos", col.externalSource ?? "Manual"],
            ["External Employee ID", col.externalEmployeeId ?? "—"],
            ["Estado sincronización", col.syncStatus],
            ["Última sync", col.lastSyncAt ? new Date(col.lastSyncAt).toLocaleString("es-MX") : "—"],
            ["Override manual", col.isManualOverride ? "Sí — bloquea sync automático" : "No"],
            ["Hash sync", "—"],
          ]} />
        )}
      </DhCard>
    </div>
  );
}
