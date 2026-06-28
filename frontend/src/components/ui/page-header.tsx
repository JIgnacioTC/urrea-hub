import { InfoButton } from "@/components/ui/info-button";
import { cn } from "@/lib/utils";

export function PageHeader({
  title,
  subtitle,
  className,
  action,
  infoTitle,
  infoContent,
}: {
  title: string;
  subtitle?: string;
  className?: string;
  action?: React.ReactNode;
  infoTitle?: string;
  infoContent?: React.ReactNode;
}) {
  return (
    <div className={cn("flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between", className)}>
      <div>
        <div className="flex items-center gap-2">
          <h1 className="text-xl font-semibold tracking-tight text-urrea-text sm:text-2xl">{title}</h1>
          {infoTitle && infoContent && <InfoButton title={infoTitle}>{infoContent}</InfoButton>}
        </div>
        {subtitle && <p className="mt-1 text-sm text-urrea-text-muted sm:text-base">{subtitle}</p>}
      </div>
      {action}
    </div>
  );
}

export function PageContainer({
  children,
  className,
}: {
  children: React.ReactNode;
  className?: string;
}) {
  return (
    <div className={cn("mx-auto w-full max-w-5xl space-y-4 px-4 py-4 sm:space-y-6 sm:px-6 sm:py-6", className)}>
      {children}
    </div>
  );
}

export function EmptyState({ message }: { message: string }) {
  return (
    <div className="flex flex-col items-center justify-center rounded-xl border border-dashed border-urrea-border bg-urrea-bg-soft/50 py-10 text-center">
      <p className="text-sm text-urrea-text-muted">{message}</p>
    </div>
  );
}

export function Alert({
  variant = "error",
  className,
  children,
}: {
  variant?: "error" | "success" | "info";
  className?: string;
  children: React.ReactNode;
}) {
  const styles = {
    error: "border-red-200 bg-red-50 text-red-800",
    success: "border-emerald-200 bg-emerald-50 text-emerald-800",
    info: "border-urrea-chrome bg-urrea-bg-soft text-urrea-text",
  };
  return (
    <div className={cn("rounded-xl border px-3.5 py-2.5 text-sm", styles[variant], className)} role="alert">
      {children}
    </div>
  );
}
