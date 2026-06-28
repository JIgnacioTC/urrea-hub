import type { DhIconName } from "@/components/dh/shared/icons";

/** Icono SVG por código de tipo de ausencia (sin emojis). */
export function absenceTypeIcon(codigo: string): DhIconName {
  if (codigo === "VAC") return "calendar";
  if (codigo === "ENTRADA_TARDE" || codigo === "SALIDA_TEMPRANO") return "clock";
  if (codigo.includes("INCAP") || codigo.includes("MEDIC")) return "shield";
  if (codigo.includes("MATER") || codigo.includes("PATER")) return "users";
  return "folder";
}

export function isVacationType(codigo: string) {
  return codigo === "VAC";
}
