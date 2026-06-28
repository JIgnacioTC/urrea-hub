# URREA Hub — Hoja de branding

Referencia oficial de color, tono visual y principios de UI para el portal y futuros módulos.

## Stack frontend (Next.js)

| Capa | Tecnología |
|------|------------|
| Framework | **Next.js 16** (App Router, RSC + Client Components) |
| Estilos | **Tailwind CSS 4** + tokens CSS `@theme` |
| Tipografía | `next/font` (Geist) |
| Componentes | `src/components/ui/` (Button, Input, Card, Badge…) |
| Utilidades | `cn()` en `src/lib/utils.ts` (clsx + tailwind-merge) |

Patrón: páginas en `app/`, interactividad con `"use client"`, estilos vía componentes UI + tokens URREA.

## Paleta de color

| Uso | Color | Hex |
|-----|-------|-----|
| Azul institucional URREA | Azul profundo | `#023764` |
| Azul secundario / agua | Azul medio | `#2E7FA8` |
| Fondo principal | Blanco | `#FFFFFF` |
| Fondo suave | Gris muy claro | `#F5F6F7` |
| Texto principal | Negro / carbón | `#1A1A1A` |
| Texto secundario | Gris medio | `#6F7478` |
| Líneas / divisores | Gris claro | `#D9DDE1` |
| Acento de interiores | Beige / arena | `#D8C7AE` |
| Materiales visuales | Cromo / plata | `#C8D0D6` |

### Tokens CSS

Definidos en [`src/app/globals.css`](src/app/globals.css) como `--urrea-*`.  
Constantes TypeScript en [`src/lib/branding.ts`](src/lib/branding.ts).

### Uso recomendado

- **Primario (`#023764`)**: botones principales, header móvil, ítems activos en navegación.
- **Secundario (`#2E7FA8`)**: links, estados informativos, iconografía activa.
- **Fondo suave (`#F5F6F7`)**: canvas de página (no blanco puro en pantallas completas).
- **Beige (`#D8C7AE`)**: acentos decorativos sutiles, badges premium, highlights de sección.
- **Cromo (`#C8D0D6`)**: bordes suaves, placeholders visuales, fondos de iconos.

---

## Referencia de estilo: Grok vs Starlink

### Grok (xAI)

- **Modo oscuro dominante**, alto contraste, sensación “espacial” y futurista.
- **Mínimo chrome**: poca ornamentación; el contenido manda.
- **Tipografía clara**, jerarquía simple (título → cuerpo → metadata).
- **Bordes redondeados** generosos, transiciones suaves.
- **Mobile-first**: una columna, targets táctiles amplios, navegación compacta.
- **Lección para URREA**: simplicidad, foco en la acción principal, sin saturar la pantalla.

### Starlink (SpaceX)

- **Modo claro**, funcional, estética de ingeniería (confiable, preciso).
- **Cards con bordes sutiles** sobre fondo claro; mucho aire entre bloques.
- **Codificación por color** para estados (azul = OK, rojo = alerta) — inmediato y legible.
- **Navegación inequívoca**: menú claro, iconos + etiquetas, flujos lineales.
- **Mobile nativo**: bottom navigation, gestos, estadísticas legibles en pantalla pequeña.
- **Lección para URREA**: claridad operativa, feedback visual de estado, confianza corporativa.

### Síntesis para URREA Hub

| Principio | Origen | Aplicación |
|-----------|--------|------------|
| Claridad sobre decoración | Starlink | Cards planas, pocos gradientes |
| Una acción principal por pantalla | Grok | CTA destacado en vacaciones/solicitudes |
| Fondo claro + acento azul profundo | URREA + Starlink | Identidad institucional |
| Bottom nav en móvil | Starlink | Portal colaborador |
| Targets ≥ 44px | Ambos | Botones, links, nav |
| Safe area (notch) | iOS HIG | `env(safe-area-inset-*)` |
| Tablas → scroll horizontal | Mobile UX | Reportes e historial |

---

## Responsive — reglas obligatorias

1. **Mobile-first**: diseñar primero para 320–428px; escalar a tablet/desktop.
2. **Navegación móvil**: barra inferior fija en portal; sidebar solo en `md+`.
3. **Touch targets**: mínimo 44×44px en botones y enlaces de navegación.
4. **Tipografía**: títulos `text-xl` móvil → `text-2xl` desktop; cuerpo mínimo 16px en inputs (evita zoom iOS).
5. **Espaciado**: padding página `px-4 py-4` móvil → `p-6` desktop.
6. **Tablas**: envolver en `overflow-x-auto`; considerar cards apiladas en `< sm`.
7. **Formularios**: una columna en móvil; dos columnas solo en `md+`.
8. **Sin hover-only**: toda acción debe ser accesible por tap.

---

## Componentes base

| Componente | Archivo |
|------------|---------|
| Shell portal / RH | `src/components/layout/Shells.tsx` |
| Tokens | `src/lib/branding.ts` |
| Estilos globales | `src/app/globals.css` |
