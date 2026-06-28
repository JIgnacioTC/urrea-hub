"use client";

import Link from "next/link";
import type { ReactNode } from "react";
import { DhIcon, type DhIconName } from "@/components/dh/shared/icons";
import { cn } from "@/lib/utils";

export function DhBreadcrumbs({ items }: { items: { label: string; href?: string }[] }) {
  return (
    <nav aria-label="Breadcrumb" className="mb-4 flex flex-wrap items-center gap-1.5 text-xs text-urrea-text-muted">
      {items.map((item, i) => (
        <span key={item.label} className="flex items-center gap-1.5">
          {i > 0 && <DhIcon name="chevron" className="h-3 w-3 rotate-0 opacity-40" />}
          {item.href ? (
            <Link href={item.href} className="hover:text-urrea-primary">{item.label}</Link>
          ) : (
            <span className="font-medium text-urrea-text">{item.label}</span>
          )}
        </span>
      ))}
    </nav>
  );
}

export function DhPageToolbar({
  children,
  onExport,
  recordCount,
}: {
  children?: ReactNode;
  onExport?: () => void;
  recordCount?: string;
}) {
  return (
    <div className="mb-4 flex flex-col gap-3 border border-urrea-border/80 bg-urrea-bg px-4 py-3 sm:flex-row sm:items-center sm:justify-between">
      <div className="flex flex-1 flex-wrap items-center gap-2">{children}</div>
      <div className="flex items-center gap-3">
        {recordCount && <span className="text-xs tabular-nums text-urrea-text-muted">{recordCount}</span>}
        {onExport !== undefined && (
          <button
            type="button"
            onClick={onExport}
            className="inline-flex h-9 items-center gap-2 border border-urrea-border bg-urrea-bg px-3 text-xs font-medium text-urrea-text transition hover:border-urrea-primary/40 hover:text-urrea-primary"
          >
            <DhIcon name="export" className="h-3.5 w-3.5" />
            Exportar
          </button>
        )}
      </div>
    </div>
  );
}

export function DhEnterpriseHeader({
  title,
  subtitle,
  icon,
  action,
  breadcrumbs,
}: {
  title: string;
  subtitle?: string;
  icon?: DhIconName;
  action?: ReactNode;
  breadcrumbs?: { label: string; href?: string }[];
}) {
  return (
    <header className="mb-6 border-b border-urrea-border/80 pb-5">
      {breadcrumbs && breadcrumbs.length > 0 && <DhBreadcrumbs items={breadcrumbs} />}
      <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
        <div className="flex gap-4">
          {icon && (
            <div className="flex h-12 w-12 shrink-0 items-center justify-center border border-urrea-primary/20 bg-urrea-primary/5 text-urrea-primary">
              <DhIcon name={icon} className="h-6 w-6" />
            </div>
          )}
          <div>
            <h1 className="text-xl font-semibold tracking-tight text-urrea-text sm:text-2xl">{title}</h1>
            {subtitle && <p className="mt-1 max-w-3xl text-sm leading-relaxed text-urrea-text-muted">{subtitle}</p>}
          </div>
        </div>
        {action && <div className="shrink-0">{action}</div>}
      </div>
    </header>
  );
}

export function DhComplianceStrip() {
  return (
    <div className="mt-8 flex flex-wrap items-center gap-4 border-t border-urrea-border/60 pt-4 text-[10px] uppercase tracking-wider text-urrea-chrome">
      <span>NOM-035-STPS-2018</span>
      <span>LFT · Art. 132</span>
      <span>ISO 27001</span>
      <span>LFPDPPP</span>
      <span className="ml-auto normal-case tracking-normal">Datos enmascarados conforme política de confidencialidad URREA</span>
    </div>
  );
}

export function DhSyncStatus({ ok, label }: { ok: boolean; label: string }) {
  return (
    <span className={cn("inline-flex items-center gap-1.5 text-xs font-medium", ok ? "text-emerald-700" : "text-amber-700")}>
      <span className={cn("h-2 w-2 rounded-full", ok ? "bg-emerald-500" : "bg-amber-500 animate-pulse")} />
      {label}
    </span>
  );
}
