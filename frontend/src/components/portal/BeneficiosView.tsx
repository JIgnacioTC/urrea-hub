"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { Button } from "@/components/ui/button";
import { PageContainer, PageHeader } from "@/components/ui/page-header";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { portalService } from "@/lib/services/portalService";
import type { BeneficiosCatalogo, DocumentoCorporativo, ProductoTienda } from "@/lib/types";
import { cn } from "@/lib/utils";

function DocCard({ doc }: { doc: DocumentoCorporativo }) {
  return (
    <button
      type="button"
      className="group flex flex-col rounded-2xl border border-urrea-border/80 bg-urrea-bg p-4 text-left transition hover:border-urrea-secondary/40 hover:shadow-soft"
    >
      <div className="flex items-start justify-between gap-2">
        <span className="text-2xl" aria-hidden>{doc.icono ?? "📄"}</span>
        {doc.paginas && (
          <span className="rounded-full bg-urrea-bg-soft px-2 py-0.5 text-[10px] font-medium text-urrea-text-muted">
            {doc.paginas} pág.
          </span>
        )}
      </div>
      <p className="mt-3 font-semibold text-urrea-text group-hover:text-urrea-primary">{doc.titulo}</p>
      <p className="mt-1 line-clamp-2 text-xs text-urrea-text-muted">{doc.descripcion}</p>
      <p className="mt-3 text-[10px] text-urrea-chrome">
        Actualizado {new Date(doc.actualizado).toLocaleDateString("es-MX")}
      </p>
    </button>
  );
}

function ProductCard({
  product,
  puntos,
  onRedeem,
  redeeming,
}: {
  product: ProductoTienda;
  puntos: number;
  onRedeem: (p: ProductoTienda) => void;
  redeeming: boolean;
}) {
  const canRedeem = puntos >= product.puntos && product.stock > 0;
  return (
    <article className="group flex flex-col overflow-hidden rounded-2xl border border-urrea-border/80 bg-urrea-bg shadow-soft transition hover:border-urrea-secondary/30 hover:shadow-soft-lg">
      <div className={cn("relative flex h-36 items-center justify-center bg-gradient-to-br", product.gradiente ?? "from-urrea-primary to-urrea-secondary")}>
        <span className="text-5xl drop-shadow-md transition group-hover:scale-110" aria-hidden>{product.icono ?? "🎁"}</span>
        {product.destacado && (
          <span className="absolute left-3 top-3 rounded-full bg-urrea-accent-sand px-2 py-0.5 text-[10px] font-bold uppercase text-urrea-primary">
            Popular
          </span>
        )}
        {product.stock < 20 && (
          <span className="absolute right-3 top-3 rounded-full bg-red-500/90 px-2 py-0.5 text-[10px] font-bold text-white">
            Últimas unidades
          </span>
        )}
      </div>
      <div className="flex flex-1 flex-col p-4">
        <p className="text-[10px] font-semibold uppercase tracking-wide text-urrea-secondary">{product.categoria}</p>
        <h3 className="mt-1 font-semibold text-urrea-text">{product.nombre}</h3>
        <div className="mt-auto flex items-end justify-between pt-4">
          <div>
            <p className="text-xl font-bold tabular-nums text-urrea-primary">{product.puntos}</p>
            <p className="text-[10px] text-urrea-text-muted">puntos URREA</p>
          </div>
          <Button
            type="button"
            disabled={!canRedeem || redeeming}
            onClick={() => onRedeem(product)}
            className="h-auto min-h-9 shrink-0 px-3 py-2 text-xs"
          >
            Canjear
          </Button>
        </div>
      </div>
    </article>
  );
}

export function BeneficiosView() {
  const [catalogo, setCatalogo] = useState<BeneficiosCatalogo | null>(null);
  const [puntos, setPuntos] = useState(0);
  const [toast, setToast] = useState("");
  const [filtroTienda, setFiltroTienda] = useState("Todos");
  const [loading, setLoading] = useState(true);
  const [redeeming, setRedeeming] = useState(false);

  const loadData = useCallback(async () => {
    const [cat, saldo] = await Promise.all([
      portalService.getBeneficiosCatalogo(2026),
      portalService.getSaldoPuntos(),
    ]);
    setCatalogo(cat);
    setPuntos(saldo.puntos);
  }, []);

  useEffect(() => {
    loadData()
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [loadData]);

  const categoriasTienda = useMemo(() => {
    if (!catalogo) return ["Todos"];
    return ["Todos", ...Array.from(new Set(catalogo.productos.map((p) => p.categoria)))];
  }, [catalogo]);

  const productosFiltrados = useMemo(() => {
    if (!catalogo) return [];
    return filtroTienda === "Todos"
      ? catalogo.productos
      : catalogo.productos.filter((p) => p.categoria === filtroTienda);
  }, [catalogo, filtroTienda]);

  async function redeem(product: ProductoTienda) {
    if (redeeming || puntos < product.puntos || product.stock <= 0) return;
    setRedeeming(true);
    try {
      const res = await portalService.canjearProducto(product.id) as { puntosRestantes: number; stockRestante: number };
      setPuntos(res.puntosRestantes);
      setCatalogo((prev) =>
        prev
          ? {
              ...prev,
              productos: prev.productos.map((p) =>
                p.id === product.id ? { ...p, stock: res.stockRestante } : p,
              ),
            }
          : prev,
      );
      setToast(`¡Canjeaste ${product.nombre}! RH confirmará la entrega.`);
      setTimeout(() => setToast(""), 4000);
    } catch (err) {
      setToast(err instanceof Error ? err.message : "No se pudo completar el canje.");
      setTimeout(() => setToast(""), 4000);
    } finally {
      setRedeeming(false);
    }
  }

  if (loading) {
    return (
      <div className="flex min-h-[40vh] items-center justify-center">
        <div className="h-8 w-8 animate-pulse rounded-full bg-urrea-secondary/30" />
      </div>
    );
  }

  if (!catalogo) {
    return (
      <div className="mx-auto max-w-6xl px-4 py-12 text-center text-sm text-urrea-text-muted">
        No se pudo cargar el catálogo de beneficios.
      </div>
    );
  }

  const corpCategories = Object.entries(catalogo.categoriaLabels);

  return (
    <PageContainer className="animate-fade-up max-w-4xl">
      <PageHeader
        title="Mis beneficios"
        subtitle="Políticas, convenios y tienda interna"
        infoTitle="Beneficios URREA"
        infoContent="Aquí encuentras documentos corporativos, convenios con proveedores y la tienda de puntos. Los puntos se acumulan por antigüedad, puntualidad y reconocimientos. Para solicitar un beneficio nuevo, ve a Mi compensación."
      />

      <div className="rounded-2xl border border-urrea-primary/15 bg-white p-4">
        <p className="text-xs font-medium uppercase tracking-wide text-urrea-text-muted">Tus puntos</p>
        <p className="text-3xl font-semibold tabular-nums text-urrea-primary">{puntos.toLocaleString("es-MX")}</p>
        <p className="mt-1 text-xs text-urrea-text-muted">Canjea en la pestaña Tienda URREA</p>
      </div>

      {toast && (
        <div className="rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm font-medium text-emerald-800">
          {toast}
        </div>
      )}

      <Tabs defaultValue="corporativo">
          <TabsList className="mb-6 w-full flex-wrap justify-start gap-1">
            <TabsTrigger value="corporativo">Consultas corporativas</TabsTrigger>
            <TabsTrigger value="convenios">Convenios y descuentos</TabsTrigger>
            <TabsTrigger value="tienda">Tienda URREA</TabsTrigger>
          </TabsList>

          <TabsContent value="corporativo" className="space-y-8">
            {corpCategories.map(([key, label]) => {
              const docs = catalogo.documentos.filter((d) => d.categoria === key);
              if (docs.length === 0) return null;
              return (
                <section key={key}>
                  <h2 className="mb-3 text-lg font-semibold text-urrea-text">{label}</h2>
                  <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
                    {docs.map((doc) => (
                      <DocCard key={doc.id} doc={doc} />
                    ))}
                  </div>
                </section>
              );
            })}

            <section>
              <h2 className="mb-3 text-lg font-semibold text-urrea-text">Días festivos 2026</h2>
              <div className="overflow-hidden rounded-2xl border border-urrea-border/80 bg-urrea-bg shadow-soft">
                <div className="overflow-x-auto">
                  <table className="w-full min-w-[480px] text-left text-sm">
                    <thead>
                      <tr className="border-b border-urrea-border bg-urrea-bg-soft/60 text-urrea-text-muted">
                        <th className="px-4 py-3 font-medium">Fecha</th>
                        <th className="px-4 py-3 font-medium">Descripción</th>
                        <th className="px-4 py-3 font-medium">Tipo</th>
                      </tr>
                    </thead>
                    <tbody>
                      {catalogo.festivos.map((d) => (
                        <tr key={d.fecha} className="border-b border-urrea-border/50 last:border-0">
                          <td className="px-4 py-3 font-medium tabular-nums text-urrea-text">
                            {new Date(d.fecha + "T12:00:00").toLocaleDateString("es-MX", {
                              weekday: "short",
                              day: "numeric",
                              month: "long",
                            })}
                          </td>
                          <td className="px-4 py-3">{d.nombre}</td>
                          <td className="px-4 py-3">
                            <span
                              className={cn(
                                "rounded-full px-2 py-0.5 text-xs font-medium",
                                d.tipo === "Oficial"
                                  ? "bg-urrea-primary/10 text-urrea-primary"
                                  : "bg-urrea-accent-sand/30 text-urrea-primary",
                              )}
                            >
                              {d.tipo}
                            </span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            </section>
          </TabsContent>

          <TabsContent value="convenios">
            <p className="mb-4 text-sm text-urrea-text-muted">
              Beneficios exclusivos para colaboradores URREA con proveedores aliados.
            </p>
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
              {catalogo.convenios.map((c) => (
                <article
                  key={c.id}
                  className="flex flex-col rounded-2xl border border-urrea-border/80 bg-urrea-bg p-5 shadow-soft transition hover:border-urrea-secondary/40 hover:shadow-soft-lg"
                >
                  <div className="flex items-start justify-between">
                    <span className="text-3xl" aria-hidden>{c.icono ?? "🤝"}</span>
                    <span className="rounded-full bg-emerald-100 px-2.5 py-1 text-xs font-bold text-emerald-800">
                      {c.descuento}
                    </span>
                  </div>
                  <h3 className="mt-3 text-lg font-semibold text-urrea-text">{c.proveedor}</h3>
                  <p className="text-xs font-medium text-urrea-secondary">{c.categoria}</p>
                  <p className="mt-2 flex-1 text-sm text-urrea-text-muted">{c.descripcion}</p>
                  <div className="mt-4 flex flex-wrap items-center justify-between gap-2 border-t border-urrea-border/60 pt-3">
                    {c.codigoPromocional ? (
                      <code className="rounded-lg bg-urrea-bg-soft px-2 py-1 text-xs font-semibold text-urrea-primary">
                        {c.codigoPromocional}
                      </code>
                    ) : (
                      <span />
                    )}
                    <span className="text-[10px] text-urrea-text-muted">Vigencia: {c.vigencia}</span>
                  </div>
                </article>
              ))}
            </div>
          </TabsContent>

          <TabsContent value="tienda">
            <div className="mb-6 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <h2 className="text-lg font-semibold text-urrea-text">Tienda interna URREA</h2>
                <p className="text-sm text-urrea-text-muted">Canjea artículos promocionales con tus puntos acumulados.</p>
              </div>
              <div className="flex items-center gap-2 rounded-2xl border border-urrea-secondary/30 bg-urrea-primary/5 px-4 py-2">
                <span className="text-sm text-urrea-text-muted">Saldo:</span>
                <span className="text-xl font-bold tabular-nums text-urrea-primary">{puntos}</span>
                <span className="text-xs text-urrea-text-muted">pts</span>
              </div>
            </div>

            <div className="mb-4 flex flex-wrap gap-2">
              {categoriasTienda.map((cat) => (
                <button
                  key={cat}
                  type="button"
                  onClick={() => setFiltroTienda(cat)}
                  className={cn(
                    "rounded-full px-4 py-2 text-sm font-medium transition",
                    filtroTienda === cat
                      ? "bg-urrea-primary text-white shadow-soft"
                      : "border border-urrea-border bg-urrea-bg text-urrea-text-muted hover:border-urrea-secondary/40",
                  )}
                >
                  {cat}
                </button>
              ))}
            </div>

            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
              {productosFiltrados.map((p) => (
                <ProductCard key={p.id} product={p} puntos={puntos} onRedeem={redeem} redeeming={redeeming} />
              ))}
            </div>
          </TabsContent>
        </Tabs>
    </PageContainer>
  );
}
