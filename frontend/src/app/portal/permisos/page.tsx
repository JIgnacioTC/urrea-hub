import { Suspense } from "react";
import { MisPermisosView } from "@/components/permisos/MisPermisosView";

export default function PermisosPage() {
  return (
    <Suspense fallback={<div className="px-4 py-8 text-sm text-gray-500">Cargando...</div>}>
      <MisPermisosView />
    </Suspense>
  );
}
