"use client";

import Link from "next/link";
import { cn } from "@/lib/utils";

export function HcmAvatar({ name }: { name: string }) {
  const initials = name.split(" ").slice(0, 2).map((w) => w[0]).join("");
  return (
    <div className="flex h-10 w-10 shrink-0 items-center justify-center border border-urrea-border bg-slate-100 text-xs font-bold text-slate-700">
      {initials}
    </div>
  );
}

export function HcmFieldGrid({ fields }: { fields: [string, string][] }) {
  return (
    <dl className="grid gap-px border border-urrea-border/60 bg-urrea-border/60 sm:grid-cols-2">
      {fields.map(([label, value]) => (
        <div key={label} className="bg-urrea-bg p-4">
          <dt className="text-[10px] font-semibold uppercase tracking-wide text-urrea-text-muted">{label}</dt>
          <dd className="mt-1 text-sm font-medium text-urrea-text">{value}</dd>
        </div>
      ))}
    </dl>
  );
}

export function HcmKpiStrip({ items }: { items: Array<{ label: string; value: string }> }) {
  return (
    <div className="grid gap-px border border-urrea-border/80 bg-urrea-border/80 sm:grid-cols-2 lg:grid-cols-4">
      {items.map((kpi) => (
        <div key={kpi.label} className="bg-urrea-bg p-4">
          <p className="text-[10px] font-semibold uppercase tracking-wide text-urrea-text-muted">{kpi.label}</p>
          <p className="mt-1 text-lg font-semibold tabular-nums text-urrea-primary">{kpi.value}</p>
        </div>
      ))}
    </div>
  );
}

export function HcmTimeline({ items }: { items: Array<{ date: string; title: string; subtitle?: string; meta?: string }> }) {
  if (items.length === 0) {
    return <p className="py-8 text-center text-sm text-urrea-text-muted">Sin registros en el historial.</p>;
  }
  return (
    <ol className="relative border-l border-urrea-border pl-6">
      {items.map((item, i) => (
        <li key={`${item.date}-${i}`} className="mb-6 ml-2">
          <span className="absolute -left-1.5 mt-1.5 h-3 w-3 rounded-full border-2 border-white bg-urrea-primary" />
          <p className="text-[10px] font-semibold uppercase tracking-wide text-urrea-text-muted">
            {new Date(item.date).toLocaleDateString("es-MX", { dateStyle: "long" })}
          </p>
          <p className="mt-0.5 text-sm font-semibold text-urrea-text">{item.title}</p>
          {item.subtitle && <p className="text-xs text-urrea-text-muted">{item.subtitle}</p>}
          {item.meta && <p className="mt-1 font-mono text-[10px] text-slate-500">{item.meta}</p>}
        </li>
      ))}
    </ol>
  );
}

export function HcmModulePanel({
  label,
  available,
  message,
  recordCount,
  children,
}: {
  label: string;
  available: boolean;
  message: string;
  recordCount: number;
  children?: React.ReactNode;
}) {
  return (
    <div className="border border-urrea-border/80 bg-white p-5">
      <div className="mb-3 flex items-start justify-between gap-3">
        <div>
          <h3 className="text-sm font-semibold text-urrea-text">{label}</h3>
          <p className="mt-1 text-xs text-urrea-text-muted">{message}</p>
        </div>
        <span className={cn(
          "shrink-0 border px-2 py-0.5 text-[10px] font-semibold uppercase",
          available ? "border-emerald-200 bg-emerald-50 text-emerald-700" : "border-amber-200 bg-amber-50 text-amber-700",
        )}>
          {available ? `${recordCount} reg.` : "Fase pendiente"}
        </span>
      </div>
      {children}
    </div>
  );
}

export function HcmEmployeeLink({ id, name, number }: { id: string; name: string; number?: string }) {
  return (
    <Link href={`/portal/hcm/personas/${id}`} className="font-medium text-urrea-primary hover:underline">
      {name}{number ? ` · ${number}` : ""}
    </Link>
  );
}

export function formatNum(n: number) {
  return n.toLocaleString("es-MX");
}
