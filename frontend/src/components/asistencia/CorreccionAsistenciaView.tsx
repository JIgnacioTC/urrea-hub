"use client";

import { FormEvent, useState } from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Input, Select, Textarea } from "@/components/ui/input";
import { Alert, PageContainer, PageHeader } from "@/components/ui/page-header";
import { attendanceService } from "@/lib/services/attendanceService";

const TIPOS = [
  { value: "EntradaOmitida", label: "Olvidé registrar entrada" },
  { value: "SalidaOmitida", label: "Olvidé registrar salida" },
  { value: "ErrorBiometrico", label: "El biométrico falló" },
  { value: "VisitaCliente", label: "Estuve en visita con cliente" },
  { value: "TrabajoCampo", label: "Trabajo en campo" },
  { value: "ErrorGeolocalizacion", label: "Error de geolocalización" },
  { value: "CambioHorario", label: "Cambio de horario autorizado" },
  { value: "Otro", label: "Otro" },
];

export function CorreccionAsistenciaView() {
  const router = useRouter();
  const [fecha, setFecha] = useState(new Date().toISOString().slice(0, 10));
  const [tipo, setTipo] = useState("EntradaOmitida");
  const [horaEntrada, setHoraEntrada] = useState("");
  const [horaSalida, setHoraSalida] = useState("");
  const [motivo, setMotivo] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function onSubmit(e: FormEvent) {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      const buildDateTime = (time: string) => {
        if (!time) return undefined;
        return new Date(`${fecha}T${time}:00`).toISOString();
      };
      await attendanceService.createCorrection({
        fecha,
        tipoCorreccion: tipo,
        horaEntradaSolicitada: buildDateTime(horaEntrada),
        horaSalidaSolicitada: buildDateTime(horaSalida),
        motivo,
      });
      router.push("/portal/asistencia");
    } catch (err) {
      setError(err instanceof Error ? err.message : "No se pudo enviar la corrección");
    } finally {
      setLoading(false);
    }
  }

  return (
    <PageContainer>
      <PageHeader title="Solicitar corrección" subtitle="La corrección no modifica el registro original hasta que el líder la apruebe." />
      <form onSubmit={onSubmit}>
        <Card title="Datos de la corrección">
          <div className="grid gap-4 sm:grid-cols-2">
            <Input label="Fecha" type="date" value={fecha} onChange={(e) => setFecha(e.target.value)} required />
            <Select label="Tipo" value={tipo} onChange={(e) => setTipo(e.target.value)}>
              {TIPOS.map((t) => (
                <option key={t.value} value={t.value}>{t.label}</option>
              ))}
            </Select>
            <Input label="Hora entrada sugerida" type="time" value={horaEntrada} onChange={(e) => setHoraEntrada(e.target.value)} />
            <Input label="Hora salida sugerida" type="time" value={horaSalida} onChange={(e) => setHoraSalida(e.target.value)} />
            <Textarea label="Motivo" value={motivo} onChange={(e) => setMotivo(e.target.value)} rows={3} className="sm:col-span-2" required />
          </div>
          {error && <Alert variant="error" className="mt-3">{error}</Alert>}
          <Button type="submit" className="mt-4" disabled={loading}>Enviar corrección</Button>
        </Card>
      </form>
    </PageContainer>
  );
}
