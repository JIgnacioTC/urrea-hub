"use client";

import { DhBadge, DhBarChart, DhCard, DhKpiCard } from "@/components/dh/shared/ui";
import { DhComplianceStrip, DhEnterpriseHeader, DhSyncStatus } from "@/components/dh/shared/page-frame";
import {
  EMPRESA,
  formatNum,
  HEADCOUNT_EMPRESA,
  METRICAS_EJECUTIVAS,
  SOLICITUDES_MODULO_EMPRESA,
} from "@/lib/dh/enterprise-metrics";
import { LOGS_INTEGRACION, TIMELINE } from "@/lib/dh/mock-data";

export function DashboardView() {
  const alertas = TIMELINE.filter((t) => t.tipo === "critico" || t.tipo === "alerta");
  const integracionIssues = LOGS_INTEGRACION.filter((l) => l.estado !== "exitoso");

  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="dashboard"
        title="Centro de mando — Desarrollo Humano"
        subtitle={`${EMPRESA.razonSocial}. Consolidado operativo de ${formatNum(METRICAS_EJECUTIVAS.colaboradoresActivos)} colaboradores activos en ${METRICAS_EJECUTIVAS.plantasOperativas} plantas y ${METRICAS_EJECUTIVAS.ubicaciones} ubicaciones.`}
        breadcrumbs={[{ label: "Desarrollo Humano", href: "/dh" }, { label: "Centro de mando" }]}
        action={
          <div className="flex flex-col items-end gap-1 text-right">
            <DhSyncStatus ok={METRICAS_EJECUTIVAS.syncErrores === 0} label="Integración SAP · CDM" />
            <p className="text-[11px] tabular-nums text-urrea-text-muted">
              Última sincronización: {new Date(METRICAS_EJECUTIVAS.syncUltimaOk).toLocaleString("es-MX")}
            </p>
          </div>
        }
      />

      {/* KPIs ejecutivos */}
      <section aria-label="Indicadores clave" className="mb-6 grid gap-px border border-urrea-border/80 bg-urrea-border/80 sm:grid-cols-2 xl:grid-cols-4">
        <DhKpiCard size="hero" label="Plantilla activa" value={formatNum(METRICAS_EJECUTIVAS.colaboradoresActivos)} hint={`Total registros HCM: ${formatNum(METRICAS_EJECUTIVAS.colaboradoresTotal)}`} delta={{ value: "1.2% vs mes ant.", positive: true }} accent="primary" />
        <DhKpiCard size="hero" label="Solicitudes en workflow" value={formatNum(METRICAS_EJECUTIVAS.solicitudesPendientes)} hint="Vacaciones, permisos, beneficios y servicios" accent="warning" alert />
        <DhKpiCard size="hero" label="Cumplimiento capacitación" value="87.3%" hint={`${METRICAS_EJECUTIVAS.cursosVencidosOProximos} cursos vencidos o por vencer (30 días)`} delta={{ value: "2.1 pts", positive: false }} accent="secondary" />
        <DhKpiCard size="hero" label="Rotación anual" value={`${METRICAS_EJECUTIVAS.rotacionAnualPct}%`} hint={`Ausentismo: ${METRICAS_EJECUTIVAS.ausentismoPct}% · Benchmark sector: 9.4%`} accent="success" />
      </section>

      <section className="mb-6 grid gap-3 sm:grid-cols-2 lg:grid-cols-4 xl:grid-cols-6">
        <DhKpiCard label="Vacaciones por autorizar" value={METRICAS_EJECUTIVAS.vacacionesPorAprobar} accent="warning" />
        <DhKpiCard label="Incidencias asistencia (hoy)" value={METRICAS_EJECUTIVAS.incidenciasAsistenciaHoy} accent="secondary" />
        <DhKpiCard label="Tickets centro de servicios" value={METRICAS_EJECUTIVAS.ticketsAbiertos} accent="primary" />
        <DhKpiCard label="Evaluaciones pendientes" value={formatNum(METRICAS_EJECUTIVAS.evaluacionesPendientes)} hint="Ciclo Q2 2026" accent="primary" />
        <DhKpiCard label="Procesos onboarding" value={METRICAS_EJECUTIVAS.onboardingActivos} accent="secondary" />
        <DhKpiCard label="Vacantes abiertas" value={METRICAS_EJECUTIVAS.vacantesAbiertas} accent="secondary" />
      </section>

      <div className="grid gap-6 xl:grid-cols-12">
        <DhCard title="Distribución de plantilla por dirección" subtitle="Headcount autorizado vs ocupado" className="xl:col-span-5">
          <DhBarChart
            data={HEADCOUNT_EMPRESA.map((h) => ({ area: h.area, count: h.count }))}
            labelKey="area"
            valueKey="count"
            showPct
          />
        </DhCard>

        <DhCard title="Carga operativa por módulo" subtitle="Solicitudes abiertas y SLA objetivo" className="xl:col-span-4">
          <ul className="space-y-3">
            {SOLICITUDES_MODULO_EMPRESA.map((s) => (
              <li key={s.modulo} className="flex items-center justify-between border-b border-urrea-border/40 pb-2 text-sm last:border-0">
                <span className="text-urrea-text">{s.modulo}</span>
                <div className="flex items-center gap-3">
                  <span className="font-semibold tabular-nums text-urrea-primary">{s.count}</span>
                  <span className="text-[10px] uppercase tracking-wide text-urrea-text-muted">SLA {s.sla}</span>
                </div>
              </li>
            ))}
          </ul>
        </DhCard>

        <DhCard title="Alertas prioritarias" subtitle="Requieren atención de DH o TI" className="xl:col-span-3">
          <ul className="space-y-2">
            {alertas.map((ev) => (
              <li key={ev.id} className="border-l-2 border-amber-500 bg-amber-50/50 px-3 py-2 text-xs">
                <p className="font-medium text-urrea-text">{ev.descripcion}</p>
                <p className="mt-0.5 text-urrea-text-muted">{ev.modulo} · {new Date(ev.fecha).toLocaleString("es-MX")}</p>
              </li>
            ))}
            {integracionIssues.slice(0, 2).map((log) => (
              <li key={log.id} className="border-l-2 border-red-500 bg-red-50/40 px-3 py-2 text-xs">
                <p className="font-medium text-urrea-text">{log.proceso}</p>
                <p className="text-urrea-text-muted">{log.mensaje}</p>
              </li>
            ))}
          </ul>
        </DhCard>

        <DhCard title="Bitácora de eventos corporativos" subtitle="Últimas 24 horas" className="xl:col-span-7">
          <div className="overflow-x-auto">
            <table className="w-full min-w-[560px] text-left text-xs">
              <thead>
                <tr className="border-b border-urrea-border text-urrea-text-muted">
                  <th className="pb-2 font-semibold uppercase tracking-wide">Fecha</th>
                  <th className="pb-2 font-semibold uppercase tracking-wide">Módulo</th>
                  <th className="pb-2 font-semibold uppercase tracking-wide">Evento</th>
                  <th className="pb-2 font-semibold uppercase tracking-wide">Responsable</th>
                </tr>
              </thead>
              <tbody>
                {TIMELINE.map((ev) => (
                  <tr key={ev.id} className="border-b border-urrea-border/40">
                    <td className="py-2.5 tabular-nums text-urrea-text-muted">{new Date(ev.fecha).toLocaleString("es-MX")}</td>
                    <td className="py-2.5"><DhBadge tone={ev.tipo === "critico" ? "danger" : ev.tipo === "alerta" ? "warning" : "neutral"}>{ev.modulo}</DhBadge></td>
                    <td className="py-2.5 text-urrea-text">{ev.descripcion}</td>
                    <td className="py-2.5 text-urrea-text-muted">{ev.usuario}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </DhCard>

        <DhCard title="Arquitectura de integración" subtitle="Flujo maestro de datos de personal" className="xl:col-span-5">
          <div className="space-y-3 text-sm">
            {[
              { n: "01", s: "SAP ERP HCM", d: "Fuente autoritativa: altas, bajas, movimientos, estructura org." },
              { n: "02", s: "CDM — Core Data Manager", d: "Normalización, validación y orquestación de sincronización." },
              { n: "03", s: "Portal Desarrollo Humano", d: "Operación de procesos, workflows y experiencia colaborador." },
              { n: "04", s: "Nómina · BI · Data Warehouse", d: "Incidencias, analítica ejecutiva y cumplimiento normativo." },
            ].map((step) => (
              <div key={step.n} className="flex gap-3 border border-urrea-border/60 p-3">
                <span className="flex h-8 w-8 shrink-0 items-center justify-center bg-urrea-primary text-xs font-bold text-white">{step.n}</span>
                <div>
                  <p className="font-semibold text-urrea-text">{step.s}</p>
                  <p className="text-xs text-urrea-text-muted">{step.d}</p>
                </div>
              </div>
            ))}
          </div>
        </DhCard>
      </div>

      <DhComplianceStrip />
    </div>
  );
}
