import { Suspense } from "react";
import { ConfiguracionPermisosView } from "@/components/permisos/ConfiguracionPermisosView";

export default function ConfiguracionPermisosPage() {
  return (
    <Suspense fallback={<div className="px-4 py-8 text-sm text-gray-500">Cargando...</div>}>
      <ConfiguracionPermisosView />
    </Suspense>
  );
}
