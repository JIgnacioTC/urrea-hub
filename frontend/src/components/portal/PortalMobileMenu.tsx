"use client";

import Link from "next/link";
import { useEffect } from "react";
import { DhIcon } from "@/components/dh/shared/icons";
import { PortalSearch } from "@/components/layout/PortalSearch";
import { isNavActive, type PortalNavSection } from "@/lib/portal/navigation";
import { cn } from "@/lib/utils";

export function PortalMobileMenu({
  open,
  onClose,
  sections,
  pathname,
  onLogout,
}: {
  open: boolean;
  onClose: () => void;
  sections: PortalNavSection[];
  pathname: string;
  onLogout: () => void;
}) {
  useEffect(() => {
    if (!open) return;
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };
    document.addEventListener("keydown", onKey);
    document.body.style.overflow = "hidden";
    return () => {
      document.removeEventListener("keydown", onKey);
      document.body.style.overflow = "";
    };
  }, [open, onClose]);

  if (!open) return null;

  return (
    <div className="fixed inset-0 z-[90] lg:hidden">
      <button type="button" aria-label="Cerrar menú" className="absolute inset-0 bg-urrea-text/30" onClick={onClose} />
      <div className="absolute inset-x-0 bottom-0 max-h-[85dvh] overflow-hidden rounded-t-2xl border-t border-urrea-border bg-white shadow-soft-lg safe-bottom animate-fade-up">
        <div className="flex items-center justify-between border-b border-urrea-border/60 px-4 py-3">
          <p className="text-sm font-semibold text-urrea-text">Todos los accesos</p>
          <button
            type="button"
            onClick={onClose}
            className="flex h-9 w-9 items-center justify-center rounded-full text-urrea-text-muted hover:bg-urrea-bg-soft"
            aria-label="Cerrar"
          >
            ✕
          </button>
        </div>
        <div className="border-b border-urrea-border/60 px-4 py-3">
          <PortalSearch sections={sections} onNavigate={onClose} />
        </div>
        <nav className="overflow-y-auto px-3 py-3" style={{ maxHeight: "calc(85dvh - 11rem)" }}>
          {sections.map((section) => (
            <div key={section.title} className="mb-4">
              <p className="mb-1.5 px-2 text-[10px] font-semibold uppercase tracking-wider text-urrea-text-muted">
                {section.title}
              </p>
              <div className="space-y-0.5">
                {section.links.map((item) => {
                  const active = isNavActive(pathname, item.href);
                  return (
                    <Link
                      key={item.href}
                      href={item.href}
                      onClick={onClose}
                      className={cn(
                        "flex min-h-11 items-center gap-3 rounded-xl px-3 py-2.5 text-sm transition",
                        active
                          ? "bg-urrea-primary/8 font-medium text-urrea-primary"
                          : "text-urrea-text hover:bg-urrea-bg-soft",
                      )}
                    >
                      <DhIcon name={item.icon} className={cn("h-[18px] w-[18px]", active ? "text-urrea-primary" : "text-urrea-text-muted")} />
                      <span>{item.label}</span>
                    </Link>
                  );
                })}
              </div>
            </div>
          ))}
        </nav>
        <div className="border-t border-urrea-border/60 px-4 py-3">
          <button
            type="button"
            onClick={() => {
              onClose();
              onLogout();
            }}
            className="w-full rounded-xl border border-urrea-border px-4 py-3 text-left text-sm text-urrea-text-muted transition hover:bg-urrea-bg-soft hover:text-urrea-text"
          >
            Cerrar sesión
          </button>
        </div>
      </div>
    </div>
  );
}

/** Icono menú (tres líneas) para bottom nav */
export function MenuIcon({ className }: { className?: string }) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" className={className} aria-hidden>
      <line x1="4" y1="7" x2="20" y2="7" />
      <line x1="4" y1="12" x2="20" y2="12" />
      <line x1="4" y1="17" x2="20" y2="17" />
    </svg>
  );
}
