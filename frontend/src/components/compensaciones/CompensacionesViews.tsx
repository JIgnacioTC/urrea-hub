"use client";

import { FormEvent, useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, StatCard } from "@/components/ui/card";
import { Input, Select, Textarea } from "@/components/ui/input";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { employeeService } from "@/lib/services/employeeService";
import {
  compensationAdminService,
  type CompensacionColaborador,
  type CompensacionDashboard,
  type SolicitudAjuste,
  type Tabulador,
} from "@/lib/services/compensationService";

export function AdminCompensacionesDashboardView() {
  const [dash, setDash] = useState<CompensacionDashboard | null>(null);
  const [tabuladores, setTabuladores] = useState<Tabulador[]>([]);
  const [colaboradores, setColaboradores] = useState<CompensacionColaborador[]>([]);

  useEffect(() => {
    compensationAdminService.getDashboard().then(setDash).catch(console.error);
    compensationAdminService.listTabuladores().then(setTabuladores).catch(console.error);
    compensationAdminService.listColaboradores().then(setColaboradores).catch(console.error);
  }, []);

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader
        title="Compensaciones"
        subtitle="Datos laborales referenciales, tabuladores y solicitudes de ajuste."
      />
      {dash && (
        <>
          <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
            <StatCard label="Colaboradores con datos" value={String(dash.colaboradoresConDatos)} accentClass="text-urrea-primary" />
            <StatCard label="Pendientes revisión" value={String(dash.solicitudesPendientes)} accentClass="text-amber-700" />
            <StatCard label="Aprobadas" value={String(dash.solicitudesAprobadas)} accentClass="text-emerald-700" />
            <StatCard label="Aplicadas (SAP)" value={String(dash.listasNomina)} accentClass="text-blue-700" />
          </div>
          <div className="grid gap-4 lg:grid-cols-2">
            <Card title="Solicitudes recientes">
              {dash.recientes.length === 0 ? (
                <EmptyState message="Sin solicitudes." />
              ) : (
                <ul className="space-y-2 text-sm">
                  {dash.recientes.map((s) => (
                    <li key={s.id}>
                      {s.colaboradorNombre} · {s.tipoAjuste} · <Badge estado={s.estado} />
                    </li>
                  ))}
                </ul>
              )}
            </Card>
            <Card title="Tabulador vigente">
              {tabuladores[0] ? (
                <div className="text-sm">
                  <p className="font-medium">{tabuladores[0].nombre}</p>
                  <ul className="mt-2 space-y-1 text-muted-foreground">
                    {tabuladores[0].bandas.map((b) => (
                      <li key={b.nivel}>
                        {b.nivel}: ${b.minimo.toLocaleString()} – ${b.maximo.toLocaleString()} {tabuladores[0].moneda}
                      </li>
                    ))}
                  </ul>
                </div>
              ) : (
                <EmptyState message="Sin tabulador configurado." />
              )}
            </Card>
          </div>
          <Card title="Colaboradores — datos laborales">
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b text-left text-muted-foreground">
                    <th className="pb-2 pr-4">Empleado</th>
                    <th className="pb-2 pr-4">Grupo</th>
                    <th className="pb-2 pr-4">Nivel</th>
                    <th className="pb-2 pr-4">Jornada</th>
                    <th className="pb-2">Visibilidad</th>
                  </tr>
                </thead>
                <tbody>
                  {colaboradores.map((c) => (
                    <tr key={c.colaboradorId} className="border-b border-border/50">
                      <td className="py-2 pr-4">{c.nombreCompleto} ({c.numeroEmpleado})</td>
                      <td className="py-2 pr-4">{c.grupoNomina ?? "—"}</td>
                      <td className="py-2 pr-4">{c.nivelSalarial ?? "—"}</td>
                      <td className="py-2 pr-4">{c.jornada ?? "—"}</td>
                      <td className="py-2">{c.nivelVisibilidad}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </Card>
        </>
      )}
    </PageContainer>
  );
}

const TIPOS_AJUSTE = [
  "CambioNivelSalarial",
  "CambioGrupoNomina",
  "CambioJornada",
  "CambioTurno",
  "Promocion",
  "AjusteTabulador",
  "AjusteExtraordinario",
];

export function AdminCompensacionesSolicitudesView() {
  const [rows, setRows] = useState<SolicitudAjuste[]>([]);
  const [colaboradores, setColaboradores] = useState<{ id: string; label: string }[]>([]);
  const [colaboradorId, setColaboradorId] = useState("");
  const [tipoAjuste, setTipoAjuste] = useState(TIPOS_AJUSTE[0]);
  const [valorNuevo, setValorNuevo] = useState("");
  const [motivo, setMotivo] = useState("");
  const [msg, setMsg] = useState("");
  const [error, setError] = useState("");

  const load = useCallback(() => compensationAdminService.listAdjustments().then(setRows), []);

  useEffect(() => {
    load().catch(console.error);
    employeeService.getEmployees({ pageSize: 100 }).then((r) =>
      setColaboradores(r.items.map((e) => ({ id: e.id, label: `${e.legalFullName} (${e.employeeNumber})` }))),
    ).catch(console.error);
  }, [load]);

  async function onCreate(e: FormEvent) {
    e.preventDefault();
    setError("");
    try {
      const created = await compensationAdminService.createAdjustment({
        colaboradorId,
        tipoAjuste,
        valorNuevo,
        motivo,
      });
      await compensationAdminService.submitAdjustment(created.id);
      setMsg("Solicitud creada y enviada.");
      setValorNuevo("");
      setMotivo("");
      await load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error");
    }
  }

  async function approve(id: string) {
    await compensationAdminService.approveAdjustment(id, "Aprobado desde admin DH");
    await load();
  }

  async function apply(id: string) {
    await compensationAdminService.applyAdjustment(id);
    await load();
  }

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Solicitudes de ajuste" subtitle="Flujo DH → Finanzas → aplicación en HCM." />
      {msg && <Alert variant="success">{msg}</Alert>}
      {error && <Alert variant="error">{error}</Alert>}

      <form onSubmit={onCreate}>
        <Card title="Nueva solicitud">
          <div className="grid gap-4 sm:grid-cols-2">
            <Select label="Colaborador" value={colaboradorId} onChange={(e) => setColaboradorId(e.target.value)} required>
              <option value="">Seleccionar…</option>
              {colaboradores.map((c) => (
                <option key={c.id} value={c.id}>{c.label}</option>
              ))}
            </Select>
            <Select label="Tipo de ajuste" value={tipoAjuste} onChange={(e) => setTipoAjuste(e.target.value)}>
              {TIPOS_AJUSTE.map((t) => (
                <option key={t} value={t}>{t}</option>
              ))}
            </Select>
            <Input label="Valor nuevo" value={valorNuevo} onChange={(e) => setValorNuevo(e.target.value)} required />
            <Textarea label="Motivo" value={motivo} onChange={(e) => setMotivo(e.target.value)} required />
          </div>
          <div className="mt-4">
            <Button type="submit">Crear y enviar</Button>
          </div>
        </Card>
      </form>

      <Card title="Bandeja">
        {rows.length === 0 ? (
          <EmptyState message="Sin solicitudes." />
        ) : (
          <ul className="space-y-3 text-sm">
            {rows.map((s) => (
              <li key={s.id} className="rounded-lg border p-3">
                <div className="flex flex-wrap items-center justify-between gap-2">
                  <span className="font-medium">{s.colaboradorNombre} · {s.tipoAjuste}</span>
                  <Badge estado={s.estado} />
                </div>
                <p className="mt-1 text-muted-foreground">{s.valorAnterior} → {s.valorNuevo} · {s.motivo}</p>
                <div className="mt-2 flex gap-2">
                  {(s.estado === "EnRevisionDh" || s.estado === "EnRevisionFinanzas") && (
                    <Button onClick={() => approve(s.id)}>Aprobar</Button>
                  )}
                  {s.estado === "Aprobado" && (
                    <Button variant="secondary" onClick={() => apply(s.id)}>Aplicar en HCM</Button>
                  )}
                </div>
              </li>
            ))}
          </ul>
        )}
      </Card>
    </PageContainer>
  );
}

export function AdminBeneficiosView() {
  const [beneficios, setBeneficios] = useState<{ id: string; codigo: string; nombre: string; descripcion?: string }[]>([]);
  const [requests, setRequests] = useState<{ id: string; colaboradorNombre: string; beneficioNombre: string; estado: string; fecha: string; monto?: number }[]>([]);

  const load = useCallback(async () => {
    const { benefitsAdminService } = await import("@/lib/services/compensationService");
    const [b, r] = await Promise.all([benefitsAdminService.listBenefits(), benefitsAdminService.listRequests()]);
    setBeneficios(b);
    setRequests(r);
  }, []);

  useEffect(() => {
    load().catch(console.error);
  }, [load]);

  async function approve(id: string) {
    const { benefitsAdminService } = await import("@/lib/services/compensationService");
    await benefitsAdminService.approve(id, "Aprobado");
    await load();
  }

  async function reject(id: string) {
    const { benefitsAdminService } = await import("@/lib/services/compensationService");
    await benefitsAdminService.reject(id, "Rechazado");
    await load();
  }

  return (
    <PageContainer className="max-w-6xl">
      <PageHeader title="Administración de beneficios" subtitle="Catálogo y solicitudes pendientes." />
      <div className="grid gap-4 lg:grid-cols-2">
        <Card title="Catálogo">
          <ul className="space-y-2 text-sm">
            {beneficios.map((b) => (
              <li key={b.id}>
                <span className="font-medium">{b.nombre}</span>
                <span className="text-muted-foreground"> ({b.codigo})</span>
                {b.descripcion && <p className="text-muted-foreground">{b.descripcion}</p>}
              </li>
            ))}
          </ul>
        </Card>
        <Card title="Solicitudes">
          {requests.length === 0 ? (
            <EmptyState message="Sin solicitudes." />
          ) : (
            <ul className="space-y-3 text-sm">
              {requests.map((r) => (
                <li key={r.id} className="rounded-lg border p-3">
                  <div className="flex justify-between">
                    <span>{r.colaboradorNombre} · {r.beneficioNombre}</span>
                    <Badge estado={r.estado} />
                  </div>
                  {r.estado === "Pendiente" && (
                    <div className="mt-2 flex gap-2">
                      <Button onClick={() => approve(r.id)}>Aprobar</Button>
                      <Button variant="secondary" onClick={() => reject(r.id)}>Rechazar</Button>
                    </div>
                  )}
                </li>
              ))}
            </ul>
          )}
        </Card>
      </div>
    </PageContainer>
  );
}

export function MiCompensacionView() {
  const [pkg, setPkg] = useState<Awaited<ReturnType<typeof import("@/lib/services/compensationService").compensationService.getMyPackage>> | null>(null);
  const [beneficioId, setBeneficioId] = useState("");
  const [justificacion, setJustificacion] = useState("");
  const [msg, setMsg] = useState("");

  const load = useCallback(async () => {
    const { compensationService } = await import("@/lib/services/compensationService");
    setPkg(await compensationService.getMyPackage());
  }, []);

  useEffect(() => {
    load().catch(console.error);
  }, [load]);

  async function onRequest(e: FormEvent) {
    e.preventDefault();
    const { compensationService } = await import("@/lib/services/compensationService");
    await compensationService.createBenefitRequest({ beneficioId, justificacion });
    setMsg("Solicitud enviada.");
    await load();
  }

  return (
    <PageContainer className="max-w-4xl">
      <PageHeader
        title="Mi compensación"
        subtitle="Beneficios activos y solicitudes. El detalle salarial depende de la política de visibilidad."
      />
      {msg && <Alert variant="success">{msg}</Alert>}
      {pkg && (
        <>
          {!pkg.muestraDetalleCompensacion && (
            <Alert variant="info">
              Tu nivel de visibilidad no incluye montos salariales. Consulta con RH para más detalle.
            </Alert>
          )}
          <div className="grid gap-4 lg:grid-cols-2">
            <Card title="Beneficios activos">
              {pkg.beneficiosActivos.length === 0 ? (
                <EmptyState message="Sin beneficios activos." />
              ) : (
                <ul className="space-y-2 text-sm">
                  {pkg.beneficiosActivos.map((b) => (
                    <li key={b.id}>{b.nombre}</li>
                  ))}
                </ul>
              )}
            </Card>
            <Card title="Mis solicitudes">
              <ul className="space-y-1 text-sm">
                {pkg.solicitudesBeneficio.map((s) => (
                  <li key={s.id}>{s.beneficioNombre} · <Badge estado={s.estado} /></li>
                ))}
                {pkg.solicitudesAjuste.map((s) => (
                  <li key={s.id}>Ajuste: {s.tipoAjuste} · <Badge estado={s.estado} /></li>
                ))}
              </ul>
            </Card>
          </div>
          <form onSubmit={onRequest}>
            <Card title="Solicitar beneficio">
              <Select label="Beneficio" value={beneficioId} onChange={(e) => setBeneficioId(e.target.value)} required>
                <option value="">Seleccionar…</option>
                {pkg.beneficiosDisponibles.map((b) => (
                  <option key={b.id} value={b.id}>{b.nombre}</option>
                ))}
              </Select>
              <Textarea label="Justificación" value={justificacion} onChange={(e) => setJustificacion(e.target.value)} />
              <div className="mt-4">
                <Button type="submit">Enviar solicitud</Button>
              </div>
            </Card>
          </form>
        </>
      )}
    </PageContainer>
  );
}
