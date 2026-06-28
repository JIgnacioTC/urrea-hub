"use client";

import { createContext, useContext, useState, type ReactNode } from "react";
import { cn } from "@/lib/utils";

type TabsContextValue = {
  value: string;
  onChange: (value: string) => void;
};

const TabsContext = createContext<TabsContextValue | null>(null);

export function Tabs({
  defaultValue,
  value: controlledValue,
  onValueChange,
  className,
  children,
}: {
  defaultValue: string;
  value?: string;
  onValueChange?: (value: string) => void;
  className?: string;
  children: ReactNode;
}) {
  const [internal, setInternal] = useState(defaultValue);
  const value = controlledValue ?? internal;
  const onChange = (next: string) => {
    setInternal(next);
    onValueChange?.(next);
  };

  return (
    <TabsContext.Provider value={{ value, onChange }}>
      <div className={className}>{children}</div>
    </TabsContext.Provider>
  );
}

export function TabsList({ className, children }: { className?: string; children: ReactNode }) {
  return (
    <div
      className={cn(
        "flex gap-1 overflow-x-auto rounded-2xl border border-urrea-border/70 bg-urrea-bg p-1 shadow-soft scrollbar-none",
        className,
      )}
      role="tablist"
    >
      {children}
    </div>
  );
}

export function TabsTrigger({ value, className, children }: { value: string; className?: string; children: ReactNode }) {
  const ctx = useContext(TabsContext);
  if (!ctx) throw new Error("TabsTrigger must be used within Tabs");
  const active = ctx.value === value;
  return (
    <button
      type="button"
      role="tab"
      aria-selected={active}
      onClick={() => ctx.onChange(value)}
      className={cn(
        "shrink-0 rounded-xl px-4 py-2.5 text-sm font-medium transition-all duration-200",
        active
          ? "bg-urrea-primary text-white shadow-soft"
          : "text-urrea-text-muted hover:bg-urrea-bg-soft hover:text-urrea-primary",
        className,
      )}
    >
      {children}
    </button>
  );
}

export function TabsContent({ value, className, children }: { value: string; className?: string; children: ReactNode }) {
  const ctx = useContext(TabsContext);
  if (!ctx) throw new Error("TabsContent must be used within Tabs");
  if (ctx.value !== value) return null;
  return (
    <div role="tabpanel" className={cn("animate-fade-up", className)}>
      {children}
    </div>
  );
}

export function useTabsValue() {
  const ctx = useContext(TabsContext);
  return ctx?.value;
}
