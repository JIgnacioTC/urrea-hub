/**
 * Capa de adaptadores para integraciones futuras.
 * Hoy consume mock-data; en producción reemplazar implementaciones por fetch a CDM/SAP.
 */
import * as mock from "./mock-data";
import type { ColaboradorHcm } from "./types";

export interface DhApiAdapter {
  getColaboradores(): Promise<ColaboradorHcm[]>;
  getDashboardKpis(): Promise<Record<string, number>>;
  triggerSyncManual(): Promise<{ ok: boolean; message: string }>;
}

/** Implementación mock — swap por `CdmDhApiAdapter` cuando exista backend. */
export const dhApi: DhApiAdapter = {
  async getColaboradores() {
    return mock.COLABORADORES;
  },
  async getDashboardKpis() {
    return {
      activos: mock.COLABORADORES.filter((c) => c.estatus === "activo").length,
      solicitudesPendientes: mock.SOLICITUDES_AUSENCIA.filter((s) => s.estado === "pendiente").length,
      ticketsAbiertos: mock.TICKETS.filter((t) => !["cerrado", "resuelto"].includes(t.estado)).length,
    };
  },
  async triggerSyncManual() {
    return { ok: true, message: "Sync simulada — conectar POST /api/integraciones/sync" };
  },
};
