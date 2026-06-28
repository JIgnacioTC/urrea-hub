/** Métricas consolidadas — escala corporativa URREA (~4,000 colaboradores). */

export const EMPRESA = {
  razonSocial: "Urrea Grupo Industrial, S.A. de C.V.",
  plataforma: "Centro Integral de Desarrollo Humano",
  version: "2026.2.0",
  entorno: "Producción (demostración)",
} as const;

export const METRICAS_EJECUTIVAS = {
  colaboradoresActivos: 4_012,
  colaboradoresTotal: 4_187,
  plantasOperativas: 14,
  ubicaciones: 38,
  centrosCosto: 126,
  unidadesOrganizativas: 89,
  solicitudesPendientes: 247,
  vacacionesPorAprobar: 89,
  incidenciasAsistenciaHoy: 34,
  cursosVencidosOProximos: 156,
  ticketsAbiertos: 42,
  evaluacionesPendientes: 318,
  onboardingActivos: 23,
  vacantesAbiertas: 47,
  syncUltimaOk: "2026-06-24T02:15:00",
  syncErrores: 2,
  rotacionAnualPct: 8.2,
  ausentismoPct: 3.1,
  climaLaboralRespuestaPct: 68,
  denunciasAbiertas: 3,
};

export const HEADCOUNT_EMPRESA = [
  { area: "Manufactura", count: 1_842, pct: 45.9 },
  { area: "Comercial", count: 687, pct: 17.1 },
  { area: "Operaciones", count: 524, pct: 13.1 },
  { area: "Desarrollo Humano", count: 98, pct: 2.4 },
  { area: "Nómina", count: 76, pct: 1.9 },
  { area: "Finanzas", count: 142, pct: 3.5 },
  { area: "Tecnología", count: 189, pct: 4.7 },
  { area: "Legal", count: 45, pct: 1.1 },
  { area: "Seguridad e Higiene", count: 312, pct: 7.8 },
  { area: "Cadena de suministro", count: 97, pct: 2.4 },
] as const;

export const SOLICITUDES_MODULO_EMPRESA = [
  { modulo: "Vacaciones y permisos", count: 247, sla: "48h" },
  { modulo: "Asistencia e incidencias", count: 186, sla: "24h" },
  { modulo: "Beneficios y prestaciones", count: 94, sla: "72h" },
  { modulo: "Servicios al colaborador", count: 42, sla: "24h" },
  { modulo: "Capacitación", count: 67, sla: "5 días" },
  { modulo: "Expediente digital", count: 31, sla: "48h" },
] as const;

export const SEDES = [
  "Monterrey HQ",
  "Monterrey Planta Norte",
  "Monterrey Planta Sur",
  "Ciudad de México",
  "Guadalajara",
  "Querétaro",
  "León",
  "Puebla",
  "Tijuana",
  "Ciudad Juárez",
  "Hermosillo",
  "Torreón",
  "Saltillo",
  "San Luis Potosí",
] as const;

export function formatNum(n: number) {
  return n.toLocaleString("es-MX");
}
