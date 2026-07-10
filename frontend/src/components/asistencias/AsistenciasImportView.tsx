"use client";

import { useState } from "react";
import { API_URL } from "@/lib/api";
import { v1 } from "@/lib/api/v1";

interface ImportResult {
  totalProcesados: number;
  incidenciasDetectadas: number;
  incidenciasJustificadas: number;
  incidenciasPorRevisar: number;
  errores: string[];
}

export function AsistenciasImportView() {
  const [file, setFile] = useState<File | null>(null);
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<ImportResult | null>(null);
  const [error, setError] = useState("");

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
      setFile(e.dataTransfer.files[0]);
    }
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      setFile(e.target.files[0]);
    }
  };

  const handleImport = async () => {
    if (!file) return;
    setLoading(true);
    setError("");
    setResult(null);

    const formData = new FormData();
    formData.append("file", file);

    try {
      const token = localStorage.getItem("urrea_token");
      const res = await fetch(`${API_URL}${v1("/permisos/importar-asistencias")}`, {
        method: "POST",
        headers: { "Authorization": `Bearer ${token}` },
        body: formData
      });

      if (!res.ok) {
        const errorData = await res.json();
        throw new Error(errorData.error || "Error al importar el archivo");
      }

      const data = await res.json();
      setResult(data);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-6">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Importación de Asistencias</h1>
          <p className="text-sm text-gray-500 mt-1">Sube el archivo Excel de asistencias para conciliar con los permisos aprobados.</p>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        <div>
          <div 
            className="border-2 border-dashed border-gray-300 rounded-xl p-10 flex flex-col items-center justify-center bg-gray-50 hover:bg-gray-100 transition cursor-pointer"
            onDragOver={handleDragOver}
            onDrop={handleDrop}
            onClick={() => document.getElementById('file-upload')?.click()}
          >
            <div className="h-16 w-16 text-gray-400 mb-4">
              <svg fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
              </svg>
            </div>
            <p className="text-sm text-gray-600 mb-2 font-medium">Arrastra tu archivo Excel aquí o haz clic para subir</p>
            <p className="text-xs text-gray-500">Soporta .xlsx</p>
            
            <input 
              id="file-upload" 
              type="file" 
              accept=".xlsx, .xls"
              className="hidden" 
              onChange={handleFileChange}
            />
          </div>

          {file && (
            <div className="mt-4 flex items-center justify-between bg-white border border-gray-200 p-4 rounded-xl shadow-sm">
              <div className="flex items-center space-x-3">
                <div className="h-10 w-10 bg-green-100 text-green-600 rounded-lg flex items-center justify-center">
                  <svg className="w-6 h-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 17v-2m3 2v-4m3 4v-6m2 10H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                  </svg>
                </div>
                <div>
                  <p className="text-sm font-medium text-gray-900">{file.name}</p>
                  <p className="text-xs text-gray-500">{(file.size / 1024).toFixed(2)} KB</p>
                </div>
              </div>
              <button 
                onClick={handleImport}
                disabled={loading}
                className="px-4 py-2 bg-urrea-blue text-white text-sm font-medium rounded-lg hover:bg-urrea-blue/90 disabled:opacity-50 transition"
              >
                {loading ? "Procesando..." : "Importar Datos"}
              </button>
            </div>
          )}

          {error && (
            <div className="mt-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm">
              {error}
            </div>
          )}
        </div>

        <div>
          {result && (
            <div className="bg-white border border-gray-200 rounded-xl shadow-sm p-6">
              <h2 className="text-lg font-bold text-gray-900 mb-4 flex items-center">
                <svg className="w-5 h-5 text-green-500 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                Resultados de la Importación
              </h2>
              
              <div className="space-y-4">
                <div className="flex justify-between items-center py-2 border-b border-gray-100">
                  <span className="text-gray-600 text-sm">Total de filas procesadas</span>
                  <span className="font-bold text-gray-900">{result.totalProcesados}</span>
                </div>
                <div className="flex justify-between items-center py-2 border-b border-gray-100">
                  <span className="text-gray-600 text-sm">Inasistencias detectadas</span>
                  <span className="font-bold text-gray-900">{result.incidenciasDetectadas}</span>
                </div>
                <div className="flex justify-between items-center py-2 border-b border-gray-100">
                  <span className="text-gray-600 text-sm">Inasistencias justificadas (permiso activo)</span>
                  <span className="font-bold text-green-600">{result.incidenciasJustificadas}</span>
                </div>
                <div className="flex justify-between items-center py-2 border-b border-gray-100">
                  <span className="text-gray-600 text-sm">Inasistencias por revisar</span>
                  <span className="font-bold text-red-600">{result.incidenciasPorRevisar}</span>
                </div>
              </div>

              {result.errores && result.errores.length > 0 && (
                <div className="mt-6">
                  <h3 className="text-sm font-semibold text-gray-900 mb-2">Advertencias / Errores en filas:</h3>
                  <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-3 max-h-40 overflow-y-auto">
                    <ul className="list-disc list-inside text-xs text-yellow-800 space-y-1">
                      {result.errores.map((err, i) => (
                        <li key={i}>{err}</li>
                      ))}
                    </ul>
                  </div>
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
