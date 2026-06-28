import { Suspense } from "react";
import { TiempoLibreView } from "@/components/vacaciones/TiempoLibreView";

export default function VacacionesPage() {
  return (
    <Suspense fallback={<div className="px-4 py-8 text-sm text-urrea-text-muted">Cargando…</div>}>
      <TiempoLibreView />
    </Suspense>
  );
}
