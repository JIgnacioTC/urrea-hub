"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useState } from "react";
import { DhIcon } from "@/components/dh/shared/icons";
import { avatarGradient, getInitials } from "@/components/portal/profile-helpers";
import { PortalMobileMenu, MenuIcon } from "@/components/portal/PortalMobileMenu";
import { getSession, isRhAdmin, type Session } from "@/lib/auth";
import {
  buildPortalSections,
  getNavItemByPath,
  isNavActive,
  PORTAL_HOME,
  type PortalNavLink,
  type PortalNavSection,
} from "@/lib/portal/navigation";
import type { ColaboradorPerfil } from "@/lib/types";
import { cn } from "@/lib/utils";

function CollaboratorSidebar({
  session,
  perfil,
  sections,
  pathname,
  onLogout,
}: {
  session: Session;
  perfil: ColaboradorPerfil | null;
  sections: PortalNavSection[];
  pathname: string;
  onLogout: () => void;
}) {
  const firstName = session.nombreCompleto.split(" ")[0];

  return (
    <aside className="hidden w-64 shrink-0 flex-col border-r border-urrea-border/70 bg-white lg:flex xl:w-72">
      <div className="border-b border-urrea-border/60 px-5 py-5">
        <div className="flex items-center gap-3">
          <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-urrea-primary text-xs font-bold text-white">
            U
          </div>
          <div className="min-w-0">
            <p className="truncate text-sm font-semibold text-urrea-text">URREA Hub</p>
            <p className="truncate text-xs text-urrea-text-muted">Hola, {firstName}</p>
          </div>
        </div>
      </div>

      <nav className="flex-1 overflow-y-auto px-3 py-4">
        <Link
          href={PORTAL_HOME.href}
          className={cn(
            "mb-4 flex min-h-10 items-center gap-3 rounded-xl px-3 py-2 text-sm font-medium transition",
            isNavActive(pathname, PORTAL_HOME.href)
              ? "bg-urrea-primary text-white"
              : "text-urrea-text-muted hover:bg-urrea-bg-soft hover:text-urrea-text",
          )}
        >
          <DhIcon name="dashboard" className="h-[18px] w-[18px]" />
          Inicio
        </Link>

        {sections.map((section) => (
          <div key={section.title} className="mb-5">
            <p className="mb-1.5 px-3 text-[10px] font-semibold uppercase tracking-wider text-urrea-text-muted">
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
                      "flex min-h-10 items-center gap-3 rounded-xl px-3 py-2 text-sm transition",
                      active
                        ? "bg-urrea-primary/8 font-medium text-urrea-primary"
                        : "text-urrea-text-muted hover:bg-urrea-bg-soft hover:text-urrea-text",
                    )}
                  >
                    <DhIcon name={item.icon} className={cn("h-[18px] w-[18px]", active ? "text-urrea-primary" : "")} />
                    <span className="truncate">{item.label}</span>
                  </Link>
                );
              })}
            </div>
          </div>
        ))}
      </nav>

      <div className="border-t border-urrea-border/60 p-4 space-y-2">
        {isRhAdmin(session) && (
          <Link href="/admin-dh" className="block text-xs text-urrea-secondary hover:underline">
            Administración DH →
          </Link>
        )}
        <button
          type="button"
          onClick={onLogout}
          className="w-full rounded-xl px-3 py-2 text-left text-xs text-urrea-text-muted transition hover:bg-urrea-bg-soft hover:text-urrea-text"
        >
          Cerrar sesión
        </button>
      </div>
    </aside>
  );
}

function CollaboratorMobileHeader({
  session,
  perfil,
  sections,
  pathname,
}: {
  session: Session;
  perfil: ColaboradorPerfil | null;
  sections: PortalNavSection[];
  pathname: string;
}) {
  const current = getNavItemByPath(pathname, sections);
  const initials = perfil
    ? getInitials(perfil.nombre, perfil.apellidoPaterno, perfil.apellidoMaterno)
    : session.nombreCompleto.slice(0, 2).toUpperCase();

  return (
    <header className="sticky top-0 z-30 border-b border-urrea-border/60 bg-white/95 backdrop-blur-md lg:hidden">
      <div className="flex h-14 items-center justify-between gap-3 px-4">
        <div className="min-w-0 flex-1">
          <p className="text-[10px] font-medium uppercase tracking-wider text-urrea-text-muted">URREA Hub</p>
          <h1 className="truncate text-base font-semibold text-urrea-text">{current?.label ?? "Inicio"}</h1>
        </div>
        <Link
          href="/portal/mi-ficha"
          className={cn(
            "flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-gradient-to-br text-xs font-bold text-white",
            avatarGradient(session.colaboradorId),
          )}
          aria-label="Mi ficha"
        >
          {initials}
        </Link>
      </div>
    </header>
  );
}

function CollaboratorDesktopHeader({
  session,
  perfil,
  sections,
  pathname,
}: {
  session: Session;
  perfil: ColaboradorPerfil | null;
  sections: PortalNavSection[];
  pathname: string;
}) {
  const current = getNavItemByPath(pathname, sections);
  const initials = perfil
    ? getInitials(perfil.nombre, perfil.apellidoPaterno, perfil.apellidoMaterno)
    : session.nombreCompleto.slice(0, 2).toUpperCase();

  return (
    <header className="sticky top-0 z-20 hidden border-b border-urrea-border/60 bg-white/95 backdrop-blur-md lg:block">
      <div className="flex h-14 items-center justify-between px-6 xl:px-8">
        <div>
          <p className="text-sm font-semibold text-urrea-text">{current?.label ?? "Portal"}</p>
          <p className="text-xs text-urrea-text-muted">{perfil?.puesto ?? session.numeroEmpleado}</p>
        </div>
        <Link href="/portal/mi-ficha" className="flex items-center gap-2.5 rounded-xl px-2 py-1 transition hover:bg-urrea-bg-soft">
          <div
            className={cn(
              "flex h-8 w-8 items-center justify-center rounded-full bg-gradient-to-br text-[10px] font-bold text-white",
              avatarGradient(session.colaboradorId),
            )}
          >
            {initials}
          </div>
          <span className="text-sm font-medium text-urrea-text">{session.nombreCompleto.split(" ")[0]}</span>
        </Link>
      </div>
    </header>
  );
}

function CollaboratorBottomNav({
  links,
  pathname,
  onOpenMenu,
  menuOpen,
}: {
  links: PortalNavLink[];
  pathname: string;
  onOpenMenu: () => void;
  menuOpen: boolean;
}) {
  const navItems = links.filter((l) => l.href !== "__menu__");

  return (
    <nav className="fixed inset-x-0 bottom-0 z-40 border-t border-urrea-border/70 bg-white/95 backdrop-blur-md lg:hidden safe-bottom">
      <div className="flex">
        {navItems.map((item) => {
          const active = isNavActive(pathname, item.href);
          return (
            <Link
              key={item.href}
              href={item.href}
              className={cn(
                "flex min-h-[3.25rem] flex-1 flex-col items-center justify-center gap-0.5 px-1 pt-2 text-[10px] font-medium",
                active ? "text-urrea-primary" : "text-urrea-text-muted",
              )}
            >
              <DhIcon name={item.icon} className={cn("h-5 w-5", active && "text-urrea-primary")} />
              <span className="truncate max-w-full">{item.shortLabel ?? item.label.split(" ")[0]}</span>
              {active && <span className="h-0.5 w-5 rounded-full bg-urrea-primary" />}
            </Link>
          );
        })}
        <button
          type="button"
          onClick={onOpenMenu}
          className={cn(
            "flex min-h-[3.25rem] flex-1 flex-col items-center justify-center gap-0.5 px-1 pt-2 text-[10px] font-medium",
            menuOpen ? "text-urrea-primary" : "text-urrea-text-muted",
          )}
          aria-label="Abrir menú"
          aria-expanded={menuOpen}
        >
          <MenuIcon className="h-5 w-5" />
          <span>Más</span>
        </button>
      </div>
    </nav>
  );
}

export function CollaboratorShellLayout({
  session,
  perfil,
  sections,
  mobileNavLinks,
  pathname,
  onLogout,
  children,
}: {
  session: Session;
  perfil: ColaboradorPerfil | null;
  sections: PortalNavSection[];
  mobileNavLinks: PortalNavLink[];
  pathname: string;
  onLogout: () => void;
  children: React.ReactNode;
}) {
  const [menuOpen, setMenuOpen] = useState(false);

  return (
    <div className="flex h-[100dvh] bg-urrea-bg-soft">
      <CollaboratorSidebar
        session={session}
        perfil={perfil}
        sections={sections}
        pathname={pathname}
        onLogout={onLogout}
      />

      <div className="flex min-w-0 flex-1 flex-col">
        <CollaboratorMobileHeader session={session} perfil={perfil} sections={sections} pathname={pathname} />
        <CollaboratorDesktopHeader session={session} perfil={perfil} sections={sections} pathname={pathname} />

        <main className="main-with-mobile-nav flex-1 overflow-y-auto">
          <div className="mx-auto w-full max-w-3xl sm:max-w-4xl lg:max-w-5xl">
            {children}
          </div>
        </main>
      </div>

      <CollaboratorBottomNav
        links={mobileNavLinks}
        pathname={pathname}
        onOpenMenu={() => setMenuOpen(true)}
        menuOpen={menuOpen}
      />

      <PortalMobileMenu
        open={menuOpen}
        onClose={() => setMenuOpen(false)}
        sections={sections}
        pathname={pathname}
        onLogout={onLogout}
      />
    </div>
  );
}
