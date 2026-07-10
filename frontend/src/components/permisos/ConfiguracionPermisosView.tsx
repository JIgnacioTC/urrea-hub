"use client";

import { useState, useEffect } from "react";
import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";

interface TipoPermiso {
  id: string;
  codigo: string;
  nombre: string;
  webhookUrl?: string;
  areaDestinoId?: string;
}

interface Area {
  id: string;
  nombre: string;
}

export function ConfiguracionPermisosView() {
  const [tipos, setTipos] = useState<TipoPermiso[]>([]);
  const [areas, setAreas] = useState<Area[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [editingId, setEditingId] = useState<string | null>(null);
  const [editForm, setEditForm] = useState<{ webhookUrl: string; areaDestinoId: string }>({ webhookUrl: "", areaDestinoId: "" });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      // Usamos el endpoint existente de tipos de permiso
      const dataTipos = await fetchApi<TipoPermiso[]>(v1("/vacaciones/tipos-ausencia"));
      setTipos(dataTipos);

      try {
        const dataAreas = await fetchApi<Area[]>(v1("/core-rh/areas"));
        setAreas(dataAreas);
      } catch (areaErr) {
        console.error("Error fetching areas:", areaErr);
      }
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = (tipo: TipoPermiso) => {
    setEditingId(tipo.id);
    setEditForm({ 
      webhookUrl: tipo.webhookUrl || "", 
      areaDestinoId: tipo.areaDestinoId || "" 
    });
  };

  const handleSave = async (id: string) => {
    try {
      await fetchApi(v1(`/permisos/configuracion/${id}`), {
        method: "PUT",
        body: JSON.stringify({
          webhookUrl: editForm.webhookUrl || null,
          areaDestinoId: editForm.areaDestinoId || null
        })
      });
      
      setTipos(tipos.map(t => t.id === id ? { ...t, webhookUrl: editForm.webhookUrl, areaDestinoId: editForm.areaDestinoId } : t));
      setEditingId(null);
    } catch (err: any) {
      alert(err.message);
    }
  };

  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Configuración de Permisos</h1>
          <p className="text-sm text-gray-500 mt-1">Configura las áreas de autorización y webhooks para la automatización (n8n) por cada tipo de permiso.</p>
        </div>
      </div>

      {error && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl mb-6 text-sm">
          {error}
        </div>
      )}

      {loading ? (
        <div className="text-sm text-gray-500">Cargando configuración...</div>
      ) : (
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-sm text-left">
              <thead className="bg-gray-50 text-gray-600 font-medium border-b border-gray-200">
                <tr>
                  <th className="px-4 py-3">Tipo de Permiso</th>
                  <th className="px-4 py-3">Área de Ruteo</th>
                  <th className="px-4 py-3">Webhook (n8n)</th>
                  <th className="px-4 py-3 text-right">Acciones</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {tipos.map(tipo => (
                  <tr key={tipo.id} className="hover:bg-gray-50/50">
                    <td className="px-4 py-3 font-medium text-gray-900">
                      {tipo.nombre}
                    </td>
                    <td className="px-4 py-3">
                      {editingId === tipo.id ? (
                        <select 
                          className="w-full border-gray-300 rounded-md shadow-sm text-sm"
                          value={editForm.areaDestinoId}
                          onChange={(e) => setEditForm({ ...editForm, areaDestinoId: e.target.value })}
                        >
                          <option value="">Jefe Inmediato (Por defecto)</option>
                          {areas.map(a => (
                            <option key={a.id} value={a.id}>{a.nombre}</option>
                          ))}
                        </select>
                      ) : (
                        <span className="text-gray-600">
                          {areas.find(a => a.id === tipo.areaDestinoId)?.nombre || "Jefe Inmediato (Por defecto)"}
                        </span>
                      )}
                    </td>
                    <td className="px-4 py-3">
                      {editingId === tipo.id ? (
                        <input 
                          type="text" 
                          className="w-full border-gray-300 rounded-md shadow-sm text-sm"
                          placeholder="https://n8n.ejemplo.com/webhook/..."
                          value={editForm.webhookUrl}
                          onChange={(e) => setEditForm({ ...editForm, webhookUrl: e.target.value })}
                        />
                      ) : (
                        <span className="text-gray-500 truncate max-w-xs block">
                          {tipo.webhookUrl || "-"}
                        </span>
                      )}
                    </td>
                    <td className="px-4 py-3 text-right">
                      {editingId === tipo.id ? (
                        <div className="flex justify-end space-x-2">
                          <button 
                            onClick={() => setEditingId(null)}
                            className="text-gray-500 hover:text-gray-700 font-medium"
                          >
                            Cancelar
                          </button>
                          <button 
                            onClick={() => handleSave(tipo.id)}
                            className="text-urrea-blue hover:text-blue-700 font-medium"
                          >
                            Guardar
                          </button>
                        </div>
                      ) : (
                        <button 
                          onClick={() => handleEdit(tipo)}
                          className="text-urrea-blue hover:text-blue-700 font-medium"
                        >
                          Editar
                        </button>
                      )}
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
