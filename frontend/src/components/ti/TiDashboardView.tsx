"use client";

import { useCallback, useEffect, useState } from "react";
import { ButtonLink } from "@/components/ui/button";
import { StatCard } from "@/components/ui/card";
import { TiExportButtons } from "@/components/ti/TiExportButtons";
import { PageContainer, PageHeader, Alert } from "@/components/ui/page-header";
import { ApiError } from "@/lib/api";
import { tiService, type EnvironmentInfo } from "@/lib/services/tiService";

export function TiDashboardView() {
  const [env, setEnv] = useState<EnvironmentInfo | null>(null);
  const [schemaCount, setSchemaCount] = useState(0);
  const [apiCount, setApiCount] = useState(0);
  const [snapshotCount, setSnapshotCount] = useState(0);
  const [error, setError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    setError(null);
    try {
      const [environment, schema, apis, snapshots] = await Promise.all([
        tiService.getEnvironment(),
        tiService.getSchema(),
        tiService.getApis(),
        tiService.listSnapshots(),
      ]);
      setEnv(environment);
      setSchemaCount(schema.tableCount);
      setApiCount(apis.endpointCount);
      setSnapshotCount(snapshots.length);
    } catch (err) {
      if (err instanceof ApiError && err.status === 401) {
        setError("Sesión expirada o sin permisos TI. Cierra sesión e ingresa de nuevo como administrador.");
      } else if (err instanceof ApiError && err.status === 404) {
        setError("La API no expone los endpoints TI. Reinicia con: ./scripts/stop.sh && ./scripts/start.sh");
      } else {
        setError(err instanceof Error ? err.message : "No se pudo cargar la información de TI.");
      }
    }
  }, []);

  useEffect(() => {
    refresh().catch(console.error);
  }, [refresh]);

  return (
    <PageContainer>
      <PageHeader
        title="Centro TI"
        subtitle="Schemas, APIs y snapshots para alinear desarrollo con producción."
      />

      {error && <Alert variant="error">{error}</Alert>}

      <div className="mb-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard label="Tablas" value={String(schemaCount)} accentClass="text-urrea-primary" />
        <StatCard label="Endpoints API" value={String(apiCount)} accentClass="text-urrea-secondary" />
        <StatCard label="Snapshots" value={String(snapshotCount)} accentClass="text-emerald-700" />
        <StatCard label="Migración" value={env?.migrationId?.slice(-12) ?? "—"} accentClass="text-indigo-700" />
      </div>

      {env && (
        <div className="mb-6 rounded-xl border border-slate-200 bg-white p-4 text-sm">
          <p><span className="font-medium">Ambiente:</span> {env.environmentName}</p>
          <p><span className="font-medium">Base de datos:</span> {env.databaseName}</p>
          <p><span className="font-medium">Migración aplicada:</span> {env.migrationId}</p>
        </div>
      )}

      <div className="mb-6 rounded-xl border border-slate-200 bg-white p-4">
        <h2 className="mb-1 text-sm font-semibold text-urrea-text">Exportar documentación</h2>
        <p className="mb-3 text-xs text-urrea-text-muted">
          Descarga archivos .txt para compartir con el equipo o comparar entre dev y producción.
        </p>
        <TiExportButtons />
      </div>

      <div className="flex flex-wrap gap-3">
        <ButtonLink href="/admin-ti">Centro Admin TI</ButtonLink>
        <ButtonLink href="/ti/schema">Ver schemas</ButtonLink>
        <ButtonLink href="/ti/apis" variant="secondary">Catálogo APIs</ButtonLink>
        <ButtonLink href="/ti/snapshots" variant="secondary">Snapshots</ButtonLink>
        <ButtonLink href="/ti/admin" variant="secondary">Explorador entidades</ButtonLink>
      </div>
    </PageContainer>
  );
}
