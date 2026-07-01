"use client";

import Link from "next/link";
import { useState } from "react";
import { DhIcon, DhIconName } from "@/components/dh/shared/icons";
import { useSidebar } from "@/components/layout/SidebarContext";
import { cn } from "@/lib/utils";
import {
  PORTAL_HOME,
  WORKSPACES,
  WorkspaceType,
  type PortalNavLink,
  type PortalNavSection,
} from "@/lib/portal/navigation";
import { isRhAdmin, isTiAdmin, type Session } from "@/lib/auth";
import { ColaboradorPerfil } from "@/lib/types";

function NavItem({
  item,
  pathname,
  collapsed,
  depth = 0,
}: {
  item: PortalNavLink;
  pathname: string;
  collapsed: boolean;
  depth?: number;
}) {
  const [isOpen, setIsOpen] = useState(false);
  const active =
    pathname === item.href ||
    (item.href !== "/portal" && pathname.startsWith(`${item.href}/`)) ||
    item.children?.some((child) => pathname === child.href || pathname.startsWith(`${child.href}/`));

  if (item.children && item.children.length > 0) {
    return (
      <div className="mb-0.5">
        <button
          onClick={() => setIsOpen(!isOpen)}
          className={cn(
            "group flex w-full items-center justify-between rounded-xl px-3 py-2 text-sm transition hover:bg-white/5",
            active ? "bg-white/10 font-medium text-white" : "text-slate-400 hover:text-white",
            collapsed && "justify-center"
          )}
        >
          <div className="flex items-center gap-3">
            {item.icon && (
              <DhIcon
                name={item.icon as DhIconName}
                className={cn("h-4 w-4 shrink-0", active ? "text-urrea-primary" : "text-slate-500 group-hover:text-slate-300")}
              />
            )}
            {!collapsed && <span className="truncate">{item.label}</span>}
          </div>
          {!collapsed && (
            <DhIcon
              name="chevron-down"
              className={cn("h-4 w-4 text-slate-500 transition-transform", isOpen && "rotate-180")}
            />
          )}
        </button>
        {isOpen && !collapsed && (
          <div className="mt-0.5 ml-7 space-y-0.5 border-l border-white/10 pl-2">
            {item.children.map((child) => (
              <NavItem key={child.href} item={child} pathname={pathname} collapsed={collapsed} depth={depth + 1} />
            ))}
          </div>
        )}
      </div>
    );
  }

  return (
    <Link
      href={item.href}
      title={collapsed ? item.label : undefined}
      className={cn(
        "group flex items-center gap-3 rounded-xl px-3 py-2 text-sm transition hover:bg-white/5 mb-0.5",
        active ? "bg-white/10 font-medium text-white" : "text-slate-400 hover:text-white",
        collapsed && "justify-center px-0"
      )}
    >
      {item.icon && (
        <DhIcon
          name={item.icon as DhIconName}
          className={cn("h-4 w-4 shrink-0", active ? "text-urrea-primary" : "text-slate-500 group-hover:text-slate-300")}
        />
      )}
      {!collapsed && <span className="truncate">{item.label}</span>}
    </Link>
  );
}

export function UnifiedSidebar({
  session,
  perfil,
  sections,
  pathname,
  onLogout,
  currentWorkspace,
  onWorkspaceChange,
}: {
  session: Session;
  perfil: ColaboradorPerfil | null;
  sections: PortalNavSection[];
  pathname: string;
  onLogout: () => void;
  currentWorkspace: WorkspaceType;
  onWorkspaceChange: (ws: WorkspaceType) => void;
}) {
  const { collapsed, toggle } = useSidebar();
  const [showWorkspaces, setShowWorkspaces] = useState(false);

  const availableWorkspaces = WORKSPACES.filter((ws) => {
    if (ws.rhOnly && !isRhAdmin(session)) return false;
    if (ws.tiOnly && !isTiAdmin(session)) return false;
    return true;
  });

  const activeWsConfig = availableWorkspaces.find((ws) => ws.id === currentWorkspace) || availableWorkspaces[0];
  const activosLabel = perfil ? session.nombreCompleto.split(" ")[0] : "URREA";

  return (
    <aside
      className={cn(
        "hidden shrink-0 flex-col bg-[#011829] text-slate-300 transition-all duration-300 lg:flex",
        collapsed ? "w-[4.5rem]" : "w-[17.5rem] xl:w-72"
      )}
    >
      <div className="border-b border-white/10 px-4 py-4">
        {/* Workspace Switcher */}
        <div className="relative">
          <button
            onClick={() => setShowWorkspaces(!showWorkspaces)}
            className={cn(
              "flex w-full items-center gap-3 rounded-lg border border-white/10 bg-white/5 p-2 transition hover:bg-white/10",
              collapsed && "justify-center p-2"
            )}
          >
            <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-md bg-urrea-primary text-white">
              <DhIcon name={activeWsConfig.icon} className="h-4 w-4" />
            </div>
            {!collapsed && (
              <div className="flex flex-1 items-center justify-between min-w-0">
                <div className="text-left">
                  <p className="truncate text-sm font-bold text-white">{activeWsConfig.label}</p>
                  <p className="text-[10px] uppercase text-slate-400">{activosLabel}</p>
                </div>
                <DhIcon name="chevron-down" className="h-4 w-4 text-slate-500" />
              </div>
            )}
          </button>

          {showWorkspaces && !collapsed && (
            <div className="absolute top-full left-0 z-50 mt-1 w-full rounded-lg border border-white/10 bg-[#011829] p-1 shadow-xl">
              {availableWorkspaces.map((ws) => (
                <button
                  key={ws.id}
                  onClick={() => {
                    onWorkspaceChange(ws.id);
                    setShowWorkspaces(false);
                  }}
                  className={cn(
                    "flex w-full items-center gap-3 rounded-md px-2 py-2 text-sm transition hover:bg-white/10",
                    ws.id === currentWorkspace ? "bg-white/5 text-white" : "text-slate-400"
                  )}
                >
                  <DhIcon name={ws.icon} className="h-4 w-4" />
                  {ws.label}
                </button>
              ))}
            </div>
          )}
        </div>
      </div>

      <nav className="flex-1 overflow-y-auto px-3 py-4 custom-scrollbar">
        {sections.map((section) => (
          <div key={section.title} className="mb-6">
            {!collapsed && (
              <p className="mb-2 px-3 text-[10px] font-bold uppercase tracking-[0.14em] text-slate-500">
                {section.title}
              </p>
            )}
            <div className="space-y-0.5">
              {section.links.map((item) => (
                <NavItem key={item.href} item={item} pathname={pathname} collapsed={collapsed} />
              ))}
            </div>
          </div>
        ))}
      </nav>

      <div className="border-t border-white/10 p-3">
        <button
          onClick={toggle}
          className={cn(
            "flex w-full items-center gap-3 rounded-lg p-2 text-slate-400 transition hover:bg-white/5 hover:text-white",
            collapsed && "justify-center"
          )}
          title={collapsed ? "Expandir" : "Colapsar"}
        >
          <DhIcon name={collapsed ? "menu" : "close"} className="h-5 w-5 shrink-0" />
          {!collapsed && <span className="text-sm">Colapsar menú</span>}
        </button>

        <button
          onClick={onLogout}
          className={cn(
            "mt-2 flex w-full items-center gap-3 rounded-lg p-2 text-slate-400 transition hover:bg-white/5 hover:text-white",
            collapsed && "justify-center"
          )}
          title={collapsed ? "Cerrar sesión" : undefined}
        >
          <DhIcon name="logout" className="h-5 w-5 shrink-0" />
          {!collapsed && <span className="text-sm">Cerrar sesión</span>}
        </button>
      </div>
    </aside>
  );
}
