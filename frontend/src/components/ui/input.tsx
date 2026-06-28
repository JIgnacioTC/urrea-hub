import { cn } from "@/lib/utils";

export function Input({
  className,
  label,
  id,
  ...props
}: React.InputHTMLAttributes<HTMLInputElement> & { label?: string }) {
  const inputId = id ?? props.name;
  return (
    <div className="w-full">
      {label && (
        <label htmlFor={inputId} className="mb-1.5 block text-sm font-medium text-urrea-text">
          {label}
        </label>
      )}
      <input
        id={inputId}
        className={cn(
          "w-full min-h-11 rounded-xl border border-urrea-border bg-urrea-bg px-3.5 py-2.5 text-base text-urrea-text",
          "placeholder:text-urrea-text-muted/80",
          "transition-all duration-200",
          "focus:border-urrea-secondary focus:outline-none focus:ring-4 focus:ring-urrea-secondary/15",
          "disabled:cursor-not-allowed disabled:bg-urrea-bg-soft disabled:text-urrea-text-muted",
          props.type === "date" || props.type === "time" ? "[color-scheme:light]" : "",
          className,
        )}
        {...props}
      />
    </div>
  );
}

export function Textarea({
  className,
  label,
  id,
  ...props
}: React.TextareaHTMLAttributes<HTMLTextAreaElement> & { label?: string }) {
  const inputId = id ?? props.name;
  return (
    <div className="w-full">
      {label && (
        <label htmlFor={inputId} className="mb-1.5 block text-sm font-medium text-urrea-text">
          {label}
        </label>
      )}
      <textarea
        id={inputId}
        className={cn(
          "w-full min-h-[5rem] rounded-xl border border-urrea-border bg-urrea-bg px-3.5 py-2.5 text-base text-urrea-text",
          "placeholder:text-urrea-text-muted/80 transition-all duration-200 resize-y",
          "focus:border-urrea-secondary focus:outline-none focus:ring-4 focus:ring-urrea-secondary/15",
          className,
        )}
        {...props}
      />
    </div>
  );
}

export function Select({
  className,
  label,
  id,
  children,
  hint,
  ...props
}: React.SelectHTMLAttributes<HTMLSelectElement> & { label?: string; hint?: string }) {
  const inputId = id ?? props.name;
  return (
    <div className="w-full">
      {label && (
        <label htmlFor={inputId} className="mb-1.5 block text-sm font-medium text-urrea-text">
          {label}
        </label>
      )}
      <div className="relative">
        <select
          id={inputId}
          className={cn(
            "w-full min-h-11 appearance-none rounded-xl border border-urrea-border bg-urrea-bg py-2.5 pl-3.5 pr-10 text-base text-urrea-text",
            "transition-all duration-200",
            "focus:border-urrea-secondary focus:outline-none focus:ring-4 focus:ring-urrea-secondary/15",
            "disabled:cursor-not-allowed disabled:bg-urrea-bg-soft disabled:text-urrea-text-muted",
            className,
          )}
          {...props}
        >
          {children}
        </select>
        <span className="pointer-events-none absolute inset-y-0 right-3 flex items-center text-urrea-text-muted">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="h-4 w-4" aria-hidden>
            <polyline points="6 9 12 15 18 9" />
          </svg>
        </span>
      </div>
      {hint && <p className="mt-1 text-xs text-urrea-text-muted">{hint}</p>}
    </div>
  );
}
