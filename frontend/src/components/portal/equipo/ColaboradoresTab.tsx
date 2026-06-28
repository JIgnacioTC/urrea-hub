"use client";

import Link from "next/link";
import { avatarGradient, initialsFromFullName } from "@/components/portal/profile-helpers";
import { EmptyState } from "@/components/ui/page-header";
import type { EquipoMiembro } from "@/lib/types";
import { cn } from "@/lib/utils";

export function ColaboradoresTab({ miembros }: { miembros: EquipoMiembro[] }) {
  if (miembros.length === 0) {
    return <EmptyState message="No tienes colaboradores asignados directamente." />;
  }

  return (
    <>
      <ul className="space-y-3 md:hidden">
        {miembros.map((m) => (
          <li key={m.id} className="rounded-xl border border-urrea-border/80 p-3">
            <div className="flex items-center gap-3">
              <div
                className={cn(
                  "flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-gradient-to-br text-xs font-bold text-white",
                  avatarGradient(m.numeroEmpleado),
                )}
              >
                {initialsFromFullName(m.nombreCompleto)}
              </div>
              <div className="min-w-0 flex-1">
                <p className="font-semibold text-urrea-text">{m.nombreCompleto}</p>
                <p className="text-xs text-urrea-text-muted">{m.puesto}</p>
              </div>
            </div>
            <Link
              href={`/portal/mi-equipo/${m.id}`}
              className="mt-3 inline-flex min-h-10 w-full items-center justify-center rounded-xl bg-urrea-primary/8 text-sm font-medium text-urrea-primary"
            >
              Ver ficha
            </Link>
          </li>
        ))}
      </ul>

      <div className="hidden overflow-x-auto md:block">
        <table className="w-full min-w-[720px] text-left text-sm">
          <thead>
            <tr className="border-b border-urrea-border text-urrea-text-muted">
              <th className="pb-2 pr-4 font-medium">Colaborador</th>
              <th className="pb-2 pr-4 font-medium">No. empleado</th>
              <th className="pb-2 pr-4 font-medium">Puesto</th>
              <th className="pb-2 pr-4 font-medium">Departamento</th>
              <th className="pb-2 pr-4 font-medium">Email</th>
              <th className="pb-2 font-medium">Acción</th>
            </tr>
          </thead>
          <tbody>
            {miembros.map((m) => (
              <tr key={m.id} className="border-b border-urrea-border/50 transition hover:bg-urrea-bg-soft/40">
                <td className="py-3 pr-4">
                  <div className="flex items-center gap-3">
                    <div
                      className={cn(
                        "flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-gradient-to-br text-[10px] font-bold text-white",
                        avatarGradient(m.numeroEmpleado),
                      )}
                    >
                      {initialsFromFullName(m.nombreCompleto)}
                    </div>
                    <span className="font-medium text-urrea-text">{m.nombreCompleto}</span>
                  </div>
                </td>
                <td className="py-3 pr-4 text-urrea-text-muted">{m.numeroEmpleado}</td>
                <td className="py-3 pr-4">{m.puesto}</td>
                <td className="py-3 pr-4 text-urrea-text-muted">{m.departamento}</td>
                <td className="py-3 pr-4 text-urrea-text-muted">{m.email}</td>
                <td className="py-3">
                  <Link
                    href={`/portal/mi-equipo/${m.id}`}
                    className="inline-flex min-h-9 items-center rounded-lg px-3 text-sm font-medium text-urrea-secondary hover:bg-urrea-primary/8"
                  >
                    Ver ficha →
                  </Link>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}
