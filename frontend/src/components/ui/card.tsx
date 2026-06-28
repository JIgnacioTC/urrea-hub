import { cn } from "@/lib/utils";

export function Card({
  title,
  description,
  className,
  children,
}: {
  title?: string;
  description?: string;
  className?: string;
  children: React.ReactNode;
}) {
  return (
    <div
      className={cn(
        "rounded-2xl border border-urrea-border/80 bg-urrea-bg p-4 shadow-soft sm:p-5",
        "transition-shadow duration-300 hover:shadow-soft-lg",
        className,
      )}
    >
      {(title || description) && (
        <div className="mb-4">
          {title && <h2 className="text-base font-semibold tracking-tight text-urrea-text sm:text-lg">{title}</h2>}
          {description && <p className="mt-1 text-sm text-urrea-text-muted">{description}</p>}
        </div>
      )}
      {children}
    </div>
  );
}

export function StatCard({
  label,
  value,
  accentClass = "text-urrea-primary",
  className,
}: {
  label: string;
  value?: number | string;
  accentClass?: string;
  className?: string;
}) {
  return (
    <Card className={cn("hover:shadow-soft-lg", className)}>
      <p className="text-xs font-medium uppercase tracking-wide text-urrea-text-muted sm:text-sm">{label}</p>
      <p className={cn("mt-2 text-2xl font-bold tabular-nums tracking-tight sm:text-3xl", accentClass)}>
        {value ?? "—"}
      </p>
    </Card>
  );
}
