"use client";

import { useEffect, useMemo, useState } from "react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Input, Textarea } from "@/components/ui/input";
import { Alert, EmptyState, PageContainer, PageHeader } from "@/components/ui/page-header";
import { absenceService } from "@/lib/services/absenceService";
import type { ResumenTipoPermiso, SolicitudAusencia, TipoAusencia } from "@/lib/types";
import { cn } from "@/lib/utils";

function formatPeriodo(s: SolicitudAusencia) {
  const fecha = new Date(s.fechaInicio).toLocaleDateString("es-MX");
  if (s.esDiaCompleto === false || s.horaInicio || s.horaFin) {
    const hora = s.tipoAusenciaCodigo === "ENTRADA_TARDE" ? s.horaInicio : s.horaFin;
    return `${fecha}${hora ? ` · ${hora}` : ""}`;
  }
  if (s.fechaInicio === s.fechaFin) return fecha;
  return `${fecha} – ${new Date(s.fechaFin).toLocaleDateString("es-MX")}`;
}

function TipoCard({
  tipo,
  resumen,
  selected,
  onSelect,
}: {
  tipo: TipoAusencia;
  resumen?: ResumenTipoPermiso;
  selected: boolean;
  onSelect: () => void;
}) {
  return (
    <button
      type="button"
      onClick={onSelect}
      className={cn(
        "group flex flex-col rounded-2xl border p-4 text-left transition-all duration-200",
        selected
          ? "border-urrea-secondary bg-urrea-primary/5 shadow-soft ring-2 ring-urrea-secondary/30"
          : "border-urrea-border/80 bg-urrea-bg hover:border-urrea-secondary/40 hover:shadow-soft",
      )}
    >
      <div className="flex items-start justify-between gap-2">
        <span className="text-2xl" aria-hidden>{tipo.icono ?? "📄"}</span>
        {tipo.remunerado === false && (
          <span className="rounded-full bg-urrea-accent-sand/30 px-2 py-0.5 text-[10px] font-semibold text-urrea-primary">
            Sin goce
          </span>
        )}
      </div>
      <p className="mt-3 font-semibold text-urrea-text group-hover:text-urrea-primary">{tipo.nombre}</p>
      <p className="mt-1 line-clamp-2 text-xs text-urrea-text-muted">{tipo.descripcion}</p>
      <p className="mt-2 text-[10px] font-medium text-urrea-secondary">{tipo.baseLegalLft}</p>
      {resumen?.diasDisponibles != null && (
        <p className="mt-2 text-xs font-semibold text-urrea-primary">
          Disponible: {resumen.diasDisponibles} día(s)
        </p>
      )}
      {tipo.esParcial && (
        <p className="mt-1 text-[10px] uppercase tracking-wide text-urrea-text-muted">Permiso parcial · 0.5 día</p>
      )}
    </button>
  );
}

export function PermisosView() {
  const [tipos, setTipos] = useState<TipoAusencia[]>([]);
  const [resumen, setResumen] = useState<ResumenTipoPermiso[]>([]);
  const [solicitudes, setSolicitudes] = useState<SolicitudAusencia[]>([]);
  const [selectedCodigo, setSelectedCodigo] = useState<string | null>(null);
  const [fecha, setFecha] = useState("");
  const [fechaFin, setFechaFin] = useState("");
  const [horaInicio, setHoraInicio] = useState("");
  const [horaFin, setHoraFin] = useState("");
  const [comentario, setComentario] = useState("");
  const [diasPreview, setDiasPreview] = useState<number | null>(null);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);

  const tipoSeleccionado = useMemo(
    () => tipos.find((t) => t.codigo === selectedCodigo) ?? null,
    [tipos, selectedCodigo],
  );

  const historial = useMemo(
    () => solicitudes.filter((s) => s.tipoAusenciaCodigo !== "VAC"),
    [solicitudes],
  );

  function loadData() {
    absenceService.getPermissionTypes().then(setTipos).catch(console.error);
    absenceService.getPermissionSummary().then(setResumen).catch(console.error);
    absenceService.getMyRequests().then(setSolicitudes).catch(console.error);
  }

  useEffect(() => { loadData(); }, []);

  useEffect(() => {
    if (!tipoSeleccionado?.esParcial && fecha && fechaFin) {
      absenceService.calculateDays(fecha, fechaFin, tipoSeleccionado?.id)
        .then((r) => setDiasPreview(r.diasHabiles))
        .catch(() => setDiasPreview(null));
      return;
    }
    if (tipoSeleccionado?.esParcial && fecha) {
      setDiasPreview(0.5);
      return;
    }
    setDiasPreview(null);
  }, [fecha, fechaFin, tipoSeleccionado]);

  useEffect(() => {
    if (tipoSeleccionado?.esParcial && fecha) setFechaFin(fecha);
  }, [fecha, tipoSeleccionado]);

  function selectTipo(codigo: string) {
    setSelectedCodigo(codigo);
    setError("");
    setSuccess("");
    setFecha("");
    setFechaFin("");
    setHoraInicio("");
    setHoraFin("");
    setComentario("");
  }

  async function submit() {
    if (!tipoSeleccionado) return;
    setError("");
    setSuccess("");
    setLoading(true);
    try {
      const fin = tipoSeleccionado.esParcial || !tipoSeleccionado.permiteMultiDia ? fecha : (fechaFin || fecha);
      await absenceService.createRequest({
        tipoAusenciaId: tipoSeleccionado.id,
        fechaInicio: fecha,
        fechaFin: fin,
        comentario: comentario || null,
        enviar: true,
        esDiaCompleto: !tipoSeleccionado.esParcial,
        horaInicio: horaInicio || null,
        horaFin: horaFin || null,
      });
      setSuccess("Permiso enviado para aprobación de tu jefe.");
      setSelectedCodigo(null);
      setFecha("");
      setFechaFin("");
      setHoraInicio("");
      setHoraFin("");
      setComentario("");
      loadData();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al solicitar permiso");
    } finally {
      setLoading(false);
    }
  }

  async function cancelar(id: string) {
    try {
      await absenceService.cancelRequest(id);
      loadData();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Error al cancelar");
    }
  }

  return (
    <PageContainer className="animate-fade-up">
      <PageHeader
        title="Permisos laborales"
        subtitle="Solicitudes conforme a la Ley Federal del Trabajo"
        infoTitle="¿Qué son los permisos?"
        infoContent="Los permisos son ausencias cortas autorizadas por ley o política interna: incapacidad, matrimonio, nacimiento, defunción de familiar, etc. Selecciona el tipo, indica fechas y envía tu solicitud. Tu jefe y RH la revisarán."
      />

      <Card className="border-urrea-secondary/20 bg-gradient-to-br from-urrea-primary/5 to-urrea-bg">
        <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <p className="text-sm font-semibold text-urrea-primary">Marco legal LFT</p>
            <p className="mt-1 max-w-2xl text-sm text-urrea-text-muted">
              Cada tipo incluye su base legal, límites anuales y reglas de goce de sueldo. Los permisos parciales
              (entrada tarde / salida temprano) registran medio día hábil.
            </p>
          </div>
          <div className="shrink-0 rounded-xl border border-urrea-border/70 bg-urrea-bg px-4 py-3 text-center">
            <p className="text-2xl font-bold text-urrea-primary">{tipos.length}</p>
            <p className="text-[10px] font-semibold uppercase tracking-wide text-urrea-text-muted">Tipos activos</p>
          </div>
        </div>
      </Card>

      <div>
        <h2 className="mb-3 text-sm font-semibold uppercase tracking-wide text-urrea-text-muted">
          Selecciona el tipo de permiso
        </h2>
        <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
          {tipos.map((tipo) => (
            <TipoCard
              key={tipo.id}
              tipo={tipo}
              resumen={resumen.find((r) => r.codigo === tipo.codigo)}
              selected={selectedCodigo === tipo.codigo}
              onSelect={() => selectTipo(tipo.codigo)}
            />
          ))}
        </div>
      </div>

      {tipoSeleccionado && (
        <Card title={`Solicitar: ${tipoSeleccionado.nombre}`}>
          <div className="mb-4 rounded-xl border border-urrea-border/60 bg-urrea-bg-soft/50 p-3">
            <p className="text-xs font-semibold text-urrea-secondary">{tipoSeleccionado.baseLegalLft}</p>
            <p className="mt-1 text-sm text-urrea-text-muted">{tipoSeleccionado.descripcion}</p>
            <div className="mt-2 flex flex-wrap gap-2">
              <span className="rounded-full bg-urrea-chrome/30 px-2.5 py-0.5 text-[10px] font-medium text-urrea-text">
                {tipoSeleccionado.remunerado ? "Con goce de sueldo" : "Sin goce de sueldo"}
              </span>
              {tipoSeleccionado.requiereComprobante && (
                <span className="rounded-full bg-urrea-accent-sand/30 px-2.5 py-0.5 text-[10px] font-medium text-urrea-primary">
                  Requiere comprobante / motivo
                </span>
              )}
            </div>
          </div>

          <div className="grid gap-4 sm:grid-cols-2">
            <Input
              label={tipoSeleccionado.esParcial ? "Fecha del permiso" : "Fecha inicio"}
              type="date"
              value={fecha}
              onChange={(e) => setFecha(e.target.value)}
            />
            {!tipoSeleccionado.esParcial && tipoSeleccionado.permiteMultiDia && (
              <Input
                label="Fecha fin"
                type="date"
                value={fechaFin}
                onChange={(e) => setFechaFin(e.target.value)}
                min={fecha}
              />
            )}
            {tipoSeleccionado.codigo === "ENTRADA_TARDE" && (
              <Input
                label="Hora estimada de entrada"
                type="time"
                value={horaInicio}
                onChange={(e) => setHoraInicio(e.target.value)}
              />
            )}
            {tipoSeleccionado.codigo === "SALIDA_TEMPRANO" && (
              <Input
                label="Hora de salida"
                type="time"
                value={horaFin}
                onChange={(e) => setHoraFin(e.target.value)}
              />
            )}
            <div>
              <p className="mb-1.5 text-sm font-medium text-urrea-text">
                {tipoSeleccionado.esParcial ? "Equivalente" : "Días hábiles"}
              </p>
              <p className="text-3xl font-semibold tabular-nums text-urrea-secondary">{diasPreview ?? "—"}</p>
            </div>
            <Textarea
              label={tipoSeleccionado.requiereComprobante ? "Motivo / referencia (obligatorio)" : "Comentario"}
              value={comentario}
              onChange={(e) => setComentario(e.target.value)}
              rows={2}
              className="sm:col-span-2"
            />
          </div>

          {error && <Alert variant="error" className="mt-3">{error}</Alert>}
          {success && <Alert variant="success" className="mt-3">{success}</Alert>}

          <div className="mt-4 flex flex-col gap-2 sm:flex-row">
            <Button type="button" disabled={loading || !fecha} onClick={submit} className="w-full sm:w-auto">
              Enviar solicitud
            </Button>
            <Button type="button" variant="ghost" onClick={() => setSelectedCodigo(null)} className="w-full sm:w-auto">
              Cancelar
            </Button>
          </div>
        </Card>
      )}

      <Card title="Mis permisos">
        {historial.length === 0 ? (
          <EmptyState message="No has solicitado permisos aún." />
        ) : (
          <ul className="space-y-3">
            {historial.map((s) => (
              <li key={s.id} className="rounded-xl border border-urrea-border/80 p-3.5">
                <div className="flex items-start justify-between gap-2">
                  <div>
                    <p className="font-medium text-urrea-text">{s.tipoAusenciaNombre}</p>
                    <p className="mt-1 text-sm text-urrea-text-muted">
                      {formatPeriodo(s)} · {s.diasSolicitados} día{s.diasSolicitados === 1 ? "" : "s"}
                    </p>
                  </div>
                  <Badge estado={s.estado} />
                </div>
                {s.comentario && <p className="mt-2 text-xs text-urrea-text-muted">{s.comentario}</p>}
                {s.estado === "Pendiente" && (
                  <Button type="button" variant="ghost" onClick={() => cancelar(s.id)} className="mt-2 h-auto min-h-0 px-0 text-red-600 hover:bg-transparent">
                    Cancelar solicitud
                  </Button>
                )}
              </li>
            ))}
          </ul>
        )}
      </Card>
    </PageContainer>
  );
}
