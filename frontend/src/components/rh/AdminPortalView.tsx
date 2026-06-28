"use client";

import { useCallback, useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { StatCard } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { PageContainer, PageHeader } from "@/components/ui/page-header";
import { portalAdminService } from "@/lib/services/portalService";
import type { FeedPost, ModuloPortal, PortalAdminEstado } from "@/lib/types";
import { cn } from "@/lib/utils";

export function AdminPortalView() {
  const [estado, setEstado] = useState<PortalAdminEstado | null>(null);
  const [publicaciones, setPublicaciones] = useState<FeedPost[]>([]);
  const [modulos, setModulos] = useState<ModuloPortal[]>([]);
  const [msg, setMsg] = useState("");
  const [nuevaPublicacion, setNuevaPublicacion] = useState({
    autorNombre: "Recursos Humanos",
    autorRol: "Comunicación interna",
    autorIniciales: "RH",
    departamento: "Corporativo",
    contenido: "",
    tipo: "General",
  });

  const refresh = useCallback(async () => {
    const [e, p, m] = await Promise.all([
      portalAdminService.getStatus(),
      portalAdminService.getPosts(),
      portalAdminService.getModules(),
    ]);
    setEstado(e);
    setPublicaciones(p);
    setModulos(m);
  }, []);

  useEffect(() => {
    refresh().catch(console.error);
  }, [refresh]);

  async function crearPublicacion() {
    if (!nuevaPublicacion.contenido.trim()) return;
    await portalAdminService.createPost({
      ...nuevaPublicacion,
      gradienteImagen: null,
      likes: 0,
      comentarios: 0,
      compartidos: 0,
      fechaPublicacion: new Date().toISOString(),
    });
    setNuevaPublicacion((prev) => ({ ...prev, contenido: "" }));
    setMsg("Publicación creada.");
    await refresh();
  }

  async function eliminarPublicacion(id: string) {
    await portalAdminService.deletePost(id);
    setMsg("Publicación eliminada.");
    await refresh();
  }

  async function guardarModulo(modulo: ModuloPortal) {
    await portalAdminService.updateModule({
      codigoModulo: modulo.codigoModulo,
      titulo: modulo.titulo,
      subtitulo: modulo.subtitulo,
      descripcion: modulo.descripcion,
      icono: modulo.icono,
      orden: modulos.indexOf(modulo) + 1,
      publicado: modulo.publicado,
    });
    setMsg(`Módulo ${modulo.codigoModulo} actualizado.`);
    await refresh();
  }

  return (
    <PageContainer>
      <PageHeader
        title="Administración del portal"
        subtitle="Base de datos, contenido y APIs · entorno desarrollo"
        action={
          <Button type="button" variant="secondary" onClick={() => refresh().catch(console.error)}>
            Actualizar
          </Button>
        }
      />
      {msg && <p className="mb-4 text-sm text-urrea-text-muted">{msg}</p>}

      <Tabs defaultValue="estado">
        <TabsList className="mb-6 flex-wrap">
          <TabsTrigger value="estado">Estado del sistema</TabsTrigger>
          <TabsTrigger value="feed">Feed del portal</TabsTrigger>
          <TabsTrigger value="modulos">Módulos Mi URREA</TabsTrigger>
        </TabsList>

        <TabsContent value="estado" className="space-y-6">
          {estado && (
            <>
              <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 lg:grid-cols-6">
                <StatCard label="Publicaciones" value={estado.publicaciones} accentClass="text-urrea-primary" />
                <StatCard label="Documentos" value={estado.documentos} accentClass="text-urrea-secondary" />
                <StatCard label="Convenios" value={estado.convenios} accentClass="text-emerald-700" />
                <StatCard label="Productos" value={estado.productos} accentClass="text-indigo-700" />
                <StatCard label="Módulos" value={estado.modulos} accentClass="text-urrea-primary" />
                <StatCard label="Colaboradores" value={estado.colaboradoresActivos} accentClass="text-urrea-secondary" />
              </div>

              <div className="rounded-2xl border border-urrea-border/80 bg-urrea-bg p-5 shadow-soft">
                <h3 className="font-semibold text-urrea-text">Base de datos</h3>
                <div className="mt-3 space-y-2 text-sm">
                  <p>
                    Estado:{" "}
                    <span className={cn("font-medium", estado.databaseOk ? "text-emerald-700" : "text-red-600")}>
                      {estado.databaseOk ? "Conectada" : "Sin conexión"}
                    </span>
                  </p>
                  <p className="text-urrea-text-muted">Entorno: {estado.entorno}</p>
                  <p className="break-all font-mono text-xs text-urrea-chrome">{estado.connectionInfo}</p>
                </div>
              </div>

              <div className="rounded-2xl border border-urrea-border/80 bg-urrea-bg p-5 shadow-soft">
                <h3 className="font-semibold text-urrea-text">Integraciones y APIs</h3>
                <p className="mt-1 text-xs text-urrea-text-muted">
                  En producción estos conectores apuntarán a nómina, directorio activo y servicios externos.
                </p>
                <div className="mt-4 space-y-3">
                  {estado.integraciones.map((i) => (
                    <div key={i.id} className="rounded-xl border border-urrea-border/60 bg-urrea-bg-soft/50 p-3">
                      <div className="flex flex-wrap items-center justify-between gap-2">
                        <p className="font-medium text-urrea-text">{i.nombre}</p>
                        <span className="rounded-full bg-urrea-primary/10 px-2 py-0.5 text-xs font-medium text-urrea-primary">
                          {i.estado}
                        </span>
                      </div>
                      <p className="text-xs text-urrea-text-muted">{i.sistemaExterno}</p>
                      {i.endpoint && <p className="mt-1 font-mono text-[10px] text-urrea-chrome">{i.endpoint}</p>}
                    </div>
                  ))}
                </div>
              </div>
            </>
          )}
        </TabsContent>

        <TabsContent value="feed" className="space-y-4">
          <div className="rounded-2xl border border-urrea-border/80 bg-urrea-bg p-4 shadow-soft">
            <h3 className="font-semibold text-urrea-text">Nueva publicación</h3>
            <textarea
              value={nuevaPublicacion.contenido}
              onChange={(e) => setNuevaPublicacion((p) => ({ ...p, contenido: e.target.value }))}
              placeholder="Contenido de la publicación..."
              className="mt-3 min-h-24 w-full rounded-xl border border-urrea-border bg-urrea-bg-soft px-3 py-2 text-sm outline-none focus:border-urrea-secondary"
            />
            <div className="mt-3 flex flex-wrap gap-2">
              <select
                value={nuevaPublicacion.tipo}
                onChange={(e) => setNuevaPublicacion((p) => ({ ...p, tipo: e.target.value }))}
                className="rounded-lg border border-urrea-border bg-urrea-bg px-3 py-2 text-sm"
              >
                <option value="General">General</option>
                <option value="Announcement">Anuncio</option>
                <option value="Recognition">Reconocimiento</option>
                <option value="Event">Evento</option>
              </select>
              <Button type="button" onClick={() => crearPublicacion().catch(console.error)}>
                Publicar
              </Button>
            </div>
          </div>

          <div className="space-y-3">
            {publicaciones.map((p) => (
              <article key={p.id} className="rounded-2xl border border-urrea-border/80 bg-urrea-bg p-4 shadow-soft">
                <div className="flex items-start justify-between gap-3">
                  <div>
                    <p className="font-semibold text-urrea-text">{p.authorName}</p>
                    <p className="text-xs text-urrea-text-muted">{p.type} · {new Date(p.createdAt).toLocaleString("es-MX")}</p>
                  </div>
                  <Button type="button" variant="secondary" className="text-xs" onClick={() => eliminarPublicacion(p.id).catch(console.error)}>
                    Eliminar
                  </Button>
                </div>
                <p className="mt-2 text-sm text-urrea-text">{p.content}</p>
              </article>
            ))}
          </div>
        </TabsContent>

        <TabsContent value="modulos" className="space-y-4">
          {modulos.map((m, idx) => (
            <div key={m.codigoModulo} className="rounded-2xl border border-urrea-border/80 bg-urrea-bg p-4 shadow-soft">
              <p className="text-xs font-semibold uppercase tracking-wide text-urrea-secondary">{m.codigoModulo}</p>
              <div className="mt-3 grid gap-3 sm:grid-cols-2">
                <label className="block text-sm">
                  <span className="text-urrea-text-muted">Título</span>
                  <input
                    value={m.titulo}
                    onChange={(e) =>
                      setModulos((prev) => prev.map((x, i) => (i === idx ? { ...x, titulo: e.target.value } : x)))
                    }
                    className="mt-1 w-full rounded-lg border border-urrea-border px-3 py-2"
                  />
                </label>
                <label className="block text-sm">
                  <span className="text-urrea-text-muted">Subtítulo</span>
                  <input
                    value={m.subtitulo ?? ""}
                    onChange={(e) =>
                      setModulos((prev) => prev.map((x, i) => (i === idx ? { ...x, subtitulo: e.target.value } : x)))
                    }
                    className="mt-1 w-full rounded-lg border border-urrea-border px-3 py-2"
                  />
                </label>
              </div>
              <label className="mt-3 block text-sm">
                <span className="text-urrea-text-muted">Descripción</span>
                <textarea
                  value={m.descripcion ?? ""}
                  onChange={(e) =>
                    setModulos((prev) => prev.map((x, i) => (i === idx ? { ...x, descripcion: e.target.value } : x)))
                  }
                  className="mt-1 min-h-20 w-full rounded-lg border border-urrea-border px-3 py-2"
                />
              </label>
              <div className="mt-3 flex flex-wrap items-center justify-between gap-2">
                <label className="flex items-center gap-2 text-sm">
                  <input
                    type="checkbox"
                    checked={m.publicado}
                    onChange={(e) =>
                      setModulos((prev) => prev.map((x, i) => (i === idx ? { ...x, publicado: e.target.checked } : x)))
                    }
                  />
                  Publicado
                </label>
                <Button type="button" variant="secondary" onClick={() => guardarModulo(m).catch(console.error)}>
                  Guardar
                </Button>
              </div>
            </div>
          ))}
        </TabsContent>
      </Tabs>
    </PageContainer>
  );
}
