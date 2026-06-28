"use client";

import { useEffect, useState } from "react";
import { ButtonLink } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { PageContainer, PageHeader } from "@/components/ui/page-header";
import { portalService } from "@/lib/services/portalService";
import type { ModuloPortal } from "@/lib/types";

export function ModulePortalView({ codigo }: { codigo: string }) {
  const [modulo, setModulo] = useState<ModuloPortal | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);

  useEffect(() => {
    portalService.getModule(codigo)
      .then(setModulo)
      .catch(() => setError(true))
      .finally(() => setLoading(false));
  }, [codigo]);

  if (loading) {
    return (
      <PageContainer className="flex min-h-[40vh] items-center justify-center">
        <div className="h-8 w-8 animate-pulse rounded-full bg-urrea-secondary/30" />
      </PageContainer>
    );
  }

  if (error || !modulo) {
    return (
      <PageContainer className="animate-fade-up">
        <PageHeader title="Módulo no disponible" subtitle="Portal URREA" />
        <Card>
          <div className="py-10 text-center text-sm text-urrea-text-muted">
            Este módulo aún no está publicado. Contacta a Recursos Humanos.
            <div className="mt-6">
              <ButtonLink href="/portal" variant="secondary">Volver al portal</ButtonLink>
            </div>
          </div>
        </Card>
      </PageContainer>
    );
  }

  return (
    <PageContainer className="animate-fade-up">
      <PageHeader title={modulo.titulo} subtitle={modulo.subtitulo} />
      <Card>
        <div className="flex flex-col items-center py-10 text-center">
          <span className="text-5xl" aria-hidden>{modulo.icono ?? "📋"}</span>
          <p className="mt-4 max-w-md text-sm text-urrea-text-muted">{modulo.descripcion}</p>
          <p className="mt-2 text-xs font-medium uppercase tracking-wide text-urrea-secondary">
            Próximamente en URREA Hub
          </p>
          <p className="mt-1 text-[10px] text-urrea-chrome">
            Contenido administrado desde el panel RH · BD desarrollo
          </p>
          <ButtonLink href="/portal" variant="secondary" className="mt-6">
            Volver al portal
          </ButtonLink>
        </div>
      </Card>
    </PageContainer>
  );
}
