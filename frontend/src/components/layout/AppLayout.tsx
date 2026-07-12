"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useState } from "react";
import { DhIcon } from "@/components/dh/shared/icons";
import { UnifiedSidebar } from "@/components/layout/UnifiedSidebar";
import { SidebarProvider } from "@/components/layout/SidebarContext";
import { PortalSearch } from "@/components/layout/PortalSearch";
import { avatarGradient, getInitials } from "@/components/portal/profile-helpers";
import { clearSession, isRhAdmin, isTiAdmin, type Session } from "@/lib/auth";
import { PortalMobileMenu, MenuIcon } from "@/components/portal/PortalMobileMenu";
import {
  WorkspaceType,
  type PortalNavLink,
  type PortalNavSection,
  getNavItemByPath,
  isNavActive,
} from "@/lib/portal/navigation";
import type { ColaboradorPerfil } from "@/lib/types";
import { cn } from "@/lib/utils";

function MobileHeader({
  session,
  perfil,
  current,
}: {
  session: Session;
  perfil: ColaboradorPerfil | null;
  current: PortalNavLink | undefined;
}) {
  const initials = perfil
    ? getInitials(perfil.nombre, perfil.apellidoPaterno, perfil.apellidoMaterno)
    : session.nombreCompleto.slice(0, 2).toUpperCase();

  return (
    <header className="sticky top-0 z-30 flex h-14 items-center justify-between border-b border-urrea-border/60 bg-white/95 px-4 backdrop-blur-md lg:hidden">
      <div className="min-w-0 flex-1">
        <p className="text-[10px] font-medium uppercase tracking-wider text-urrea-text-muted">URREA Hub</p>
        <h1 className="truncate text-base font-semibold text-urrea-text">{current?.label ?? "Inicio"}</h1>
      </div>
      <Link
        href="/portal/mi-ficha"
        className={cn(
          "flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-gradient-to-br text-xs font-bold text-white",
          avatarGradient(session.colaboradorId)
        )}
      >
        {initials}
      </Link>
    </header>
  );
}

function DesktopHeader({
  session,
  perfil,
  current,
  sections,
}: {
  session: Session;
  perfil: ColaboradorPerfil | null;
  current: PortalNavLink | undefined;
  sections: PortalNavSection[];
}) {
  const initials = perfil
    ? getInitials(perfil.nombre, perfil.apellidoPaterno, perfil.apellidoMaterno)
    : session.nombreCompleto.slice(0, 2).toUpperCase();

  return (
    <header className="sticky top-0 z-20 hidden h-14 items-center justify-between border-b border-urrea-border/60 bg-white/95 px-6 backdrop-blur-md lg:flex xl:px-8">
      <div className="flex flex-1 items-center gap-4">
        <div className="min-w-0 flex-1 max-w-sm">
          <PortalSearch sections={sections} />
        </div>
      </div>

      <div className="flex items-center gap-3 border-l border-urrea-border/60 pl-4">
        <div className="text-right">
          <p className="text-xs font-semibold text-urrea-text">{session.nombreCompleto}</p>
          <p className="text-[10px] text-urrea-text-muted">{perfil?.puesto ?? session.numeroEmpleado}</p>
        </div>
        <Link href="/portal/mi-ficha" className="flex items-center gap-2.5 rounded-xl transition hover:opacity-80">
          <div
            className={cn(
              "flex h-9 w-9 items-center justify-center rounded-lg bg-gradient-to-br text-[10px] font-bold text-white",
              avatarGradient(session.colaboradorId)
            )}
          >
            {initials}
          </div>
        </Link>
      </div>
    </header>
  );
}

function MobileBottomNav({
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
  const navItems = links.slice(0, 4);

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
                active ? "text-urrea-primary" : "text-urrea-text-muted"
              )}
            >
              <DhIcon name={item.icon || "dashboard"} className={cn("h-5 w-5", active && "text-urrea-primary")} />
              <span className="truncate max-w-full">{item.shortLabel ?? item.label.split(" ")[0]}</span>
              {active && <span className="h-0.5 w-5 rounded-full bg-urrea-primary mt-0.5" />}
            </Link>
          );
        })}
        <button
          type="button"
          onClick={onOpenMenu}
          className={cn(
            "flex min-h-[3.25rem] flex-1 flex-col items-center justify-center gap-0.5 px-1 pt-2 text-[10px] font-medium",
            menuOpen ? "text-urrea-primary" : "text-urrea-text-muted"
          )}
        >
          <MenuIcon className="h-5 w-5" />
          <span>Menú</span>
        </button>
      </div>
    </nav>
  );
}

export function AppLayout({
  session,
  perfil,
  sections,
  mobileNavLinks,
  initialWorkspace,
  children,
}: {
  session: Session;
  perfil: ColaboradorPerfil | null;
  sections: PortalNavSection[];
  mobileNavLinks: PortalNavLink[];
  initialWorkspace: WorkspaceType;
  children: React.ReactNode;
}) {
  const router = useRouter();
  const pathname = usePathname();
  const [menuOpen, setMenuOpen] = useState(false);
  const [workspace, setWorkspace] = useState<WorkspaceType>(initialWorkspace);

  const handleLogout = () => {
    clearSession();
    router.replace("/login");
  };

  const handleWorkspaceChange = (ws: WorkspaceType) => {
    setWorkspace(ws);
    if (ws === "portal") router.push("/portal");
    else if (ws === "rh") router.push("/rh/dashboard");
    else if (ws === "ti") router.push("/ti");
    else if (ws === "admin-ti") router.push("/admin-ti");
    else if (ws === "admin-dh") router.push("/admin-dh");
  };

  const currentNavItem = getNavItemByPath(pathname, sections);

  return (
    <SidebarProvider>
      <div className="flex h-[100dvh] bg-slate-50">
        <UnifiedSidebar
          session={session}
          perfil={perfil}
          sections={sections}
          pathname={pathname}
          onLogout={handleLogout}
          currentWorkspace={workspace}
          onWorkspaceChange={handleWorkspaceChange}
        />

        <div className="flex min-w-0 flex-1 flex-col">
          <MobileHeader session={session} perfil={perfil} current={currentNavItem} />
          <DesktopHeader session={session} perfil={perfil} current={currentNavItem} sections={sections} />

          <main className="main-with-mobile-nav flex-1 overflow-y-auto">
            <div className="mx-auto w-full max-w-7xl p-4 lg:p-8">
              {children}
            </div>
          </main>
        </div>

        <MobileBottomNav
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
          onLogout={handleLogout}
        />
      </div>
    </SidebarProvider>
  );
}
