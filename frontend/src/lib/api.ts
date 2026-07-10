import { clearSession } from "@/lib/auth";

export const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5018";

export class ApiError extends Error {
  constructor(
    message: string,
    public status: number,
  ) {
    super(message);
  }
}

function getToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem("urrea_token");
}

export async function fetchApi<T>(path: string, init?: RequestInit): Promise<T> {
  const token = getToken();
  const response = await fetch(`${API_URL}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...init?.headers,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    if (response.status === 401 && typeof window !== "undefined" && !path.includes("/auth/login")) {
      clearSession();
      window.location.href = "/login";
    }
    const body = await response.text();
    let message = `API error: ${response.status}`;
    try {
      const json = JSON.parse(body) as { error?: string };
      if (json.error) message = json.error;
    } catch {
      /* ignore */
    }
    throw new ApiError(message, response.status);
  }

  if (response.status === 204) return undefined as T;
  return response.json() as Promise<T>;
}

export async function fetchApiBlob(path: string): Promise<Blob> {
  const token = getToken();
  const response = await fetch(`${API_URL}${path}`, {
    headers: token ? { Authorization: `Bearer ${token}` } : {},
    cache: "no-store",
  });
  if (!response.ok) {
    if (response.status === 401 && typeof window !== "undefined" && !path.includes("/auth/login")) {
      clearSession();
      window.location.href = "/login";
    }
    throw new ApiError("Error al descargar", response.status);
  }
  return response.blob();
}
