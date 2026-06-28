"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { ConfidencialTag, DhBadge, DhBarChart, DhCard, DhKpiCard, estatusTone } from "@/components/dh/shared/ui";
import { DhComplianceStrip, DhEnterpriseHeader } from "@/components/dh/shared/page-frame";
import { HEADCOUNT_EMPRESA, METRICAS_EJECUTIVAS, formatNum } from "@/lib/dh/enterprise-metrics";
import { DENUNCIAS, ENCUESTAS, LOGS_INTEGRACION, TICKETS } from "@/lib/dh/mock-data";

export function DenunciasView() {
  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="shield"
        title="Canal de denuncias y ética corporativa"
        subtitle="Buzón confidencial administrado por el Comité de Ética. Acceso restringido conforme LFPDPPP e ISO 27001."
        breadcrumbs={[{ label: "Desarrollo Humano", href: "/dh" }, { label: "Canal de denuncias" }]}
        action={<a href="/dh/denuncias/registrar" className="text-sm font-medium text-urrea-primary hover:underline">Portal de registro confidencial</a>}
      />
      <div className="mb-4 border border-red-200 bg-red-50/60 px-4 py-3 text-sm text-red-950">
        <ConfidencialTag /> Información clasificada. Visible únicamente para roles autorizados de Desarrollo Humano y Comité de Ética.
      </div>
      <DhCard title="Casos en gestión" subtitle="Identificadores enmascarados · sin exposición de datos personales" noPadding>
        <table className="w-full text-left text-sm">
          <thead className="bg-slate-50 text-[11px] uppercase tracking-wide text-urrea-text-muted">
            <tr className="border-b border-urrea-border">
              <th className="px-4 py-3">Token</th>
              <th className="px-4 py-3">Clasificación</th>
              <th className="px-4 py-3">Estado</th>
              <th className="px-4 py-3">Fecha recepción</th>
              <th className="px-4 py-3">Investigador</th>
            </tr>
          </thead>
          <tbody>
            {DENUNCIAS.map((d) => (
              <tr key={d.id} className="border-b border-urrea-border/40">
                <td className="px-4 py-3 font-mono text-xs">{d.token}</td>
                <td className="px-4 py-3 capitalize">{d.tipo.replace("_", " ")}</td>
                <td className="px-4 py-3"><DhBadge tone={estatusTone(d.estado)}>{d.estado.replace("_", " ")}</DhBadge></td>
                <td className="px-4 py-3 tabular-nums">{new Date(d.fecha).toLocaleDateString("es-MX")}</td>
                <td className="px-4 py-3 text-urrea-text-muted">{d.responsable ?? "Pendiente de asignación"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </DhCard>
    </div>
  );
}

export function IntegracionesView() {
  const [logs, setLogs] = useState(LOGS_INTEGRACION);
  const [syncMsg, setSyncMsg] = useState("");

  function syncManual() {
    setSyncMsg("Proceso de sincronización manual en ejecución…");
    setTimeout(() => {
      setLogs((prev) => [{
        id: `l${Date.now()}`,
        proceso: "Sincronización manual autorizada",
        origen: "SAP HCM",
        destino: "CDM",
        registrosOk: METRICAS_EJECUTIVAS.colaboradoresTotal,
        registrosError: 0,
        estado: "exitoso",
        fecha: new Date().toISOString(),
      }, ...prev]);
      setSyncMsg(`Proceso finalizado. ${formatNum(METRICAS_EJECUTIVAS.colaboradoresTotal)} registros procesados correctamente.`);
    }, 1500);
  }

  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="integration"
        title="Centro de integraciones SAP · CDM · Nómina"
        subtitle="Monitoreo operativo de interfaces corporativas. Sincronización programada 02:00 CST y ejecución manual bajo control de TI."
        breadcrumbs={[{ label: "Desarrollo Humano", href: "/dh" }, { label: "Integraciones" }]}
        action={<Button type="button" variant="secondary" onClick={syncManual}>Ejecutar sincronización manual</Button>}
      />
      {syncMsg && <p className="mb-4 border border-emerald-200 bg-emerald-50 px-4 py-2 text-sm text-emerald-900">{syncMsg}</p>}

      <div className="mb-6 grid gap-px border border-urrea-border/80 bg-urrea-border/80 sm:grid-cols-3">
        <DhKpiCard label="SAP HCM → CDM" value="Operativo" accent="success" hint="Ventana nocturna · SLA 99.5%" />
        <DhKpiCard label="CDM → Portal DH" value="Degradado" accent="warning" hint={`${METRICAS_EJECUTIVAS.syncErrores} registros con incidencia de mapeo`} alert />
        <DhKpiCard label="Portal → Nómina" value="Operativo" accent="success" hint="Reporte diario 06:30 CST" />
      </div>

      <DhCard title="Bitácora técnica de interfaces" subtitle="Últimas ejecuciones registradas">
        <ul className="space-y-2">
          {logs.map((log) => (
            <li key={log.id} className="border border-urrea-border/60 p-4 text-sm">
              <div className="flex flex-wrap items-center justify-between gap-2">
                <p className="font-semibold text-urrea-text">{log.proceso}</p>
                <DhBadge tone={log.estado === "exitoso" ? "success" : log.estado === "parcial" ? "warning" : "danger"}>{log.estado}</DhBadge>
              </div>
              <p className="mt-1 text-xs text-urrea-text-muted">{log.origen} → {log.destino}</p>
              <p className="text-xs tabular-nums text-urrea-text-muted">Procesados: {formatNum(log.registrosOk)} · Errores: {log.registrosError}</p>
              <p className="text-[11px] text-urrea-chrome">{new Date(log.fecha).toLocaleString("es-MX")}</p>
              {log.mensaje && <p className="mt-1 text-xs text-amber-900">{log.mensaje}</p>}
            </li>
          ))}
        </ul>
      </DhCard>

      <DhCard title="Catálogo de procesos de integración" className="mt-6">
        <div className="flex flex-wrap gap-2 text-[11px] font-semibold uppercase tracking-wide text-urrea-text-muted">
          {["Alta de colaborador", "Baja", "Actualización", "Cambio de puesto", "Centro de costo", "Jefe inmediato", "Unidad organizativa", "Reingreso", "Permisos", "Asistencias", "Prenómina", "Headcount DW"].map((p) => (
            <span key={p} className="border border-urrea-border bg-slate-50 px-3 py-1.5">{p}</span>
          ))}
        </div>
      </DhCard>
    </div>
  );
}

export function AnaliticaView() {
  const clima = ENCUESTAS.find((e) => e.tipo === "clima")?.tasaRespuesta ?? METRICAS_EJECUTIVAS.climaLaboralRespuestaPct;

  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="analytics"
        title="Analítica ejecutiva de capital humano"
        subtitle="Indicadores consolidados para Dirección General y Desarrollo Humano. Exportación a Power BI y Data Warehouse corporativo."
        breadcrumbs={[{ label: "Desarrollo Humano", href: "/dh" }, { label: "Analítica" }]}
      />

      <div className="mb-6 grid gap-px border border-urrea-border/80 bg-urrea-border/80 sm:grid-cols-2 lg:grid-cols-4">
        <DhKpiCard label="Headcount activo" value={formatNum(METRICAS_EJECUTIVAS.colaboradoresActivos)} accent="primary" />
        <DhKpiCard label="Rotación anual" value={`${METRICAS_EJECUTIVAS.rotacionAnualPct}%`} accent="warning" />
        <DhKpiCard label="Ausentismo" value={`${METRICAS_EJECUTIVAS.ausentismoPct}%`} accent="secondary" />
        <DhKpiCard label="Índice clima laboral" value={`${clima}%`} hint="Tasa de respuesta encuesta 2026" accent="success" />
        <DhKpiCard label="Tickets abiertos" value={METRICAS_EJECUTIVAS.ticketsAbiertos} accent="primary" />
        <DhKpiCard label="Cumplimiento capacitación" value="87.3%" accent="success" />
        <DhKpiCard label="Evaluaciones pendientes" value={formatNum(METRICAS_EJECUTIVAS.evaluacionesPendientes)} accent="warning" />
        <DhKpiCard label="Casos ética abiertos" value={METRICAS_EJECUTIVAS.denunciasAbiertas} accent="secondary" />
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        <DhCard title="Distribución de plantilla" subtitle="Por dirección de negocio">
          <DhBarChart data={HEADCOUNT_EMPRESA.map((h) => ({ area: h.area, count: h.count }))} labelKey="area" valueKey="count" showPct />
        </DhCard>
        <DhCard title="Centro de servicios" subtitle="Tickets por categoría · mes en curso">
          <DhBarChart data={[
            { cat: "Nómina", n: 128 }, { cat: "Vacaciones", n: 94 }, { cat: "Accesos", n: 67 },
            { cat: "Beneficios", n: 52 }, { cat: "Capacitación", n: 41 },
          ]} labelKey="cat" valueKey="n" />
        </DhCard>
      </div>

      <DhComplianceStrip />
    </div>
  );
}

export function DenunciaPublicaView() {
  const [token, setToken] = useState("");
  const [submitted, setSubmitted] = useState(false);

  return (
    <div className="mx-auto max-w-lg animate-fade-up">
      <DhEnterpriseHeader
        icon="shield"
        title="Registro confidencial"
        subtitle="Canal independiente administrado por el Comité de Ética de URREA. Su identidad puede permanecer en anonimato."
        breadcrumbs={[{ label: "Canal de denuncias", href: "/dh/denuncias" }, { label: "Registro" }]}
      />
      {!submitted ? (
        <DhCard>
          <select className="mb-3 w-full border border-urrea-border px-3 py-2 text-sm outline-none focus:border-urrea-primary">
            <option>Acoso laboral</option><option>Fraude</option><option>Seguridad</option><option>Conflicto de interés</option><option>Conducta inapropiada</option><option>Otro</option>
          </select>
          <textarea className="mb-3 w-full border border-urrea-border px-3 py-2 text-sm outline-none focus:border-urrea-primary" rows={5} placeholder="Describa los hechos con el mayor detalle posible…" />
          <Button type="button" className="w-full" onClick={() => { setToken("URR-X7K2-2026"); setSubmitted(true); }}>Enviar reporte confidencial</Button>
        </DhCard>
      ) : (
        <DhCard>
          <p className="text-sm text-urrea-text">Su reporte ha sido registrado. Conserve el siguiente token para seguimiento:</p>
          <p className="mt-3 font-mono text-lg font-bold text-urrea-primary">{token}</p>
        </DhCard>
      )}
    </div>
  );
}
