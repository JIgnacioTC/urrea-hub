# 🚀 Guía de Despliegue — URREA Hub

## Arquitectura de producción

```
[ Usuario ] ──▶ [ Vercel ] ──▶ (Next.js frontend)
                    │
                    │ HTTPS + Bearer Token
                    ▼
              [ Railway ] ──▶ (.NET 7 Backend API, Dockerfile)
                    │
                    ▼
              [ Railway PostgreSQL ]
```

- **Frontend**: Next.js en Vercel.
- **Backend**: API .NET 7 en Railway, desplegada vía el [Dockerfile](Dockerfile) del repo.
- **Base de datos**: PostgreSQL, como servicio administrado dentro del mismo proyecto de Railway.

---

## PARTE 1 — Backend + Base de datos en Railway

### 1. Crear el proyecto

1. Ir a [railway.app](https://railway.app) → **New Project → Deploy from GitHub repo**
2. Seleccionar el repositorio `urrea-hub`
3. Railway detecta el [Dockerfile](Dockerfile) automáticamente y lo usa para el build (no hace falta configurar buildpacks)

> [!IMPORTANT]
> El Dockerfile usa imágenes `dotnet/sdk:7.0-alpine` y `dotnet/aspnet:7.0-alpine` — deben coincidir con el `TargetFramework` (`net7.0`) de los 4 proyectos del `.sln`. Si en algún momento se actualiza el proyecto a otra versión de .NET, hay que actualizar el Dockerfile en el mismo cambio, o el contenedor arranca con el error `You must install or update .NET to run this application` y nunca llega a conectarse a la base de datos (parece un problema de DB pero no lo es).

### 2. Añadir el servicio de PostgreSQL

En el mismo proyecto de Railway → **New Service → Database → PostgreSQL**

Railway provisiona el servicio y expone automáticamente estas variables en ese servicio: `DATABASE_URL`, `DATABASE_PUBLIC_URL`, `PGHOST`, `PGPORT`, `PGUSER`, `PGPASSWORD`, `PGDATABASE`.

> [!WARNING]
> `DATABASE_URL` viene en formato URI (`postgres://user:pass@host:port/db`). El backend usa **Npgsql**, que espera el formato clásico `Host=...;Port=...;Database=...`. **No se puede pegar `DATABASE_URL` directamente** en la variable de conexión del backend — hay que armarla a partir de las variables `PG*` (ver paso siguiente). Si se pega `DATABASE_URL` tal cual, el backend crashea al arrancar con `System.ArgumentException: Host can't be null` y nunca corre las migraciones, lo que se manifiesta como "no aparecen tablas" en el panel de Postgres.

### 3. Variables de entorno del servicio backend

En el servicio del backend (el que corre el Dockerfile) → **Variables**, configurar:

| Variable | Valor / descripción |
|---|---|
| `ConnectionStrings__UrreaHubDb` | Ver ejemplo abajo — connection string Npgsql armada con referencias a las variables del servicio Postgres |
| `Frontend__Url` | URL de Vercel, ej. `https://urrea-hub.vercel.app,http://localhost:3000` (acepta varios orígenes separados por coma) |
| `Jwt__Secret` | Cadena aleatoria segura de 32+ caracteres |
| `Jwt__Issuer` | `UrreaHub` |
| `Jwt__Audience` | `UrreaHub` |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

Opcionales (dejar vacíos si no aplica todavía):

| Variable | Descripción |
|---|---|
| `AzureAd__TenantId` / `AzureAd__ClientId` / `AzureAd__ClientSecret` | Solo si se activa el envío de notificaciones por Microsoft Graph. Sin esto, el backend usa un `LoggingNotificationSender` que solo loguea (no falla). |
| `NominaSync__Enabled` | `false` por defecto en producción; no requiere configuración extra a menos que se conecte un adaptador real. |

> [!TIP]
> El `__` (doble guion bajo) es cómo .NET mapea variables de entorno a configuración jerárquica: `ConnectionStrings__UrreaHubDb` equivale a `ConnectionStrings:UrreaHubDb` en `appsettings.json`.

### Connection string de ejemplo (Railway PostgreSQL)

```
Host=${{Postgres.PGHOST}};Port=${{Postgres.PGPORT}};Database=${{Postgres.PGDATABASE}};Username=${{Postgres.PGUSER}};Password=${{Postgres.PGPASSWORD}};SSL Mode=Require;Trust Server Certificate=True
```

Railway interpola `${{NombreDelServicio.VARIABLE}}` con el valor real de otro servicio del mismo proyecto en tiempo de deploy. Ajustá `Postgres` al nombre exacto que tenga ese servicio en tu proyecto (aparece en el panel lateral).

### 4. Puerto

El [Dockerfile](Dockerfile) fija `ASPNETCORE_URLS=http://+:8080` y `EXPOSE 8080`. Railway lo detecta y expone el servicio automáticamente — no requiere configuración manual de puerto.

---

## PARTE 2 — Frontend en Vercel

### 1. Conectar el repositorio

1. Ir a [vercel.com](https://vercel.com) → **New Project**
2. Importar el repositorio de GitHub
3. En **"Root Directory"** escribir: `frontend`
4. Framework: **Next.js** (se detecta automático — ya hay un [vercel.json](frontend/vercel.json) en el repo)

### 2. Variables de entorno

En Vercel → **Settings → Environment Variables**, añadir:

| Variable | Valor |
|---|---|
| `NEXT_PUBLIC_API_URL` | URL pública del backend en Railway, ej. `https://urrea-hub-production.up.railway.app` (sin slash al final) |

> [!IMPORTANT]
> Es la única variable requerida. Sin ella el frontend usa el fallback `http://localhost:5018` (ver [next.config.ts](frontend/next.config.ts)) y falla en producción.

### 3. Deploy

Clic en **Deploy**. Vercel corre `npm install && npm run build` automáticamente. Cada push a `main` re-despliega; los PRs generan previews.

---

## PARTE 3 — Conectar ambos lados (CORS)

Una vez que Vercel te da la URL final del frontend (ej. `https://urrea-hub.vercel.app`), volvé a Railway → servicio del backend → **Variables** → actualizá `Frontend__Url` para incluirla:

```
Frontend__Url=https://urrea-hub.vercel.app,http://localhost:3000
```

El backend arma la política de CORS a partir de esta variable en tiempo de arranque (`Program.cs`, política `"Frontend"`), separando por coma. Si esta variable no incluye la URL exacta de Vercel, el navegador bloqueará las llamadas con error de CORS.

---

## PARTE 4 — Verificación post-despliegue

### Checklist

- [ ] El deploy del backend en Railway está `Active` sin reinicios en loop (revisar **Deployments → logs**)
- [ ] En los logs del backend aparecen las líneas `MigrateAsync` sin reintentos infinitos, y luego `Now listening on: http://+:8080`
- [ ] En Postgres (**Database → Data**) ya aparecen tablas — si sigue diciendo "You have no tables", el backend nunca migró (ver Troubleshooting)
- [ ] `https://TU_BACKEND.railway.app/api/health` responde `{"status":"healthy",...}`
- [ ] `https://TU_BACKEND.railway.app/swagger` carga
- [ ] Frontend carga en la URL de Vercel y el login funciona (probar con una credencial demo si ya corrieron los seeders)
- [ ] Sin errores de CORS en la consola del navegador (DevTools → Network → cualquier llamada a `/api/...`)

### Credenciales demo (si los seeders ya corrieron)

| Usuario | Identificador | Rol | Password |
|---|---|---|---|
| Patricia Ruiz | `patricia.ruiz@urrea.com` | RH Admin | `Urrea2026!` |
| María López | `maria.lopez@urrea.com` | Colaborador | `Urrea2026!` |

---

## Troubleshooting

Problemas reales encontrados al desplegar esta app y cómo se resolvieron:

### "You must install or update .NET to run this application"

En los logs del backend aparece algo como:
```
Framework: 'Microsoft.NETCore.App', version '7.0.0' (x64)
The following frameworks were found:
  8.0.28 at [/usr/share/dotnet/shared/Microsoft.NETCore.App]
```
Esto **no es un problema de base de datos** aunque a veces se confunda con uno (el síntoma visible es que nunca aparecen tablas, porque el proceso muere antes de llegar a `Database.MigrateAsync()`). Significa que el runtime de la imagen Docker no coincide con el `TargetFramework` de los proyectos. Ya está resuelto en este repo (Dockerfile fijado a `7.0-alpine`), pero si se actualiza el `TargetFramework` de los `.csproj` en el futuro, hay que actualizar el Dockerfile en el mismo cambio.

### "You have no tables" en el panel de Postgres de Railway

Significa que el backend nunca corrió exitosamente `Database.MigrateAsync()` (que se ejecuta automáticamente al arrancar, ver `Program.cs`). Revisar en este orden:
1. **Logs del backend** (Deployments → logs del último deploy) — ahí va a estar la excepción real.
2. **`ConnectionStrings__UrreaHubDb`** — confirmar que no sea `DATABASE_URL` pegado directo (ver advertencia en Parte 1, paso 2) y que las referencias `${{Postgres.XXX}}` apunten al nombre real del servicio Postgres.
3. Una vez corregida la variable, Railway redeploya solo; si no, forzar un redeploy manual.

### Errores de CORS en el navegador

`Frontend__Url` en el backend no incluye la URL exacta de Vercel (con `https://`, sin slash final). Ver Parte 3.

---

## Alternativa: Azure

Si en vez de Railway se prefiere Azure para el backend:

1. Crear **Azure App Service** (Linux, contenedor personalizado apuntando al mismo Dockerfile) o **Azure Database for PostgreSQL** como servicio administrado.
2. En **Configuration → Application Settings** añadir las mismas variables que en Railway (Parte 1, paso 3).
3. Connection string de ejemplo:

```
Host=tu-server.postgres.database.azure.com;Port=5432;Database=UrreaHub;Username=admin;Password=TU_PASSWORD;SSL Mode=Require;Trust Server Certificate=True
```

---

## Archivos relevantes

| Archivo | Descripción |
|---|---|
| [Dockerfile](Dockerfile) | Build multi-stage del backend (.NET 7 SDK → runtime alpine) |
| [.dockerignore](.dockerignore) | Exclusiones del build Docker |
| [UrreaHub.Api/appsettings.Production.json](UrreaHub.Api/appsettings.Production.json) | Template de config de producción — todos los valores vacíos se inyectan por variables de entorno |
| [UrreaHub.Api/Program.cs](UrreaHub.Api/Program.cs) | Migración automática al arrancar, CORS multi-origen, switch de compatibilidad de timestamps de Npgsql |
| [frontend/vercel.json](frontend/vercel.json) | Config de Vercel |
| [frontend/next.config.ts](frontend/next.config.ts) | Config Next.js — expone `NEXT_PUBLIC_API_URL` al bundle del navegador |
| [frontend/.env.production.example](frontend/.env.production.example) | Referencia de variables de entorno para Vercel |
