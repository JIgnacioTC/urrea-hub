import { Suspense } from "react";
import { AsistenciasImportView } from "@/components/asistencias/AsistenciasImportView";

export default function AsistenciasPage() {
  return (
    <Suspense fallback={<div className="px-4 py-8 text-sm text-gray-500">Cargando...</div>}>
      <AsistenciasImportView />
    </Suspense>
  );
}
