"use client";

import Link from "next/link";
import { useMemo, useState } from "react";
import { Button } from "@/components/ui/button";
import {
  ConfidencialTag,
  DhBadge,
  DhCard,
  DhDataTable,
  DhSearchInput,
  DhSelect,
  estatusTone,
} from "@/components/dh/shared/ui";
import { DhEnterpriseHeader, DhPageToolbar } from "@/components/dh/shared/page-frame";
import { METRICAS_EJECUTIVAS, SEDES, formatNum } from "@/lib/dh/enterprise-metrics";
import { AREAS, COLABORADORES, DOCUMENTOS, getColaborador } from "@/lib/dh/mock-data";
import type { ColaboradorHcm } from "@/lib/dh/types";
import { cn } from "@/lib/utils";

function Avatar({ name }: { name: string }) {
  const initials = name.split(" ").slice(0, 2).map((w) => w[0]).join("");
  return (
    <div className="flex h-9 w-9 shrink-0 items-center justify-center border border-urrea-border bg-slate-100 text-[11px] font-bold text-slate-700">
      {initials}
    </div>
  );
}

export function HcmListView() {
  const [search, setSearch] = useState("");
  const [area, setArea] = useState("all");
  const [estatus, setEstatus] = useState("all");
  const [sede, setSede] = useState("all");

  const filtered = useMemo(() => {
    return COLABORADORES.filter((c) => {
      if (area !== "all" && c.area !== area) return false;
      if (estatus !== "all" && c.estatus !== estatus) return false;
      if (sede !== "all" && !c.ubicacion.includes(sede.split(" ")[0])) return false;
      if (search) {
        const q = search.toLowerCase();
        return (
          c.nombrePreferido.toLowerCase().includes(q) ||
          c.numeroEmpleado.includes(q) ||
          c.puesto.toLowerCase().includes(q) ||
          c.rfc.toLowerCase().includes(q)
        );
      }
      return true;
    });
  }, [search, area, estatus, sede]);

  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="users"
        title="Gestión de personas (HCM)"
        subtitle="Registro maestro de colaboradores sincronizado desde SAP vía CDM. Expediente único auditado conforme LFPDPPP y políticas internas URREA."
        breadcrumbs={[
          { label: "Desarrollo Humano", href: "/dh" },
          { label: "HCM" },
        ]}
        action={<Button type="button" variant="secondary" className="text-xs">Alta manual (excepción)</Button>}
      />

      <div className="mb-4 grid gap-px border border-urrea-border/80 bg-urrea-border/80 sm:grid-cols-4">
        <div className="bg-urrea-bg p-4">
          <p className="text-[10px] font-semibold uppercase tracking-wide text-urrea-text-muted">Plantilla activa</p>
          <p className="mt-1 text-xl font-semibold tabular-nums text-urrea-primary">{formatNum(METRICAS_EJECUTIVAS.colaboradoresActivos)}</p>
        </div>
        <div className="bg-urrea-bg p-4">
          <p className="text-[10px] font-semibold uppercase tracking-wide text-urrea-text-muted">Centros de costo</p>
          <p className="mt-1 text-xl font-semibold tabular-nums">{METRICAS_EJECUTIVAS.centrosCosto}</p>
        </div>
        <div className="bg-urrea-bg p-4">
          <p className="text-[10px] font-semibold uppercase tracking-wide text-urrea-text-muted">Unidades org.</p>
          <p className="mt-1 text-xl font-semibold tabular-nums">{METRICAS_EJECUTIVAS.unidadesOrganizativas}</p>
        </div>
        <div className="bg-urrea-bg p-4">
          <p className="text-[10px] font-semibold uppercase tracking-wide text-urrea-text-muted">Última sync CDM</p>
          <p className="mt-1 text-sm font-medium">{new Date(METRICAS_EJECUTIVAS.syncUltimaOk).toLocaleString("es-MX")}</p>
        </div>
      </div>

      <DhPageToolbar
        recordCount={`Muestra ${filtered.length} registros · Total corporativo ${formatNum(METRICAS_EJECUTIVAS.colaboradoresTotal)}`}
        onExport={() => undefined}
      >
        <div className="min-w-[200px] flex-1 sm:max-w-xs"><DhSearchInput value={search} onChange={setSearch} placeholder="Nombre, número, RFC, puesto…" /></div>
        <DhSelect value={area} onChange={setArea} options={[{ value: "all", label: "Todas las direcciones" }, ...AREAS.map((a) => ({ value: a, label: a }))]} />
        <DhSelect value={sede} onChange={setSede} options={[{ value: "all", label: "Todas las sedes" }, ...SEDES.map((s) => ({ value: s, label: s }))]} />
        <DhSelect value={estatus} onChange={setEstatus} options={[
          { value: "all", label: "Todos los estatus" },
          { value: "activo", label: "Activo" },
          { value: "baja", label: "Baja" },
          { value: "reingreso", label: "Reingreso" },
          { value: "pendiente", label: "Alta pendiente" },
        ]} />
      </DhPageToolbar>

      <DhDataTable
        footer={`Página 1 de ${Math.ceil(METRICAS_EJECUTIVAS.colaboradoresTotal / 50)} · Orden: apellido paterno, nombre · Fuente: SAP HCM vía CDM`}
      >
        <table className="w-full min-w-[1024px] text-left text-sm">
          <thead className="bg-slate-50 text-[11px] uppercase tracking-wide text-urrea-text-muted">
            <tr className="border-b border-urrea-border">
              <th className="px-4 py-3 font-semibold">Colaborador</th>
              <th className="px-4 py-3 font-semibold">No. empleado</th>
              <th className="px-4 py-3 font-semibold">Puesto</th>
              <th className="px-4 py-3 font-semibold">Dirección</th>
              <th className="px-4 py-3 font-semibold">C.C.</th>
              <th className="px-4 py-3 font-semibold">Jefe inmediato</th>
              <th className="px-4 py-3 font-semibold">Estatus</th>
              <th className="px-4 py-3 font-semibold">Sede</th>
              <th className="px-4 py-3 font-semibold">Antigüedad</th>
            </tr>
          </thead>
          <tbody>
            {filtered.map((c, i) => (
              <tr key={c.id} className={cn("border-b border-urrea-border/40 hover:bg-slate-50/80", i % 2 === 1 && "bg-slate-50/30")}>
                <td className="px-4 py-3">
                  <Link href={`/dh/hcm/${c.id}`} className="flex items-center gap-3 font-medium text-urrea-primary hover:underline">
                    <Avatar name={c.nombrePreferido} />
                    <span>{c.nombrePreferido}</span>
                  </Link>
                </td>
                <td className="px-4 py-3 font-mono text-xs tabular-nums">{c.numeroEmpleado}</td>
                <td className="px-4 py-3 text-urrea-text">{c.puesto}</td>
                <td className="px-4 py-3 text-urrea-text-muted">{c.area}</td>
                <td className="px-4 py-3 font-mono text-xs">{c.centroCosto}</td>
                <td className="px-4 py-3 text-xs text-urrea-text-muted">{c.jefeNombre}</td>
                <td className="px-4 py-3"><DhBadge tone={estatusTone(c.estatus)}>{c.estatus}</DhBadge></td>
                <td className="px-4 py-3 text-xs">{c.ubicacion}</td>
                <td className="px-4 py-3 tabular-nums text-xs">{c.antiguedadAnios}a</td>
              </tr>
            ))}
          </tbody>
        </table>
      </DhDataTable>
    </div>
  );
}

const TABS = ["Información personal", "Datos laborales", "Expediente documental", "Movimientos", "Vacaciones", "Asistencia", "Capacitación", "Desempeño", "Beneficios", "Auditoría"] as const;

export function HcmDetailView({ id }: { id: string }) {
  const col = getColaborador(id);
  const [tab, setTab] = useState<(typeof TABS)[number]>("Información personal");

  if (!col) {
    return <p className="py-16 text-center text-sm text-urrea-text-muted">Registro no localizado en el directorio corporativo.</p>;
  }

  const docs = DOCUMENTOS.filter((d) => d.colaboradorId === id);

  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="users"
        title={col.nombreLegal}
        subtitle={`${col.puesto} · ${col.area} · Número de empleado ${col.numeroEmpleado}`}
        breadcrumbs={[
          { label: "Desarrollo Humano", href: "/dh" },
          { label: "HCM", href: "/dh/hcm" },
          { label: col.nombrePreferido },
        ]}
        action={<Link href="/dh/hcm" className="text-sm font-medium text-urrea-secondary hover:underline">← Directorio de personas</Link>}
      />

      <div className="mb-5 flex flex-wrap items-center gap-3 border border-urrea-border/80 bg-slate-50 px-4 py-3">
        <Avatar name={col.nombrePreferido} />
        <DhBadge tone={estatusTone(col.estatus)}>{col.estatus}</DhBadge>
        <span className="text-xs text-urrea-text-muted">Antigüedad: {col.antiguedadAnios} años</span>
        <span className="text-xs text-urrea-text-muted">Contrato: {col.tipoContrato}</span>
        {col.confidencial && <ConfidencialTag />}
      </div>

      <div className="mb-0 flex gap-0 overflow-x-auto border-b border-urrea-border bg-white">
        {TABS.map((t) => (
          <button
            key={t}
            type="button"
            onClick={() => setTab(t)}
            className={cn(
              "shrink-0 border-b-2 px-4 py-3 text-xs font-semibold uppercase tracking-wide transition",
              tab === t ? "border-urrea-primary text-urrea-primary" : "border-transparent text-urrea-text-muted hover:text-urrea-text",
            )}
          >
            {t}
          </button>
        ))}
      </div>

      <DhCard className="border-t-0">
        {tab === "Información personal" && <PersonalTab c={col} />}
        {tab === "Datos laborales" && <LaboralTab c={col} />}
        {tab === "Expediente documental" && <DocsTab docs={docs} />}
        {tab !== "Información personal" && tab !== "Datos laborales" && tab !== "Expediente documental" && (
          <div className="py-10 text-center">
            <p className="text-sm font-medium text-urrea-text">Módulo {tab}</p>
            <p className="mt-2 max-w-lg mx-auto text-sm text-urrea-text-muted">
              Vista operativa conectada al ecosistema URREA. Los datos de {tab.toLowerCase()} se consolidan desde SAP/CDM y los sistemas satélite de Desarrollo Humano.
            </p>
          </div>
        )}
      </DhCard>
    </div>
  );
}

function PersonalTab({ c }: { c: ColaboradorHcm }) {
  const fields: [string, string][] = [
    ["Nombre legal", c.nombreLegal],
    ["Nombre preferido", c.nombrePreferido],
    ["RFC", c.rfc],
    ["CURP", c.curp],
    ["NSS", c.nss],
    ["Correo corporativo", c.email],
    ["Teléfono", c.telefono],
    ["Domicilio fiscal", c.domicilio],
  ];
  return (
    <dl className="grid gap-px border border-urrea-border/60 bg-urrea-border/60 sm:grid-cols-2">
      {fields.map(([label, value]) => (
        <div key={label} className="bg-urrea-bg p-4">
          <dt className="text-[10px] font-semibold uppercase tracking-wide text-urrea-text-muted">{label}</dt>
          <dd className="mt-1 text-sm font-medium text-urrea-text">{value}</dd>
        </div>
      ))}
    </dl>
  );
}

function LaboralTab({ c }: { c: ColaboradorHcm }) {
  const fields: [string, string][] = [
    ["Puesto", c.puesto],
    ["Dirección / área", c.area],
    ["Centro de costo", c.centroCosto],
    ["Jefe inmediato", c.jefeNombre],
    ["Tipo de contrato", c.tipoContrato],
    ["Jornada", c.jornada],
    ["Sede / ubicación", c.ubicacion],
    ["Fecha de ingreso", new Date(c.fechaIngreso).toLocaleDateString("es-MX", { dateStyle: "long" })],
    ["Compensación mensual", c.confidencial ? "Información restringida" : `$${c.salarioMensual.toLocaleString("es-MX")} MXN`],
  ];
  return (
    <dl className="grid gap-px border border-urrea-border/60 bg-urrea-border/60 sm:grid-cols-2">
      {fields.map(([label, value]) => (
        <div key={label} className="bg-urrea-bg p-4">
          <dt className="text-[10px] font-semibold uppercase tracking-wide text-urrea-text-muted">{label}</dt>
          <dd className="mt-1 text-sm font-medium text-urrea-text">{value}</dd>
        </div>
      ))}
    </dl>
  );
}

function DocsTab({ docs }: { docs: typeof DOCUMENTOS }) {
  if (docs.length === 0) {
    return <p className="text-sm text-urrea-text-muted">No hay documentos registrados en el expediente digital para este colaborador.</p>;
  }
  return (
    <table className="w-full text-left text-sm">
      <thead className="bg-slate-50 text-[11px] uppercase tracking-wide text-urrea-text-muted">
        <tr>
          <th className="px-3 py-2">Documento</th>
          <th className="px-3 py-2">Tipo</th>
          <th className="px-3 py-2">Versión</th>
          <th className="px-3 py-2">Estado</th>
        </tr>
      </thead>
      <tbody>
        {docs.map((d) => (
          <tr key={d.id} className="border-t border-urrea-border/40">
            <td className="px-3 py-2.5 font-medium">{d.nombre}</td>
            <td className="px-3 py-2.5 text-urrea-text-muted">{d.tipo}</td>
            <td className="px-3 py-2.5 font-mono text-xs">{d.version}</td>
            <td className="px-3 py-2.5"><DhBadge tone={estatusTone(d.estado.replace("_", " "))}>{d.estado.replace("_", " ")}</DhBadge></td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
