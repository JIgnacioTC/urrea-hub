"use client";

import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import type { DhRole } from "./types";
import { DH_ROLE_LABELS } from "./types";

const STORAGE_KEY = "dh_demo_role";

interface DhRoleContextValue {
  role: DhRole;
  setRole: (role: DhRole) => void;
  roleLabel: string;
}

const DhRoleContext = createContext<DhRoleContextValue | null>(null);

export function DhRoleProvider({ children }: { children: ReactNode }) {
  const [role, setRoleState] = useState<DhRole>("admin_dh");

  useEffect(() => {
    const saved = localStorage.getItem(STORAGE_KEY) as DhRole | null;
    if (saved && DH_ROLE_LABELS[saved]) setRoleState(saved);
  }, []);

  function setRole(r: DhRole) {
    setRoleState(r);
    localStorage.setItem(STORAGE_KEY, r);
  }

  return (
    <DhRoleContext.Provider value={{ role, setRole, roleLabel: DH_ROLE_LABELS[role] }}>
      {children}
    </DhRoleContext.Provider>
  );
}

export function useDhRole() {
  const ctx = useContext(DhRoleContext);
  if (!ctx) throw new Error("useDhRole must be used within DhRoleProvider");
  return ctx;
}

export const ALL_DH_ROLES: DhRole[] = ["admin_dh", "key_user", "lider", "colaborador", "ti"];
