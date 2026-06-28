"use client";

import { cn } from "@/lib/utils";
import type { ReactNode } from "react";

export function DhKpiCard({
  label,
  value,
  hint,
  delta,
  accent = "primary",
  alert,
  size = "default",
}: {
  label: string;
  value: string | number;
  hint?: string;
  delta?: { value: string; positive?: boolean };
  accent?: "primary" | "secondary" | "success" | "warning" | "danger";
  alert?: boolean;
  size?: "default" | "hero";
}) {
  const accents = {
    primary: "text-urrea-primary",
    secondary: "text-slate-700",
    success: "text-emerald-800",
    warning: "text-amber-800",
    danger: "text-red-700",
  };
  return (
    <div
      className={cn(
        "border bg-urrea-bg",
        size === "hero" ? "p-5 sm:p-6" : "p-4",
        alert ? "border-l-4 border-l-amber-500 border-urrea-border/80 bg-amber-50/30" : "border-urrea-border/80",
      )}
    >
      <p className="text-[11px] font-semibold uppercase tracking-[0.08em] text-urrea-text-muted">{label}</p>
      <div className="mt-2 flex items-end justify-between gap-2">
        <p className={cn("font-semibold tabular-nums tracking-tight", accents[accent], size === "hero" ? "text-3xl sm:text-4xl" : "text-2xl")}>
          {value}
        </p>
        {delta && (
          <span className={cn("text-xs font-medium tabular-nums", delta.positive ? "text-emerald-700" : "text-red-600")}>
            {delta.positive ? "▲" : "▼"} {delta.value}
          </span>
        )}
      </div>
      {hint && <p className="mt-2 text-xs leading-relaxed text-urrea-text-muted">{hint}</p>}
    </div>
  );
}

export function DhBadge({ children, tone = "neutral" }: { children: ReactNode; tone?: "neutral" | "success" | "warning" | "danger" | "info" }) {
  const tones = {
    neutral: "border border-urrea-border bg-urrea-bg-soft text-urrea-text-muted",
    success: "border border-emerald-200 bg-emerald-50 text-emerald-900",
    warning: "border border-amber-200 bg-amber-50 text-amber-900",
    danger: "border border-red-200 bg-red-50 text-red-900",
    info: "border border-sky-200 bg-sky-50 text-sky-900",
  };
  return (
    <span className={cn("inline-flex items-center px-2 py-0.5 text-[11px] font-semibold uppercase tracking-wide", tones[tone])}>
      {children}
    </span>
  );
}

export function ConfidencialTag() {
  return (
    <span className="inline-flex items-center gap-1 border border-red-200 bg-red-50 px-2 py-0.5 text-[10px] font-bold uppercase tracking-widest text-red-800">
      Confidencial
    </span>
  );
}

export function DhBarChart({ data, labelKey, valueKey, showPct }: { data: Record<string, string | number>[]; labelKey: string; valueKey: string; showPct?: boolean }) {
  const max = Math.max(...data.map((d) => Number(d[valueKey])), 1);
  const total = data.reduce((s, d) => s + Number(d[valueKey]), 0);
  return (
    <div className="space-y-3">
      {data.map((d) => {
        const val = Number(d[valueKey]);
        return (
          <div key={String(d[labelKey])}>
            <div className="mb-1 flex justify-between gap-2 text-xs">
              <span className="truncate font-medium text-urrea-text">{String(d[labelKey])}</span>
              <span className="shrink-0 tabular-nums text-urrea-text-muted">
                {val.toLocaleString("es-MX")}
                {showPct && total > 0 && ` · ${((val / total) * 100).toFixed(1)}%`}
              </span>
            </div>
            <div className="h-1.5 overflow-hidden bg-slate-100">
              <div className="h-full bg-urrea-primary transition-all" style={{ width: `${(val / max) * 100}%` }} />
            </div>
          </div>
        );
      })}
    </div>
  );
}

export function DhPieLegend({ items }: { items: { label: string; value: number; color: string }[] }) {
  const total = items.reduce((s, i) => s + i.value, 0) || 1;
  return (
    <div className="space-y-2.5">
      {items.map((item) => (
        <div key={item.label} className="flex items-center justify-between text-sm">
          <div className="flex items-center gap-2.5">
            <span className="h-2.5 w-2.5 shrink-0" style={{ backgroundColor: item.color }} />
            <span className="text-urrea-text-muted">{item.label}</span>
          </div>
          <span className="font-semibold tabular-nums text-urrea-text">{Math.round((item.value / total) * 100)}%</span>
        </div>
      ))}
    </div>
  );
}

export function DhCard({ title, subtitle, children, action, className, noPadding }: { title?: string; subtitle?: string; children: ReactNode; action?: ReactNode; className?: string; noPadding?: boolean }) {
  return (
    <div className={cn("border border-urrea-border/80 bg-urrea-bg", className)}>
      {(title || action) && (
        <div className="flex items-start justify-between gap-3 border-b border-urrea-border/60 px-5 py-4">
          <div>
            {title && <h3 className="text-sm font-semibold text-urrea-text">{title}</h3>}
            {subtitle && <p className="mt-0.5 text-xs text-urrea-text-muted">{subtitle}</p>}
          </div>
          {action}
        </div>
      )}
      <div className={noPadding ? undefined : "p-5"}>{children}</div>
    </div>
  );
}

export function DhModal({ open, title, onClose, children }: { open: boolean; title: string; onClose: () => void; children: ReactNode }) {
  if (!open) return null;
  return (
    <div className="fixed inset-0 z-50 flex items-end justify-center bg-slate-900/50 p-4 sm:items-center">
      <div className="max-h-[90vh] w-full max-w-lg overflow-y-auto border border-urrea-border bg-urrea-bg shadow-2xl">
        <div className="flex items-center justify-between border-b border-urrea-border px-5 py-4">
          <h3 className="text-base font-semibold text-urrea-text">{title}</h3>
          <button type="button" onClick={onClose} aria-label="Cerrar" className="px-2 py-1 text-urrea-text-muted hover:text-urrea-text">✕</button>
        </div>
        <div className="p-5">{children}</div>
      </div>
    </div>
  );
}

export function DhSearchInput({ value, onChange, placeholder }: { value: string; onChange: (v: string) => void; placeholder?: string }) {
  return (
    <input
      type="search"
      value={value}
      onChange={(e) => onChange(e.target.value)}
      placeholder={placeholder ?? "Buscar..."}
      className="h-9 w-full border border-urrea-border bg-urrea-bg px-3 text-sm outline-none focus:border-urrea-primary focus:ring-1 focus:ring-urrea-primary/30"
    />
  );
}

export function DhSelect({ value, onChange, options }: { value: string; onChange: (v: string) => void; options: { value: string; label: string }[] }) {
  return (
    <select
      value={value}
      onChange={(e) => onChange(e.target.value)}
      className="h-9 border border-urrea-border bg-urrea-bg px-3 text-sm outline-none focus:border-urrea-primary"
    >
      {options.map((o) => (
        <option key={o.value} value={o.value}>{o.label}</option>
      ))}
    </select>
  );
}

export function DhDataTable({ children, footer }: { children: ReactNode; footer?: ReactNode }) {
  return (
    <div className="overflow-hidden border border-urrea-border/80 bg-urrea-bg">
      <div className="overflow-x-auto">{children}</div>
      {footer && <div className="border-t border-urrea-border/60 bg-slate-50/80 px-4 py-3 text-xs text-urrea-text-muted">{footer}</div>}
    </div>
  );
}

export function DhEmpty({ message }: { message: string }) {
  return <p className="py-12 text-center text-sm text-urrea-text-muted">{message}</p>;
}

export function estatusTone(estatus: string): "success" | "warning" | "danger" | "neutral" | "info" {
  if (["activo", "aprobada", "completado", "firmado", "exitoso", "resuelto", "cerrado"].some((x) => estatus.includes(x))) return "success";
  if (["pendiente", "en_proceso", "detectada", "parcial", "en_riesgo", "vencido"].some((x) => estatus.includes(x))) return "warning";
  if (["rechazada", "fallido", "critico", "baja"].some((x) => estatus.includes(x))) return "danger";
  if (["investigacion", "triage", "asignado"].some((x) => estatus.includes(x))) return "info";
  return "neutral";
}
