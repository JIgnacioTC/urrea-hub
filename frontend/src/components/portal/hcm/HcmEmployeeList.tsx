"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { DhEnterpriseHeader } from "@/components/dh/shared/page-frame";
import { DhBadge, DhDataTable, DhSearchInput, DhSelect, estatusTone } from "@/components/dh/shared/ui";
import { HcmAvatar, formatNum } from "@/components/portal/hcm/shared";
import { employeeService, type EmployeeListParams } from "@/lib/services/employeeService";
import type { EmployeeListItem, HcmCatalogs } from "@/lib/types/hcm";
import type { PagedResult } from "@/lib/types";
import { cn } from "@/lib/utils";

export function HcmEmployeeListView() {
  const [search, setSearch] = useState("");
  const [departmentId, setDepartmentId] = useState("all");
  const [locationId, setLocationId] = useState("all");
  const [positionId, setPositionId] = useState("all");
  const [costCenterId, setCostCenterId] = useState("all");
  const [contractTypeId, setContractTypeId] = useState("all");
  const [status, setStatus] = useState("all");
  const [syncStatus, setSyncStatus] = useState("all");
  const [externalSource, setExternalSource] = useState("all");
  const [page, setPage] = useState(1);
  const [data, setData] = useState<PagedResult<EmployeeListItem> | null>(null);
  const [catalogs, setCatalogs] = useState<HcmCatalogs | null>(null);

  useEffect(() => {
    employeeService.getCatalogs().then(setCatalogs).catch(console.error);
  }, []);

  useEffect(() => {
    const params: EmployeeListParams = { page, pageSize: 50, search: search || undefined };
    if (departmentId !== "all") params.departmentId = departmentId;
    if (locationId !== "all") params.locationId = locationId;
    if (positionId !== "all") params.positionId = positionId;
    if (costCenterId !== "all") params.costCenterId = costCenterId;
    if (contractTypeId !== "all") params.contractTypeId = contractTypeId;
    if (status !== "all") params.status = status;
    if (syncStatus !== "all") params.syncStatus = syncStatus;
    if (externalSource !== "all") params.externalSource = externalSource;
    employeeService.getEmployees(params).then(setData).catch(console.error);
  }, [search, departmentId, locationId, positionId, costCenterId, contractTypeId, status, syncStatus, externalSource, page]);

  const totalPages = data ? Math.max(1, Math.ceil(data.total / data.pageSize)) : 1;

  return (
    <div className="animate-fade-up">
      <DhEnterpriseHeader
        icon="users"
        title="Gestión de personas (HCM)"
        subtitle="Directorio maestro corporativo. Fuente única de colaborador con trazabilidad SAP/manual."
        breadcrumbs={[
          { label: "Portal", href: "/portal" },
          { label: "Núcleo HCM", href: "/portal/hcm" },
          { label: "Personas" },
        ]}
        action={
          <Link href="/portal/hcm/calidad-datos">
            <Button type="button" variant="secondary" className="text-xs">Calidad de datos</Button>
          </Link>
        }
      />

      <div className="mb-4 space-y-3 border border-urrea-border/80 bg-white px-4 py-3">
        <div className="min-w-[200px] max-w-md">
          <DhSearchInput value={search} onChange={(v) => { setSearch(v); setPage(1); }} placeholder="Nombre, número, RFC, puesto…" />
        </div>
        <div className="flex flex-wrap gap-2">
          <DhSelect value={departmentId} onChange={(v) => { setDepartmentId(v); setPage(1); }} options={[{ value: "all", label: "Departamento" }, ...(catalogs?.departments.map((d) => ({ value: d.id, label: d.name })) ?? [])]} />
          <DhSelect value={positionId} onChange={(v) => { setPositionId(v); setPage(1); }} options={[{ value: "all", label: "Puesto" }, ...(catalogs?.positions.map((p) => ({ value: p.id, label: p.name })) ?? [])]} />
          <DhSelect value={costCenterId} onChange={(v) => { setCostCenterId(v); setPage(1); }} options={[{ value: "all", label: "C.C." }, ...(catalogs?.costCenters.map((c) => ({ value: c.id, label: c.name })) ?? [])]} />
          <DhSelect value={locationId} onChange={(v) => { setLocationId(v); setPage(1); }} options={[{ value: "all", label: "Ubicación" }, ...(catalogs?.locations.map((s) => ({ value: s.id, label: s.name })) ?? [])]} />
          <DhSelect value={contractTypeId} onChange={(v) => { setContractTypeId(v); setPage(1); }} options={[{ value: "all", label: "Contrato" }, ...(catalogs?.contractTypes.map((c) => ({ value: c.id, label: c.name })) ?? [])]} />
          <DhSelect value={status} onChange={(v) => { setStatus(v); setPage(1); }} options={[{ value: "all", label: "Activos" }, { value: "activo", label: "Activo" }, { value: "baja", label: "Baja" }]} />
          <DhSelect value={syncStatus} onChange={(v) => { setSyncStatus(v); setPage(1); }} options={[{ value: "all", label: "Sync" }, { value: "Synced", label: "Synced" }, { value: "Pending", label: "Pending" }, { value: "ManualOverride", label: "Manual" }]} />
          <DhSelect value={externalSource} onChange={(v) => { setExternalSource(v); setPage(1); }} options={[{ value: "all", label: "Origen" }, { value: "SAP/CDM", label: "SAP/CDM" }, { value: "Manual", label: "Manual" }, { value: "Nomina", label: "Nómina" }]} />
        </div>
        <p className="text-xs text-urrea-text-muted">{data ? `${formatNum(data.total)} colaboradores` : "Cargando…"}</p>
      </div>

      <DhDataTable footer={`Página ${page} de ${totalPages} · Orden: apellido, nombre`}>
        <table className="w-full min-w-[1280px] text-left text-sm">
          <thead className="bg-slate-50 text-[11px] uppercase tracking-wide text-urrea-text-muted">
            <tr className="border-b border-urrea-border">
              <th className="px-3 py-3 font-semibold">Colaborador</th>
              <th className="px-3 py-3 font-semibold">No. empleado</th>
              <th className="px-3 py-3 font-semibold">Puesto</th>
              <th className="px-3 py-3 font-semibold">Área</th>
              <th className="px-3 py-3 font-semibold">C.C.</th>
              <th className="px-3 py-3 font-semibold">Jefe</th>
              <th className="px-3 py-3 font-semibold">Ubicación</th>
              <th className="px-3 py-3 font-semibold">Contrato</th>
              <th className="px-3 py-3 font-semibold">Estatus</th>
              <th className="px-3 py-3 font-semibold">Origen</th>
              <th className="px-3 py-3 font-semibold">Sync</th>
              <th className="px-3 py-3 font-semibold">Antig.</th>
            </tr>
          </thead>
          <tbody>
            {data?.items.map((c, i) => (
              <tr key={c.id} className={cn("border-b border-urrea-border/40 hover:bg-slate-50/80", i % 2 === 1 && "bg-slate-50/30")}>
                <td className="px-3 py-3">
                  <Link href={`/portal/hcm/personas/${c.id}`} className="flex items-center gap-2 font-medium text-urrea-primary hover:underline">
                    <HcmAvatar name={c.legalFullName} />
                    <span>{c.preferredName ?? c.legalFullName}</span>
                  </Link>
                </td>
                <td className="px-3 py-3 font-mono text-xs">{c.employeeNumber}</td>
                <td className="px-3 py-3">{c.position}</td>
                <td className="px-3 py-3 text-urrea-text-muted">{c.area ?? c.department}</td>
                <td className="px-3 py-3 font-mono text-xs">{c.costCenter ?? "—"}</td>
                <td className="px-3 py-3 text-xs">{c.managerName ?? "—"}</td>
                <td className="px-3 py-3 text-xs">{c.location ?? "—"}</td>
                <td className="px-3 py-3 text-xs">{c.contractType}</td>
                <td className="px-3 py-3"><DhBadge tone={estatusTone(c.status)}>{c.status}</DhBadge></td>
                <td className="px-3 py-3 text-xs">{c.externalSource ?? "Manual"}</td>
                <td className="px-3 py-3"><DhBadge tone={c.syncStatus === "Synced" ? "success" : "warning"}>{c.syncStatus}</DhBadge></td>
                <td className="px-3 py-3 tabular-nums text-xs">{c.tenureYears}a</td>
              </tr>
            ))}
          </tbody>
        </table>
      </DhDataTable>

      {totalPages > 1 && (
        <div className="mt-4 flex justify-end gap-2">
          <Button type="button" variant="secondary" className="text-xs" disabled={page <= 1} onClick={() => setPage((p) => p - 1)}>Anterior</Button>
          <Button type="button" variant="secondary" className="text-xs" disabled={page >= totalPages} onClick={() => setPage((p) => p + 1)}>Siguiente</Button>
        </div>
      )}
    </div>
  );
}
