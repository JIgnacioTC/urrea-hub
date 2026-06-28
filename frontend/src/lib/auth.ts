"use client";

export interface Session {
  token: string;
  colaboradorId: string;
  nombreCompleto: string;
  numeroEmpleado: string;
  roles: string[];
}

const SESSION_KEY = "urrea_session";
const TOKEN_KEY = "urrea_token";

export function saveSession(session: Session) {
  localStorage.setItem(SESSION_KEY, JSON.stringify(session));
  localStorage.setItem(TOKEN_KEY, session.token);
}

export function getSession(): Session | null {
  if (typeof window === "undefined") return null;
  const raw = localStorage.getItem(SESSION_KEY);
  if (!raw) return null;
  try {
    return JSON.parse(raw) as Session;
  } catch {
    return null;
  }
}

export function clearSession() {
  localStorage.removeItem(SESSION_KEY);
  localStorage.removeItem(TOKEN_KEY);
}

export function hasRole(session: Session | null, role: string) {
  return session?.roles.includes(role) ?? false;
}

export function isRhAdmin(session: Session | null) {
  return hasRole(session, "RhAdmin");
}

export function isTiAdmin(session: Session | null) {
  return hasRole(session, "TiAdmin") || isRhAdmin(session);
}

export function isJefe(session: Session | null) {
  return hasRole(session, "Jefe") || isRhAdmin(session);
}
