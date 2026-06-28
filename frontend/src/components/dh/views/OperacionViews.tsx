"use client";

import Link from "next/link";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { DhBadge, DhCard, DhModal, estatusTone } from "@/components/dh/shared/ui";
import { DhEnterpriseHeader } from "@/components/dh/shared/page-frame";
import { METRICAS_EJECUTIVAS } from "@/lib/dh/enterprise-metrics";
import {
  CANDIDATOS,
  DOCUMENTOS,
  INCIDENCIAS_ASISTENCIA,
  ONBOARDINGS,
  SOLICITUDES_AUSENCIA,
  TIPOS_PERMISO,
  VACANTES,
} from "@/lib/dh/mock-data";

const ETAPAS = ["nuevo", "filtrado", "entrevista", "oferta", "aceptado", "rechazado"] as const;

export function ReclutamientoView() {
  const [selectedVacante, setSelectedVacante] = useState<string | null>(null);

  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="recruitment"
        title="Reclutamiento y selección de personal"
        subtitle="Gestión integral del funnel de talento: vacante, evaluación, oferta, alta en SAP y sincronización CDM hacia HCM."
        breadcrumbs={[{ label: "Desarrollo Humano", href: "/dh" }, { label: "Reclutamiento" }]}
      />

      <div className="mb-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {VACANTES.filter((v) => v.estatus === "abierta").map((v) => (
          <button
            key={v.id}
            type="button"
            onClick={() => setSelectedVacante(v.id)}
            className="rounded-2xl border border-urrea-border/80 bg-urrea-bg p-4 text-left shadow-soft transition hover:border-urrea-secondary/40"
          >
            <DhBadge tone="info">{v.estatus}</DhBadge>
            <p className="mt-2 font-semibold text-urrea-text">{v.titulo}</p>
            <p className="text-xs text-urrea-text-muted">{v.area} · {v.ubicacion}</p>
            <p className="mt-2 text-sm tabular-nums text-urrea-primary">{v.candidatosCount} candidatos</p>
          </button>
        ))}
      </div>

      <DhCard title="Pipeline Kanban de candidatos">
        <div className="flex gap-3 overflow-x-auto pb-2">
          {ETAPAS.map((etapa) => {
            const items = CANDIDATOS.filter((c) => c.etapa === etapa && (!selectedVacante || c.vacanteId === selectedVacante));
            return (
              <div key={etapa} className="min-w-[200px] flex-1 rounded-xl bg-urrea-bg-soft/80 p-3">
                <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-urrea-text-muted">{etapa}</p>
                <div className="space-y-2">
                  {items.map((c) => (
                    <div key={c.id} className="rounded-lg border border-urrea-border/60 bg-urrea-bg p-3 text-sm shadow-soft">
                      <p className="font-medium text-urrea-text">{c.nombre}</p>
                      <p className="text-xs text-urrea-text-muted">Score: {c.score}</p>
                      {c.altaSap !== "na" && (
                        <DhBadge tone={c.altaSap === "completada" ? "success" : "warning"}>SAP: {c.altaSap}</DhBadge>
                      )}
                      {c.etapa === "aceptado" && (
                        <Link href="/dh/onboarding" className="mt-2 block text-xs font-medium text-urrea-secondary hover:underline">
                          → Enviar a onboarding
                        </Link>
                      )}
                    </div>
                  ))}
                </div>
              </div>
            );
          })}
        </div>
      </DhCard>
    </div>
  );
}

export function OnboardingView() {
  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="onboarding"
        title="Onboarding e incorporación"
        subtitle={`${METRICAS_EJECUTIVAS.onboardingActivos} procesos activos con checklists parametrizados por rol, sede y nivel jerárquico.`}
        breadcrumbs={[{ label: "Desarrollo Humano", href: "/dh" }, { label: "Onboarding" }]}
      />

      <div className="space-y-6">
        {ONBOARDINGS.map((ob) => (
          <DhCard key={ob.id} title={`${ob.colaboradorNombre} — ${ob.puesto}`}>
            <div className="mb-4 flex flex-wrap items-center gap-4">
              <div className="flex-1">
                <div className="h-3 overflow-hidden rounded-full bg-urrea-bg-soft">
                  <div className="h-full rounded-full bg-gradient-to-r from-urrea-primary to-urrea-secondary" style={{ width: `${ob.avance}%` }} />
                </div>
                <p className="mt-1 text-sm text-urrea-text-muted">{ob.avance}% completado · Inicio {new Date(ob.fechaInicio).toLocaleDateString("es-MX")}</p>
              </div>
              {ob.tareas.some((t) => t.estado === "vencido") && <DhBadge tone="danger">Tareas vencidas</DhBadge>}
            </div>
            <ul className="space-y-2">
              {ob.tareas.map((t) => (
                <li key={t.id} className="flex flex-wrap items-center justify-between gap-2 rounded-xl border border-urrea-border/60 p-3 text-sm">
                  <div>
                    <p className="font-medium text-urrea-text">{t.nombre}</p>
                    <p className="text-xs text-urrea-text-muted">Responsable: {t.responsable} · Límite: {new Date(t.fechaLimite).toLocaleDateString("es-MX")}</p>
                  </div>
                  <DhBadge tone={estatusTone(t.estado.replace("_", " "))}>{t.estado.replace("_", " ")}</DhBadge>
                </li>
              ))}
            </ul>
          </DhCard>
        ))}
      </div>
    </div>
  );
}

export function ExpedienteView() {
  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="folder"
        title="Expediente digital del colaborador"
        subtitle="Repositorio documental corporativo con control de versiones, firmas electrónicas y trazabilidad auditada."
        breadcrumbs={[{ label: "Desarrollo Humano", href: "/dh" }, { label: "Expediente digital" }]}
      />

      <DhCard>
        <div className="overflow-x-auto">
          <table className="w-full min-w-[700px] text-left text-sm">
            <thead>
              <tr className="border-b border-urrea-border text-urrea-text-muted">
                <th className="px-3 py-3">Documento</th>
                <th className="px-3 py-3">Tipo</th>
                <th className="px-3 py-3">Versión</th>
                <th className="px-3 py-3">Estado</th>
                <th className="px-3 py-3">Vigencia</th>
              </tr>
            </thead>
            <tbody>
              {DOCUMENTOS.map((d) => (
                <tr key={d.id} className="border-b border-urrea-border/50">
                  <td className="px-3 py-3 font-medium">{d.nombre}</td>
                  <td className="px-3 py-3">{d.tipo}</td>
                  <td className="px-3 py-3">{d.version}</td>
                  <td className="px-3 py-3"><DhBadge tone={estatusTone(d.estado.replace("_", " "))}>{d.estado.replace("_", " ")}</DhBadge></td>
                  <td className="px-3 py-3">{d.vigencia ? new Date(d.vigencia).toLocaleDateString("es-MX") : "—"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </DhCard>

      <DhCard title="Bitácora de cambios (muestra)" className="mt-6">
        <ul className="space-y-2 text-sm">
          <li className="rounded-xl bg-urrea-bg-soft/60 p-3">Patricia Ruiz · 2026-06-20 · Actualizó contrato v3 → v4</li>
          <li className="rounded-xl bg-urrea-bg-soft/60 p-3">Sistema CDM · 2026-06-18 · Sincronizó aviso privacidad desde SAP</li>
        </ul>
      </DhCard>
    </div>
  );
}

export function VacacionesDhView() {
  const [modal, setModal] = useState(false);
  const [solicitudes, setSolicitudes] = useState(SOLICITUDES_AUSENCIA);

  function aprobar(id: string) {
    setSolicitudes((prev) => prev.map((s) => (s.id === id ? { ...s, estado: "aprobada" as const } : s)));
  }

  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="calendar"
        title="Administración de vacaciones y permisos"
        subtitle="Motor de ausencias conforme LFT y política URREA. Workflow de aprobación, cálculo de saldos e incidencias a nómina."
        breadcrumbs={[{ label: "Desarrollo Humano", href: "/dh" }, { label: "Vacaciones y permisos" }]}
        action={<Button type="button" onClick={() => setModal(true)}>Registrar solicitud</Button>}
      />

      <div className="mb-6 grid gap-4 sm:grid-cols-3">
        <DhCard><p className="text-xs text-urrea-text-muted">Saldo vacaciones (demo)</p><p className="text-3xl font-bold text-urrea-primary">12 días</p></DhCard>
        <DhCard><p className="text-xs text-urrea-text-muted">Pendientes aprobación</p><p className="text-3xl font-bold text-amber-700">{solicitudes.filter((s) => s.estado === "pendiente").length}</p></DhCard>
        <DhCard><p className="text-xs text-urrea-text-muted">Tipos de permiso</p><p className="text-3xl font-bold text-urrea-secondary">{TIPOS_PERMISO.length}</p></DhCard>
      </div>

      <DhCard title="Solicitudes">
        <ul className="space-y-2">
          {solicitudes.map((s) => (
            <li key={s.id} className="flex flex-wrap items-center justify-between gap-2 rounded-xl border border-urrea-border/60 p-3 text-sm">
              <div>
                <p className="font-medium text-urrea-text">{s.colaboradorNombre} — {s.tipo}</p>
                <p className="text-xs text-urrea-text-muted">{s.fechaInicio} a {s.fechaFin} · {s.dias} día(s)</p>
              </div>
              <div className="flex items-center gap-2">
                <DhBadge tone={estatusTone(s.estado)}>{s.estado}</DhBadge>
                {s.estado === "pendiente" && (
                  <Button type="button" variant="secondary" className="h-8 text-xs" onClick={() => aprobar(s.id)}>Aprobar</Button>
                )}
              </div>
            </li>
          ))}
        </ul>
      </DhCard>

      <DhModal open={modal} title="Nueva solicitud" onClose={() => setModal(false)}>
        <p className="mb-3 text-sm text-urrea-text-muted">Simulación de formulario — en producción conecta a motor de vacaciones y LFT.</p>
        <select className="mb-3 w-full rounded-xl border border-urrea-border px-3 py-2 text-sm">
          {TIPOS_PERMISO.map((t) => <option key={t}>{t}</option>)}
        </select>
        <Button type="button" className="w-full" onClick={() => setModal(false)}>Enviar solicitud</Button>
      </DhModal>
    </div>
  );
}

export function AsistenciaView() {
  const comercial = INCIDENCIAS_ASISTENCIA.filter((i) => i.area === "Comercial");

  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="clock"
        title="Control de asistencia e incidencias"
        subtitle="Consolidación biométrica, aplicación móvil y captura autorizada. Validación gerencial y reporte diario a nómina."
        breadcrumbs={[{ label: "Desarrollo Humano", href: "/dh" }, { label: "Asistencia" }]}
      />

      <DhCard title="Incidencias recientes">
        <table className="w-full text-left text-sm">
          <thead>
            <tr className="border-b border-urrea-border text-urrea-text-muted">
              <th className="py-2">Colaborador</th><th className="py-2">Fecha</th><th className="py-2">Tipo</th><th className="py-2">Origen</th><th className="py-2">Estado</th>
            </tr>
          </thead>
          <tbody>
            {INCIDENCIAS_ASISTENCIA.map((i) => (
              <tr key={i.id} className="border-b border-urrea-border/50">
                <td className="py-2">{i.colaboradorNombre}</td>
                <td className="py-2">{i.fecha}</td>
                <td className="py-2">{i.tipo.replace("_", " ")}</td>
                <td className="py-2">{i.origen.replace("_", " ")}</td>
                <td className="py-2"><DhBadge tone={estatusTone(i.estado.replace("_", " "))}>{i.estado.replace("_", " ")}</DhBadge></td>
              </tr>
            ))}
          </tbody>
        </table>
      </DhCard>

      <DhCard title="Fuerza de ventas / Comercial" className="mt-6">
        <p className="mb-3 text-sm text-urrea-text-muted">Control especial de registros de campo con geolocalización y reporte diario a nómina.</p>
        <ul className="space-y-2">
          {comercial.map((i) => (
            <li key={i.id} className="flex justify-between rounded-xl bg-urrea-bg-soft/60 p-3 text-sm">
              <span>{i.colaboradorNombre} — {i.tipo}</span>
              <DhBadge tone="info">App móvil</DhBadge>
            </li>
          ))}
        </ul>
      </DhCard>
    </div>
  );
}
