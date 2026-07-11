"use client";

import { useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { employeeService } from "@/lib/services/employeeService";
import type { EmployeeListItem } from "@/lib/types/hcm";

export function AdminPermisosRemotosView() {
  const [employees, setEmployees] = useState<EmployeeListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [saving, setSaving] = useState<string | null>(null);
  const [msg, setMsg] = useState("");
  const [error, setError] = useState("");
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const PAGE_SIZE = 25;

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const res = await employeeService.getEmployees({
        page,
        pageSize: PAGE_SIZE,
        search: search || undefined,
      });
      setEmployees(res.items);
      setTotal(res.total);
    } catch (e) {
      setError(e instanceof Error ? e.message : "Error al cargar colaboradores.");
    } finally {
      setLoading(false);
    }
  }, [page, search]);

  useEffect(() => { load(); }, [load]);

  async function toggleRemoto(emp: EmployeeListItem) {
    setSaving(emp.id);
    setError("");
    setMsg("");
    try {
      await employeeService.updateEmployee(emp.id, {
        puedenChecarRemotamente: !emp.puedenChecarRemotamente,
      });
      setMsg(`Permiso actualizado: ${emp.legalFullName}`);
      setEmployees(prev =>
        prev.map(e => e.id === emp.id ? { ...e, puedenChecarRemotamente: !e.puedenChecarRemotamente } : e)
      );
    } catch (e) {
      setError(e instanceof Error ? e.message : "Error al actualizar.");
    } finally {
      setSaving(null);
    }
  }

  const totalPages = Math.ceil(total / PAGE_SIZE);

  return (
    <PageContainer className="max-w-5xl">
      <PageHeader
        title="Permisos de Checado Remoto"
        subtitle="Gestiona qué colaboradores pueden registrar asistencia remotamente desde el portal o app móvil."
      />

      {/* Search */}
      <div className="mb-6">
        <Input
          id="search-remote"
          placeholder="Buscar por nombre o número de empleado..."
          value={search}
          onChange={e => { setSearch(e.target.value); setPage(1); }}
          className="max-w-md"
        />
      </div>

      {msg && <Alert variant="success">{msg}</Alert>}
      {error && <Alert variant="error">{error}</Alert>}

      {loading ? (
        <p className="py-10 text-center text-sm text-urrea-text-muted">Cargando colaboradores…</p>
      ) : employees.length === 0 ? (
        <EmptyState message="No se encontraron colaboradores con ese criterio de búsqueda." />
      ) : (
        <>
          {/* Summary badge */}
          <div className="mb-3 flex items-center gap-3">
            <span className="text-xs text-urrea-text-muted">
              {employees.filter(e => e.puedenChecarRemotamente).length} de {employees.length} mostrados con checado remoto activo
            </span>
          </div>

          {/* Table */}
          <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-xs">
            <table className="min-w-full text-sm">
              <thead className="border-b border-slate-100 bg-slate-50/60">
                <tr>
                  <th className="px-4 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wide">Colaborador</th>
                  <th className="px-4 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wide">No. Empleado</th>
                  <th className="px-4 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wide">Puesto</th>
                  <th className="px-4 py-3 text-left text-xs font-semibold text-slate-500 uppercase tracking-wide">Departamento</th>
                  <th className="px-4 py-3 text-center text-xs font-semibold text-slate-500 uppercase tracking-wide">Checado Remoto</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {employees.map(emp => (
                  <tr
                    key={emp.id}
                    className="group transition-colors hover:bg-slate-50/70"
                  >
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-3">
                        <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-urrea-primary/10 text-xs font-semibold text-urrea-primary">
                          {emp.legalFullName?.charAt(0) ?? "?"}
                        </div>
                        <div>
                          <p className="font-medium text-slate-900 leading-tight">{emp.legalFullName}</p>
                          <p className="text-xs text-slate-400">{emp.employeeNumber ?? emp.department}</p>
                        </div>
                      </div>
                    </td>
                    <td className="px-4 py-3 font-mono text-xs text-slate-600">{emp.employeeNumber ?? "—"}</td>
                    <td className="px-4 py-3 text-slate-700">{emp.position ?? "—"}</td>
                    <td className="px-4 py-3 text-slate-700">{emp.department ?? "—"}</td>
                    <td className="px-4 py-3 text-center">
                      <button
                        type="button"
                        disabled={saving === emp.id}
                        onClick={() => toggleRemoto(emp)}
                        aria-label={emp.puedenChecarRemotamente ? "Desactivar checado remoto" : "Activar checado remoto"}
                        className={`
                          relative inline-flex h-6 w-11 items-center rounded-full transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-urrea-primary/30 focus:ring-offset-1
                          ${emp.puedenChecarRemotamente
                            ? "bg-urrea-primary"
                            : "bg-slate-200"}
                          ${saving === emp.id ? "opacity-60 cursor-not-allowed" : "cursor-pointer"}
                        `}
                      >
                        <span className={`
                          inline-block h-4 w-4 transform rounded-full bg-white shadow-sm transition-transform duration-200
                          ${emp.puedenChecarRemotamente ? "translate-x-6" : "translate-x-1"}
                        `} />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="mt-4 flex items-center justify-between">
              <p className="text-xs text-urrea-text-muted">
                Mostrando {(page - 1) * PAGE_SIZE + 1}–{Math.min(page * PAGE_SIZE, total)} de {total}
              </p>
              <div className="flex gap-2">
                <Button
                  type="button"
                  variant="secondary"
                  disabled={page === 1}
                  onClick={() => setPage(p => p - 1)}
                >
                  Anterior
                </Button>
                <Button
                  type="button"
                  variant="secondary"
                  disabled={page >= totalPages}
                  onClick={() => setPage(p => p + 1)}
                >
                  Siguiente
                </Button>
              </div>
            </div>
          )}
        </>
      )}
    </PageContainer>
  );
}
