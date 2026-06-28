"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { ButtonLink } from "@/components/ui/button";
import { StatCard } from "@/components/ui/card";
import { Alert, PageContainer, PageHeader } from "@/components/ui/page-header";
import { ApiError } from "@/lib/api";
import { securityAdminService } from "@/lib/services/securityAdminService";

export function AdminTiDashboardView() {
  const [roleCount, setRoleCount] = useState(0);
  const [permisoCount, setPermisoCount] = useState(0);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    Promise.all([securityAdminService.listRoles(), securityAdminService.listPermissions()])
      .then(([roles, permisos]) => {
        setRoleCount(roles.length);
        setPermisoCount(permisos.length);
      })
      .catch((err) => {
        if (err instanceof ApiError && err.status === 403) {
          setError("Sin permisos de Administrador TI. Cierra sesión e ingresa de nuevo.");
        } else {
          setError(err instanceof Error ? err.message : "No se pudo cargar el resumen.");
        }
      });
  }, []);

  return (
    <PageContainer>
      <PageHeader
        title="Centro de Administración TI"
        subtitle="Seguridad, plataforma, integraciones y auditoría técnica."
      />

      {error && <Alert variant="error">{error}</Alert>}

      <div className="mb-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard label="Roles" value={String(roleCount)} accentClass="text-urrea-primary" />
        <StatCard label="Permisos" value={String(permisoCount)} accentClass="text-urrea-secondary" />
      </div>

      <div className="grid gap-4 sm:grid-cols-2">
        <Link
          href="/admin-ti/seguridad/matriz"
          className="rounded-xl border border-slate-200 bg-white p-4 transition hover:border-urrea-primary"
        >
          <h2 className="font-semibold">Matriz rol-permiso</h2>
          <p className="mt-1 text-sm text-slate-500">Configura qué puede hacer cada rol del sistema.</p>
        </Link>
        <Link
          href="/admin-ti/seguridad/asignaciones"
          className="rounded-xl border border-slate-200 bg-white p-4 transition hover:border-urrea-primary"
        >
          <h2 className="font-semibold">Asignación de roles</h2>
          <p className="mt-1 text-sm text-slate-500">Busca colaboradores y revisa permisos efectivos.</p>
        </Link>
      </div>

      <div className="mt-6 flex flex-wrap gap-3">
        <ButtonLink href="/admin-ti/seguridad/roles">Administrar roles</ButtonLink>
        <ButtonLink href="/ti" variant="secondary">Centro TI técnico</ButtonLink>
      </div>
    </PageContainer>
  );
}
