"use client";

import { useEffect, useId, useRef, useState } from "react";
import { cn } from "@/lib/utils";

type InfoButtonProps = {
  title: string;
  children: React.ReactNode;
  className?: string;
  /** Etiqueta accesible del botón (por defecto: "Más información") */
  label?: string;
};

export function InfoButton({ title, children, className, label = "Más información" }: InfoButtonProps) {
  const [open, setOpen] = useState(false);
  const titleId = useId();
  const panelRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!open) return;
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") setOpen(false);
    };
    document.addEventListener("keydown", onKey);
    document.body.style.overflow = "hidden";
    return () => {
      document.removeEventListener("keydown", onKey);
      document.body.style.overflow = "";
    };
  }, [open]);

  return (
    <>
      <button
        type="button"
        aria-label={label}
        aria-expanded={open}
        aria-controls={titleId}
        onClick={(e) => {
          e.stopPropagation();
          e.preventDefault();
          setOpen(true);
        }}
        className={cn(
          "inline-flex h-6 w-6 shrink-0 items-center justify-center rounded-full border border-urrea-border/80 bg-urrea-bg text-[11px] font-semibold text-urrea-text-muted transition hover:border-urrea-secondary/50 hover:text-urrea-secondary focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-urrea-secondary/30",
          className,
        )}
      >
        i
      </button>

      {open && (
        <div className="fixed inset-0 z-[100] flex items-end justify-center sm:items-center sm:p-4">
          <button
            type="button"
            aria-label="Cerrar"
            className="absolute inset-0 bg-urrea-text/25 backdrop-blur-[2px]"
            onClick={() => setOpen(false)}
          />
          <div
            ref={panelRef}
            role="dialog"
            aria-modal="true"
            aria-labelledby={titleId}
            className="relative w-full max-w-md animate-fade-up rounded-t-2xl border border-urrea-border/60 bg-white p-5 shadow-soft-lg sm:rounded-2xl safe-bottom"
          >
            <div className="mb-4 flex items-start justify-between gap-3">
              <h2 id={titleId} className="text-base font-semibold text-urrea-text">
                {title}
              </h2>
              <button
                type="button"
                onClick={() => setOpen(false)}
                className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full text-urrea-text-muted transition hover:bg-urrea-bg-soft hover:text-urrea-text"
                aria-label="Cerrar"
              >
                ✕
              </button>
            </div>
            <div className="max-h-[60vh] overflow-y-auto text-sm leading-relaxed text-urrea-text-muted">
              {children}
            </div>
          </div>
        </div>
      )}
    </>
  );
}

/** Título de sección con ayuda contextual opcional */
export function SectionHeading({
  title,
  infoTitle,
  infoContent,
  className,
}: {
  title: string;
  infoTitle?: string;
  infoContent?: React.ReactNode;
  className?: string;
}) {
  return (
    <div className={cn("flex items-center gap-2", className)}>
      <h2 className="text-sm font-semibold text-urrea-text">{title}</h2>
      {infoTitle && infoContent && <InfoButton title={infoTitle}>{infoContent}</InfoButton>}
    </div>
  );
}
