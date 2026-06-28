"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useMemo, useState, type ReactNode } from "react";
import { DhIcon } from "@/components/dh/shared/icons";
import { DhSyncStatus } from "@/components/dh/shared/page-frame";
import { DhSearchInput } from "@/components/dh/shared/ui";
import { ALL_DH_ROLES, DhRoleProvider, useDhRole } from "@/lib/dh/role-context";
import { EMPRESA, METRICAS_EJECUTIVAS } from "@/lib/dh/enterprise-metrics";
import { DH_GROUP_LABELS, filterNavByRole, getNavItemByPath } from "@/lib/dh/navigation";
import { DH_ROLE_LABELS, type DhRole } from "@/lib/dh/types";
import { cn } from "@/lib/utils";

function DhTopBar() {
  const { role, setRole, roleLabel } = useDhRole();
  const pathname = usePathname();
  const current = getNavItemByPath(pathname);
  const [search, setSearch] = useState("");

  return (
    <header className="sticky top-0 z-30 border-b border-slate-200 bg-white">
      <div className="flex h-14 items-center gap-4 px-4 lg:px-6">
        <div className="hidden min-w-0 flex-1 lg:block">
          <p className="truncate text-sm font-semibold text-urrea-text">{current?.label ?? "Centro de mando"}</p>
          <p className="truncate text-[11px] text-urrea-text-muted">{EMPRESA.entorno}</p>
        </div>

        <div className="flex flex-1 items-center gap-3 lg:max-w-md lg:flex-none">
          <div className="relative flex-1">
            <DhSearchInput value={search} onChange={setSearch} placeholder="Buscar colaborador, folio, curso…" />
          </div>
        </div>

        <DhSyncStatus ok={METRICAS_EJECUTIVAS.syncErrores === 0} label={`SAP·CDM · ${METRICAS_EJECUTIVAS.syncErrores} incidencias`} />

        <button type="button" className="relative flex h-9 w-9 items-center justify-center border border-urrea-border text-urrea-text-muted hover:border-urrea-primary/30 hover:text-urrea-primary" aria-label="Notificaciones">
          <DhIcon name="bell" />
          <span className="absolute -right-0.5 -top-0.5 flex h-4 w-4 items-center justify-center bg-red-600 text-[9px] font-bold text-white">7</span>
        </button>

        <div className="hidden items-center gap-2 border-l border-urrea-border pl-4 sm:flex">
          <select
            value={role}
            onChange={(e) => setRole(e.target.value as DhRole)}
            className="h-9 max-w-[11rem] border border-urrea-border bg-slate-50 px-2 text-xs font-medium text-urrea-text outline-none focus:border-urrea-primary"
            title="Perfil de acceso (demostración RBAC)"
          >
            {ALL_DH_ROLES.map((r) => (
              <option key={r} value={r}>{DH_ROLE_LABELS[r]}</option>
            ))}
          </select>
          <div className="text-right">
            <p className="text-xs font-semibold text-urrea-text">Patricia Ruiz</p>
            <p className="text-[10px] text-urrea-text-muted">{roleLabel}</p>
          </div>
        </div>
      </div>
    </header>
  );
}

function DhSidebar() {
  const pathname = usePathname();
  const { role } = useDhRole();
  const nav = useMemo(() => filterNavByRole(role), [role]);

  const groups = useMemo(() => {
    const map = new Map<string, typeof nav>();
    for (const item of nav) {
      const list = map.get(item.group) ?? [];
      list.push(item);
      map.set(item.group, list);
    }
    return map;
  }, [nav]);

  return (
    <aside className="hidden w-[17.5rem] shrink-0 flex-col bg-[#011829] text-slate-300 lg:flex xl:w-72">
      <div className="border-b border-white/10 px-5 py-5">
        <div className="flex items-center gap-3">
          <div className="flex h-10 w-10 items-center justify-center border border-white/20 bg-white/5">
            <DhIcon name="building" className="h-5 w-5 text-white" />
          </div>
          <div className="min-w-0">
            <p className="truncate text-sm font-bold tracking-wide text-white">URREA</p>
            <p className="truncate text-[10px] uppercase tracking-[0.12em] text-slate-400">Desarrollo Humano</p>
          </div>
        </div>
        <p className="mt-3 text-[10px] leading-relaxed text-slate-500">
          {EMPRESA.plataforma} · v{EMPRESA.version}
        </p>
      </div>

      <nav className="flex-1 overflow-y-auto px-3 py-4">
        {Array.from(groups.entries()).map(([group, items]) => (
          <div key={group} className="mb-6">
            <p className="mb-2 px-3 text-[10px] font-bold uppercase tracking-[0.14em] text-slate-500">
              {DH_GROUP_LABELS[group as keyof typeof DH_GROUP_LABELS]}
            </p>
            <div className="space-y-0.5">
              {items.map((item) => {
                const active = item.href === "/dh" ? pathname === "/dh" : pathname.startsWith(item.href);
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
                    <DhIcon name={item.icon} className={cn("h-4 w-4", active ? "text-white" : "text-slate-500 group-hover:text-slate-300")} />
                    <span className="truncate leading-snug">{item.label}</span>
                  </Link>
                );
              })}
            </div>
          </div>
        ))}
      </nav>

      <div className="border-t border-white/10 p-4">
        <p className="text-[10px] leading-relaxed text-slate-500">
          {METRICAS_EJECUTIVAS.colaboradoresActivos.toLocaleString("es-MX")} colaboradores activos · {METRICAS_EJECUTIVAS.plantasOperativas} plantas
        </p>
        <Link href="/portal" className="mt-2 block text-[11px] text-slate-400 hover:text-white">
          Portal colaborador operativo →
        </Link>
      </div>
    </aside>
  );
}

function DhMobileNav() {
  const pathname = usePathname();
  const { role } = useDhRole();
  const nav = filterNavByRole(role).slice(0, 5);

  return (
    <nav className="fixed inset-x-0 bottom-0 z-40 border-t border-slate-200 bg-white lg:hidden safe-bottom">
      <div className="flex justify-around">
        {nav.map((item) => {
          const active = item.href === "/dh" ? pathname === "/dh" : pathname.startsWith(item.href);
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

function DhShellInner({ children }: { children: ReactNode }) {
  return (
    <div className="flex h-[100dvh] bg-slate-100 lg:flex-row">
      <DhSidebar />
      <div className="flex min-w-0 flex-1 flex-col">
        <DhTopBar />
        <main className="main-with-mobile-nav flex-1 overflow-y-auto">
          <div className="mx-auto max-w-[1600px] px-4 py-5 lg:px-8 lg:py-7">{children}</div>
        </main>
      </div>
      <DhMobileNav />
    </div>
  );
}

export function DhShell({ children }: { children: ReactNode }) {
  return (
    <DhRoleProvider>
      <DhShellInner>{children}</DhShellInner>
    </DhRoleProvider>
  );
}

/** @deprecated Use DhEnterpriseHeader from page-frame */
export function DhPageHeader({ title, subtitle, action }: { title: string; subtitle?: string; action?: ReactNode }) {
  return (
    <div className="mb-6 flex flex-col gap-3 border-b border-urrea-border/80 pb-5 sm:flex-row sm:items-start sm:justify-between">
      <div>
        <h1 className="text-xl font-semibold text-urrea-text sm:text-2xl">{title}</h1>
        {subtitle && <p className="mt-1 text-sm text-urrea-text-muted">{subtitle}</p>}
      </div>
      {action}
    </div>
  );
}
