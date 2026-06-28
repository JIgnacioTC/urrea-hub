"use client";

import { useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, StatCard } from "@/components/ui/card";
import { Alert, PageContainer, PageHeader } from "@/components/ui/page-header";
import { onboardingService, type PlanOnboarding, type TareaOnboarding } from "@/lib/services/onboardingService";

function TaskRow({ tarea, onComplete }: { tarea: TareaOnboarding; onComplete: (id: string) => void }) {
  return (
    <li className="flex flex-wrap items-center justify-between gap-2 rounded-xl border border-urrea-border/60 p-3 text-sm">
      <div>
        <p className="font-medium text-urrea-text">{tarea.titulo}</p>
        <p className="text-xs text-urrea-text-muted">
          {tarea.responsables.map((r) => r.nombre).join(", ") || "Sin responsable"}
          {tarea.fechaCompromiso && ` · Límite ${new Date(tarea.fechaCompromiso).toLocaleDateString("es-MX")}`}
        </p>
      </div>
      <div className="flex items-center gap-2">
        {tarea.vencida && !tarea.completada && <span className="text-xs font-medium text-red-600">Vencida</span>}
        {tarea.completada ? (
          <span className="text-xs font-medium text-emerald-700">Completada</span>
        ) : (
          <Button type="button" variant="secondary" onClick={() => onComplete(tarea.id)}>Marcar completa</Button>
        )}
      </div>
    </li>
  );
}

export function MiOnboardingView() {
  const [plan, setPlan] = useState<PlanOnboarding | null>(null);
  const [error, setError] = useState("");

  const load = useCallback(() => {
    onboardingService.getMyPlan().then((p) => setPlan(p ?? null)).catch(console.error);
  }, []);

  useEffect(() => { load(); }, [load]);

  async function completar(id: string) {
    setError("");
    try {
      await onboardingService.completeTask(id);
      load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al completar tarea");
    }
  }

  return (
    <PageContainer>
      <PageHeader
        title="Mi onboarding"
        subtitle="Plan de incorporación, tareas y checklist de bienvenida."
      />
      {error && <Alert variant="error">{error}</Alert>}

      {!plan ? (
        <Card><p className="text-sm text-urrea-text-muted">No tienes un plan de onboarding activo.</p></Card>
      ) : (
        <>
          <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
            <StatCard label="Avance" value={`${plan.avancePorcentaje}%`} accentClass="text-urrea-primary" />
            <StatCard label="Pendientes" value={String(plan.tareasPendientes)} accentClass="text-amber-700" />
            <StatCard label="Vencidas" value={String(plan.tareasVencidas)} accentClass="text-red-600" />
            <StatCard label="Estado" value={plan.estado} accentClass="text-emerald-700" />
          </div>

          <Card title={plan.nombre}>
            <p className="mb-3 text-sm text-urrea-text-muted">
              Inicio {new Date(plan.fechaInicio).toLocaleDateString("es-MX")}
              {plan.puesto && ` · ${plan.puesto}`}
            </p>
            <div className="mb-4 h-3 overflow-hidden rounded-full bg-urrea-bg-soft">
              <div
                className="h-full rounded-full bg-gradient-to-r from-urrea-primary to-urrea-secondary"
                style={{ width: `${plan.avancePorcentaje}%` }}
              />
            </div>
            <ul className="space-y-2">
              {plan.tareas.map((t) => (
                <TaskRow key={t.id} tarea={t} onComplete={completar} />
              ))}
            </ul>
          </Card>

          {plan.checklist.length > 0 && (
            <Card title="Checklist">
              <ul className="space-y-1 text-sm">
                {plan.checklist.map((c) => (
                  <li key={c.id} className={c.completado ? "text-emerald-700" : "text-urrea-text-muted"}>
                    {c.completado ? "✓" : "○"} {c.item}
                  </li>
                ))}
              </ul>
            </Card>
          )}
        </>
      )}
    </PageContainer>
  );
}

export function TeamOnboardingView() {
  const [plans, setPlans] = useState<Awaited<ReturnType<typeof onboardingService.getTeamPlans>>>([]);
  const [tasks, setTasks] = useState<TareaOnboarding[]>([]);
  const [error, setError] = useState("");

  const load = useCallback(async () => {
    const [p, t] = await Promise.all([
      onboardingService.getTeamPlans(),
      onboardingService.getTeamPendingTasks(),
    ]);
    setPlans(p);
    setTasks(t);
  }, []);

  useEffect(() => { load().catch(console.error); }, [load]);

  async function completar(id: string) {
    setError("");
    try {
      await onboardingService.completeTask(id);
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error");
    }
  }

  return (
    <PageContainer>
      <PageHeader title="Onboarding de mi equipo" subtitle="Planes activos y tareas pendientes de validación." />
      {error && <Alert variant="error">{error}</Alert>}

      <Card title="Planes del equipo">
        {plans.length === 0 ? (
          <p className="text-sm text-urrea-text-muted">Sin planes activos.</p>
        ) : (
          <ul className="space-y-2 text-sm">
            {plans.map((p) => (
              <li key={p.id}>
                <span className="font-medium">{p.colaboradorNombre}</span> — {p.nombre} · {p.avancePorcentaje}%
                {p.tareasVencidas > 0 && <span className="ml-2 text-red-600">({p.tareasVencidas} vencidas)</span>}
              </li>
            ))}
          </ul>
        )}
      </Card>

      <Card title="Tareas pendientes">
        {tasks.length === 0 ? (
          <p className="text-sm text-urrea-text-muted">Sin tareas pendientes.</p>
        ) : (
          <ul className="space-y-2">
            {tasks.map((t) => (
              <TaskRow key={t.id} tarea={t} onComplete={completar} />
            ))}
          </ul>
        )}
      </Card>
    </PageContainer>
  );
}
