/** URREA Hub — tokens de branding. Ver frontend/BRANDING.md */

export const brand = {
  primary: "#023764",
  secondary: "#2E7FA8",
  background: "#FFFFFF",
  backgroundSoft: "#F5F6F7",
  text: "#1A1A1A",
  textMuted: "#6F7478",
  border: "#D9DDE1",
  accentSand: "#D8C7AE",
  chrome: "#C8D0D6",
} as const;

export const layout = {
  /** Altura mínima recomendada para targets táctiles (iOS HIG) */
  touchTarget: 44,
  /** Padding inferior main cuando hay bottom nav */
  mobileNavOffset: "4.5rem",
} as const;

export type BrandColor = keyof typeof brand;
