import type { DhIconName } from "@/components/dh/shared/icons";
import type { Session } from "@/lib/auth";
import { isJefe, isRhAdmin, isTiAdmin } from "@/lib/auth";

export type PortalNavLink = {
  href: string;
  label: string;
  shortLabel?: string;
  icon: DhIconName;
  jefeOnly?: boolean;
  rhOnly?: boolean;
  tiOnly?: boolean;
  children?: PortalNavLink[];
};

export type WorkspaceType = "portal" | "rh" | "ti" | "admin-ti" | "admin-dh";

export type WorkspaceConfig = {
  id: WorkspaceType;
  label: string;
  shortLabel: string;
  icon: DhIconName;
  rhOnly?: boolean;
  tiOnly?: boolean;
};

export const WORKSPACES: WorkspaceConfig[] = [
  { id: "portal", label: "Portal colaborador", shortLabel: "Portal", icon: "users" },
  { id: "rh", label: "Administración RH", shortLabel: "Admin RH", icon: "dashboard", rhOnly: true },
  { id: "ti", label: "Plataforma TI", shortLabel: "Admin TI", icon: "analytics", tiOnly: true },
];

export type PortalNavSection = {
  title: string;
  links: PortalNavLink[];
  jefeOnly?: boolean;
  rhOnly?: boolean;
  tiOnly?: boolean;
};

export const PORTAL_HOME: PortalNavLink = {
  href: "/portal",
  label: "Portal",
  shortLabel: "Inicio",
  icon: "dashboard",
};

export const NUCLEO_HCM_SECTION: PortalNavSection = {
  title: "Núcleo HCM",
  rhOnly: true,
  links: [
    { href: "/portal/hcm", label: "Centro HCM", shortLabel: "HCM", icon: "dashboard", rhOnly: true },
    { href: "/portal/hcm/personas", label: "Gestión de personas", shortLabel: "Personas", icon: "users", rhOnly: true },
    { href: "/portal/hcm/calidad-datos", label: "Calidad de datos", shortLabel: "Calidad", icon: "analytics", rhOnly: true },
    { href: "/portal/hcm/organigrama", label: "Organigrama", shortLabel: "Org.", icon: "chart", rhOnly: true },
  ],
};

const MI_DIA_LINKS: PortalNavLink[] = [
  { href: "/portal/mi-ficha", label: "Mi ficha", shortLabel: "Ficha", icon: "users" },
  { href: "/portal/asistencia", label: "Mi asistencia", shortLabel: "Asist.", icon: "clock" },
  { href: "/portal/vacaciones", label: "Vacaciones y permisos", shortLabel: "Tiempo", icon: "calendar" },
  { href: "/portal/permisos", label: "Mis Permisos", shortLabel: "Permisos", icon: "calendar" },
  { href: "/portal/onboarding", label: "Mi onboarding", shortLabel: "Onboard.", icon: "onboarding" },
];

const MI_DESARROLLO_LINKS: PortalNavLink[] = [
  { href: "/portal/beneficios", label: "Mis beneficios", shortLabel: "Benef.", icon: "gift" },
  { href: "/portal/mi-compensacion", label: "Mi compensación", shortLabel: "Comp.", icon: "analytics" },
  { href: "/portal/capacitaciones", label: "Mis capacitaciones", shortLabel: "Capac.", icon: "education" },
  { href: "/portal/evaluaciones", label: "Mis evaluaciones", shortLabel: "Eval.", icon: "chart" },
  { href: "/portal/encuestas", label: "Mis encuestas", shortLabel: "Encuest.", icon: "survey" },
  { href: "/portal/reconocimientos", label: "Reconocimientos", shortLabel: "Recon.", icon: "gift" },
  { href: "/portal/procesos", label: "Procesos internos", shortLabel: "Proc.", icon: "folder" },
];

const MI_EQUIPO_LINKS: PortalNavLink[] = [
  { href: "/portal/mi-equipo", label: "Mi equipo", shortLabel: "Equipo", icon: "users", jefeOnly: true },
  { href: "/portal/aprobaciones", label: "Aprobaciones", shortLabel: "Aprob.", icon: "calendar", jefeOnly: true },
  { href: "/portal/equipo/asistencia", label: "Asistencia del equipo", shortLabel: "Eq. asist.", icon: "clock", jefeOnly: true },
  { href: "/portal/equipo/asistencia/pendientes", label: "Validar asistencia", shortLabel: "Validar", icon: "clock", jefeOnly: true },
  { href: "/portal/equipo/onboarding", label: "Onboarding del equipo", shortLabel: "Onboard.", icon: "onboarding", jefeOnly: true },
  { href: "/portal/requisiciones", label: "Mis requisiciones", shortLabel: "Req.", icon: "folder", jefeOnly: true },
  { href: "/portal/aprobaciones/requisiciones", label: "Aprobar requisiciones", shortLabel: "Aprob.", icon: "folder", jefeOnly: true },
];

/** @deprecated Secciones agrupadas en MI_DIA_LINKS, MI_DESARROLLO_LINKS, MI_EQUIPO_LINKS */
const RH_ADMIN_SECTION: PortalNavSection = {
  title: "Administración RH",
  rhOnly: true,
    links: [
      { href: "/rh/dashboard", label: "Dashboard RH", shortLabel: "RH", icon: "dashboard", rhOnly: true },
      { href: "/rh/admin/portal", label: "Portal y contenido", shortLabel: "Portal", icon: "folder", rhOnly: true },
      { href: "/ti", label: "Centro TI", shortLabel: "TI", icon: "analytics", tiOnly: true },
      { href: "/rh/permisos", label: "Catálogo permisos", shortLabel: "Cat.", icon: "calendar", rhOnly: true },
    { href: "/rh/permisos/solicitudes", label: "Solicitudes", shortLabel: "Solic.", icon: "calendar", rhOnly: true },
    { href: "/rh/reportes", label: "Reportes", shortLabel: "Report.", icon: "analytics", rhOnly: true },
  ],
};

export const TI_SECTIONS: PortalNavSection[] = [
  {
    title: "Plataforma",
    links: [
      { href: "/ti", label: "Centro TI", shortLabel: "Inicio", icon: "dashboard" },
      { href: "/ti/schema", label: "Schemas BD", shortLabel: "Schema", icon: "folder" },
      { href: "/ti/apis", label: "Catálogo APIs", shortLabel: "APIs", icon: "analytics" },
      { href: "/ti/snapshots", label: "Snapshots dev/prod", shortLabel: "Snap.", icon: "calendar" },
    ],
  },
  {
    title: "Administración",
    links: [
      { href: "/admin-ti", label: "Centro Admin TI", shortLabel: "Admin TI", icon: "analytics" },
      { href: "/ti/admin", label: "Explorador de entidades", shortLabel: "Entidades", icon: "users" },
      { href: "/rh/dashboard", label: "Administración RH", shortLabel: "RH", icon: "dashboard", rhOnly: true },
    ],
  },
  {
    title: "Enlaces",
    links: [{ href: "/portal", label: "Portal colaborador", shortLabel: "Portal", icon: "dashboard" }],
  },
];

export const ADMIN_TI_SECTIONS: PortalNavSection[] = [
  {
    title: "Resumen",
    links: [{ href: "/admin-ti", label: "Dashboard TI", shortLabel: "Inicio", icon: "dashboard" }],
  },
  {
    title: "Seguridad",
    links: [
      { href: "/admin-ti/seguridad/roles", label: "Roles", shortLabel: "Roles", icon: "users" },
      { href: "/admin-ti/seguridad/permisos", label: "Permisos", shortLabel: "Perm.", icon: "folder" },
      { href: "/admin-ti/seguridad/matriz", label: "Matriz rol-permiso", shortLabel: "Matriz", icon: "analytics" },
      { href: "/admin-ti/seguridad/asignaciones", label: "Asignación de roles", shortLabel: "Asign.", icon: "users" },
    ],
  },
  {
    title: "Plataforma",
    links: [
      { href: "/ti/admin", label: "Explorador entidades", shortLabel: "Entidades", icon: "folder" },
      { href: "/ti", label: "Centro TI técnico", shortLabel: "TI", icon: "analytics" },
    ],
  },
  {
    title: "Enlaces",
    links: [{ href: "/portal", label: "Portal colaborador", shortLabel: "Portal", icon: "dashboard" }],
  },
];

export const ADMIN_DH_SECTIONS: PortalNavSection[] = [
  {
    title: "Resumen",
    links: [{ href: "/admin-dh", label: "Dashboard DH", shortLabel: "Inicio", icon: "dashboard" }],
  },
  {
    title: "Organización",
    links: [
      { href: "/admin-dh/organizacion/jefes", label: "Asignación de jefes", shortLabel: "Jefes", icon: "users" },
      { href: "/admin-dh/organizacion/areas", label: "Áreas", shortLabel: "Áreas", icon: "folder" },
      { href: "/admin-dh/organizacion/departamentos", label: "Departamentos", shortLabel: "Depto.", icon: "folder" },
      { href: "/admin-dh/organizacion/puestos", label: "Puestos", shortLabel: "Puestos", icon: "folder" },
      { href: "/admin-dh/organizacion/sedes", label: "Sedes", shortLabel: "Sedes", icon: "folder" },
      { href: "/admin-dh/organizacion/centros-costo", label: "Centros de costo", shortLabel: "CC", icon: "folder" },
    ],
  },
  {
    title: "Vacaciones y permisos",
    links: [
      { href: "/admin-dh/vacaciones/solicitudes", label: "Solicitudes", shortLabel: "Solic.", icon: "calendar" },
      { href: "/admin-dh/vacaciones/saldos", label: "Saldos", shortLabel: "Saldos", icon: "calendar" },
      { href: "/admin-dh/vacaciones/politicas", label: "Políticas", shortLabel: "Polít.", icon: "folder" },
      { href: "/admin-dh/vacaciones/tipos-ausencia", label: "Tipos de ausencia", shortLabel: "Tipos", icon: "folder" },
      { href: "/admin-dh/vacaciones/calendarios", label: "Calendarios", shortLabel: "Cal.", icon: "calendar" },
      { href: "/admin-dh/vacaciones/nomina", label: "Incidencias nómina", shortLabel: "Nómina", icon: "analytics" },
    ],
  },
  {
    title: "Control de asistencia",
    links: [
      { href: "/admin-dh/asistencia", label: "Monitor de asistencia", shortLabel: "Monitor", icon: "calendar" },
      { href: "/admin-dh/asistencia/incidencias", label: "Incidencias", shortLabel: "Incid.", icon: "calendar" },
      { href: "/admin-dh/asistencia/turnos", label: "Turnos", shortLabel: "Turnos", icon: "folder" },
      { href: "/admin-dh/asistencia/reglas", label: "Reglas", shortLabel: "Reglas", icon: "folder" },
      { href: "/admin-dh/asistencia/nomina", label: "Reporte nómina", shortLabel: "Nómina", icon: "analytics" },
      { href: "/admin-dh/asistencia/comercial", label: "Fuerza de ventas", shortLabel: "Comercial", icon: "users" },
    ],
  },
  {
    title: "Onboarding",
    links: [
      { href: "/admin-dh/onboarding", label: "Monitor onboarding", shortLabel: "Monitor", icon: "folder" },
      { href: "/admin-dh/onboarding/planes", label: "Planes", shortLabel: "Planes", icon: "folder" },
      { href: "/admin-dh/onboarding/plantillas", label: "Plantillas", shortLabel: "Plant.", icon: "folder" },
    ],
  },
  {
    title: "Compensaciones y beneficios",
    links: [
      { href: "/admin-dh/compensaciones", label: "Compensaciones", shortLabel: "Comp.", icon: "analytics" },
      { href: "/admin-dh/compensaciones/solicitudes", label: "Solicitudes de ajuste", shortLabel: "Ajustes", icon: "folder" },
      { href: "/admin-dh/beneficios", label: "Beneficios", shortLabel: "Benef.", icon: "gift" },
    ],
  },
  {
    title: "Workforce & talent",
    links: [
      { href: "/admin-dh/requisiciones", label: "Requisiciones", shortLabel: "Req.", icon: "folder" },
      { href: "/admin-dh/reclutamiento", label: "Reclutamiento", shortLabel: "Reclut.", icon: "users" },
      { href: "/admin-dh/reclutamiento/vacantes", label: "Vacantes", shortLabel: "Vac.", icon: "folder" },
      { href: "/admin-dh/reclutamiento/pipeline", label: "Pipeline", shortLabel: "Pipe.", icon: "analytics" },
    ],
  },
  {
    title: "Operación",
    links: [
      { href: "/portal/hcm/personas", label: "Gestión de personas", shortLabel: "Personas", icon: "users", rhOnly: true },
      { href: "/rh/dashboard", label: "Dashboard RH", shortLabel: "RH", icon: "dashboard", rhOnly: true },
    ],
  },
  {
    title: "Enlaces",
    links: [{ href: "/portal", label: "Portal colaborador", shortLabel: "Portal", icon: "dashboard" }],
  },
];

export const RH_SECTIONS: PortalNavSection[] = [
  {
    title: "General",
    links: [
      { href: "/rh/dashboard", label: "Dashboard", shortLabel: "Inicio", icon: "dashboard" },
      { href: "/admin-dh", label: "Centro Admin DH", shortLabel: "Admin DH", icon: "folder" },
    ],
  },
  NUCLEO_HCM_SECTION,
  {
    title: "Control de asistencia",
    links: [
      { href: "/rh/asistencias", label: "Importar asistencias", shortLabel: "Asistencias", icon: "clock" },
    ]
  },
  {
    title: "Permisos y ausencias",
    links: [
      { href: "/rh/permisos", label: "Catálogo LFT", shortLabel: "Catálogo", icon: "calendar" },
      { href: "/rh/permisos/solicitudes", label: "Solicitudes", shortLabel: "Solic.", icon: "calendar" },
      { href: "/rh/reportes", label: "Reportes", shortLabel: "Report.", icon: "analytics" },
      { href: "/rh/configuracion-permisos", label: "Configurar permisos", shortLabel: "Config", icon: "calendar" },
    ],
  },
  {
    title: "Portal y contenido",
    links: [{ href: "/rh/admin/portal", label: "Administración portal", shortLabel: "Portal", icon: "folder" }],
  },
  { title: "Colaborador", links: [{ href: "/portal", label: "Portal colaborador", shortLabel: "Portal", icon: "dashboard" }] },
];

export function isNavActive(pathname: string, href: string) {
  if (href === "/portal" || href === "/rh/dashboard" || href === "/admin-ti" || href === "/admin-dh") return pathname === href;
  if (href === "/portal/hcm") return pathname === "/portal/hcm";
  if (href === "/portal/mi-ficha") {
    return pathname === href || pathname === "/portal/perfil" || pathname.startsWith("/portal/mi-ficha/");
  }
  return pathname === href || pathname.startsWith(`${href}/`);
}

export function filterNavSections(sections: PortalNavSection[], session: Session) {
  return sections
    .filter((section) => {
      if (section.rhOnly && !isRhAdmin(session)) return false;
      if (section.tiOnly && !isTiAdmin(session)) return false;
      if (section.jefeOnly && !isJefe(session)) return false;
      return true;
    })
    .map((section) => ({
      ...section,
      links: section.links.filter((link) => {
        if (link.rhOnly && !isRhAdmin(session)) return false;
        if (link.tiOnly && !isTiAdmin(session)) return false;
        if (link.jefeOnly && !isJefe(session)) return false;
        return true;
      }),
    }))
    .filter((section) => section.links.length > 0);
}

export function buildPortalSections(session: Session): PortalNavSection[] {
  const filterLinks = (links: PortalNavLink[]) =>
    links.filter((link) => !link.jefeOnly || isJefe(session));

  const sections: PortalNavSection[] = [
    { title: "Mi día a día", links: filterLinks(MI_DIA_LINKS) },
    { title: "Desarrollo y beneficios", links: filterLinks(MI_DESARROLLO_LINKS) },
  ];

  const equipoLinks = filterLinks(MI_EQUIPO_LINKS);
  if (equipoLinks.length > 0) {
    sections.push({ title: "Liderazgo", links: equipoLinks, jefeOnly: true });
  }

  if (isRhAdmin(session)) {
    sections.unshift(NUCLEO_HCM_SECTION);
    sections.push(RH_ADMIN_SECTION);
  }
  return sections;
}

export function buildMobileNav(session: Session): PortalNavLink[] {
  return [
    PORTAL_HOME,
    { href: "/portal/asistencia", label: "Asistencia", shortLabel: "Asist.", icon: "clock" },
    { href: "/portal/vacaciones", label: "Tiempo libre", shortLabel: "Tiempo", icon: "calendar" },
    { href: "/portal/beneficios", label: "Beneficios", shortLabel: "Benef.", icon: "gift" },
  ];
}

export function getNavItemByPath(pathname: string, sections: PortalNavSection[]): PortalNavLink | undefined {
  const all = [PORTAL_HOME, ...sections.flatMap((s) => s.links)];
  return all.find((n) => isNavActive(pathname, n.href));
}

export function flattenRhLinks(sections: PortalNavSection[]) {
  return sections.flatMap((section) => section.links);
}

export function flattenTiLinks(sections: PortalNavSection[]) {
  return sections.flatMap((section) => section.links);
}
