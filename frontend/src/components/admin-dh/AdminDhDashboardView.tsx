"use client";

import Link from "next/link";
import { PageContainer, PageHeader } from "@/components/ui/page-header";

const LINKS = [
  { href: "/admin-dh/asistencia", title: "Monitor de asistencia", desc: "KPIs del día, registros e incidencias operativas." },
  { href: "/admin-dh/onboarding", title: "Monitor de onboarding", desc: "Planes activos, avance y tareas vencidas." },
  { href: "/admin-dh/asistencia/nomina", title: "Prenómina de asistencia", desc: "Generar corte y enviar incidencias a nómina." },
  { href: "/admin-dh/organizacion/jefes", title: "Asignación de jefes", desc: "Administra la relación jefe directo por colaborador." },
  { href: "/admin-dh/vacaciones/solicitudes", title: "Solicitudes de ausencia", desc: "Supervisión administrativa de vacaciones y permisos." },
  { href: "/admin-dh/vacaciones/saldos", title: "Saldos de vacaciones", desc: "Recalcular y ajustar saldos con motivo." },
  { href: "/admin-dh/vacaciones/politicas", title: "Políticas de vacaciones", desc: "Días anuales, antigüedad y acumulación." },
  { href: "/admin-dh/organizacion/areas", title: "Áreas", desc: "Catálogo de áreas organizacionales." },
  { href: "/admin-dh/organizacion/departamentos", title: "Departamentos", desc: "Departamentos por área y sede." },
  { href: "/admin-dh/organizacion/puestos", title: "Puestos", desc: "Puestos y niveles jerárquicos." },
  { href: "/admin-dh/organizacion/sedes", title: "Sedes", desc: "Ubicaciones físicas." },
  { href: "/admin-dh/organizacion/centros-costo", title: "Centros de costo", desc: "Centros de costo contables." },
];

export function AdminDhDashboardView() {
  return (
    <PageContainer>
      <PageHeader
        title="Centro de Administración DH"
        subtitle="Estructura organizacional, colaboradores y configuración funcional de Desarrollo Humano."
      />

      <div className="grid gap-4 sm:grid-cols-2">
        {LINKS.map((link) => (
          <Link
            key={link.href}
            href={link.href}
            className="rounded-xl border border-slate-200 bg-white p-4 transition hover:border-urrea-primary"
          >
            <h2 className="font-semibold">{link.title}</h2>
            <p className="mt-1 text-sm text-slate-500">{link.desc}</p>
          </Link>
        ))}
      </div>
    </PageContainer>
  );
}
