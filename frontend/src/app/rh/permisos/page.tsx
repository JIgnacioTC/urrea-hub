"use client";

import { useEffect, useState } from "react";
import { Card } from "@/components/ui/card";
import { PageContainer, PageHeader } from "@/components/ui/page-header";
import { absenceService } from "@/lib/services/absenceService";
import type { TipoAusencia } from "@/lib/types";

export default function RhPermisosCatalogPage() {
  const [tipos, setTipos] = useState<TipoAusencia[]>([]);

  useEffect(() => {
    absenceService.getPermissionTypes()
      .then((data) => setTipos(data.sort((a, b) => (a.orden ?? 0) - (b.orden ?? 0))))
      .catch(console.error);
  }, []);

  return (
    <PageContainer>
      <PageHeader
        title="Catálogo de permisos LFT"
        subtitle="Tipos configurados conforme a la Ley Federal del Trabajo"
      />

      <div className="grid gap-4 lg:grid-cols-2">
        {tipos.map((tipo) => (
          <Card key={tipo.id}>
            <div className="flex items-start gap-3">
              <span className="text-3xl" aria-hidden>{tipo.icono ?? "📄"}</span>
              <div className="min-w-0 flex-1">
                <div className="flex flex-wrap items-center gap-2">
                  <h3 className="font-semibold text-urrea-text">{tipo.nombre}</h3>
                  <code className="rounded bg-urrea-bg-soft px-1.5 py-0.5 text-[10px] text-urrea-text-muted">{tipo.codigo}</code>
                </div>
                <p className="mt-1 text-xs font-medium text-urrea-secondary">{tipo.baseLegalLft}</p>
                <p className="mt-2 text-sm text-urrea-text-muted">{tipo.descripcion}</p>
                <dl className="mt-3 grid grid-cols-2 gap-2 text-xs">
                  <div>
                    <dt className="text-urrea-text-muted">Categoría</dt>
                    <dd className="font-medium text-urrea-text">{tipo.categoria}</dd>
                  </div>
                  <div>
                    <dt className="text-urrea-text-muted">Goce de sueldo</dt>
                    <dd className="font-medium text-urrea-text">{tipo.remunerado ? "Sí" : "No"}</dd>
                  </div>
                  <div>
                    <dt className="text-urrea-text-muted">Parcial</dt>
                    <dd className="font-medium text-urrea-text">{tipo.esParcial ? "Sí (0.5 día)" : "No"}</dd>
                  </div>
                  <div>
                    <dt className="text-urrea-text-muted">Límite anual</dt>
                    <dd className="font-medium text-urrea-text">{tipo.diasMaximosAnuales ?? tipo.diasMaximosEvento ?? "—"}</dd>
                  </div>
                </dl>
              </div>
            </div>
          </Card>
        ))}
      </div>
    </PageContainer>
  );
}
