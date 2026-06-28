"use client";

import { Badge } from "@/components/ui/badge";
import { EmptyState } from "@/components/ui/page-header";
import type { CapacitacionEquipo } from "@/lib/types";

export function CapacitacionesTab({ capacitaciones }: { capacitaciones: CapacitacionEquipo[] }) {
  if (capacitaciones.length === 0) {
    return (
      <EmptyState message="Tu equipo aún no tiene cursos asignados por RH." />
    );
  }

  return (
    <>
      <p className="mb-4 text-sm text-urrea-text-muted">
        Cursos creados y asignados por Recursos Humanos para los miembros de tu equipo.
      </p>
      <ul className="space-y-3 md:hidden">
        {capacitaciones.map((c) => (
          <li key={c.inscripcionId} className="rounded-xl border border-urrea-border/80 p-3">
            <p className="font-medium text-urrea-text">{c.cursoNombre}</p>
            <p className="text-sm text-urrea-text-muted">{c.colaboradorNombre}</p>
            <div className="mt-2 flex flex-wrap gap-2">
              <Badge estado={c.estado} />
              {c.puntuacion != null && <span className="text-xs text-urrea-secondary">Calificación: {c.puntuacion}</span>}
            </div>
          </li>
        ))}
      </ul>
      <div className="hidden overflow-x-auto md:block">
        <table className="w-full min-w-[800px] text-left text-sm">
          <thead>
            <tr className="border-b border-urrea-border text-urrea-text-muted">
              <th className="pb-2 pr-4 font-medium">Colaborador</th>
              <th className="pb-2 pr-4 font-medium">Curso</th>
              <th className="pb-2 pr-4 font-medium">Modalidad</th>
              <th className="pb-2 pr-4 font-medium">Horas</th>
              <th className="pb-2 pr-4 font-medium">Inscripción</th>
              <th className="pb-2 pr-4 font-medium">Estado</th>
              <th className="pb-2 font-medium">Calificación</th>
            </tr>
          </thead>
          <tbody>
            {capacitaciones.map((c) => (
              <tr key={c.inscripcionId} className="border-b border-urrea-border/50">
                <td className="py-3 pr-4">
                  <p className="font-medium text-urrea-text">{c.colaboradorNombre}</p>
                  <p className="text-xs text-urrea-text-muted">{c.numeroEmpleado}</p>
                </td>
                <td className="py-3 pr-4">
                  <p>{c.cursoNombre}</p>
                  <p className="text-xs text-urrea-text-muted">{c.cursoCodigo}</p>
                </td>
                <td className="py-3 pr-4 text-urrea-text-muted">{c.modalidad ?? "—"}</td>
                <td className="py-3 pr-4">{c.duracionHoras}h</td>
                <td className="py-3 pr-4 text-urrea-text-muted">
                  {new Date(c.fechaInscripcion).toLocaleDateString("es-MX")}
                </td>
                <td className="py-3 pr-4"><Badge estado={c.estado} /></td>
                <td className="py-3">
                  {c.puntuacion != null ? (
                    <span className={c.aprobado ? "text-emerald-700" : "text-urrea-text-muted"}>
                      {c.puntuacion}{c.aprobado ? " ✓" : ""}
                    </span>
                  ) : "—"}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
}
