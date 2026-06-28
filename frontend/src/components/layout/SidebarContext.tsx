"use client";

import { createContext, useCallback, useContext, useEffect, useState, type ReactNode } from "react";
import { getSidebarCollapsed, setSidebarCollapsed } from "@/lib/sidebar-prefs";

type SidebarContextValue = {
  collapsed: boolean;
  toggle: () => void;
  setCollapsed: (value: boolean) => void;
};

const SidebarContext = createContext<SidebarContextValue | null>(null);

export function SidebarProvider({ children }: { children: ReactNode }) {
  const [collapsed, setCollapsedState] = useState(false);

  useEffect(() => {
    setCollapsedState(getSidebarCollapsed());
  }, []);

  const setCollapsed = useCallback((value: boolean) => {
    setCollapsedState(value);
    setSidebarCollapsed(value);
  }, []);

  const toggle = useCallback(() => {
    setCollapsedState((prev) => {
      const next = !prev;
      setSidebarCollapsed(next);
      return next;
    });
  }, []);

  return (
    <SidebarContext.Provider value={{ collapsed, toggle, setCollapsed }}>
      {children}
    </SidebarContext.Provider>
  );
}

export function useSidebar() {
  const ctx = useContext(SidebarContext);
  if (!ctx) throw new Error("useSidebar must be used within SidebarProvider");
  return ctx;
}
