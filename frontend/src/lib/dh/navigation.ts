import type { DhIconName } from "@/components/dh/shared/icons";
import type { DhRole } from "./types";

export interface DhNavItem {
  href: string;
  label: string;
  icon: DhIconName;
  shortLabel?: string;
  roles: DhRole[] | "all";
  group: "core" | "operaciones" | "talento" | "gobierno";
}

export const DH_NAV: DhNavItem[] = [
  { href: "/dh", label: "Centro de mando", icon: "dashboard", shortLabel: "Inicio", roles: "all", group: "core" },
  { href: "/dh/hcm", label: "Gestión de personas (HCM)", icon: "users", roles: ["admin_dh", "key_user", "lider", "ti"], group: "core" },
  { href: "/dh/reclutamiento", label: "Reclutamiento y selección", icon: "recruitment", roles: ["admin_dh", "key_user"], group: "operaciones" },
  { href: "/dh/onboarding", label: "Onboarding e incorporación", icon: "onboarding", roles: ["admin_dh", "key_user", "lider"], group: "operaciones" },
  { href: "/dh/expediente", label: "Expediente digital", icon: "folder", roles: ["admin_dh", "key_user", "lider", "colaborador"], group: "operaciones" },
  { href: "/dh/vacaciones", label: "Vacaciones y permisos", icon: "calendar", roles: "all", group: "operaciones" },
  { href: "/dh/asistencia", label: "Control de asistencia", icon: "clock", roles: ["admin_dh", "key_user", "lider", "ti"], group: "operaciones" },
  { href: "/dh/capacitacion", label: "Capacitación y LMS", icon: "education", roles: "all", group: "talento" },
  { href: "/dh/desempeno", label: "Desempeño, OKR y 360°", icon: "chart", roles: ["admin_dh", "key_user", "lider", "colaborador"], group: "talento" },
  { href: "/dh/encuestas", label: "Encuestas organizacionales", icon: "survey", roles: ["admin_dh", "key_user", "lider", "colaborador"], group: "talento" },
  { href: "/dh/comunicacion", label: "Comunicación interna", icon: "communication", roles: "all", group: "talento" },
  { href: "/dh/beneficios", label: "Beneficios y prestaciones", icon: "gift", roles: "all", group: "talento" },
  { href: "/dh/servicios", label: "Centro de servicios", icon: "support", roles: "all", group: "talento" },
  { href: "/dh/denuncias", label: "Canal de denuncias", icon: "shield", roles: ["admin_dh", "key_user"], group: "gobierno" },
  { href: "/dh/integraciones", label: "Integraciones SAP · CDM", icon: "integration", roles: ["admin_dh", "ti"], group: "gobierno" },
  { href: "/dh/analitica", label: "Analítica ejecutiva", icon: "analytics", roles: ["admin_dh", "key_user", "ti"], group: "gobierno" },
];

export const DH_GROUP_LABELS: Record<DhNavItem["group"], string> = {
  core: "Núcleo HCM",
  operaciones: "Operaciones de personal",
  talento: "Talento y experiencia",
  gobierno: "Gobierno, riesgo e integraciones",
};

export function filterNavByRole(role: DhRole): DhNavItem[] {
  return DH_NAV.filter((item) => item.roles === "all" || item.roles.includes(role));
}

export function getNavItemByPath(pathname: string): DhNavItem | undefined {
  return DH_NAV.find((n) => (n.href === "/dh" ? pathname === "/dh" : pathname.startsWith(n.href)));
}
