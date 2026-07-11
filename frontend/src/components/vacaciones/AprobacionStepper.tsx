import { DhIcon } from "@/components/dh/shared/icons";
import type { PasoAprobacion } from "@/lib/types";
import { cn } from "@/lib/utils";

function stepVisual(estado: string) {
  switch (estado) {
    case "Aprobada":
      return { icon: "check" as const, circle: "border-emerald-500 bg-emerald-500 text-white", label: "text-emerald-700" };
    case "Rechazada":
      return { icon: "close" as const, circle: "border-red-500 bg-red-500 text-white", label: "text-red-700" };
    default:
      return { icon: null, circle: "border-urrea-border bg-white text-urrea-text-muted", label: "text-urrea-text-muted" };
  }
}

/** Indica visualmente en qué paso de la cadena de aprobación se encuentra una solicitud. */
export function AprobacionStepper({ pasos, compact = false }: { pasos: PasoAprobacion[]; compact?: boolean }) {
  if (pasos.length === 0) return null;

  const pasoActualIdx = pasos.findIndex((p) => p.estado === "Pendiente");

  return (
    <div className={cn("flex items-start", compact ? "gap-0" : "gap-0 py-1")}>
      {pasos.map((paso, idx) => {
        const visual = stepVisual(paso.estado);
        const esActual = idx === pasoActualIdx;
        const esUltimo = idx === pasos.length - 1;

        return (
          <div key={paso.orden} className={cn("flex items-center", !esUltimo && "flex-1")}>
            <div className="flex flex-col items-center">
              <div
                className={cn(
                  "flex shrink-0 items-center justify-center rounded-full border-2 font-semibold transition-colors",
                  compact ? "h-6 w-6 text-[10px]" : "h-8 w-8 text-xs",
                  visual.circle,
                  esActual && "ring-4 ring-urrea-primary/15",
                )}
                title={`${paso.nivelLabel}: ${paso.estado}`}
              >
                {visual.icon ? <DhIcon name={visual.icon} className={compact ? "h-3 w-3" : "h-4 w-4"} /> : idx + 1}
              </div>
              {!compact && (
                <div className="mt-1.5 max-w-[5.5rem] text-center">
                  <p className={cn("text-[11px] font-medium leading-tight", visual.label)}>{paso.nivelLabel}</p>
                  {paso.aprobadorNombre && paso.estado !== "Pendiente" && (
                    <p className="truncate text-[10px] text-urrea-text-muted">{paso.aprobadorNombre}</p>
                  )}
                </div>
              )}
            </div>
            {!esUltimo && (
              <div
                className={cn(
                  "h-0.5 flex-1 rounded-full transition-colors",
                  compact ? "mx-1" : "mx-2 mt-4",
                  paso.estado === "Aprobada" ? "bg-emerald-500" : "bg-urrea-border",
                )}
              />
            )}
          </div>
        );
      })}
    </div>
  );
}
