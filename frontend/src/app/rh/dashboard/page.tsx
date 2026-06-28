"use client";

import { useEffect, useState } from "react";
import { Button, ButtonLink } from "@/components/ui/button";
import { StatCard } from "@/components/ui/card";
import { PageContainer, PageHeader } from "@/components/ui/page-header";
import { rhService } from "@/lib/services/teamService";
import type { RhDashboard } from "@/lib/types";

export default function RhDashboardPage() {
  const [dashboard, setDashboard] = useState<RhDashboard | null>(null);
  const [syncMsg, setSyncMsg] = useState("");

  useEffect(() => {
    rhService.getDashboard().then(setDashboard).catch(console.error);
  }, []);

  async function syncNomina() {
    try {
      const res = await rhService.syncPayroll();
      setSyncMsg(`Sync completado: ${res.sincronizados} registros`);
    } catch {
      setSyncMsg("Error en sync");
    }
  }

  return (
    <PageContainer>
      <PageHeader
        title="Dashboard RH"
        action={
          <Button type="button" variant="secondary" onClick={syncNomina} className="w-full sm:w-auto">
            Sync nómina
          </Button>
        }
      />
      {syncMsg && <p className="text-sm text-urrea-text-muted">{syncMsg}</p>}

      <div className="grid grid-cols-2 gap-3 sm:gap-4 lg:grid-cols-4">
        <StatCard label="Pendientes" value={dashboard?.solicitudesPendientes} accentClass="text-urrea-primary" />
        <StatCard label="Aprobadas (mes)" value={dashboard?.aprobadasMes} accentClass="text-emerald-700" />
        <StatCard label="Activos" value={dashboard?.colaboradoresActivos} accentClass="text-urrea-secondary" />
        <StatCard label="Rechazadas (mes)" value={dashboard?.solicitudesRechazadasMes} accentClass="text-red-600" />
      </div>

      <div className="mt-6">
        <ButtonLink href="/admin-dh">Centro Admin DH</ButtonLink>
      </div>
    </PageContainer>
  );
}
