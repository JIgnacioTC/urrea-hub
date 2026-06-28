"use client";

import { useRouter } from "next/navigation";
import { FormEvent, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Input, Select, Textarea } from "@/components/ui/input";
import { Alert, PageContainer, PageHeader } from "@/components/ui/page-header";
import { absenceService } from "@/lib/services/absenceService";
import type { CalculateDaysResult, SaldoVacaciones, TipoAusencia } from "@/lib/types";

export function NuevaSolicitudView() {
  const router = useRouter();
  const [tipos, setTipos] = useState<TipoAusencia[]>([]);
  const [saldo, setSaldo] = useState<SaldoVacaciones | null>(null);
  const [tipoId, setTipoId] = useState("");
  const [fechaInicio, setFechaInicio] = useState("");
  const [fechaFin, setFechaFin] = useState("");
  const [horaInicio, setHoraInicio] = useState("");
  const [horaFin, setHoraFin] = useState("");
  const [comentario, setComentario] = useState("");
  const [preview, setPreview] = useState<CalculateDaysResult | null>(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const tipo = tipos.find((t) => t.id === tipoId);

  useEffect(() => {
    absenceService.getRequestTypes().then(setTipos).catch(console.error);
    absenceService.getMyBalance().then(setSaldo).catch(console.error);
  }, []);

  useEffect(() => {
    if (!fechaInicio || !fechaFin) { setPreview(null); return; }
    absenceService.calculateDays(fechaInicio, fechaFin, tipoId || undefined)
      .then(setPreview)
      .catch(() => setPreview(null));
  }, [fechaInicio, fechaFin, tipoId]);

  async function submit(enviar: boolean) {
    if (!tipoId) { setError("Selecciona un tipo de ausencia."); return; }
    setError("");
    setLoading(true);
    try {
      await absenceService.createRequest({
        tipoAusenciaId: tipoId,
        fechaInicio,
        fechaFin,
        comentario: comentario || null,
        enviar,
        esDiaCompleto: !tipo?.esParcial,
        horaInicio: horaInicio || null,
        horaFin: horaFin || null,
      });
      router.push("/portal/vacaciones");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al crear solicitud");
    } finally {
      setLoading(false);
    }
  }

  function onSubmit(e: FormEvent) {
    e.preventDefault();
    submit(true);
  }

  return (
    <PageContainer>
      <PageHeader title="Nueva solicitud" subtitle={`Saldo disponible: ${saldo?.diasDisponibles ?? saldo?.diasPendientes ?? "—"} días`} />

      <form onSubmit={onSubmit}>
        <Card title="Datos de la solicitud">
          <div className="grid gap-4 sm:grid-cols-2">
            <Select label="Tipo de ausencia" value={tipoId} onChange={(e) => setTipoId(e.target.value)} className="sm:col-span-2">
              <option value="">Seleccionar…</option>
              {tipos.map((t) => (
                <option key={t.id} value={t.id}>{t.icono ? `${t.icono} ` : ""}{t.nombre}</option>
              ))}
            </Select>
            <Input label="Fecha inicio" type="date" value={fechaInicio} onChange={(e) => setFechaInicio(e.target.value)} required />
            <Input label="Fecha fin" type="date" value={fechaFin} onChange={(e) => setFechaFin(e.target.value)} required />
            {tipo?.esParcial && (
              <>
                <Input label="Hora inicio" type="time" value={horaInicio} onChange={(e) => setHoraInicio(e.target.value)} />
                <Input label="Hora fin" type="time" value={horaFin} onChange={(e) => setHoraFin(e.target.value)} />
              </>
            )}
            <Textarea label="Comentario" value={comentario} onChange={(e) => setComentario(e.target.value)} rows={2} className="sm:col-span-2" />
          </div>

          {tipo && (
            <div className="mt-4 rounded-lg bg-slate-50 p-3 text-xs text-slate-600">
              {tipo.descuentaSaldo && <p>Descuenta saldo de vacaciones</p>}
              {tipo.requiereComprobante && <p>Requiere comprobante o motivo detallado</p>}
              {tipo.requiereAprobacion && <p>Requiere aprobación del jefe</p>}
              {tipo.esParcial && <p>Permiso parcial (medio día)</p>}
            </div>
          )}

          {preview && (
            <div className="mt-4 grid gap-2 sm:grid-cols-3">
              <p className="text-sm"><span className="font-medium">Días calculados:</span> {preview.diasHabiles}</p>
              {preview.saldoPosterior != null && (
                <p className="text-sm"><span className="font-medium">Saldo posterior:</span> {preview.saldoPosterior}</p>
              )}
              {preview.excedeSaldo && <Alert variant="error">Excede el saldo disponible</Alert>}
              {preview.tieneTraslape && <Alert variant="info">Hay traslape con otra solicitud activa</Alert>}
            </div>
          )}

          {error && <Alert variant="error" className="mt-3">{error}</Alert>}

          <div className="mt-4 flex flex-wrap gap-2">
            <Button type="submit" disabled={loading || !tipoId || preview?.excedeSaldo}>Enviar solicitud</Button>
            <Button type="button" variant="secondary" disabled={loading} onClick={() => submit(false)}>Guardar borrador</Button>
          </div>
        </Card>
      </form>
    </PageContainer>
  );
}
