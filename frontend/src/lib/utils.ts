import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

/** Combina clases Tailwind sin conflictos — patrón estándar en Next.js moderno */
export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}
