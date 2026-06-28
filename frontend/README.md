# Frontend — Next.js + Tailwind CSS

Portal URREA Hub construido con **Next.js 16** y **Tailwind CSS 4**.

## Tailwind CSS

| Archivo | Rol |
|---------|-----|
| [`postcss.config.mjs`](postcss.config.mjs) | Plugin `@tailwindcss/postcss` |
| [`src/app/globals.css`](src/app/globals.css) | `@import "tailwindcss"` + tokens `@theme` URREA |
| [`src/lib/utils.ts`](src/lib/utils.ts) | `cn()` para combinar clases Tailwind |
| [`src/components/ui/`](src/components/ui/) | Componentes con utilidades Tailwind |

### Colores de marca (clases Tailwind)

```
bg-urrea-primary      #023764
bg-urrea-secondary    #2E7FA8
bg-urrea-bg           #FFFFFF
bg-urrea-bg-soft      #F5F6F7
text-urrea-text       #1A1A1A
text-urrea-text-muted #6F7478
border-urrea-border    #D9DDE1
```

Sombras: `shadow-soft`, `shadow-soft-lg`, `shadow-glow`

### Reglas de estilo

1. **Solo Tailwind** en componentes y páginas (`className="..."`).
2. **No CSS modules** ni estilos inline salvo casos excepcionales (p. ej. color dinámico en calendario).
3. Tokens de marca en `globals.css` → usar como `bg-urrea-primary`, no hex sueltos.
4. Componentes reutilizables en `components/ui/` (`Button`, `Input`, `Card`…).
5. Responsive con prefijos `sm:`, `md:`, `lg:` (mobile-first).

### Ejemplo

```tsx
import { Button, ButtonLink } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { PageContainer, PageHeader } from "@/components/ui/page-header";

export default function MiPagina() {
  return (
    <PageContainer>
      <PageHeader title="Título" subtitle="Subtítulo" />
      <Card title="Sección" className="grid gap-4 sm:grid-cols-2">
        <Button>Acción</Button>
        <ButtonLink href="/portal" variant="secondary">Volver</ButtonLink>
      </Card>
    </PageContainer>
  );
}
```

Branding completo: [`BRANDING.md`](BRANDING.md)

## Desarrollo

```bash
npm run dev    # http://localhost:3000
npm run build
```

Variables: `.env.local` → `NEXT_PUBLIC_API_URL=http://localhost:5018`
