"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { DhBadge, DhBarChart, DhCard, DhModal, DhPieLegend, estatusTone } from "@/components/dh/shared/ui";
import { DhEnterpriseHeader } from "@/components/dh/shared/page-frame";
import { METRICAS_EJECUTIVAS } from "@/lib/dh/enterprise-metrics";
import {
  BENEFICIOS_SOLICITUDES,
  CURSOS,
  ENCUESTAS,
  FEED,
  OBJETIVOS_OKR,
  TICKETS,
} from "@/lib/dh/mock-data";

export function CapacitacionView() {
  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="education"
        title="Universidad corporativa URREA (LMS)"
        subtitle={`Plataforma de aprendizaje con contenidos SCORM/xAPI, rutas por puesto y cumplimiento normativo. ${METRICAS_EJECUTIVAS.cursosVencidosOProximos} capacitaciones obligatorias vencidas o próximas a vencer.`}
        breadcrumbs={[{ label: "Desarrollo Humano", href: "/dh" }, { label: "Capacitación" }]}
      />
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {CURSOS.map((c) => (
          <DhCard key={c.id}>
            <div className="flex items-start justify-between gap-2">
              <p className="font-semibold text-urrea-text">{c.nombre}</p>
              {c.obligatorio && <DhBadge tone="warning">Obligatorio</DhBadge>}
            </div>
            <p className="mt-1 text-xs text-urrea-text-muted">{c.categoria} · {c.modalidad} · {c.duracionHoras}h</p>
            <div className="mt-3 flex items-center justify-between">
              <DhBadge tone={estatusTone(c.estatus.replace("_", " "))}>{c.estatus.replace("_", " ")}</DhBadge>
              {c.calificacion != null && <span className="text-sm font-bold text-urrea-primary">{c.calificacion}%</span>}
            </div>
            <p className="mt-2 text-[10px] text-urrea-chrome">Límite: {new Date(c.fechaLimite).toLocaleDateString("es-MX")}</p>
          </DhCard>
        ))}
      </div>
    </div>
  );
}

export function DesempenoView() {
  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="chart"
        title="Gestión del desempeño y OKR"
        subtitle="Metodología SMART/OKR con evaluación multifuente 360°. Ciclo de calibración trimestral y planes de desarrollo individual."
        breadcrumbs={[{ label: "Desarrollo Humano", href: "/dh" }, { label: "Desempeño" }]}
      />

      <DhCard title="Objetivos individuales / OKR" className="mb-6">
        <ul className="space-y-3">
          {OBJETIVOS_OKR.map((o) => (
            <li key={o.id} className="rounded-xl border border-urrea-border/60 p-4">
              <div className="flex flex-wrap items-start justify-between gap-2">
                <div>
                  <p className="font-medium text-urrea-text">{o.objetivo}</p>
                  <p className="text-xs text-urrea-text-muted">{o.colaboradorNombre} · KPI: {o.kpi} · {o.periodo}</p>
                  <p className="text-xs text-urrea-secondary">Alineación: {o.alineacion}</p>
                </div>
                <DhBadge tone={estatusTone(o.estado.replace("_", " "))}>{o.estado.replace("_", " ")}</DhBadge>
              </div>
              <div className="mt-3 h-2 overflow-hidden rounded-full bg-urrea-bg-soft">
                <div className="h-full rounded-full bg-urrea-primary" style={{ width: `${o.avance}%` }} />
              </div>
              <p className="mt-1 text-right text-xs tabular-nums text-urrea-text-muted">{o.avance}%</p>
            </li>
          ))}
        </ul>
      </DhCard>

      <DhCard title="Evaluación 360 (simulación)">
        <div className="grid gap-4 sm:grid-cols-2">
          <div>
            <p className="mb-2 text-sm font-medium text-urrea-text">Fuentes de evaluación</p>
            <DhPieLegend items={[
              { label: "Autoevaluación", value: 20, color: "#023764" },
              { label: "Jefe", value: 30, color: "#2E7FA8" },
              { label: "Pares", value: 25, color: "#10b981" },
              { label: "Subordinados", value: 25, color: "#d8c7ae" },
            ]} />
          </div>
          <div className="rounded-xl bg-urrea-bg-soft/60 p-4 text-sm text-urrea-text-muted">
            <p className="font-medium text-urrea-text">Radar de competencias</p>
            <p className="mt-2">Liderazgo · Colaboración · Innovación · Orientación a resultados · Comunicación</p>
            <p className="mt-3 text-xs">Resultado consolidado Q2 2026: <strong className="text-urrea-primary">4.2 / 5.0</strong></p>
          </div>
        </div>
      </DhCard>
    </div>
  );
}

export function EncuestasView() {
  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="survey"
        title="Encuestas organizacionales"
        subtitle="Clima laboral, pulse, onboarding y salida. Segmentación por dirección, anonimato configurable y analítica comparativa."
        breadcrumbs={[{ label: "Desarrollo Humano", href: "/dh" }, { label: "Encuestas" }]}
      />
      <div className="space-y-3">
        {ENCUESTAS.map((e) => (
          <DhCard key={e.id}>
            <div className="flex flex-wrap items-center justify-between gap-2">
              <div>
                <p className="font-semibold text-urrea-text">{e.titulo}</p>
                <p className="text-xs text-urrea-text-muted">{e.tipo} · {e.audiencia} · {e.anonima ? "Anónima" : "Identificada"}</p>
              </div>
              <div className="text-right">
                <DhBadge tone={estatusTone(e.estatus)}>{e.estatus}</DhBadge>
                <p className="mt-1 text-sm font-bold text-urrea-primary">{e.tasaRespuesta}% respuesta</p>
              </div>
            </div>
            {e.estatus === "activa" && (
              <div className="mt-3 h-2 overflow-hidden rounded-full bg-urrea-bg-soft">
                <div className="h-full rounded-full bg-urrea-secondary" style={{ width: `${e.tasaRespuesta}%` }} />
              </div>
            )}
          </DhCard>
        ))}
      </div>
    </div>
  );
}

export function ComunicacionView() {
  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="communication"
        title="Comunicación interna y reconocimientos"
        subtitle="Canal corporativo segmentado por dirección, sede y comunidad. Métricas de alcance y participación."
        breadcrumbs={[{ label: "Desarrollo Humano", href: "/dh" }, { label: "Comunicación" }]}
      />
      <div className="space-y-4">
        {FEED.map((p) => (
          <DhCard key={p.id}>
            <div className="flex items-center gap-2">
              <p className="font-semibold text-urrea-text">{p.autor}</p>
              <DhBadge tone="neutral">{p.area}</DhBadge>
              <DhBadge tone="info">{p.tipo}</DhBadge>
            </div>
            <p className="mt-2 text-sm text-urrea-text">{p.contenido}</p>
            <p className="mt-3 text-xs text-urrea-text-muted">{p.reacciones} reacciones · {p.comentarios} comentarios</p>
          </DhCard>
        ))}
      </div>
      <DhCard title="Ranking reconocimientos (mes)" className="mt-6">
        <DhBarChart data={[
          { nombre: "Operaciones MTY", puntos: 48 },
          { nombre: "Comercial Norte", puntos: 35 },
          { nombre: "DH", puntos: 28 },
        ]} labelKey="nombre" valueKey="puntos" />
      </DhCard>
    </div>
  );
}

export function BeneficiosDhView() {
  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="gift"
        title="Beneficios y prestaciones"
        subtitle="Portal de autogestión de prestaciones, esquemas flexibles, reembolsos y administración de dependientes conforme política URREA."
        breadcrumbs={[{ label: "Desarrollo Humano", href: "/dh" }, { label: "Beneficios" }]}
      />
      <div className="mb-6 grid gap-3 sm:grid-cols-3">
        {["Vales de despensa", "Seguro médico", "Telemedicina", "Reembolsos", "Beneficios flexibles", "Convenios"].map((b) => (
          <div key={b} className="rounded-2xl border border-urrea-border/80 bg-urrea-bg p-4 shadow-soft">
            <p className="font-medium text-urrea-text">{b}</p>
            <p className="mt-1 text-xs text-urrea-text-muted">Autogestión disponible</p>
          </div>
        ))}
      </div>
      <DhCard title="Solicitudes recientes">
        <ul className="space-y-2">
          {BENEFICIOS_SOLICITUDES.map((b) => (
            <li key={b.id} className="flex justify-between rounded-xl border border-urrea-border/60 p-3 text-sm">
              <span>{b.colaboradorNombre} — {b.beneficio}</span>
              <DhBadge tone={estatusTone(b.estatus)}>{b.estatus}</DhBadge>
            </li>
          ))}
        </ul>
      </DhCard>
    </div>
  );
}

export function ServiciosView() {
  const [modal, setModal] = useState(false);
  const [tickets, setTickets] = useState(TICKETS);

  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="support"
        title="Centro de servicios al colaborador"
        subtitle="Mesa de ayuda corporativa con SLA por categoría, base de conocimiento y canal omnicanal."
        breadcrumbs={[{ label: "Desarrollo Humano", href: "/dh" }, { label: "Centro de servicios" }]}
        action={<Button type="button" onClick={() => setModal(true)}>Registrar ticket</Button>}
      />

      <div className="mb-6 grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
        {["Nómina", "Vacaciones", "Beneficios", "Accesos"].map((cat) => (
          <div key={cat} className="rounded-xl border border-urrea-border/60 bg-urrea-bg-soft/50 p-4 text-sm">
            <p className="font-medium text-urrea-text">FAQ · {cat}</p>
            <p className="mt-1 text-xs text-urrea-text-muted">12 artículos</p>
          </div>
        ))}
      </div>

      <DhCard title="Chatbot simulado" className="mb-6">
        <div className="rounded-xl bg-urrea-bg-soft/80 p-4 text-sm">
          <p className="text-urrea-text-muted">Asistente virtual URREA DH. Seleccione una consulta frecuente o describa su solicitud.</p>
          <div className="mt-3 flex flex-wrap gap-2">
            {["Consultar saldo vacaciones", "Estado de ticket", "Subir documento"].map((q) => (
              <button key={q} type="button" className="rounded-full border border-urrea-border px-3 py-1 text-xs hover:bg-urrea-bg">{q}</button>
            ))}
          </div>
        </div>
      </DhCard>

      <DhCard title="Tickets">
        <ul className="space-y-2">
          {tickets.map((t) => (
            <li key={t.id} className="flex flex-wrap items-center justify-between gap-2 rounded-xl border border-urrea-border/60 p-3 text-sm">
              <div>
                <p className="font-medium text-urrea-text">{t.folio} — {t.asunto}</p>
                <p className="text-xs text-urrea-text-muted">{t.categoria} · SLA {t.slaHoras}h · {t.solicitante}</p>
              </div>
              <DhBadge tone={estatusTone(t.estado.replace("_", " "))}>{t.estado.replace("_", " ")}</DhBadge>
            </li>
          ))}
        </ul>
      </DhCard>

      <DhModal open={modal} title="Levantar ticket" onClose={() => setModal(false)}>
        <input className="mb-2 w-full rounded-xl border px-3 py-2 text-sm" placeholder="Asunto" />
        <textarea className="mb-3 w-full rounded-xl border px-3 py-2 text-sm" placeholder="Describe tu solicitud..." rows={4} />
        <Button type="button" className="w-full" onClick={() => setModal(false)}>Enviar ticket</Button>
      </DhModal>
    </div>
  );
}
