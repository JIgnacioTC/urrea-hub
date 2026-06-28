"use client";

import { useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, StatCard } from "@/components/ui/card";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { requisitionAdminService, recruitmentAdminService, type Oferta, type Postulacion, type Vacante } from "@/lib/services/recruitmentAdminService";
import type { RequisicionResumen } from "@/lib/services/requisitionService";

const ETAPAS = ["Recibida", "EnRevision", "Entrevista", "Oferta", "Contratado", "Rechazado"];

export function AdminRequisicionesView() {
  const [rows, setRows] = useState<RequisicionResumen[]>([]);
  const [msg, setMsg] = useState("");

  const load = useCallback(() => requisitionAdminService.listAll().then((r) => setRows(r as RequisicionResumen[])), []);

  useEffect(() => { load().catch(console.error); }, [load]);

  async function convertir(id: string) {
    await requisitionAdminService.convertToVacancy(id);
    setMsg("Vacante creada.");
    load();
  }

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Bandeja de requisiciones" subtitle="Valida presupuesto y convierte en vacante." />
      {msg && <Alert variant="success">{msg}</Alert>}
      {rows.length === 0 ? <EmptyState message="Sin requisiciones." /> : (
        <div className="overflow-x-auto rounded-xl border bg-white">
          <table className="min-w-full text-left text-sm">
            <thead><tr className="border-b bg-slate-50 text-slate-500">
              <th className="px-3 py-2">Folio</th><th className="px-3 py-2">Título</th><th className="px-3 py-2">Solicitante</th>
              <th className="px-3 py-2">Estado</th><th className="px-3 py-2" />
            </tr></thead>
            <tbody>
              {(rows as RequisicionResumen[]).map((r) => (
                <tr key={r.id} className="border-b">
                  <td className="px-3 py-2">{r.folio}</td>
                  <td className="px-3 py-2">{r.titulo}</td>
                  <td className="px-3 py-2">{r.solicitanteNombre}</td>
                  <td className="px-3 py-2"><Badge estado={r.estado} /></td>
                  <td className="px-3 py-2">
                    {r.estado === "Aprobada" && (
                      <button type="button" className="text-urrea-primary hover:underline" onClick={() => convertir(r.id)}>Convertir en vacante</button>
                    )}
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

export function AdminReclutamientoDashboardView() {
  const [dash, setDash] = useState<Awaited<ReturnType<typeof recruitmentAdminService.getDashboard>> | null>(null);

  useEffect(() => { recruitmentAdminService.getDashboard().then(setDash).catch(console.error); }, []);

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Atracción de talento" subtitle="Vacantes, pipeline y ofertas → onboarding." />
      {dash && (
        <>
          <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
            <StatCard label="Vacantes abiertas" value={String(dash.vacantesAbiertas)} accentClass="text-urrea-primary" />
            <StatCard label="Candidatos activos" value={String(dash.candidatosActivos)} accentClass="text-blue-700" />
            <StatCard label="Req. por convertir" value={String(dash.requisicionesPorConvertir)} accentClass="text-amber-700" />
            <StatCard label="Entrevistas" value={String(dash.entrevistasProgramadas)} accentClass="text-emerald-700" />
            <StatCard label="Ofertas enviadas" value={String(dash.ofertasEnviadas)} accentClass="text-purple-700" />
            <StatCard label="Ofertas aceptadas" value={String(dash.ofertasAceptadas)} accentClass="text-emerald-700" />
          </div>
          <Card title="Vacantes recientes">
            <ul className="space-y-2 text-sm">
              {dash.vacantesRecientes.map((v) => (
                <li key={v.id}>{v.codigo} · {v.titulo} · {v.candidatosCount} candidatos · {v.estado}</li>
              ))}
            </ul>
          </Card>
        </>
      )}
    </PageContainer>
  );
}

export function AdminPipelineView() {
  const [pipeline, setPipeline] = useState<Postulacion[]>([]);
  const [offers, setOffers] = useState<Oferta[]>([]);
  const [msg, setMsg] = useState("");

  const load = useCallback(async () => {
    setPipeline(await recruitmentAdminService.listPipeline());
    setOffers(await recruitmentAdminService.listOffers());
  }, []);

  useEffect(() => { load().catch(console.error); }, [load]);

  async function avanzar(id: string, estado: string) {
    await recruitmentAdminService.changeStage(id, estado);
    load();
  }

  async function ofertar(postulacionId: string) {
    await recruitmentAdminService.createOffer(postulacionId, 35000);
    setMsg("Oferta creada.");
    load();
  }

  async function onboarding(ofertaId: string) {
    const res = await recruitmentAdminService.acceptAndOnboard(ofertaId);
    setMsg(`Alta ${res.numeroEmpleado} y plan onboarding creado.`);
    load();
  }

  const byStage = ETAPAS.reduce<Record<string, Postulacion[]>>((acc, e) => {
    acc[e] = pipeline.filter((p) => p.estado === e);
    return acc;
  }, {});

  return (
    <PageContainer className="max-w-7xl">
      <PageHeader title="Pipeline Kanban" subtitle="Mueve candidatos por etapa hasta onboarding." />
      {msg && <Alert variant="success">{msg}</Alert>}
      <div className="flex gap-3 overflow-x-auto pb-2">
        {ETAPAS.map((etapa) => (
          <div key={etapa} className="min-w-[200px] flex-1 rounded-xl bg-urrea-bg-soft/80 p-3">
            <p className="mb-2 text-xs font-semibold uppercase text-urrea-text-muted">{etapa}</p>
            <div className="space-y-2">
              {(byStage[etapa] ?? []).map((p) => (
                <div key={p.id} className="rounded-lg border bg-white p-3 text-sm shadow-soft">
                  <p className="font-medium">{p.candidatoNombre}</p>
                  <p className="text-xs text-urrea-text-muted">{p.vacanteTitulo}</p>
                  {etapa === "Entrevista" && (
                    <button type="button" className="mt-2 text-xs text-urrea-primary hover:underline" onClick={() => avanzar(p.id, "Oferta")}>→ Oferta</button>
                  )}
                  {etapa === "Oferta" && (
                    <button type="button" className="mt-2 text-xs text-urrea-primary hover:underline" onClick={() => ofertar(p.id)}>Crear oferta</button>
                  )}
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
      <Card title="Ofertas" className="mt-6">
        {offers.length === 0 ? <p className="text-sm text-urrea-text-muted">Sin ofertas.</p> : (
          <ul className="space-y-2 text-sm">
            {offers.map((o) => (
              <li key={o.id} className="flex flex-wrap items-center justify-between gap-2">
                <span>{o.candidatoNombre} · {o.vacanteTitulo} · ${o.salarioOfrecido.toLocaleString()} {o.moneda}</span>
                {!o.aceptada && (
                  <Button type="button" variant="secondary" onClick={() => onboarding(o.id)}>Aceptar → Onboarding</Button>
                )}
                {o.aceptada && <Badge estado="Aceptada" />}
              </li>
            ))}
          </ul>
        )}
      </Card>
    </PageContainer>
  );
}

export function AdminVacantesView() {
  const [vacantes, setVacantes] = useState<Vacante[]>([]);
  useEffect(() => { recruitmentAdminService.listVacancies().then(setVacantes).catch(console.error); }, []);
  return (
    <PageContainer>
      <PageHeader title="Vacantes" subtitle="Vacantes ligadas a requisiciones aprobadas." />
      {vacantes.map((v) => (
        <Card key={v.id}><p className="font-semibold">{v.codigo} — {v.titulo}</p>
          <p className="text-sm text-urrea-text-muted">Req. {v.requisicionFolio ?? "—"} · {v.candidatosCount} candidatos · {v.estado}</p></Card>
      ))}
    </PageContainer>
  );
}
