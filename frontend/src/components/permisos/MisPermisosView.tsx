"use client";

import { useState, useEffect } from "react";
import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";

interface Permiso {
  id: string;
  colaboradorId: string;
  nombreColaborador: string;
  tipoAusenciaId: string;
  nombreTipo: string;
  codigoTipo: string;
  fechaInicio: string;
  fechaFin: string;
  diasSolicitados: number;
  comentario: string;
  estado: string;
  createdAt: string;
}

export function MisPermisosView() {
  const [permisos, setPermisos] = useState<Permiso[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    fetchMisPermisos();
  }, []);

  const fetchMisPermisos = async () => {
    try {
      const data = await fetchApi<Permiso[]>(v1("/permisos/mis-casos"));
      setPermisos(data);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const getStatusBadge = (estado: string) => {
    switch (estado) {
      case "Borrador": return <span className="px-2 py-1 text-xs rounded-full bg-gray-100 text-gray-800 font-medium">Borrador</span>;
      case "Pendiente": return <span className="px-2 py-1 text-xs rounded-full bg-yellow-100 text-yellow-800 font-medium">Pendiente</span>;
      case "Aprobada": return <span className="px-2 py-1 text-xs rounded-full bg-green-100 text-green-800 font-medium">Aprobado</span>;
      case "Rechazada": return <span className="px-2 py-1 text-xs rounded-full bg-red-100 text-red-800 font-medium">Rechazado</span>;
      case "Cancelada": return <span className="px-2 py-1 text-xs rounded-full bg-gray-200 text-gray-600 font-medium">Cancelado</span>;
      default: return <span className="px-2 py-1 text-xs rounded-full bg-gray-100 text-gray-800 font-medium">{estado}</span>;
    }
  };

  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Mis Permisos</h1>
          <p className="text-sm text-gray-500 mt-1">Consulta el estado de los permisos que has solicitado.</p>
        </div>
      </div>

      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl mb-6 text-sm">
          {error}
        </div>
      )}

      {loading ? (
        <div className="text-sm text-gray-500">Cargando permisos...</div>
      ) : permisos.length === 0 ? (
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-8 text-center">
          <p className="text-gray-500 text-sm">No has solicitado ningún permiso aún.</p>
        </div>
      ) : (
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-sm text-left">
              <thead className="bg-gray-50 text-gray-600 font-medium border-b border-gray-200">
                <tr>
                  <th className="px-4 py-3">Tipo de Permiso</th>
                  <th className="px-4 py-3">Fechas</th>
                  <th className="px-4 py-3">Días</th>
                  <th className="px-4 py-3">Estado</th>
                  <th className="px-4 py-3">Comentario</th>
                  <th className="px-4 py-3 text-right">Creado</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {permisos.map(p => (
                  <tr key={p.id} className="hover:bg-gray-50/50">
                    <td className="px-4 py-3 font-medium text-gray-900">
                      {p.nombreTipo}
                    </td>
                    <td className="px-4 py-3 text-gray-600">
                      {new Date(p.fechaInicio).toLocaleDateString()}
                      {p.fechaInicio !== p.fechaFin && ` al ${new Date(p.fechaFin).toLocaleDateString()}`}
                    </td>
                    <td className="px-4 py-3 text-gray-600">
                      {p.diasSolicitados}
                    </td>
                    <td className="px-4 py-3">
                      {getStatusBadge(p.estado)}
                    </td>
                    <td className="px-4 py-3 text-gray-500 max-w-xs truncate">
                      {p.comentario || "-"}
                    </td>
                    <td className="px-4 py-3 text-gray-500 text-right">
                      {new Date(p.createdAt).toLocaleDateString()}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}
