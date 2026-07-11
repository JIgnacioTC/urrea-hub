"use client";

import { useEffect, useState } from "react";
import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";
import { Button } from "@/components/ui/button";
import { PageContainer, PageHeader, Alert } from "@/components/ui/page-header";

interface ConfiguracionGlobal {
  id: string;
  clave: string;
  valor: string;
  descripcion?: string;
}

export function TiConfiguracionesView() {
  const [configs, setConfigs] = useState<ConfiguracionGlobal[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [editingClave, setEditingClave] = useState<string | null>(null);
  const [editValor, setEditValor] = useState("");

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await fetchApi<ConfiguracionGlobal[]>(v1("/ti/configuraciones"));
      setConfigs(data);
    } catch (err: any) {
      setError(err.message || "Error al cargar las configuraciones.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const handleEdit = (config: ConfiguracionGlobal) => {
    setEditingClave(config.clave);
    setEditValor(config.valor);
    setSuccess(null);
  };

  const handleSave = async (clave: string) => {
    setError(null);
    setSuccess(null);
    try {
      const target = configs.find(c => c.clave === clave);
      await fetchApi(v1(`/ti/configuraciones/${clave}`), {
        method: "PUT",
        body: JSON.stringify({
          valor: editValor,
          descripcion: target?.descripcion
        })
      });
      setConfigs(configs.map(c => c.clave === clave ? { ...c, valor: editValor } : c));
      setEditingClave(null);
      setSuccess(`Configuración '${clave}' actualizada correctamente.`);
    } catch (err: any) {
      setError(err.message || "No se pudo actualizar la configuración.");
    }
  };

  return (
    <PageContainer className="max-w-4xl">
      <PageHeader
        title="Variables Globales de TI"
        subtitle="Configura claves del sistema, endpoints de integración y variables globales de la plataforma."
      />

      {error && <Alert variant="error" className="mb-4">{error}</Alert>}
      {success && <Alert variant="success" className="mb-4">{success}</Alert>}

      {loading ? (
        <div className="flex items-center justify-center p-8 bg-white border border-slate-200 rounded-2xl">
          <span className="text-slate-500 text-sm">Cargando variables...</span>
        </div>
      ) : (
        <div className="space-y-4">
          <div className="bg-slate-50 rounded-2xl p-5 border border-slate-100 flex flex-col gap-2 mb-6">
            <h3 className="font-semibold text-slate-800 text-sm">Integración con Power Automate</h3>
            <p className="text-xs text-slate-500 leading-relaxed">
              Aquí puedes definir el endpoint del webhook/API de Power Automate. Al activar notificaciones (Teams/Correo) en los diferentes tipos de permiso de ausencia, la plataforma utilizará esta variable global para despachar la ejecución y automatización del flujo de aprobación.
            </p>
          </div>

          <div className="grid gap-4">
            {configs.map((cfg) => {
              const isEditing = editingClave === cfg.clave;
              return (
                <div 
                  key={cfg.id} 
                  className={`rounded-2xl border bg-white p-5 shadow-sm transition-all duration-200 ${
                    isEditing ? "border-urrea-primary ring-2 ring-urrea-primary/10" : "border-slate-200 hover:border-slate-350"
                  }`}
                >
                  <div className="flex flex-col gap-3">
                    <div className="flex items-start justify-between">
                      <div>
                        <span className="font-mono text-xs font-bold text-slate-500 bg-slate-100 px-2 py-0.5 rounded">
                          {cfg.clave}
                        </span>
                        <p className="mt-1 text-sm text-slate-600">
                          {cfg.descripcion || "Sin descripción proporcionada."}
                        </p>
                      </div>
                      
                      {!isEditing && (
                        <button
                          type="button"
                          onClick={() => handleEdit(cfg)}
                          className="text-xs font-semibold text-urrea-primary hover:underline"
                        >
                          Editar valor
                        </button>
                      )}
                    </div>

                    {isEditing ? (
                      <div className="flex flex-col gap-3 mt-2">
                        <textarea
                          rows={3}
                          className="w-full rounded-xl border border-slate-200 px-3 py-2 text-sm font-mono focus:border-urrea-primary focus:ring-1 focus:ring-urrea-primary"
                          value={editValor}
                          onChange={(e) => setEditValor(e.target.value)}
                          placeholder="Escribe el valor de la variable aquí..."
                        />
                        <div className="flex justify-end gap-2 text-xs">
                          <button
                            type="button"
                            onClick={() => setEditingClave(null)}
                            className="rounded-xl border border-slate-200 bg-white px-3 py-1.5 font-semibold text-slate-700 hover:bg-slate-50"
                          >
                            Cancelar
                          </button>
                          <button
                            type="button"
                            onClick={() => handleSave(cfg.clave)}
                            className="rounded-xl bg-urrea-primary px-3 py-1.5 font-semibold text-white hover:bg-opacity-90"
                          >
                            Guardar cambios
                          </button>
                        </div>
                      </div>
                    ) : (
                      <div className="mt-1 rounded-xl bg-slate-50/50 border border-slate-100 p-3">
                        <span className="block text-xs font-semibold text-slate-400 mb-1">VALOR ACTUAL</span>
                        <span className="font-mono text-xs break-all text-slate-800">
                          {cfg.valor ? cfg.valor : <em className="text-slate-400 font-sans">[Vacío / No configurado]</em>}
                        </span>
                      </div>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      )}
    </PageContainer>
  );
}
