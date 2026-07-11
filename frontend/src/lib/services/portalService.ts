import { fetchApi } from "@/lib/api";
import { v1 } from "@/lib/api/v1";
import type {
  BeneficiosCatalogo,
  ColaboradorPerfil,
  FeedComment,
  FeedPost,
  ModuloPortal,
  PortalAdminEstado,
  ToggleReaccionResult,
} from "@/lib/types";

export const portalService = {
  getMe: () => fetchApi<ColaboradorPerfil>(v1("/portal/me")),

  getFeed: () => fetchApi<FeedPost[]>(v1("/portal/feed")),

  createPost: (contenido: string, tipo: "General" | "Recognition" = "General", gradienteImagen?: string) =>
    fetchApi<FeedPost>(v1("/portal/feed"), {
      method: "POST",
      body: JSON.stringify({ contenido, tipo, gradienteImagen }),
    }),

  deletePost: (postId: string) => fetchApi<void>(v1(`/portal/feed/${postId}`), { method: "DELETE" }),

  toggleLike: (postId: string) =>
    fetchApi<ToggleReaccionResult>(v1(`/portal/feed/${postId}/like`), { method: "POST" }),

  getComments: (postId: string) => fetchApi<FeedComment[]>(v1(`/portal/feed/${postId}/comentarios`)),

  addComment: (postId: string, contenido: string) =>
    fetchApi<FeedComment>(v1(`/portal/feed/${postId}/comentarios`), {
      method: "POST",
      body: JSON.stringify({ contenido }),
    }),

  getModule: (codigo: string) => fetchApi<ModuloPortal>(v1(`/portal/modulos/${codigo}`)),

  getBeneficiosCatalogo: (anio = 2026) =>
    fetchApi<BeneficiosCatalogo>(v1(`/portal/beneficios/catalogo?anio=${anio}`)),

  getSaldoPuntos: () => fetchApi<{ puntos: number }>(v1("/portal/beneficios/tienda/saldo")),

  canjearProducto: (productoId: string) =>
    fetchApi(v1("/portal/beneficios/tienda/canjear"), {
      method: "POST",
      body: JSON.stringify({ productoId }),
    }),
};

export const portalAdminService = {
  getStatus: () => fetchApi<PortalAdminEstado>(v1("/rh/admin/portal/status")),

  getPosts: () => fetchApi<FeedPost[]>(v1("/rh/admin/portal/posts")),

  createPost: (body: unknown) =>
    fetchApi<FeedPost>(v1("/rh/admin/portal/posts"), {
      method: "POST",
      body: JSON.stringify(body),
    }),

  deletePost: (id: string) =>
    fetchApi(v1(`/rh/admin/portal/posts/${id}`), { method: "DELETE" }),

  getModules: () => fetchApi<ModuloPortal[]>(v1("/rh/admin/portal/modulos")),

  updateModule: (body: unknown) =>
    fetchApi<ModuloPortal>(v1("/rh/admin/portal/modulos"), {
      method: "PUT",
      body: JSON.stringify(body),
    }),
};
