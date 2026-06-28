import Link from "next/link";
import { cn } from "@/lib/utils";

export const buttonVariants = {
  primary:
    "bg-urrea-primary text-white shadow-md shadow-urrea-primary/20 hover:bg-urrea-secondary hover:shadow-lg hover:shadow-urrea-secondary/25",
  secondary:
    "border border-urrea-border bg-urrea-bg text-urrea-text hover:bg-urrea-bg-soft hover:border-urrea-chrome",
  ghost: "text-urrea-text-muted hover:bg-urrea-bg-soft hover:text-urrea-text",
  danger: "border border-red-200 bg-red-50 text-red-700 hover:bg-red-100",
} as const;

export type ButtonVariant = keyof typeof buttonVariants;

const buttonBase =
  "inline-flex min-h-11 min-w-11 items-center justify-center gap-2 rounded-xl px-4 py-2.5 text-sm font-medium no-underline transition-all duration-200 ease-out active:scale-[0.98] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-urrea-secondary/40 focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50";

export function buttonClass(variant: ButtonVariant = "primary", className?: string) {
  return cn(buttonBase, buttonVariants[variant], className);
}

export function Button({
  variant = "primary",
  className,
  children,
  ...props
}: React.ButtonHTMLAttributes<HTMLButtonElement> & { variant?: ButtonVariant }) {
  return (
    <button className={buttonClass(variant, className)} {...props}>
      {children}
    </button>
  );
}

export function ButtonLink({
  href,
  variant = "primary",
  className,
  children,
}: {
  href: string;
  variant?: ButtonVariant;
  className?: string;
  children: React.ReactNode;
}) {
  return (
    <Link href={href} className={buttonClass(variant, className)}>
      {children}
    </Link>
  );
}
