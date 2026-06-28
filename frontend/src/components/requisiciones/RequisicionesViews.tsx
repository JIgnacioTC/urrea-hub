"use client";

import Link from "next/link";
import { FormEvent, useCallback, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card } from "@/components/ui/card";
import { Input, Textarea } from "@/components/ui/input";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { requisitionService, type RequisicionResumen } from "@/lib/services/requisitionService";

export function MisRequisicionesView() {
  const [rows, setRows] = useState<RequisicionResumen[]>([]);

  useEffect(() => {
    requisitionService.list().then(setRows).catch(console.error);
  }, []);

  return (
    <PageContainer>
      <PageHeader
        title="Mis requisiciones"
        subtitle="Solicitudes de personal con justificación y presupuesto."
        action={<Link href="/portal/requisiciones/nueva" className="text-sm text-urrea-primary hover:underline">Nueva requisición</Link>}
      />
      {rows.length === 0 ? (
        <EmptyState message="Sin requisiciones." />
      ) : (
        <div className="overflow-x-auto rounded-xl border bg-white">
          <table className="min-w-full text-left text-sm">
            <thead>
              <tr className="border-b bg-slate-50 text-slate-500">
                <th className="px-3 py-2">Folio</th><th className="px-3 py-2">Título</th><th className="px-3 py-2">Vacantes</th>
                <th className="px-3 py-2">Estado</th><th className="px-3 py-2">Aprobador</th><th className="px-3 py-2" />
              </tr>
            </thead>
            <tbody>
              {rows.map((r) => (
                <tr key={r.id} className="border-b">
                  <td className="px-3 py-2 font-medium">{r.folio}</td>
                  <td className="px-3 py-2">{r.titulo}</td>
                  <td className="px-3 py-2">{r.vacantesSolicitadas}</td>
                  <td className="px-3 py-2"><Badge estado={r.estado} /></td>
                  <td className="px-3 py-2">{r.aprobadorActual ?? "—"}</td>
                  <td className="px-3 py-2">
                    {r.estado === "Borrador" && (
                      <button type="button" className="text-urrea-primary hover:underline" onClick={() => requisitionService.submit(r.id).then(() => requisitionService.list().then(setRows))}>
                        Enviar
                      </button>
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

export function NuevaRequisicionView() {
  const router = useRouter();
  const [error, setError] = useState("");

  async function onSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    const fd = new FormData(e.currentTarget);
    setError("");
    try {
      const req = await requisitionService.create({
        titulo: fd.get("titulo"),
        vacantesSolicitadas: Number(fd.get("vacantes")),
        motivo: fd.get("motivo"),
        impactoNegocio: fd.get("impacto"),
        descripcionPuesto: fd.get("descripcion"),
        montoAutorizado: Number(fd.get("monto")),
        moneda: "MXN",
        centroCostoCodigo: fd.get("cc"),
      });
      await requisitionService.submit(req.id);
      router.push("/portal/requisiciones");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error");
    }
  }

  return (
    <PageContainer className="max-w-2xl">
      <PageHeader title="Nueva requisición" subtitle="Justifica la necesidad de contratar antes de abrir vacante." />
      {error && <Alert variant="error">{error}</Alert>}
      <form onSubmit={onSubmit}>
        <Card title="Datos de la requisición">
          <div className="grid gap-4">
            <Input label="Título" name="titulo" required />
            <Input label="Vacantes solicitadas" name="vacantes" type="number" defaultValue={1} required />
            <Textarea label="Motivo" name="motivo" rows={2} required />
            <Textarea label="Impacto de negocio" name="impacto" rows={2} />
            <Textarea label="Descripción de puesto" name="descripcion" rows={3} required />
            <Input label="Monto estimado (MXN)" name="monto" type="number" required />
            <Input label="Centro de costo" name="cc" defaultValue="CC001" />
          </div>
          <Button type="submit" className="mt-4">Crear y enviar</Button>
        </Card>
      </form>
    </PageContainer>
  );
}

export function RequisicionesPendientesView() {
  const [rows, setRows] = useState<RequisicionResumen[]>([]);
  const [error, setError] = useState("");

  const load = useCallback(() => requisitionService.pendingApprovals().then(setRows), []);

  useEffect(() => { load().catch(console.error); }, [load]);

  async function actuar(id: string, accion: "aprobar" | "rechazar") {
    setError("");
    try {
      if (accion === "aprobar") await requisitionService.approve(id);
      else await requisitionService.reject(id, "Rechazada por líder");
      load();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error");
    }
  }

  return (
    <PageContainer>
      <PageHeader title="Requisiciones pendientes" subtitle="Aprueba o rechaza solicitudes de personal." />
      {error && <Alert variant="error">{error}</Alert>}
      {rows.length === 0 ? (
        <Card><p className="text-sm text-urrea-text-muted">Sin pendientes.</p></Card>
      ) : (
        rows.map((r) => (
          <Card key={r.id}>
            <p className="font-semibold">{r.folio} — {r.titulo}</p>
            <p className="text-sm text-urrea-text-muted">{r.solicitanteNombre} · {r.vacantesSolicitadas} vacante(s)</p>
            <div className="mt-3 flex gap-2">
              <Button type="button" onClick={() => actuar(r.id, "aprobar")}>Aprobar</Button>
              <Button type="button" variant="danger" onClick={() => actuar(r.id, "rechazar")}>Rechazar</Button>
            </div>
          </Card>
        ))
      )}
    </PageContainer>
  );
}
