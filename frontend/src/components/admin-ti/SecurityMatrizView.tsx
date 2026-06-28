"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { Button } from "@/components/ui/button";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import {
  securityAdminService,
  type RolePermissionMatrixDto,
  type RolDto,
} from "@/lib/services/securityAdminService";

export function SecurityMatrizView() {
  const [matrix, setMatrix] = useState<RolePermissionMatrixDto | null>(null);
  const [draft, setDraft] = useState<Record<string, Record<string, boolean>>>({});
  const [modulo, setModulo] = useState("");
  const [selectedRole, setSelectedRole] = useState<RolDto | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setError(null);
    const data = await securityAdminService.getMatrix();
    setMatrix(data);
    const initial: Record<string, Record<string, boolean>> = {};
    for (const role of data.roles) {
      initial[role.codigo] = {};
      for (const row of data.rows) {
        initial[role.codigo][row.permisoId] = row.byRoleCodigo[role.codigo] ?? false;
      }
    }
    setDraft(initial);
    setSelectedRole((prev) => prev ?? data.roles[0] ?? null);
  }, []);

  useEffect(() => {
    load().catch((err) => setError(err instanceof Error ? err.message : "Error al cargar matriz."));
  }, [load]);

  const modulos = useMemo(() => {
    if (!matrix) return [];
    return Array.from(new Set(matrix.rows.map((r) => r.modulo))).sort();
  }, [matrix]);

  const filteredRows = useMemo(() => {
    if (!matrix) return [];
    if (!modulo) return matrix.rows;
    return matrix.rows.filter((r) => r.modulo === modulo);
  }, [matrix, modulo]);

  function toggle(roleCodigo: string, permisoId: string) {
    setDraft((prev) => ({
      ...prev,
      [roleCodigo]: {
        ...prev[roleCodigo],
        [permisoId]: !prev[roleCodigo]?.[permisoId],
      },
    }));
  }

  async function saveRole(role: RolDto) {
    if (!matrix) return;
    setSaving(true);
    setError(null);
    setSuccess(null);
    try {
      const permisoIds = matrix.rows
        .filter((row) => draft[role.codigo]?.[row.permisoId])
        .map((row) => row.permisoId);
      await securityAdminService.updateRolePermissions(role.id, permisoIds);
      setSuccess(`Permisos actualizados para ${role.nombre}.`);
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "No se pudo guardar.");
    } finally {
      setSaving(false);
    }
  }

  if (!matrix) {
    return (
      <PageContainer>
        <PageHeader title="Matriz rol-permiso" subtitle="Cargando…" />
        {error && <Alert variant="error">{error}</Alert>}
      </PageContainer>
    );
  }

  return (
    <PageContainer className="max-w-[90rem]">
      <PageHeader
        title="Matriz rol-permiso"
        subtitle="Filas: permisos. Columnas: roles. Activa o desactiva acceso por celda."
      />

      {error && <Alert variant="error">{error}</Alert>}
      {success && <Alert variant="success">{success}</Alert>}

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
        {selectedRole && (
          <Button onClick={() => saveRole(selectedRole)} disabled={saving}>
            {saving ? "Guardando…" : `Guardar ${selectedRole.nombre}`}
          </Button>
        )}
      </div>

      {filteredRows.length === 0 ? (
        <EmptyState message="No hay permisos para mostrar." />
      ) : (
        <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white">
          <table className="min-w-full text-left text-xs">
            <thead>
              <tr className="border-b bg-slate-50 text-slate-500">
                <th className="sticky left-0 z-10 bg-slate-50 px-3 py-2">Permiso</th>
                <th className="px-3 py-2">Módulo</th>
                {matrix.roles.map((role) => (
                  <th key={role.id} className="px-3 py-2 text-center">
                    <button
                      type="button"
                      className={`font-semibold ${selectedRole?.id === role.id ? "text-urrea-primary" : ""}`}
                      onClick={() => setSelectedRole(role)}
                    >
                      {role.codigo}
                    </button>
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {filteredRows.map((row) => (
                <tr key={row.permisoId} className="border-b border-slate-100">
                  <td className="sticky left-0 z-10 bg-white px-3 py-2">
                    <p className="font-medium">{row.codigo}</p>
                    <p className="text-slate-500">{row.nombre}</p>
                  </td>
                  <td className="px-3 py-2">{row.modulo}</td>
                  {matrix.roles.map((role) => (
                    <td key={role.id} className="px-3 py-2 text-center">
                      <input
                        type="checkbox"
                        checked={draft[role.codigo]?.[row.permisoId] ?? false}
                        onChange={() => toggle(role.codigo, row.permisoId)}
                        aria-label={`${row.codigo} — ${role.codigo}`}
                      />
                    </td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </PageContainer>
  );
}
