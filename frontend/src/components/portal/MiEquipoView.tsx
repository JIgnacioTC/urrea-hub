"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { AdnObjetivosTab } from "@/components/portal/equipo/AdnObjetivosTab";
import { CapacitacionesTab } from "@/components/portal/equipo/CapacitacionesTab";
import { ColaboradoresTab } from "@/components/portal/equipo/ColaboradoresTab";
import { FeedbackTab } from "@/components/portal/equipo/FeedbackTab";
import { PlanesAccionTab } from "@/components/portal/equipo/PlanesAccionTab";
import { Card } from "@/components/ui/card";
import { PageContainer, PageHeader } from "@/components/ui/page-header";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { teamService } from "@/lib/services/teamService";
import { getSession, isJefe } from "@/lib/auth";
import type { CapacitacionEquipo, EquipoMiembro, FeedbackEquipo, PlanAccion } from "@/lib/types";

export function MiEquipoView() {
  const router = useRouter();
  const [miembros, setMiembros] = useState<EquipoMiembro[]>([]);
  const [planes, setPlanes] = useState<PlanAccion[]>([]);
  const [feedbacks, setFeedbacks] = useState<FeedbackEquipo[]>([]);
  const [capacitaciones, setCapacitaciones] = useState<CapacitacionEquipo[]>([]);
  const [loading, setLoading] = useState(true);

  function loadData() {
    Promise.all([
      teamService.getMembers(),
      teamService.getActionPlans(),
      teamService.getFeedback(),
      teamService.getTraining(),
    ])
      .then(([m, p, f, c]) => {
        setMiembros(m);
        setPlanes(p);
        setFeedbacks(f);
        setCapacitaciones(c);
      })
      .catch(console.error)
      .finally(() => setLoading(false));
  }

  useEffect(() => {
    const session = getSession();
    if (!session || !isJefe(session)) {
      router.replace("/portal");
      return;
    }
    loadData();
  }, [router]);

  if (loading) {
    return (
      <PageContainer className="animate-pulse space-y-4">
        <div className="h-8 w-56 rounded-lg bg-urrea-chrome/40" />
        <div className="h-12 rounded-2xl bg-urrea-chrome/30" />
        <div className="h-64 rounded-2xl bg-urrea-chrome/20" />
      </PageContainer>
    );
  }

  return (
    <PageContainer className="animate-fade-up">
      <PageHeader
        title="Mi equipo"
        subtitle={`Gestión integral · ${miembros.length} colaborador(es)`}
      />

      <div className="mb-4 flex flex-wrap gap-3">
        <Link
          href="/portal/aprobaciones"
          className="rounded-xl border border-urrea-border/70 bg-urrea-bg px-4 py-2 text-sm font-medium text-urrea-secondary transition hover:border-urrea-secondary/40"
        >
          Aprobaciones pendientes →
        </Link>
      </div>

      <Tabs defaultValue="colaboradores">
        <TabsList className="mb-4">
          <TabsTrigger value="colaboradores">Colaboradores</TabsTrigger>
          <TabsTrigger value="planes">Planes de Acción</TabsTrigger>
          <TabsTrigger value="feedback">Feedback</TabsTrigger>
          <TabsTrigger value="adn">ADN URREA Objetivos</TabsTrigger>
          <TabsTrigger value="capacitaciones">Capacitaciones</TabsTrigger>
        </TabsList>

        <Card>
          <TabsContent value="colaboradores" className="p-1">
            <ColaboradoresTab miembros={miembros} />
          </TabsContent>
          <TabsContent value="planes" className="p-1">
            <PlanesAccionTab miembros={miembros} planes={planes} onRefresh={loadData} />
          </TabsContent>
          <TabsContent value="feedback" className="p-1">
            <FeedbackTab miembros={miembros} feedbacks={feedbacks} onRefresh={loadData} />
          </TabsContent>
          <TabsContent value="adn" className="p-1">
            <AdnObjetivosTab />
          </TabsContent>
          <TabsContent value="capacitaciones" className="p-1">
            <CapacitacionesTab capacitaciones={capacitaciones} />
          </TabsContent>
        </Card>
      </Tabs>
    </PageContainer>
  );
}
