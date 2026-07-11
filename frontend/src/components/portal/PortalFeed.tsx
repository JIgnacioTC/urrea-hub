"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import { DhIcon } from "@/components/dh/shared/icons";
import { InfoButton, SectionHeading } from "@/components/ui/info-button";
import { Badge } from "@/components/ui/badge";
import { Button, ButtonLink } from "@/components/ui/button";
import { avatarGradient, initialsFromFullName } from "@/components/portal/profile-helpers";
import { portalService } from "@/lib/services/portalService";
import { absenceService } from "@/lib/services/absenceService";
import { getSession, isJefe } from "@/lib/auth";
import { formatFeedTime } from "@/lib/portal-feed";
import { PORTAL_HELP, PORTAL_QUICK_ACTIONS } from "@/lib/portal/portal-home";
import type { ColaboradorPerfil, FeedComment, FeedPost, SaldoVacaciones, SolicitudAusencia } from "@/lib/types";
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

function Avatar({ seed, initials, size = 9 }: { seed: string; initials: string; size?: number }) {
  return (
    <div
      className={cn(
        "flex shrink-0 items-center justify-center rounded-full bg-gradient-to-br font-bold text-white",
        avatarGradient(seed),
      )}
      style={{ height: `${size * 0.25}rem`, width: `${size * 0.25}rem`, fontSize: size >= 9 ? "10px" : "9px" }}
    >
      {initials}
    </div>
  );
}

function ComposeBox({
  authorName,
  authorInitials,
  onPublish,
}: {
  authorName: string;
  authorInitials: string;
  onPublish: (contenido: string, tipo: "General" | "Recognition") => Promise<void>;
}) {
  const [content, setContent] = useState("");
  const [tipo, setTipo] = useState<"General" | "Recognition">("General");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  async function submit() {
    if (!content.trim()) return;
    setLoading(true);
    setError("");
    try {
      await onPublish(content.trim(), tipo);
      setContent("");
      setTipo("General");
    } catch (err) {
      setError(err instanceof Error ? err.message : "No se pudo publicar");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="rounded-2xl border border-urrea-border/70 bg-white p-4">
      <div className="flex gap-3">
        <Avatar seed={authorName} initials={authorInitials} />
        <div className="min-w-0 flex-1">
          <textarea
            value={content}
            onChange={(e) => setContent(e.target.value)}
            placeholder={`¿Qué quieres compartir con URREA, ${authorName.split(" ")[0]}?`}
            rows={2}
            className="w-full resize-none rounded-xl border-0 bg-transparent text-[15px] leading-relaxed text-urrea-text placeholder:text-urrea-text-muted focus:outline-none"
          />
          {error && <p className="mt-1 text-xs font-medium text-red-600">{error}</p>}
          <div className="mt-2 flex items-center justify-between gap-2 border-t border-urrea-border/60 pt-3">
            <div className="flex gap-1.5">
              <button
                type="button"
                onClick={() => setTipo("General")}
                className={cn(
                  "rounded-full px-3 py-1.5 text-xs font-medium transition",
                  tipo === "General" ? "bg-urrea-primary text-white" : "bg-urrea-bg-soft text-urrea-text-muted hover:text-urrea-text",
                )}
              >
                General
              </button>
              <button
                type="button"
                onClick={() => setTipo("Recognition")}
                className={cn(
                  "rounded-full px-3 py-1.5 text-xs font-medium transition",
                  tipo === "Recognition" ? "bg-urrea-primary text-white" : "bg-urrea-bg-soft text-urrea-text-muted hover:text-urrea-text",
                )}
              >
                🎉 Reconocimiento
              </button>
            </div>
            <Button type="button" onClick={submit} disabled={loading || !content.trim()} className="!min-h-9 !px-4 !py-1.5">
              Publicar
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}

function CommentsSection({ postId, myInitials }: { postId: string; myInitials: string }) {
  const [comments, setComments] = useState<FeedComment[] | null>(null);
  const [draft, setDraft] = useState("");
  const [sending, setSending] = useState(false);

  useEffect(() => {
    portalService.getComments(postId).then(setComments).catch(() => setComments([]));
  }, [postId]);

  async function submit() {
    if (!draft.trim()) return;
    setSending(true);
    try {
      const created = await portalService.addComment(postId, draft.trim());
      setComments((prev) => [...(prev ?? []), created]);
      setDraft("");
    } catch {
      /* silencioso */
    } finally {
      setSending(false);
    }
  }

  return (
    <div className="mt-3 space-y-3 border-t border-urrea-border/60 pt-3">
      {comments === null ? (
        <p className="text-xs text-urrea-text-muted">Cargando comentarios…</p>
      ) : (
        comments.map((c) => (
          <div key={c.id} className="flex items-start gap-2.5">
            <Avatar seed={c.authorName} initials={c.authorInitials} size={7} />
            <div className="min-w-0 flex-1 rounded-2xl bg-urrea-bg-soft px-3 py-2">
              <p className="text-xs font-semibold text-urrea-text">{c.authorName}</p>
              <p className="text-sm leading-snug text-urrea-text">{c.content}</p>
            </div>
          </div>
        ))
      )}
      <div className="flex items-center gap-2.5">
        <Avatar seed="tú" initials={myInitials} size={7} />
        <div className="flex min-w-0 flex-1 items-center gap-2 rounded-full border border-urrea-border/70 bg-white pl-3 pr-1.5">
          <input
            value={draft}
            onChange={(e) => setDraft(e.target.value)}
            onKeyDown={(e) => {
              if (e.key === "Enter" && !e.shiftKey) {
                e.preventDefault();
                submit();
              }
            }}
            placeholder="Escribe un comentario…"
            className="min-h-9 w-full bg-transparent text-sm text-urrea-text placeholder:text-urrea-text-muted focus:outline-none"
          />
          <button
            type="button"
            onClick={submit}
            disabled={sending || !draft.trim()}
            className="flex h-7 w-7 shrink-0 items-center justify-center rounded-full text-urrea-primary transition hover:bg-urrea-primary/10 disabled:opacity-40"
            aria-label="Enviar comentario"
          >
            <DhIcon name="send" className="h-4 w-4" />
          </button>
        </div>
      </div>
    </div>
  );
}

function FeedPostCard({
  post,
  myInitials,
  onToggleLike,
  onDelete,
}: {
  post: FeedPost;
  myInitials: string;
  onToggleLike: (id: string) => void;
  onDelete: (id: string) => void;
}) {
  const [showComments, setShowComments] = useState(false);

  return (
    <article className="rounded-2xl border border-urrea-border/70 bg-white p-4">
      <div className="flex items-start gap-3">
        <Avatar seed={post.authorName} initials={post.authorInitials} />
        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-center gap-2">
            <p className="text-sm font-semibold text-urrea-text">{post.authorName}</p>
            {post.type !== "general" && (
              <span className="rounded-md bg-urrea-bg-soft px-1.5 py-0.5 text-[10px] font-medium text-urrea-text-muted">
                {TYPE_LABELS[post.type]}
              </span>
            )}
            {post.isOwnPost && (
              <button
                type="button"
                onClick={() => onDelete(post.id)}
                className="ml-auto flex h-7 w-7 items-center justify-center rounded-full text-urrea-text-muted transition hover:bg-red-50 hover:text-red-600"
                aria-label="Eliminar publicación"
              >
                <DhIcon name="trash" className="h-3.5 w-3.5" />
              </button>
            )}
          </div>
          <p className="text-[11px] text-urrea-text-muted">
            {post.authorRole && `${post.authorRole} · `}
            {formatFeedTime(post.createdAt)}
          </p>
          <p className="mt-2 whitespace-pre-line text-sm leading-relaxed text-urrea-text">{post.content}</p>

          <div className="mt-3 flex items-center gap-1 border-t border-urrea-border/60 pt-2">
            <button
              type="button"
              onClick={() => onToggleLike(post.id)}
              className={cn(
                "flex flex-1 items-center justify-center gap-1.5 rounded-lg py-1.5 text-xs font-medium transition",
                post.likedByMe ? "text-red-600 hover:bg-red-50" : "text-urrea-text-muted hover:bg-urrea-bg-soft",
              )}
            >
              <DhIcon name={post.likedByMe ? "heart-filled" : "heart"} className="h-4 w-4" />
              {post.likes > 0 ? post.likes : "Me gusta"}
            </button>
            <button
              type="button"
              onClick={() => setShowComments((v) => !v)}
              className={cn(
                "flex flex-1 items-center justify-center gap-1.5 rounded-lg py-1.5 text-xs font-medium transition",
                showComments ? "text-urrea-primary bg-urrea-primary/5" : "text-urrea-text-muted hover:bg-urrea-bg-soft",
              )}
            >
              <DhIcon name="comment" className="h-4 w-4" />
              {post.comments > 0 ? post.comments : "Comentar"}
            </button>
          </div>

          {showComments && <CommentsSection postId={post.id} myInitials={myInitials} />}
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
  const myInitials = session ? initialsFromFullName(session.nombreCompleto) : "?";
  const pendientes = solicitudes.filter((s) => s.estado === "Pendiente").length;
  const jefe = isJefe(session);

  const quickActions = useMemo(
    () => PORTAL_QUICK_ACTIONS.filter((a) => !a.jefeOnly || jefe),
    [jefe],
  );

  async function handlePublish(contenido: string, tipo: "General" | "Recognition") {
    const created = await portalService.createPost(contenido, tipo);
    setFeedPosts((prev) => [created, ...prev]);
  }

  async function handleToggleLike(id: string) {
    setFeedPosts((prev) =>
      prev.map((p) =>
        p.id === id
          ? { ...p, likedByMe: !p.likedByMe, likes: p.likes + (p.likedByMe ? -1 : 1) }
          : p,
      ),
    );
    try {
      const result = await portalService.toggleLike(id);
      setFeedPosts((prev) => prev.map((p) => (p.id === id ? { ...p, likedByMe: result.liked, likes: result.totalLikes } : p)));
    } catch {
      // revertir si falla
      setFeedPosts((prev) =>
        prev.map((p) =>
          p.id === id
            ? { ...p, likedByMe: !p.likedByMe, likes: p.likes + (p.likedByMe ? -1 : 1) }
            : p,
        ),
      );
    }
  }

  async function handleDelete(id: string) {
    if (!confirm("¿Eliminar esta publicación?")) return;
    setFeedPosts((prev) => prev.filter((p) => p.id !== id));
    try {
      await portalService.deletePost(id);
    } catch {
      portalService.getFeed().then(setFeedPosts).catch(console.error);
    }
  }

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

      {/* Accesos rápidos — franja horizontal compacta */}
      <section>
        <div className="-mx-4 flex gap-2.5 overflow-x-auto px-4 pb-1 sm:mx-0 sm:grid sm:grid-cols-3 sm:gap-3 sm:overflow-visible sm:px-0 lg:grid-cols-4">
          {quickActions.map((action) => (
            <div key={action.href} className="w-[8.5rem] shrink-0 sm:w-auto">
              <QuickActionCard {...action} />
            </div>
          ))}
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

      {/* Feed social */}
      <section>
        <SectionHeading
          title="Feed URREA"
          infoTitle={PORTAL_HELP.announcements.title}
          infoContent={PORTAL_HELP.announcements.content}
          className="mb-3"
        />
        <div className="space-y-3">
          <ComposeBox authorName={session?.nombreCompleto ?? "Colaborador"} authorInitials={myInitials} onPublish={handlePublish} />
          {feedPosts.map((post) => (
            <FeedPostCard key={post.id} post={post} myInitials={myInitials} onToggleLike={handleToggleLike} onDelete={handleDelete} />
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
    </div>
  );
}
