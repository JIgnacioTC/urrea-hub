"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Alert } from "@/components/ui/page-header";
import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";
import { saveSession } from "@/lib/auth";
import type { LoginResponse } from "@/lib/types";

export default function LoginPage() {
  const router = useRouter();
  const [identificador, setIdentificador] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      const res = await fetchApi<LoginResponse>(v1("/auth/login"), {
        method: "POST",
        body: JSON.stringify({ identificador, password }),
      });
      saveSession({
        token: res.token,
        colaboradorId: res.colaboradorId,
        nombreCompleto: res.nombreCompleto,
        numeroEmpleado: res.numeroEmpleado,
        roles: res.roles,
      });
      router.push(res.roles.includes("RhAdmin") ? "/rh/dashboard" : "/portal");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al iniciar sesión");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="flex min-h-[100dvh] flex-col lg:flex-row">
      {/* Hero — Next.js + gradiente de marca */}
      <div className="gradient-urrea relative flex flex-col justify-end px-6 py-10 text-white lg:w-[45%] lg:justify-center lg:px-12 lg:py-16">
        <div className="pointer-events-none absolute inset-0 overflow-hidden">
          <div className="absolute -right-20 -top-20 h-64 w-64 rounded-full bg-white/5 blur-3xl" />
          <div className="absolute -bottom-16 -left-16 h-48 w-48 rounded-full bg-urrea-accent-sand/10 blur-2xl" />
        </div>
        <div className="relative animate-fade-up">
          <p className="text-[11px] font-semibold uppercase tracking-[0.3em] text-urrea-chrome">Plataforma RH</p>
          <h1 className="mt-3 text-3xl font-bold tracking-tight sm:text-4xl lg:text-5xl">URREA Hub</h1>
          <p className="mt-4 max-w-sm text-base leading-relaxed text-urrea-chrome/95">
            Portal del colaborador optimizado para celular. Vacaciones, permisos y perfil en un solo lugar.
          </p>
        </div>
      </div>

      {/* Formulario */}
      <div className="flex flex-1 items-center justify-center gradient-urrea-subtle px-4 py-8 safe-bottom lg:px-10">
        <Card className="animate-fade-up-delay-1 w-full max-w-md shadow-soft-lg hover:shadow-soft-lg">
          <h2 className="text-xl font-semibold tracking-tight text-urrea-text">Iniciar sesión</h2>
          <p className="mt-1 text-sm text-urrea-text-muted">Número de empleado, correo o RFC</p>

          <form onSubmit={handleSubmit} className="mt-6 space-y-4">
            <Input
              label="Identificador"
              id="identificador"
              type="text"
              inputMode="text"
              autoComplete="username"
              value={identificador}
              onChange={(e) => setIdentificador(e.target.value)}
              placeholder="1003, email@urrea.com o RFC"
              required
            />
            <Input
              label="Contraseña"
              id="password"
              type="password"
              autoComplete="current-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
            {error && <Alert variant="error">{error}</Alert>}
            <Button type="submit" disabled={loading} className="w-full">
              {loading ? "Ingresando..." : "Iniciar sesión"}
            </Button>
          </form>

          <p className="mt-6 text-center text-xs">
            <Link href="/dh" className="text-sm font-medium text-urrea-primary underline-offset-2 hover:underline">
              Acceder al Centro Integral de Desarrollo Humano
            </Link>
          </p>
        </Card>
      </div>
    </div>
  );
}
