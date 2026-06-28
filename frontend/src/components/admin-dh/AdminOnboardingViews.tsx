"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, StatCard } from "@/components/ui/card";
import { Input, Select } from "@/components/ui/input";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { employeeService } from "@/lib/services/employeeService";
import {
  onboardingAdminService,
  type OnboardingDashboard,
  type PlanResumen,
  type PlantillaOnboarding,
} from "@/lib/services/onboardingAdminService";

export function AdminOnboardingDashboardView() {
  const [dash, setDash] = useState<OnboardingDashboard | null>(null);

  useEffect(() => {
    onboardingAdminService.getDashboard().then(setDash).catch(console.error);
  }, []);

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Monitor de onboarding" subtitle="Planes activos, avance y tareas vencidas." />
      {dash && (
        <>
          <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-5">
            <StatCard label="Planes activos" value={String(dash.planesActivos)} accentClass="text-urrea-primary" />
            <StatCard label="Completados" value={String(dash.planesCompletados)} accentClass="text-emerald-700" />
            <StatCard label="Tareas pendientes" value={String(dash.tareasPendientes)} accentClass="text-amber-700" />
            <StatCard label="Tareas vencidas" value={String(dash.tareasVencidas)} accentClass="text-red-600" />
            <StatCard label="Avance promedio" value={`${dash.promedioAvance}%`} accentClass="text-blue-700" />
          </div>
          <Card title="Planes recientes">
            {dash.planesRecientes.length === 0 ? (
              <EmptyState message="Sin planes." />
            ) : (
              <ul className="space-y-2 text-sm">
                {dash.planesRecientes.map((p) => (
                  <li key={p.id}>
                    {p.colaboradorNombre} · {p.nombre} · {p.avancePorcentaje}% · <Badge estado={p.estado} />
                  </li>
                ))}
              </ul>
            )}
          </Card>
        </>
      )}
    </PageContainer>
  );
}

export function AdminOnboardingPlanesView() {
  const [rows, setRows] = useState<PlanResumen[]>([]);
  const [templates, setTemplates] = useState<PlantillaOnboarding[]>([]);
  const [colaboradores, setColaboradores] = useState<{ id: string; label: string }[]>([]);
  const [colaboradorId, setColaboradorId] = useState("");
  const [plantilla, setPlantilla] = useState("ADMIN");
  const [fechaInicio, setFechaInicio] = useState(new Date().toISOString().slice(0, 10));
  const [msg, setMsg] = useState("");
  const [error, setError] = useState("");

  const load = useCallback(() => onboardingAdminService.listPlans().then(setRows), []);

  useEffect(() => {
    load().catch(console.error);
    onboardingAdminService.listTemplates().then(setTemplates).catch(console.error);
    employeeService.getEmployees({ pageSize: 100 }).then((r) =>
      setColaboradores(r.items.map((e) => ({ id: e.id, label: `${e.legalFullName} (${e.employeeNumber})` }))),
    ).catch(console.error);
  }, [load]);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setError("");
    try {
      await onboardingAdminService.createPlan({
        colaboradorId,
        plantillaCodigo: plantilla,
        fechaInicio,
      });
      setMsg("Plan creado.");
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error");
    }
  }

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Planes de onboarding" subtitle="Crear e iniciar planes desde plantillas." />
      {msg && <Alert variant="success">{msg}</Alert>}
      {error && <Alert variant="error">{error}</Alert>}

      <form onSubmit={onSubmit}>
        <Card title="Nuevo plan">
          <div className="grid gap-4 sm:grid-cols-3">
            <Select label="Colaborador" value={colaboradorId} onChange={(e) => setColaboradorId(e.target.value)} required>
              <option value="">Seleccionar…</option>
              {colaboradores.map((c) => (
                <option key={c.id} value={c.id}>{c.label}</option>
              ))}
            </Select>
            <Select label="Plantilla" value={plantilla} onChange={(e) => setPlantilla(e.target.value)}>
              {templates.map((t) => (
                <option key={t.codigo} value={t.codigo}>{t.nombre}</option>
              ))}
            </Select>
            <Input label="Fecha inicio" type="date" value={fechaInicio} onChange={(e) => setFechaInicio(e.target.value)} required />
          </div>
          <Button type="submit" className="mt-4">Crear plan</Button>
        </Card>
      </form>

      {rows.length === 0 ? (
        <EmptyState message="Sin planes registrados." />
      ) : (
        <div className="overflow-x-auto rounded-xl border bg-white">
          <table className="min-w-full text-left text-sm">
            <thead>
              <tr className="border-b bg-slate-50 text-slate-500">
                <th className="px-3 py-2">Colaborador</th>
                <th className="px-3 py-2">Plan</th>
                <th className="px-3 py-2">Inicio</th>
                <th className="px-3 py-2">Avance</th>
                <th className="px-3 py-2">Estado</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((r) => (
                <tr key={r.id} className="border-b">
                  <td className="px-3 py-2 font-medium">{r.colaboradorNombre}</td>
                  <td className="px-3 py-2">{r.nombre}</td>
                  <td className="px-3 py-2">{new Date(r.fechaInicio).toLocaleDateString("es-MX")}</td>
                  <td className="px-3 py-2">{r.avancePorcentaje}%</td>
                  <td className="px-3 py-2"><Badge estado={r.estado} /></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </PageContainer>
  );
}

export function AdminOnboardingPlantillasView() {
  const [templates, setTemplates] = useState<PlantillaOnboarding[]>([]);

  useEffect(() => {
    onboardingAdminService.listTemplates().then(setTemplates).catch(console.error);
  }, []);

  return (
    <PageContainer className="max-w-4xl">
      <PageHeader title="Plantillas de onboarding" subtitle="Checklists parametrizados por tipo de incorporación." />
      <div className="space-y-3">
        {templates.map((t) => (
          <Card key={t.codigo}>
            <p className="font-semibold">{t.nombre} <span className="text-sm font-normal text-urrea-text-muted">({t.codigo})</span></p>
            <p className="text-sm text-urrea-text-muted">{t.descripcion}</p>
            <p className="mt-2 text-sm">{t.tareasCount} tareas: {t.tareasPreview.join(" · ")}…</p>
          </Card>
        ))}
      </div>
    </PageContainer>
  );
}
