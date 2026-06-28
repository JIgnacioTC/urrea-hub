"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { ApiError } from "@/lib/api";
import { securityAdminService, type RolDto } from "@/lib/services/securityAdminService";

const emptyForm = { codigo: "", nombre: "", descripcion: "", isActive: true };

export function SecurityRolesView() {
  const [roles, setRoles] = useState<RolDto[]>([]);
  const [form, setForm] = useState(emptyForm);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setError(null);
    try {
      setRoles(await securityAdminService.listRoles());
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al cargar roles.");
    }
  }, []);

  useEffect(() => {
    load().catch(console.error);
  }, [load]);

  function startEdit(role: RolDto) {
    setEditingId(role.id);
    setForm({
      codigo: role.codigo,
      nombre: role.nombre,
      descripcion: role.descripcion ?? "",
      isActive: role.isActive,
    });
  }

  function cancelEdit() {
    setEditingId(null);
    setForm(emptyForm);
  }

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setSaving(true);
    setError(null);
    try {
      const dto = {
        codigo: form.codigo.trim(),
        nombre: form.nombre.trim(),
        descripcion: form.descripcion.trim() || undefined,
        isActive: form.isActive,
      };
      if (editingId) {
        await securityAdminService.updateRole(editingId, dto);
      } else {
        await securityAdminService.createRole(dto);
      }
      cancelEdit();
      await load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "No se pudo guardar el rol.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader
        title="Roles del sistema"
        subtitle="Crea y administra roles de acceso. Los permisos se configuran en la matriz rol-permiso."
      />

      {error && <Alert variant="error">{error}</Alert>}

      <form onSubmit={onSubmit} className="rounded-xl border border-slate-200 bg-white p-4">
        <h2 className="mb-3 text-sm font-semibold">{editingId ? "Editar rol" : "Nuevo rol"}</h2>
        <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
          <input
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
            placeholder="Código (ej. RhAdmin)"
            value={form.codigo}
            onChange={(e) => setForm({ ...form, codigo: e.target.value })}
            required
            disabled={!!editingId}
          />
          <input
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm"
            placeholder="Nombre visible"
            value={form.nombre}
            onChange={(e) => setForm({ ...form, nombre: e.target.value })}
            required
          />
          <input
            className="rounded-lg border border-slate-200 px-3 py-2 text-sm sm:col-span-2"
            placeholder="Descripción"
            value={form.descripcion}
            onChange={(e) => setForm({ ...form, descripcion: e.target.value })}
          />
        </div>
        <div className="mt-3 flex items-center gap-4">
          <label className="flex items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={form.isActive}
              onChange={(e) => setForm({ ...form, isActive: e.target.checked })}
            />
            Activo
          </label>
          <Button type="submit" disabled={saving}>
            {saving ? "Guardando…" : editingId ? "Actualizar" : "Crear rol"}
          </Button>
          {editingId && (
            <Button type="button" variant="secondary" onClick={cancelEdit}>
              Cancelar
            </Button>
          )}
        </div>
      </form>

      {roles.length === 0 ? (
        <EmptyState message="No hay roles registrados." />
      ) : (
        <div className="overflow-x-auto rounded-xl border border-slate-200 bg-white">
          <table className="min-w-full text-left text-sm">
            <thead>
              <tr className="border-b bg-slate-50 text-slate-500">
                <th className="px-4 py-3">Código</th>
                <th className="px-4 py-3">Nombre</th>
                <th className="px-4 py-3">Permisos</th>
                <th className="px-4 py-3">Colaboradores</th>
                <th className="px-4 py-3">Estado</th>
                <th className="px-4 py-3" />
              </tr>
            </thead>
            <tbody>
              {roles.map((role) => (
                <tr key={role.id} className="border-b border-slate-100">
                  <td className="px-4 py-3 font-mono text-xs">{role.codigo}</td>
                  <td className="px-4 py-3">{role.nombre}</td>
                  <td className="px-4 py-3">{role.permisoCount}</td>
                  <td className="px-4 py-3">{role.colaboradorCount}</td>
                  <td className="px-4 py-3">{role.isActive ? "Activo" : "Inactivo"}</td>
                  <td className="px-4 py-3">
                    <button type="button" className="text-sm text-urrea-primary hover:underline" onClick={() => startEdit(role)}>
                      Editar
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </PageContainer>
  );
}
