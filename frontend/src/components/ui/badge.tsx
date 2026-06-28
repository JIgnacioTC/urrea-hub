import { cn } from "@/lib/utils";
import { ESTADO_COLORS, ESTADO_LABELS } from "@/lib/types";

export function Badge({
  estado,
  className,
  children,
}: {
  estado?: string;
  className?: string;
  children?: React.ReactNode;
}) {
  const label = estado ? (ESTADO_LABELS[estado] ?? estado) : children;
  const colors = estado ? (ESTADO_COLORS[estado] ?? "bg-urrea-bg-soft text-urrea-text-muted") : "";

  return (
    <span
      className={cn(
        "inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-semibold tracking-wide",
        colors,
        className,
      )}
    >
      {label}
    </span>
  );
}
