"use client";

import { useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import {
  securityAdminService,
  type ColaboradorAccessDetailDto,
  type ColaboradorAccessSummaryDto,
  type RolDto,
} from "@/lib/services/securityAdminService";

export function SecurityAsignacionesView() {
  const [search, setSearch] = useState("");
  const [results, setResults] = useState<ColaboradorAccessSummaryDto[]>([]);
  const [selected, setSelected] = useState<ColaboradorAccessDetailDto | null>(null);
  const [allRoles, setAllRoles] = useState<RolDto[]>([]);
  const [roleToAssign, setRoleToAssign] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    securityAdminService.listRoles().then(setAllRoles).catch(console.error);
  }, []);

  const searchColaboradores = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setResults(await securityAdminService.searchColaboradores(search.trim() || undefined));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error en búsqueda.");
    } finally {
      setLoading(false);
    }
  }, [search]);

  useEffect(() => {
    const t = setTimeout(() => {
      searchColaboradores().catch(console.error);
    }, 300);
    return () => clearTimeout(t);
  }, [searchColaboradores]);

  async function loadDetail(id: string) {
    setError(null);
    try {
      setSelected(await securityAdminService.getColaboradorAccess(id));
    } catch (err) {
      setError(err instanceof Error ? err.message : "No se pudo cargar el detalle.");
    }
  }

  async function assignRole() {
    if (!selected || !roleToAssign) return;
    setError(null);
    try {
      await securityAdminService.assignRole(selected.id, roleToAssign);
      setRoleToAssign("");
      await loadDetail(selected.id);
      await searchColaboradores();
    } catch (err) {
      setError(err instanceof Error ? err.message : "No se pudo asignar el rol.");
    }
  }

  async function removeRole(rolId: string) {
    if (!selected) return;
    setError(null);
    try {
      await securityAdminService.removeRole(selected.id, rolId);
      await loadDetail(selected.id);
      await searchColaboradores();
    } catch (err) {
      setError(err instanceof Error ? err.message : "No se pudo quitar el rol.");
    }
  }

  const assignedIds = new Set(selected?.roles.map((r) => r.id) ?? []);
  const availableRoles = allRoles.filter((r) => r.isActive && !assignedIds.has(r.id));

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader
        title="Asignación de roles"
        subtitle="Busca colaboradores, revisa roles actuales y permisos efectivos combinados."
      />

      {error && <Alert variant="error">{error}</Alert>}

      <input
        className="w-full max-w-md rounded-lg border border-slate-200 px-3 py-2 text-sm"
        placeholder="Buscar por nombre o número de empleado…"
        value={search}
        onChange={(e) => setSearch(e.target.value)}
      />

      <div className="grid gap-4 lg:grid-cols-[320px_1fr]">
        <div className="max-h-[70vh] overflow-y-auto rounded-xl border border-slate-200 bg-white">
          {loading && <p className="p-4 text-sm text-slate-500">Buscando…</p>}
          {!loading && results.length === 0 && (
            <p className="p-4 text-sm text-slate-500">Sin resultados.</p>
          )}
          {results.map((c) => (
            <button
              key={c.id}
              type="button"
              onClick={() => loadDetail(c.id)}
              className={`block w-full border-b border-slate-100 px-4 py-3 text-left text-sm hover:bg-slate-50 ${
                selected?.id === c.id ? "bg-slate-100" : ""
              }`}
            >
              <p className="font-medium">{c.nombreCompleto}</p>
              <p className="text-xs text-slate-500">
                {c.numeroEmpleado} · {c.puesto}
              </p>
              <p className="mt-1 text-xs text-slate-400">{c.roles.join(", ") || "Sin roles"}</p>
            </button>
          ))}
        </div>

        <div className="rounded-xl border border-slate-200 bg-white p-4">
          {!selected && <EmptyState message="Selecciona un colaborador para ver su acceso." />}
          {selected && (
            <div className="space-y-4">
              <div>
                <h2 className="text-lg font-semibold">{selected.nombreCompleto}</h2>
                <p className="text-sm text-slate-500">
                  Número empleado: {selected.numeroEmpleado} · {selected.puesto} · {selected.departamento}
                  {selected.area ? ` · ${selected.area}` : ""}
                </p>
              </div>

              <div>
                <h3 className="mb-2 text-sm font-semibold">Roles asignados</h3>
                <div className="flex flex-wrap gap-2">
                  {selected.roles.length === 0 && (
                    <span className="text-sm text-slate-500">Sin roles asignados.</span>
                  )}
                  {selected.roles.map((role) => (
                    <span
                      key={role.id}
                      className="inline-flex items-center gap-2 rounded-full bg-slate-100 px-3 py-1 text-xs font-medium"
                    >
                      {role.nombre}
                      <button
                        type="button"
                        className="text-red-600 hover:underline"
                        onClick={() => removeRole(role.id)}
                      >
                        Quitar
                      </button>
                    </span>
                  ))}
                </div>
              </div>

              <div className="flex flex-wrap items-end gap-2">
                <div>
                  <label className="mb-1 block text-xs text-slate-500">Asignar rol</label>
                  <select
                    className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
                    value={roleToAssign}
                    onChange={(e) => setRoleToAssign(e.target.value)}
                  >
                    <option value="">Seleccionar…</option>
                    {availableRoles.map((r) => (
                      <option key={r.id} value={r.id}>
                        {r.nombre}
                      </option>
                    ))}
                  </select>
                </div>
                <Button type="button" onClick={assignRole} disabled={!roleToAssign}>
                  Asignar
                </Button>
              </div>

              <div>
                <h3 className="mb-2 text-sm font-semibold">Permisos efectivos</h3>
                <div className="flex flex-wrap gap-1.5">
                  {selected.permisosEfectivos.length === 0 && (
                    <span className="text-sm text-slate-500">Sin permisos efectivos.</span>
                  )}
                  {selected.permisosEfectivos.map((p) => (
                    <span key={p} className="rounded bg-emerald-50 px-2 py-0.5 font-mono text-xs text-emerald-800">
                      {p}
                    </span>
                  ))}
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </PageContainer>
  );
}
