"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { avatarGradient, getInitials, getTenureLabel } from "@/components/portal/profile-helpers";
import { ButtonLink } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { PageContainer } from "@/components/ui/page-header";
import { teamService } from "@/lib/services/teamService";
import { getSession, isJefe } from "@/lib/auth";
import { cn } from "@/lib/utils";
import type { ColaboradorPerfil } from "@/lib/types";

export function ColaboradorFichaView() {
  const params = useParams();
  const router = useRouter();
  const id = params.id as string;
  const [ficha, setFicha] = useState<ColaboradorPerfil | null>(null);

  useEffect(() => {
    const session = getSession();
    if (!session || !isJefe(session)) {
      router.replace("/portal");
      return;
    }
    teamService.getMember(id)
      .then(setFicha)
      .catch(() => router.replace("/portal/mi-equipo"));
  }, [id, router]);

  if (!ficha) {
    return (
      <PageContainer className="animate-pulse">
        <div className="h-8 w-48 rounded-lg bg-urrea-chrome/40" />
        <div className="mt-6 h-40 rounded-2xl bg-urrea-chrome/30" />
      </PageContainer>
    );
  }

  const initials = getInitials(ficha.nombre, ficha.apellidoPaterno, ficha.apellidoMaterno);

  return (
    <PageContainer>
      <Link href="/portal/mi-equipo" className="mb-4 inline-flex text-sm font-medium text-urrea-secondary hover:underline">
        ← Volver a Mi equipo
      </Link>

      <div className="overflow-hidden rounded-2xl border border-urrea-border/80 bg-urrea-bg shadow-soft">
        <div className="h-24 bg-gradient-to-r from-urrea-primary to-urrea-secondary sm:h-28" />
        <div className="px-4 pb-5 sm:px-6">
          <div className="-mt-12 flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
            <div className="flex items-end gap-4">
              <div
                className={cn(
                  "flex h-24 w-24 shrink-0 items-center justify-center rounded-2xl border-4 border-urrea-bg bg-gradient-to-br text-2xl font-bold text-white shadow-md",
                  avatarGradient(ficha.numeroEmpleado),
                )}
              >
                {initials}
              </div>
              <div className="min-w-0 pb-1">
                <h1 className="text-xl font-bold text-urrea-text sm:text-2xl">
                  {ficha.nombre} {ficha.apellidoPaterno}
                </h1>
                <p className="text-sm text-urrea-text-muted">{ficha.puesto}</p>
                <p className="text-xs text-urrea-chrome">#{ficha.numeroEmpleado}</p>
              </div>
            </div>
            {ficha.email && (
              <a
                href={`mailto:${ficha.email}`}
                className="inline-flex min-h-11 items-center justify-center rounded-xl border border-urrea-border px-4 text-sm font-medium text-urrea-primary no-underline hover:bg-urrea-bg-soft"
              >
                Contactar
              </a>
            )}
          </div>
        </div>
      </div>

      <div className="mt-4 grid gap-4 sm:grid-cols-2">
        <Card title="Información laboral">
          <dl className="space-y-3 text-sm">
            <div><dt className="text-urrea-text-muted">Departamento</dt><dd className="font-medium">{ficha.departamento}</dd></div>
            <div><dt className="text-urrea-text-muted">Sede</dt><dd className="font-medium">{ficha.sede ?? "—"}</dd></div>
            <div><dt className="text-urrea-text-muted">Antigüedad</dt><dd className="font-medium">{getTenureLabel(ficha.fechaIngreso)}</dd></div>
            <div><dt className="text-urrea-text-muted">Ingreso</dt><dd className="font-medium">{new Date(ficha.fechaIngreso).toLocaleDateString("es-MX")}</dd></div>
            {ficha.jefeDirecto && <div><dt className="text-urrea-text-muted">Jefe directo</dt><dd className="font-medium">{ficha.jefeDirecto}</dd></div>}
          </dl>
        </Card>
        <Card title="Contacto">
          <dl className="space-y-3 text-sm">
            <div><dt className="text-urrea-text-muted">Email</dt><dd className="font-medium">{ficha.email}</dd></div>
            <div><dt className="text-urrea-text-muted">Teléfono</dt><dd className="font-medium">{ficha.telefono ?? "—"}</dd></div>
            <div><dt className="text-urrea-text-muted">RFC</dt><dd className="font-medium">{ficha.rfc ?? "—"}</dd></div>
          </dl>
        </Card>
      </div>

      <Card title="Acciones rápidas" className="mt-4">
        <div className="flex flex-wrap gap-3">
          <ButtonLink href="/portal/mi-equipo" variant="secondary">Ver en Mi equipo</ButtonLink>
        </div>
      </Card>
    </PageContainer>
  );
}
