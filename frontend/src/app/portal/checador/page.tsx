"use client";

import { useEffect, useState, useRef } from "react";
import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";
import { attendanceService, type ChecadorResult } from "@/lib/services/attendanceService";
import { employeeService } from "@/lib/services/employeeService";
import type { HcmCatalogItem } from "@/lib/types/hcm";

function fmtTime(iso?: string) {
  if (!iso) return "—";
  return new Date(iso).toLocaleTimeString("es-MX", { hour: "2-digit", minute: "2-digit", second: "2-digit" });
}

export default function ChecadorSedePage() {
  const [sedes, setSedes] = useState<HcmCatalogItem[]>([]);
  const [selectedSedeId, setSelectedSedeId] = useState("");
  const [barcode, setBarcode] = useState("");
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<ChecadorResult | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  
  const inputRef = useRef<HTMLInputElement>(null);
  const timerRef = useRef<NodeJS.Timeout | null>(null);

  // Load locations/sedes
  useEffect(() => {
    employeeService.getCatalogs()
      .then((cats) => {
        setSedes(cats.locations);
        // Recover selected Sede from localStorage if exists
        const saved = localStorage.getItem("checador_sede_id");
        if (saved && cats.locations.some(s => s.id === saved)) {
          setSelectedSedeId(saved);
        } else if (cats.locations.length > 0) {
          setSelectedSedeId(cats.locations[0].id);
          localStorage.setItem("checador_sede_id", cats.locations[0].id);
        }
      })
      .catch(console.error);
  }, []);

  // Autofocus input field continuously
  useEffect(() => {
    const focusInput = () => {
      if (inputRef.current) {
        inputRef.current.focus();
      }
    };
    focusInput();
    const interval = setInterval(focusInput, 2000);
    return () => clearInterval(interval);
  }, []);

  const handleSedeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const val = e.target.value;
    setSelectedSedeId(val);
    localStorage.setItem("checador_sede_id", val);
    if (inputRef.current) {
      inputRef.current.focus();
    }
  };

  const handleScanSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const cleanCode = barcode.trim();
    if (!cleanCode) return;

    if (!selectedSedeId) {
      setErrorMessage("Por favor selecciona una sede de la lista.");
      return;
    }

    if (timerRef.current) {
      clearTimeout(timerRef.current);
    }

    setLoading(true);
    setResult(null);
    setErrorMessage(null);

    try {
      const res = await attendanceService.verifyAndRegisterChecador({
        numeroEmpleado: cleanCode,
        sedeId: selectedSedeId,
      });

      setResult(res);

      if (res.success) {
        // Success auto-clear after 3.5 seconds
        timerRef.current = setTimeout(() => {
          setResult(null);
          setBarcode("");
        }, 3500);
      } else {
        // Warning/Error auto-clear after 5 seconds
        timerRef.current = setTimeout(() => {
          setResult(null);
          setBarcode("");
        }, 5000);
      }
    } catch (err: any) {
      setErrorMessage(err.message || "Error de comunicación con el servidor.");
      timerRef.current = setTimeout(() => {
        setErrorMessage(null);
        setBarcode("");
      }, 5000);
    } finally {
      setLoading(false);
      setBarcode("");
    }
  };

  return (
    <div className="min-h-screen bg-slate-950 text-white flex flex-col font-sans selection:bg-urrea-secondary selection:text-white">
      {/* Kiosk Header */}
      <header className="border-b border-slate-800 bg-slate-900/80 backdrop-blur px-6 py-4 flex flex-col sm:flex-row items-center justify-between gap-4">
        <div className="flex items-center space-x-3">
          <div className="h-10 w-10 rounded-xl gradient-urrea flex items-center justify-center shadow-lg">
            <svg className="w-5 h-5 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <div>
            <h1 className="text-lg font-bold tracking-tight">Estación de Asistencia</h1>
            <p className="text-xs text-slate-400">Terminal de Checado Automatizada</p>
          </div>
        </div>

        {/* Sede selector */}
        <div className="flex items-center space-x-3 w-full sm:w-auto">
          <label className="text-xs font-semibold text-slate-400 whitespace-nowrap">SEDE / ESTACIÓN:</label>
          <select
            value={selectedSedeId}
            onChange={handleSedeChange}
            className="bg-slate-800 border border-slate-700 rounded-xl px-3 py-1.5 text-sm text-white focus:outline-none focus:ring-2 focus:ring-urrea-secondary w-full sm:w-64"
          >
            {sedes.length === 0 ? (
              <option value="">Cargando sedes...</option>
            ) : (
              sedes.map((s) => (
                <option key={s.id} value={s.id}>{s.name}</option>
              ))
            )}
          </select>
        </div>
      </header>

      {/* Main Body */}
      <main className="flex-1 flex flex-col items-center justify-center px-4 py-8">
        <div className="w-full max-w-2xl bg-slate-900 border border-slate-800 rounded-3xl p-8 shadow-2xl flex flex-col items-center text-center">
          
          {/* Scan Prompt Icon */}
          {!result && !errorMessage && !loading && (
            <div className="mb-8">
              <div className="h-24 w-24 rounded-full bg-slate-800 flex items-center justify-center mx-auto mb-4 animate-pulse">
                <svg className="w-12 h-12 text-slate-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M12 4v1m6 11h2m-6 0h-2v4m0-11v3m0 0h.01M12 12h4.01M16 20h4M4 12h4m12 0h.01M5 8h2a1 1 0 001-1V5a1 1 0 00-1-1H5a1 1 0 00-1 1v2a1 1 0 001 1zm12 0h2a1 1 0 001-1V5a1 1 0 00-1-1h-2a1 1 0 00-1 1v2a1 1 0 001 1zM5 20h2a1 1 0 001-1v-2a1 1 0 00-1-1H5a1 1 0 00-1 1v2a1 1 0 001 1z" />
                </svg>
              </div>
              <h2 className="text-xl font-bold">Escanea tu Código de Barras</h2>
              <p className="text-sm text-slate-400 mt-1 max-w-sm">
                Coloca tu credencial frente al lector para registrar tu asistencia de hoy.
              </p>
            </div>
          )}

          {/* Loading state */}
          {loading && (
            <div className="mb-8 py-8">
              <div className="animate-spin rounded-full h-12 w-12 border-4 border-urrea-secondary border-t-transparent mx-auto"></div>
              <p className="text-slate-400 text-sm mt-4">Buscando colaborador y registrando...</p>
            </div>
          )}

          {/* Success screen */}
          {result && result.success && (
            <div className="mb-8 w-full animate-fade-up">
              <div className="h-24 w-24 rounded-full bg-emerald-500/10 border-2 border-emerald-500 flex items-center justify-center mx-auto mb-4">
                <svg className="w-12 h-12 text-emerald-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M5 13l4 4L19 7" />
                </svg>
              </div>
              <span className="inline-block px-3 py-1 rounded-full text-xs font-bold bg-emerald-500/20 text-emerald-400 mb-2">
                REGISTRO EXITOSO: {result.tipoRegistro?.toUpperCase()}
              </span>
              <h2 className="text-3xl font-extrabold text-white tracking-tight">{result.empleadoNombre}</h2>
              <p className="text-slate-400 font-mono text-sm mt-1">No. Empleado: {result.numeroEmpleado}</p>
              
              <div className="mt-6 p-4 rounded-2xl bg-slate-800/50 border border-slate-700/50 inline-grid grid-cols-2 gap-x-8 gap-y-2 text-left max-w-md mx-auto">
                <div>
                  <span className="text-slate-400 text-[10px] uppercase font-bold tracking-wider">Hora Registro</span>
                  <p className="text-lg font-bold text-white font-mono">{fmtTime(result.horaRegistro)}</p>
                </div>
                <div>
                  <span className="text-slate-400 text-[10px] uppercase font-bold tracking-wider">Turno Asignado</span>
                  <p className="text-sm font-semibold text-white truncate">{result.turnoNombre || "Sin turno"}</p>
                </div>
                <div className="col-span-2 border-t border-slate-700/60 pt-2 mt-1">
                  <span className="text-slate-400 text-[10px] uppercase font-bold tracking-wider">Horario de Turno</span>
                  <p className="text-xs font-medium text-slate-300">{result.turnoHorario || "N/A"}</p>
                </div>
              </div>
            </div>
          )}

          {/* Validation Warning / Error from API */}
          {result && !result.success && (
            <div className="mb-8 w-full animate-fade-up">
              <div className="h-24 w-24 rounded-full bg-rose-500/10 border-2 border-rose-500 flex items-center justify-center mx-auto mb-4">
                <svg className="w-12 h-12 text-rose-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2.5} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                </svg>
              </div>
              <span className="inline-block px-3 py-1 rounded-full text-xs font-bold bg-rose-500/20 text-rose-400 mb-2">
                ACCESO DENEGADO / ALERTA
              </span>
              <h2 className="text-2xl font-extrabold text-white tracking-tight">{result.empleadoNombre || "Colaborador"}</h2>
              {result.numeroEmpleado && (
                <p className="text-slate-400 font-mono text-sm mt-0.5">No. Empleado: {result.numeroEmpleado}</p>
              )}
              
              <div className="mt-4 p-4 rounded-xl bg-rose-950/30 border border-rose-800/40 text-rose-200 text-sm max-w-md mx-auto">
                <p className="font-semibold text-rose-400">Motivo del rechazo:</p>
                <p className="mt-1 font-medium">{result.error}</p>
              </div>
            </div>
          )}

          {/* General Exception Error */}
          {errorMessage && (
            <div className="mb-8 w-full animate-fade-up">
              <div className="h-20 w-20 rounded-full bg-amber-500/10 border-2 border-amber-500 flex items-center justify-center mx-auto mb-4">
                <svg className="w-10 h-10 text-amber-500" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                </svg>
              </div>
              <h3 className="text-lg font-bold text-amber-400">Error de Escaneo</h3>
              <p className="text-slate-300 text-sm mt-2 max-w-sm mx-auto">
                {errorMessage}
              </p>
            </div>
          )}

          {/* Form scanner catcher */}
          <form onSubmit={handleScanSubmit} className="w-full max-w-md">
            <input
              ref={inputRef}
              type="text"
              className="w-full bg-slate-950 border-2 border-slate-800 rounded-2xl px-4 py-4 text-center font-mono text-xl tracking-widest text-white placeholder-slate-650 focus:border-urrea-secondary focus:outline-none focus:ring-4 focus:ring-urrea-secondary/15 transition-all"
              placeholder="ESCANEAR CÓDIGO..."
              value={barcode}
              onChange={(e) => setBarcode(e.target.value)}
              disabled={loading}
              autoComplete="off"
            />
            <button type="submit" className="hidden">Submit</button>
          </form>
        </div>
      </main>

      {/* Footer info */}
      <footer className="border-t border-slate-900 bg-slate-950 px-6 py-4 flex flex-col sm:flex-row items-center justify-between text-xs text-slate-500 gap-2">
        <p>© Urrea. Todos los derechos reservados.</p>
        <div className="flex items-center space-x-1">
          <span className="h-2 w-2 rounded-full bg-emerald-500 animate-ping"></span>
          <span>Terminal de Checado Conectada y Activa</span>
        </div>
      </footer>
    </div>
  );
}
