/** Rutas canónicas API v1 — preferir sobre legacy `/api/*`. */
export const API_V1 = "/api/v1";

export function v1(path: string) {
  const normalized = path.startsWith("/") ? path : `/${path}`;
  return `${API_V1}${normalized}`;
}

/** Legacy sin versionar — mantener solo durante migración. */
export function legacy(path: string) {
  const normalized = path.startsWith("/") ? path : `/${path}`;
  return `/api${normalized}`;
}
