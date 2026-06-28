"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { DhIcon, type DhIconName } from "@/components/dh/shared/icons";
import { Badge } from "@/components/ui/badge";
import { ButtonLink } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { PageContainer, PageHeader } from "@/components/ui/page-header";
import { absenceTypeIcon } from "@/lib/absence-icons";
import { avatarGradient, getInitials, getTenureLabel, initialsFromFullName } from "@/components/portal/profile-helpers";
import { portalService } from "@/lib/services/portalService";
import { absenceService } from "@/lib/services/absenceService";
import { getSession, isJefe, isRhAdmin } from "@/lib/auth";
import { cn } from "@/lib/utils";
import type { ColaboradorPerfil, SaldoVacaciones, SolicitudAusencia } from "@/lib/types";

function ProfileSkeleton() {
  return (
    <PageContainer className="animate-pulse space-y-4">
      <div className="h-8 w-48 rounded-lg bg-urrea-chrome/40" />
      <div className="flex gap-4">
        <div className="h-20 w-20 shrink-0 rounded-2xl bg-urrea-chrome/50" />
        <div className="flex-1 space-y-2 pt-2">
          <div className="h-6 w-2/3 rounded-lg bg-urrea-chrome/40" />
          <div className="h-4 w-1/2 rounded-lg bg-urrea-chrome/30" />
        </div>
      </div>
    </PageContainer>
  );
}

function StatTile({ icon, label, value }: { icon: DhIconName; label: string; value: string }) {
  return (
    <div className="flex flex-col rounded-2xl border border-urrea-border/70 bg-white p-4">
      <span className="mb-2 flex h-8 w-8 items-center justify-center rounded-lg bg-urrea-primary/8 text-urrea-primary">
        <DhIcon name={icon} className="h-4 w-4" />
      </span>
      <p className="text-xl font-semibold tabular-nums text-urrea-text">{value}</p>
      <p className="text-xs text-urrea-text-muted">{label}</p>
    </div>
  );
}

function InfoRow({ icon, label, value }: { icon: DhIconName; label: string; value: string }) {
  return (
    <div className="flex items-center gap-3 rounded-xl border border-urrea-border/50 bg-urrea-bg-soft/30 px-3 py-2.5">
      <span className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-white text-urrea-secondary ring-1 ring-urrea-border/40">
        <DhIcon name={icon} className="h-4 w-4" />
      </span>
      <div className="min-w-0">
        <p className="text-[11px] font-medium uppercase tracking-wide text-urrea-text-muted">{label}</p>
        <p className="truncate text-sm font-medium text-urrea-text">{value}</p>
      </div>
    </div>
  );
}

function PersonRow({
  name,
  subtitle,
  badge,
  seed,
  href,
}: {
  name: string;
  subtitle: string;
  badge?: string;
  seed: string;
  href?: string;
}) {
  const inner = (
    <div className="flex items-center gap-3 rounded-xl border border-urrea-border/60 bg-white p-3 transition hover:border-urrea-secondary/30">
      <div
        className={cn(
          "flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-gradient-to-br text-xs font-bold text-white",
          avatarGradient(seed),
        )}
      >
        {initialsFromFullName(name)}
      </div>
      <div className="min-w-0 flex-1">
        <p className="truncate text-sm font-semibold text-urrea-text">{name}</p>
        <p className="truncate text-xs text-urrea-text-muted">{subtitle}</p>
        {badge && (
          <span className="mt-1 inline-block rounded-md bg-urrea-bg-soft px-2 py-0.5 text-[10px] font-medium text-urrea-text-muted">
            {badge}
          </span>
        )}
      </div>
      {href && <DhIcon name="chevron" className="h-4 w-4 shrink-0 text-urrea-chrome" />}
    </div>
  );
  if (href) return <Link href={href}>{inner}</Link>;
  return inner;
}

const QUICK_LINKS: { href: string; label: string; icon: DhIconName }[] = [
  { href: "/portal/vacaciones", label: "Vacaciones y permisos", icon: "calendar" },
  { href: "/portal/asistencia", label: "Mi asistencia", icon: "clock" },
  { href: "/portal/beneficios", label: "Mis beneficios", icon: "gift" },
  { href: "/portal/mi-compensacion", label: "Mi compensación", icon: "analytics" },
];

export function ProfileView() {
  const [perfil, setPerfil] = useState<ColaboradorPerfil | null>(null);
  const [saldo, setSaldo] = useState<SaldoVacaciones | null>(null);
  const [actividad, setActividad] = useState<SolicitudAusencia[]>([]);
  const session = getSession();

  useEffect(() => {
    portalService.getMe().then(setPerfil).catch(console.error);
    absenceService.getMyBalance().then(setSaldo).catch(console.error);
    absenceService.getMyRequests()
      .then((list) => setActividad(list.slice(0, 5)))
      .catch(console.error);
  }, []);

  if (!perfil) return <ProfileSkeleton />;

  const nombreCompleto = `${perfil.nombre} ${perfil.apellidoPaterno} ${perfil.apellidoMaterno ?? ""}`.trim();
  const initials = getInitials(perfil.nombre, perfil.apellidoPaterno, perfil.apellidoMaterno);
  const tenure = getTenureLabel(perfil.fechaIngreso);
  const roles: string[] = [];
  if (session && isRhAdmin(session)) roles.push("RH Admin");
  if (session && isJefe(session)) roles.push("Líder");

  return (
    <PageContainer className="max-w-3xl animate-fade-up">
      <PageHeader
        title="Mi ficha"
        subtitle="Tu información laboral en URREA"
        infoTitle="Mi ficha"
        infoContent="Consulta tus datos de contacto, puesto y conexiones organizacionales. Los datos de nómina son de solo lectura y los administra el sistema central."
      />

      {/* Identidad */}
      <div className="flex flex-col gap-4 rounded-2xl border border-urrea-border/70 bg-white p-5 sm:flex-row sm:items-center">
        <div
          className={cn(
            "flex h-20 w-20 shrink-0 items-center justify-center rounded-2xl bg-gradient-to-br text-2xl font-bold text-white",
            avatarGradient(perfil.numeroEmpleado),
          )}
        >
          {initials}
        </div>
        <div className="min-w-0 flex-1">
          <h2 className="text-xl font-semibold text-urrea-text">{nombreCompleto}</h2>
          <p className="text-sm font-medium text-urrea-secondary">{perfil.puesto}</p>
          <p className="text-sm text-urrea-text-muted">
            {perfil.departamento}
            {perfil.sede ? ` · ${perfil.sede}` : ""}
          </p>
          <div className="mt-2 flex flex-wrap gap-2">
            <span className="rounded-md bg-urrea-primary/8 px-2 py-0.5 text-xs font-semibold text-urrea-primary">
              #{perfil.numeroEmpleado}
            </span>
            {roles.map((r) => (
              <span key={r} className="rounded-md bg-urrea-bg-soft px-2 py-0.5 text-xs font-medium text-urrea-text-muted">
                {r}
              </span>
            ))}
          </div>
        </div>
        {perfil.email && (
          <a
            href={`mailto:${perfil.email}`}
            className="inline-flex min-h-11 shrink-0 items-center justify-center gap-2 rounded-xl border border-urrea-border px-4 text-sm font-medium text-urrea-text no-underline transition hover:bg-urrea-bg-soft"
          >
            <DhIcon name="communication" className="h-4 w-4" />
            Email
          </a>
        )}
      </div>

      {/* Métricas */}
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
        <StatTile icon="clock" label="Antigüedad" value={tenure} />
        <StatTile icon="calendar" label="Vacaciones" value={`${saldo?.diasPendientes ?? "—"}`} />
        <StatTile icon="users" label="Mi equipo" value={`${perfil.subordinados.length}`} />
        <StatTile icon="folder" label="Solicitudes" value={`${actividad.length}`} />
      </div>

      {/* Datos laborales */}
      <Card title="Datos laborales">
        <div className="grid gap-2 sm:grid-cols-2">
          <InfoRow icon="building" label="Departamento" value={perfil.departamento} />
          <InfoRow icon="building" label="Sede" value={perfil.sede ?? "Sin sede asignada"} />
          <InfoRow
            icon="calendar"
            label="Fecha de ingreso"
            value={new Date(perfil.fechaIngreso).toLocaleDateString("es-MX", { month: "long", year: "numeric" })}
          />
          {perfil.telefono && <InfoRow icon="communication" label="Teléfono" value={perfil.telefono} />}
          {perfil.jefeDirecto && <InfoRow icon="users" label="Reporta a" value={perfil.jefeDirecto} />}
        </div>
      </Card>

      {/* Actividad */}
      <Card title="Actividad reciente">
        {actividad.length === 0 ? (
          <p className="text-sm text-urrea-text-muted">Sin solicitudes recientes.</p>
        ) : (
          <ul className="space-y-2">
            {actividad.map((s) => (
              <li key={s.id} className="flex items-center gap-3 rounded-xl border border-urrea-border/50 px-3 py-2.5">
                <span className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-urrea-bg-soft text-urrea-primary">
                  <DhIcon name={absenceTypeIcon(s.tipoAusenciaCodigo)} className="h-4 w-4" />
                </span>
                <div className="min-w-0 flex-1">
                  <p className="truncate text-sm font-medium text-urrea-text">{s.tipoAusenciaNombre}</p>
                  <p className="text-xs text-urrea-text-muted">
                    {new Date(s.fechaInicio).toLocaleDateString("es-MX")} · {s.diasSolicitados} días
                  </p>
                </div>
                <Badge estado={s.estado} />
              </li>
            ))}
          </ul>
        )}
        <Link href="/portal/vacaciones" className="mt-3 inline-flex items-center gap-1 text-sm font-medium text-urrea-secondary hover:underline">
          Ver todo
          <DhIcon name="chevron" className="h-3.5 w-3.5" />
        </Link>
      </Card>

      {/* Red organizacional */}
      {(perfil.jefeDirecto || perfil.subordinados.length > 0) && (
        <Card title="Mi red">
          <div className="space-y-3">
            {perfil.jefeDirecto && (
              <div>
                <p className="mb-2 text-xs font-medium uppercase tracking-wide text-urrea-text-muted">Jefe directo</p>
                <PersonRow name={perfil.jefeDirecto} subtitle="Liderazgo" seed={perfil.jefeDirecto} />
              </div>
            )}
            {perfil.subordinados.length > 0 && (
              <div>
                <p className="mb-2 text-xs font-medium uppercase tracking-wide text-urrea-text-muted">
                  Equipo ({perfil.subordinados.length})
                </p>
                <div className="space-y-2">
                  {perfil.subordinados.map((s) => (
                    <PersonRow
                      key={s.id}
                      name={s.nombreCompleto}
                      subtitle={s.puesto}
                      badge={`#${s.numeroEmpleado}`}
                      seed={s.numeroEmpleado}
                      href={`/portal/mi-equipo/${s.id}`}
                    />
                  ))}
                </div>
              </div>
            )}
          </div>
        </Card>
      )}

      {/* Nómina readonly */}
      <Card title="Identidad laboral" description="Sincronizado desde nómina · solo lectura">
        <div className="mb-3 flex items-center gap-2 rounded-xl bg-urrea-bg-soft px-3 py-2 text-xs text-urrea-text-muted">
          <DhIcon name="shield" className="h-4 w-4 shrink-0 text-urrea-secondary" />
          Datos administrados por el sistema de nómina.
        </div>
        <dl className="space-y-3 text-sm">
          <NominaRow label="Email corporativo" value={perfil.email} />
          <NominaRow label="RFC" value={perfil.rfc ?? "—"} mono />
          <NominaRow label="No. empleado" value={perfil.numeroEmpleado} mono />
          <NominaRow
            label="Última sync"
            value={perfil.nominaSyncAt ? new Date(perfil.nominaSyncAt).toLocaleString("es-MX") : "—"}
          />
        </dl>
      </Card>

      {/* Accesos */}
      <Card title="Accesos rápidos">
        <nav className="grid gap-1 sm:grid-cols-2">
          {QUICK_LINKS.map((item) => (
            <Link
              key={item.href}
              href={item.href}
              className="flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium text-urrea-text transition hover:bg-urrea-bg-soft"
            >
              <DhIcon name={item.icon} className="h-4 w-4 text-urrea-text-muted" />
              {item.label}
            </Link>
          ))}
          {session && isJefe(session) && (
            <Link
              href="/portal/aprobaciones"
              className="flex items-center gap-3 rounded-xl px-3 py-2.5 text-sm font-medium text-urrea-text transition hover:bg-urrea-bg-soft"
            >
              <DhIcon name="shield" className="h-4 w-4 text-urrea-text-muted" />
              Aprobar solicitudes
            </Link>
          )}
        </nav>
      </Card>

      <div className="flex flex-wrap gap-2">
        <ButtonLink href="/portal/vacaciones?tipo=vac">Solicitar vacaciones</ButtonLink>
        <ButtonLink href="/portal/vacaciones?tipo=permiso" variant="secondary">
          Solicitar permiso
        </ButtonLink>
      </div>
    </PageContainer>
  );
}

function NominaRow({ label, value, mono }: { label: string; value: string; mono?: boolean }) {
  return (
    <div className="flex flex-col gap-0.5 border-b border-urrea-border/40 pb-2 last:border-0 last:pb-0">
      <dt className="text-[11px] font-medium uppercase tracking-wide text-urrea-text-muted">{label}</dt>
      <dd className={cn("font-medium text-urrea-text break-all", mono && "font-mono text-xs")}>{value}</dd>
    </div>
  );
}
