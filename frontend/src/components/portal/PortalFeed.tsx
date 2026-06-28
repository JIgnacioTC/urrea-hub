"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import { DhIcon } from "@/components/dh/shared/icons";
import { InfoButton, SectionHeading } from "@/components/ui/info-button";
import { Badge } from "@/components/ui/badge";
import { ButtonLink } from "@/components/ui/button";
import { avatarGradient, getInitials } from "@/components/portal/profile-helpers";
import { portalService } from "@/lib/services/portalService";
import { absenceService } from "@/lib/services/absenceService";
import { getSession, isJefe } from "@/lib/auth";
import { formatFeedTime } from "@/lib/portal-feed";
import { PORTAL_HELP, PORTAL_QUICK_ACTIONS } from "@/lib/portal/portal-home";
import type { ColaboradorPerfil, FeedPost, SaldoVacaciones, SolicitudAusencia } from "@/lib/types";
import { cn } from "@/lib/utils";

const TYPE_LABELS: Record<FeedPost["type"], string> = {
  announcement: "Anuncio",
  recognition: "Reconocimiento",
  event: "Evento",
  general: "General",
};

function greeting() {
  const h = new Date().getHours();
  if (h < 12) return "Buenos días";
  if (h < 19) return "Buenas tardes";
  return "Buenas noches";
}

function QuickActionCard({
  href,
  label,
  description,
  icon,
  infoTitle,
  infoContent,
  accent = "neutral",
}: (typeof PORTAL_QUICK_ACTIONS)[number]) {
  const accentStyles = {
    primary: "border-urrea-primary/15 bg-urrea-primary/[0.03] hover:border-urrea-primary/30",
    secondary: "border-urrea-secondary/15 bg-urrea-secondary/[0.03] hover:border-urrea-secondary/30",
    neutral: "border-urrea-border/70 bg-white hover:border-urrea-border",
  };

  return (
    <div
      className={cn(
        "group relative flex min-h-[5.5rem] flex-col justify-between rounded-2xl border p-4 transition",
        accentStyles[accent ?? "neutral"],
      )}
    >
      <Link href={href} className="absolute inset-0 z-0 rounded-2xl" aria-label={label} />
      <div className="relative z-10 flex items-start justify-between gap-2 pointer-events-none">
        <span className="flex h-10 w-10 items-center justify-center rounded-xl bg-white text-urrea-primary shadow-sm ring-1 ring-urrea-border/50">
          <DhIcon name={icon} className="h-5 w-5" />
        </span>
        <span className="pointer-events-auto">
          <InfoButton title={infoTitle} label={`Ayuda: ${label}`}>
            {infoContent}
          </InfoButton>
        </span>
      </div>
      <div className="relative z-10 pointer-events-none">
        <p className="text-sm font-semibold text-urrea-text">{label}</p>
        <p className="text-xs text-urrea-text-muted">{description}</p>
      </div>
    </div>
  );
}

function AnnouncementCard({ post }: { post: FeedPost }) {
  return (
    <article className="rounded-2xl border border-urrea-border/70 bg-white p-4">
      <div className="flex items-start gap-3">
        <div
          className={cn(
            "flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-gradient-to-br text-[10px] font-bold text-white",
            avatarGradient(post.authorName),
          )}
        >
          {post.authorInitials}
        </div>
        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-center gap-2">
            <p className="text-sm font-medium text-urrea-text">{post.authorName}</p>
            <span className="rounded-md bg-urrea-bg-soft px-1.5 py-0.5 text-[10px] font-medium text-urrea-text-muted">
              {TYPE_LABELS[post.type]}
            </span>
          </div>
          <p className="text-[11px] text-urrea-text-muted">{formatFeedTime(post.createdAt)}</p>
          <p className="mt-2 text-sm leading-relaxed text-urrea-text">{post.content}</p>
        </div>
      </div>
    </article>
  );
}

export function PortalFeed() {
  const session = getSession();
  const [perfil, setPerfil] = useState<ColaboradorPerfil | null>(null);
  const [saldo, setSaldo] = useState<SaldoVacaciones | null>(null);
  const [solicitudes, setSolicitudes] = useState<SolicitudAusencia[]>([]);
  const [feedPosts, setFeedPosts] = useState<FeedPost[]>([]);

  useEffect(() => {
    portalService.getMe().then(setPerfil).catch(console.error);
    absenceService.getMyBalance().then(setSaldo).catch(console.error);
    absenceService.getMyRequests().then(setSolicitudes).catch(console.error);
    portalService.getFeed().then(setFeedPosts).catch(console.error);
  }, []);

  const firstName = session?.nombreCompleto.split(" ")[0] ?? "Colaborador";
  const pendientes = solicitudes.filter((s) => s.estado === "Pendiente").length;
  const jefe = isJefe(session);

  const quickActions = useMemo(
    () => PORTAL_QUICK_ACTIONS.filter((a) => !a.jefeOnly || jefe),
    [jefe],
  );

  return (
    <div className="space-y-6 animate-fade-up px-4 py-4 sm:px-6 sm:py-6">
      {/* Saludo */}
      <section>
        <div className="flex items-start justify-between gap-3">
          <div>
            <p className="text-sm text-urrea-text-muted">{greeting()},</p>
            <h1 className="text-2xl font-semibold tracking-tight text-urrea-text sm:text-3xl">{firstName}</h1>
            <p className="mt-0.5 text-sm text-urrea-text-muted">{perfil?.puesto ?? "Portal colaborador URREA"}</p>
          </div>
          <InfoButton title={PORTAL_HELP.home.title}>{PORTAL_HELP.home.content}</InfoButton>
        </div>
      </section>

      {/* Estado rápido */}
      {(saldo || pendientes > 0) && (
        <section className="grid gap-3 sm:grid-cols-2">
          {saldo && (
            <div className="rounded-2xl border border-urrea-primary/15 bg-white p-4">
              <p className="text-xs font-medium uppercase tracking-wide text-urrea-text-muted">Vacaciones {saldo.anio}</p>
              <p className="mt-1 text-3xl font-semibold tabular-nums text-urrea-primary">{saldo.diasPendientes}</p>
              <p className="text-sm text-urrea-text-muted">días disponibles</p>
              <ButtonLink href="/portal/vacaciones?tipo=vac" className="mt-3 w-full sm:w-auto">
                Solicitar vacaciones
              </ButtonLink>
            </div>
          )}
          {pendientes > 0 && (
            <div className="rounded-2xl border border-amber-200 bg-amber-50/80 p-4">
              <div className="flex items-center gap-2">
                <p className="text-sm font-medium text-amber-900">{pendientes} solicitud(es) en proceso</p>
                <InfoButton title="Solicitudes en trámite">
                  Tus solicitudes de vacaciones o permisos están siendo revisadas. Recibirás notificación cuando se resuelvan.
                </InfoButton>
              </div>
              <Link href="/portal/vacaciones" className="mt-2 inline-block text-sm font-medium text-amber-800 underline-offset-2 hover:underline">
                Ver mis solicitudes
              </Link>
            </div>
          )}
          {jefe && (
            <div className="rounded-2xl border border-urrea-border/70 bg-white p-4 sm:col-span-2">
              <div className="flex items-center justify-between gap-2">
                <p className="text-sm font-medium text-urrea-text">Como jefe</p>
                <InfoButton title={PORTAL_HELP.pending.title}>{PORTAL_HELP.pending.content}</InfoButton>
              </div>
              <div className="mt-3 flex flex-wrap gap-2">
                <ButtonLink href="/portal/aprobaciones" variant="secondary">
                  Aprobaciones
                </ButtonLink>
                <ButtonLink href="/portal/mi-equipo" variant="ghost">
                  Mi equipo
                </ButtonLink>
              </div>
            </div>
          )}
        </section>
      )}

      {/* Accesos rápidos */}
      <section>
        <SectionHeading
          title="Accesos rápidos"
          infoTitle="Accesos rápidos"
          infoContent="Los trámites más frecuentes en un solo toque. Toca el ícono ℹ️ en cada tarjeta para saber qué hace cada sección."
          className="mb-3"
        />
        <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 lg:grid-cols-4">
          {quickActions.map((action) => (
            <QuickActionCard key={action.href} {...action} />
          ))}
        </div>
      </section>

      {/* Actividad reciente */}
      {solicitudes.length > 0 && (
        <section>
          <SectionHeading title="Tu actividad reciente" className="mb-3" />
          <ul className="space-y-2">
            {solicitudes.slice(0, 4).map((s) => (
              <li key={s.id} className="flex items-center justify-between gap-3 rounded-xl border border-urrea-border/60 bg-white px-4 py-3">
                <div className="min-w-0">
                  <p className="truncate text-sm font-medium text-urrea-text">{s.tipoAusenciaNombre}</p>
                  <p className="text-xs text-urrea-text-muted">
                    {new Date(s.fechaInicio).toLocaleDateString("es-MX", { day: "numeric", month: "short" })}
                  </p>
                </div>
                <Badge estado={s.estado} />
              </li>
            ))}
          </ul>
        </section>
      )}

      {/* Comunicados */}
      {feedPosts.length > 0 && (
        <section>
          <SectionHeading
            title="Comunicados URREA"
            infoTitle={PORTAL_HELP.announcements.title}
            infoContent={PORTAL_HELP.announcements.content}
            className="mb-3"
          />
          <div className="space-y-3">
            {feedPosts.slice(0, 5).map((post) => (
              <AnnouncementCard key={post.id} post={post} />
            ))}
          </div>
        </section>
      )}
    </div>
  );
}
