"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useEffect, useMemo, useState } from "react";
import { DhIcon } from "@/components/dh/shared/icons";
import { DhSearchInput } from "@/components/dh/shared/ui";
import { CollaboratorShellLayout } from "@/components/layout/CollaboratorShell";
import { avatarGradient, getInitials } from "@/components/portal/profile-helpers";
import { portalService } from "@/lib/services/portalService";
import { clearSession, getSession, isRhAdmin, isTiAdmin, type Session } from "@/lib/auth";
import {
  ADMIN_DH_SECTIONS,
  ADMIN_TI_SECTIONS,
  buildMobileNav,
  buildPortalSections,
  filterNavSections,
  flattenRhLinks,
  flattenTiLinks,
  getNavItemByPath,
  isNavActive,
  PORTAL_HOME,
  RH_SECTIONS,
  TI_SECTIONS,
  type PortalNavLink,
  type PortalNavSection,
} from "@/lib/portal/navigation";
import type { ColaboradorPerfil } from "@/lib/types";
import { cn } from "@/lib/utils";

export { Card, StatCard } from "@/components/ui/card";
export { Badge as EstadoBadge } from "@/components/ui/badge";
export { PageHeader, PageContainer, EmptyState, Alert } from "@/components/ui/page-header";
export { InfoButton, SectionHeading } from "@/components/ui/info-button";

function LoadingScreen() {
  return (
    <div className="flex h-[100dvh] items-center justify-center bg-slate-100">
      <div className="h-8 w-8 animate-pulse rounded-full bg-urrea-primary/20" />
    </div>
  );
}

function EnterpriseTopBar({
  session,
  perfil,
  sections,
  variant,
}: {
  session: Session;
  perfil: ColaboradorPerfil | null;
  sections: PortalNavSection[];
  variant: "portal" | "rh" | "ti";
}) {
  const pathname = usePathname();
  const current = getNavItemByPath(pathname, sections);
  const [search, setSearch] = useState("");

  return (
    <header className="sticky top-0 z-30 border-b border-slate-200 bg-white">
      <div className="flex h-14 items-center gap-4 px-4 lg:px-6">
        <div className="hidden min-w-0 flex-1 lg:block">
          <p className="truncate text-sm font-semibold text-urrea-text">{current?.label ?? "Portal URREA"}</p>
          <p className="truncate text-[11px] text-urrea-text-muted">
            {variant === "rh" ? "Administración RH" : variant === "ti" ? "Plataforma TI" : perfil?.puesto ?? "Colaborador URREA"}
          </p>
        </div>

        <div className="flex flex-1 items-center gap-3 lg:max-w-md lg:flex-none">
          <div className="relative flex-1">
            <DhSearchInput value={search} onChange={setSearch} placeholder="Buscar en el portal…" />
          </div>
        </div>

        <div className="hidden items-center gap-3 border-l border-urrea-border pl-4 sm:flex">
          <div
            className={cn(
              "flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-gradient-to-br text-xs font-bold text-white",
              avatarGradient(session.colaboradorId),
            )}
          >
            {perfil
              ? getInitials(perfil.nombre, perfil.apellidoPaterno, perfil.apellidoMaterno)
              : session.nombreCompleto.slice(0, 2).toUpperCase()}
          </div>
          <div className="text-right">
            <p className="text-xs font-semibold text-urrea-text">{session.nombreCompleto}</p>
            <p className="text-[10px] text-urrea-text-muted">
              {isRhAdmin(session) ? "Administrador RH" : perfil?.departamento ?? session.numeroEmpleado}
            </p>
          </div>
        </div>
      </div>
    </header>
  );
}

function EnterpriseSidebar({
  session,
  perfil,
  sections,
  pathname,
  onLogout,
  variant,
}: {
  session: Session;
  perfil: ColaboradorPerfil | null;
  sections: PortalNavSection[];
  pathname: string;
  onLogout: () => void;
  variant: "portal" | "rh" | "ti";
}) {
  const activosLabel = perfil ? session.nombreCompleto.split(" ")[0] : "URREA";

  return (
    <aside className="hidden w-[17.5rem] shrink-0 flex-col bg-[#011829] text-slate-300 lg:flex xl:w-72">
      <div className="border-b border-white/10 px-5 py-5">
        <div className="flex items-center gap-3">
          <div className="flex h-10 w-10 items-center justify-center border border-white/20 bg-white/5">
            <DhIcon name="building" className="h-5 w-5 text-white" />
          </div>
          <div className="min-w-0">
            <p className="truncate text-sm font-bold tracking-wide text-white">URREA</p>
            <p className="truncate text-[10px] uppercase tracking-[0.12em] text-slate-400">
              {variant === "rh" ? "Administración RH" : "Portal colaborador"}
            </p>
          </div>
        </div>
        <p className="mt-3 text-[10px] leading-relaxed text-slate-500">
          Hub · v1.0 · {activosLabel}
        </p>
      </div>

      <nav className="flex-1 overflow-y-auto px-3 py-4">
        <div className="mb-6">
          <Link
            href={variant === "rh" ? "/rh/dashboard" : PORTAL_HOME.href}
            className={cn(
              "group flex min-h-10 items-center gap-3 border-l-2 px-3 py-2 text-[13px] font-medium transition",
              isNavActive(pathname, variant === "rh" ? "/rh/dashboard" : PORTAL_HOME.href)
                ? "border-l-white bg-white/10 text-white"
                : "border-l-transparent text-slate-400 hover:bg-white/5 hover:text-white",
            )}
          >
            <DhIcon
              name="dashboard"
              className={cn(
                "h-4 w-4",
                isNavActive(pathname, variant === "rh" ? "/rh/dashboard" : PORTAL_HOME.href)
                  ? "text-white"
                  : "text-slate-500 group-hover:text-slate-300",
              )}
            />
            <span>{variant === "rh" ? "Dashboard RH" : "Inicio portal"}</span>
          </Link>
        </div>

        {sections.map((section) => (
          <div key={section.title} className="mb-6">
            <p className="mb-2 px-3 text-[10px] font-bold uppercase tracking-[0.14em] text-slate-500">
              {section.title}
            </p>
            <div className="space-y-0.5">
              {section.links.map((item) => {
                const active = isNavActive(pathname, item.href);
                return (
                  <Link
                    key={item.href}
                    href={item.href}
                    className={cn(
                      "group flex min-h-10 items-center gap-3 border-l-2 px-3 py-2 text-[13px] font-medium transition",
                      active
                        ? "border-l-white bg-white/10 text-white"
                        : "border-l-transparent text-slate-400 hover:bg-white/5 hover:text-white",
                    )}
                  >
                    <DhIcon
                      name={item.icon}
                      className={cn("h-4 w-4", active ? "text-white" : "text-slate-500 group-hover:text-slate-300")}
                    />
                    <span className="truncate leading-snug">{item.label}</span>
                  </Link>
                );
              })}
            </div>
          </div>
        ))}
      </nav>

      <div className="border-t border-white/10 p-4">
        {variant === "portal" && isRhAdmin(session) && (
          <Link href="/rh/dashboard" className="mb-2 block text-[11px] text-slate-400 hover:text-white">
            Panel administración RH →
          </Link>
        )}
        {variant === "rh" && (
          <Link href="/portal" className="mb-2 block text-[11px] text-slate-400 hover:text-white">
            Portal colaborador →
          </Link>
        )}
        <button
          type="button"
          onClick={onLogout}
          className="w-full border border-white/10 px-3 py-2 text-left text-[11px] text-slate-400 transition hover:border-white/20 hover:text-white"
        >
          Cerrar sesión
        </button>
      </div>
    </aside>
  );
}

function MobileHeader({
  session,
  perfil,
  onLogout,
}: {
  session: Session;
  perfil: ColaboradorPerfil | null;
  onLogout: () => void;
}) {
  return (
    <header className="sticky top-0 z-30 border-b border-slate-200 bg-[#011829] text-white lg:hidden">
      <div className="flex items-center justify-between px-4 py-3.5">
        <div className="min-w-0 flex-1">
          <p className="text-[10px] font-medium uppercase tracking-[0.2em] text-slate-400">URREA Hub</p>
          <h1 className="truncate text-lg font-semibold tracking-tight">{session.nombreCompleto}</h1>
          <p className="truncate text-xs text-slate-400">{perfil?.puesto ?? "Portal"}</p>
        </div>
        <button
          type="button"
          onClick={onLogout}
          className="ml-2 shrink-0 border border-white/20 px-3 py-2 text-xs font-medium text-white/90 transition hover:bg-white/10"
        >
          Salir
        </button>
      </div>
    </header>
  );
}

function MobileBottomNav({ links, pathname }: { links: PortalNavLink[]; pathname: string }) {
  return (
    <nav className="fixed inset-x-0 bottom-0 z-40 border-t border-slate-200 bg-white lg:hidden safe-bottom">
      <div className="flex justify-around">
        {links.slice(0, 5).map((item) => {
          const active = isNavActive(pathname, item.href);
          return (
            <Link
              key={item.href}
              href={item.href}
              className={cn(
                "flex min-h-14 flex-1 flex-col items-center justify-center gap-1 px-1 text-[10px] font-semibold",
                active ? "text-urrea-primary" : "text-urrea-text-muted",
              )}
            >
              <DhIcon name={item.icon} className="h-4 w-4" />
              <span className="truncate">{item.shortLabel ?? item.label.split(" ")[0]}</span>
            </Link>
          );
        })}
      </div>
    </nav>
  );
}

function EnterpriseShellLayout({
  session,
  perfil,
  sections,
  pathname,
  onLogout,
  variant,
  mobileNavLinks,
  children,
}: {
  session: Session;
  perfil: ColaboradorPerfil | null;
  sections: PortalNavSection[];
  pathname: string;
  onLogout: () => void;
  variant: "portal" | "rh" | "ti";
  mobileNavLinks: PortalNavLink[];
  children: React.ReactNode;
}) {
  return (
    <div className="flex h-[100dvh] bg-slate-100 lg:flex-row">
      <EnterpriseSidebar
        session={session}
        perfil={perfil}
        sections={sections}
        pathname={pathname}
        onLogout={onLogout}
        variant={variant}
      />

      <div className="flex min-w-0 flex-1 flex-col">
        <MobileHeader session={session} perfil={perfil} onLogout={onLogout} />
        <EnterpriseTopBar session={session} perfil={perfil} sections={sections} variant={variant} />
        <main className="main-with-mobile-nav flex-1 overflow-y-auto">
          <div className="mx-auto max-w-[1600px] px-4 py-5 lg:px-8 lg:py-7">{children}</div>
        </main>
      </div>

      <MobileBottomNav links={mobileNavLinks} pathname={pathname} />
    </div>
  );
}

function PortalShellInner({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  const router = useRouter();
  const [session, setSession] = useState<Session | null>(null);
  const [perfil, setPerfil] = useState<ColaboradorPerfil | null>(null);

  useEffect(() => {
    const s = getSession();
    if (!s) {
      router.replace("/login");
      return;
    }
    setSession(s);
    portalService.getMe().then(setPerfil).catch(console.error);
  }, [router]);

  const sections = useMemo(() => (session ? buildPortalSections(session) : []), [session]);
  const mobileNavLinks = useMemo(() => (session ? buildMobileNav(session) : []), [session]);

  if (!session) return <LoadingScreen />;

  return (
    <CollaboratorShellLayout
      session={session}
      perfil={perfil}
      sections={sections}
      mobileNavLinks={mobileNavLinks}
      pathname={pathname}
      onLogout={() => {
        clearSession();
        router.replace("/login");
      }}
    >
      {children}
    </CollaboratorShellLayout>
  );
}

function RhShellInner({
  children,
  sectionsOverride,
}: {
  children: React.ReactNode;
  sectionsOverride?: PortalNavSection[];
}) {
  const pathname = usePathname();
  const router = useRouter();
  const [session, setSession] = useState<Session | null>(null);
  const [perfil, setPerfil] = useState<ColaboradorPerfil | null>(null);

  useEffect(() => {
    const s = getSession();
    if (!s) {
      router.replace("/login");
      return;
    }
    if (!isRhAdmin(s)) {
      router.replace("/portal");
      return;
    }
    setSession(s);
    portalService.getMe().then(setPerfil).catch(console.error);
  }, [router]);

  const sections = useMemo(
    () => (session ? filterNavSections(sectionsOverride ?? RH_SECTIONS, session) : []),
    [session, sectionsOverride],
  );

  if (!session) return <LoadingScreen />;

  return (
    <EnterpriseShellLayout
      session={session}
      perfil={perfil}
      sections={sections}
      pathname={pathname}
      onLogout={() => {
        clearSession();
        router.replace("/login");
      }}
      variant="rh"
      mobileNavLinks={flattenRhLinks(sections)}
    >
      {children}
    </EnterpriseShellLayout>
  );
}

function TiShellInner({
  children,
  sectionsOverride,
}: {
  children: React.ReactNode;
  sectionsOverride?: PortalNavSection[];
}) {
  const pathname = usePathname();
  const router = useRouter();
  const [session, setSession] = useState<Session | null>(null);
  const [perfil, setPerfil] = useState<ColaboradorPerfil | null>(null);

  useEffect(() => {
    const s = getSession();
    if (!s) {
      router.replace("/login");
      return;
    }
    if (!isTiAdmin(s)) {
      router.replace("/portal");
      return;
    }
    setSession(s);
    portalService.getMe().then(setPerfil).catch(console.error);
  }, [router]);

  const sections = useMemo(
    () => (session ? filterNavSections(sectionsOverride ?? TI_SECTIONS, session) : []),
    [session, sectionsOverride],
  );

  if (!session) return <LoadingScreen />;

  return (
    <EnterpriseShellLayout
      session={session}
      perfil={perfil}
      sections={sections}
      pathname={pathname}
      onLogout={() => {
        clearSession();
        router.replace("/login");
      }}
      variant="ti"
      mobileNavLinks={flattenTiLinks(sections)}
    >
      {children}
    </EnterpriseShellLayout>
  );
}

export function PortalShell({ children }: { children: React.ReactNode }) {
  return <PortalShellInner>{children}</PortalShellInner>;
}

export function RhShell({ children }: { children: React.ReactNode }) {
  return <RhShellInner>{children}</RhShellInner>;
}

export function TiShell({ children }: { children: React.ReactNode }) {
  return <TiShellInner>{children}</TiShellInner>;
}

export function AdminTiShell({ children }: { children: React.ReactNode }) {
  return <TiShellInner sectionsOverride={ADMIN_TI_SECTIONS}>{children}</TiShellInner>;
}

export function AdminDhShell({ children }: { children: React.ReactNode }) {
  return <RhShellInner sectionsOverride={ADMIN_DH_SECTIONS}>{children}</RhShellInner>;
}
