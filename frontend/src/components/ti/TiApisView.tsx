"use client";

import { useEffect, useMemo, useState } from "react";
import { PageContainer, PageHeader } from "@/components/ui/page-header";
import { TiExportButtons } from "@/components/ti/TiExportButtons";
import { tiService, type ApiEndpoint } from "@/lib/services/tiService";

const METHOD_COLORS: Record<string, string> = {
  GET: "bg-emerald-100 text-emerald-800",
  POST: "bg-blue-100 text-blue-800",
  PUT: "bg-amber-100 text-amber-800",
  DELETE: "bg-red-100 text-red-800",
  PATCH: "bg-purple-100 text-purple-800",
};

export function TiApisView() {
  const [endpoints, setEndpoints] = useState<ApiEndpoint[]>([]);
  const [search, setSearch] = useState("");

  useEffect(() => {
    tiService.getApis().then((a) => setEndpoints(a.endpoints)).catch(console.error);
  }, []);

  const filtered = useMemo(() => {
    const q = search.trim().toLowerCase();
    if (!q) return endpoints;
    return endpoints.filter(
      (e) =>
        e.route.toLowerCase().includes(q) ||
        e.controller.toLowerCase().includes(q) ||
        e.method.toLowerCase().includes(q),
    );
  }, [endpoints, search]);

  return (
    <PageContainer>
      <PageHeader title="Catálogo de APIs" subtitle="Endpoints descubiertos en la API URREA Hub." />

      <div className="mb-4">
        <TiExportButtons kinds={["apis"]} variant="secondary" />
      </div>

      <input
        className="mb-4 w-full max-w-md rounded-lg border border-slate-200 px-3 py-2 text-sm"
        placeholder="Buscar ruta, controlador o método…"
        value={search}
        onChange={(e) => setSearch(e.target.value)}
      />

      <div className="overflow-hidden rounded-xl border border-slate-200 bg-white">
        <table className="min-w-full text-left text-sm">
          <thead className="bg-slate-50 text-slate-500">
            <tr>
              <th className="px-4 py-3">Método</th>
              <th className="px-4 py-3">Ruta</th>
              <th className="px-4 py-3">Controlador</th>
              <th className="px-4 py-3">Auth</th>
            </tr>
          </thead>
          <tbody>
            {filtered.map((e) => (
              <tr key={`${e.method}-${e.route}-${e.action}`} className="border-t border-slate-100">
                <td className="px-4 py-2">
                  <span className={`rounded px-2 py-0.5 text-xs font-semibold ${METHOD_COLORS[e.method] ?? "bg-slate-100"}`}>
                    {e.method}
                  </span>
                </td>
                <td className="px-4 py-2 font-mono text-xs">{e.route}</td>
                <td className="px-4 py-2 text-slate-600">{e.controller}.{e.action}</td>
                <td className="px-4 py-2 text-xs text-slate-500">{e.authPolicies.join(", ") || "Authenticated"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </PageContainer>
  );
}
