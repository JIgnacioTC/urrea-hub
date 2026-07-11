import type { DhIconName } from "@/components/dh/shared/icons";

export type PortalQuickAction = {
  href: string;
  label: string;
  description: string;
  icon: DhIconName;
  infoTitle: string;
  infoContent: string;
  jefeOnly?: boolean;
  accent?: "primary" | "secondary" | "neutral";
};

export const PORTAL_QUICK_ACTIONS: PortalQuickAction[] = [
  {
    href: "/portal/asistencia",
    label: "Asistencia y horarios",
    description: "Entrada, salida y turno",
    icon: "clock",
    infoTitle: "Control de asistencia y horarios",
    infoContent:
      "Registra tu entrada y salida del día, consulta tu turno activo y solicita cambios de horario. Si olvidaste marcar o hubo un error, puedes solicitar una corrección desde la misma pantalla.",
    accent: "primary",
  },
  {
    href: "/portal/vacaciones",
    label: "Vacaciones y permisos",
    description: "Solicitar y consultar",
    icon: "calendar",
    infoTitle: "Vacaciones y permisos",
    infoContent:
      "Un solo formulario para vacaciones y permisos LFT. Indica fechas, revisa días hábiles y envía para aprobación.",
    accent: "primary",
  },
  {
    href: "/portal/beneficios",
    label: "Beneficios",
    description: "SGMM, vales y más",
    icon: "gift",
    infoTitle: "Beneficios",
    infoContent:
      "Explora el catálogo de beneficios URREA, consulta convenios y solicita alta o cambios. Algunos requieren aprobación de RH.",
    accent: "secondary",
  },
  {
    href: "/portal/mi-ficha",
    label: "Mi ficha",
    description: "Datos personales",
    icon: "users",
    infoTitle: "Mi ficha",
    infoContent:
      "Tu información de contacto, puesto, antigüedad y datos laborales. Si detectas algo incorrecto, contacta a Recursos Humanos.",
    accent: "neutral",
  },
  {
    href: "/portal/mi-compensacion",
    label: "Compensación",
    description: "Paquete y solicitudes",
    icon: "analytics",
    infoTitle: "Mi compensación",
    infoContent:
      "Consulta beneficios activos y solicitudes relacionadas con tu paquete de compensación. Los montos salariales dependen de la política de visibilidad de tu puesto.",
    accent: "neutral",
  },
  {
    href: "/portal/onboarding",
    label: "Onboarding",
    description: "Plan de ingreso",
    icon: "onboarding",
    infoTitle: "Onboarding",
    infoContent:
      "Si eres nuevo o en proceso de incorporación, aquí verás tus tareas pendientes: documentos, inducciones, accesos y fechas compromiso.",
    accent: "neutral",
  },
  {
    href: "/portal/mi-equipo",
    label: "Mi equipo",
    description: "Gestión de jefe",
    icon: "users",
    infoTitle: "Mi equipo",
    infoContent:
      "Como jefe, consulta a tus colaboradores directos, aprueba solicitudes y da seguimiento a asistencia, vacaciones y onboarding del equipo.",
    jefeOnly: true,
    accent: "primary",
  },
];

export const PORTAL_HELP = {
  home: {
    title: "Tu portal URREA",
    content:
      "Desde aquí accedes a lo esencial del día a día: asistencia, vacaciones, permisos y beneficios. Usa los accesos rápidos o el menú inferior en tu celular.",
  },
  announcements: {
    title: "Comunicados",
    content:
      "Anuncios y reconocimientos de la organización. Por ahora son informativos; pronto podrás interactuar con ellos.",
  },
  pending: {
    title: "Pendientes",
    content:
      "Solicitudes que requieren tu atención como jefe: vacaciones, permisos, asistencia o requisiciones de personal.",
  },
} as const;
