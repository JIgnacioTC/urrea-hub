"use client";

import { useEffect, useState } from "react";
import { Card } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { PageContainer, PageHeader } from "@/components/ui/page-header";
import { rhService } from "@/lib/services/teamService";
import type { ColaboradorResumen, PagedResult } from "@/lib/types";

export default function RhColaboradoresPage() {
  const [data, setData] = useState<PagedResult<ColaboradorResumen> | null>(null);
  const [search, setSearch] = useState("");

  useEffect(() => {
    rhService.getEmployees(1, 50, search || undefined)
      .then(setData)
      .catch(console.error);
  }, [search]);

  return (
    <PageContainer>
      <PageHeader title="Colaboradores" />

      <Input
        type="search"
        placeholder="Buscar..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        className="max-w-md"
      />

      <Card>
        <ul className="space-y-3 md:hidden">
          {data?.items.map((c) => (
            <li key={c.id} className="rounded-xl border border-urrea-border p-3">
              <p className="font-medium text-urrea-text">{c.nombreCompleto}</p>
              <p className="text-xs text-urrea-text-muted">{c.numeroEmpleado} · {c.puesto}</p>
            </li>
          ))}
        </ul>
        <div className="hidden overflow-x-auto md:block">
          <table className="w-full min-w-[480px] text-left text-sm">
            <thead>
              <tr className="border-b border-urrea-border text-urrea-text-muted">
                <th className="pb-2 pr-4 font-medium">No. empleado</th>
                <th className="pb-2 pr-4 font-medium">Nombre</th>
                <th className="pb-2 font-medium">Puesto</th>
              </tr>
            </thead>
            <tbody>
              {data?.items.map((c) => (
                <tr key={c.id} className="border-b border-urrea-border/60">
                  <td className="py-3 pr-4 font-mono text-xs">{c.numeroEmpleado}</td>
                  <td className="py-3 pr-4">{c.nombreCompleto}</td>
                  <td className="py-3">{c.puesto}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <p className="mt-4 text-xs text-urrea-text-muted">{data?.total ?? 0} colaboradores</p>
      </Card>
    </PageContainer>
  );
}
