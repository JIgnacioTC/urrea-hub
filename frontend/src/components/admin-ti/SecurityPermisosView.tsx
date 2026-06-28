"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { securityAdminService, type PermisoDto } from "@/lib/services/securityAdminService";

export function SecurityPermisosView() {
  const [permisos, setPermisos] = useState<PermisoDto[]>([]);
  const [modulo, setModulo] = useState("");
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setError(null);
    try {
      setPermisos(await securityAdminService.listPermissions(modulo || undefined));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al cargar permisos.");
    }
  }, [modulo]);

  useEffect(() => {
    load().catch(console.error);
  }, [load]);

  const modulos = useMemo(() => {
    const set = new Set(permisos.map((p) => p.modulo));
    return Array.from(set).sort();
  }, [permisos]);

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader
        title="Permisos del sistema"
        subtitle="Catálogo de permisos disponibles por módulo. Se asignan a roles desde la matriz."
      />

      {error && <Alert variant="error">{error}</Alert>}

      <div className="flex flex-wrap items-center gap-3">
        <select
          className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
          value={modulo}
          onChange={(e) => setModulo(e.target.value)}
        >
          <option value="">Todos los módulos</option>
          {modulos.map((m) => (
            <option key={m} value={m}>
              {m}
            </option>
          ))}
        </select>
      </div>

      {permisos.length === 0 ? (
        <EmptyState message="No hay permisos para el filtro seleccionado." />
      ) : (
        <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white">
          <table className="min-w-full text-left text-sm">
            <thead>
              <tr className="border-b bg-slate-50 text-slate-500">
                <th className="px-4 py-3">Código</th>
                <th className="px-4 py-3">Módulo</th>
                <th className="px-4 py-3">Nombre</th>
                <th className="px-4 py-3">Descripción</th>
                <th className="px-4 py-3">Estado</th>
              </tr>
            </thead>
            <tbody>
              {permisos.map((p) => (
                <tr key={p.id} className="border-b border-slate-100">
                  <td className="px-4 py-3 font-mono text-xs">{p.codigo}</td>
                  <td className="px-4 py-3">{p.modulo}</td>
                  <td className="px-4 py-3">{p.nombre}</td>
                  <td className="px-4 py-3 text-slate-500">{p.descripcion ?? "—"}</td>
                  <td className="px-4 py-3">{p.isActive ? "Activo" : "Inactivo"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </PageContainer>
  );
}
