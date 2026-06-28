# URREA Hub

Plataforma integral de Recursos Humanos para URREA.

## Stack

| Capa | Tecnología |
|------|------------|
| Backend | C# .NET 7 (ASP.NET Core Web API) |
| Frontend | Next.js 16 + **Tailwind CSS 4** + TypeScript |
| Base de datos | SQL Server 2022 |

## Estructura del proyecto

```
urrea-hub/
├── UrreaHub.Domain/          # Entidades de dominio (11 módulos MVP)
├── UrreaHub.Infrastructure/  # EF Core, DbContext, migraciones
├── UrreaHub.Api/             # API REST + Swagger
├── frontend/                 # Next.js (UI)
└── docker-compose.yml        # SQL Server local
```

## Módulos MVP

1. **Core RH** — Colaborador, Puesto, Área, Departamento, Sede, Centro de costo, Relación laboral
2. **Organización** — Organigrama, Posición, Vacante, Movimiento organizacional
3. **Vacaciones/Permisos** — Política, Saldo, Solicitud, Tipo de ausencia, Calendario, Días inhábiles, Aprobación
4. **Requisiciones** — Requisición, Justificación, Presupuesto, Perfil, Aprobadores, Historial
5. **Reclutamiento** — Vacante, Publicación, Candidato, Postulación, CV, Entrevista, Evaluación, Oferta
6. **Onboarding** — Plan, Tarea, Responsable, Evidencia, Checklist, Fecha compromiso
7. **Documentos** — Expediente, Documento, Tipo, Vigencia, Firma, Versión, Confidencialidad
8. **Desempeño** — Ciclo, Objetivo, Competencia, Evaluación, Feedback, Resultado
9. **Capacitación** — Curso, Inscripción, Evidencia, Evaluación, Constancia
10. **Beneficios** — Beneficio, Solicitud, Elegibilidad, Aprobación
11. **Auditoría** — Bitácora, Cambio estado, Notificación, Integración, Error integración

Cada módulo tiene su **schema** en SQL Server (ej. `CoreRH`, `Vacaciones`, `Reclutamiento`).

## Requisitos previos

| Herramienta | Versión mínima | Notas |
|-------------|----------------|-------|
| .NET SDK | 7+ | Backend API |
| Node.js | 20+ | Frontend Next.js |
| npm | 10+ | Dependencias frontend |
| Homebrew | — | Instalación de herramientas en macOS |
| Colima | — | Motor Docker ligero (Mac Apple Silicon) |
| Docker CLI | — | Cliente Docker |
| Docker Compose | — | Plugin para levantar SQL Server |

> **Mac Apple Silicon:** se usa **Azure SQL Edge** en lugar de SQL Server 2022, porque la imagen oficial amd64 no arranca bien en ARM.

### Inicio con un solo comando

```bash
cd ~/urrea-hub
chmod +x scripts/start.sh scripts/stop.sh   # solo la primera vez
./scripts/start.sh
```

Para detener API, frontend y SQL:

```bash
./scripts/stop.sh
```

## Inicio manual

### 1. Motor Docker (solo la primera vez)

```bash
colima start --cpu 2 --memory 4 --disk 20
```

### 2. Base de datos

```bash
docker compose up -d
```

### 2. Backend

```bash
cd UrreaHub.Api
dotnet run
```

La API aplica migraciones automáticamente al iniciar.

- Swagger: http://localhost:5018/swagger
- Health: `GET /api/health`
- Módulos: `GET /api/modulos`

### 3. Frontend

```bash
cd frontend
npm run dev
```

Abre http://localhost:3000/login

### Branding y responsive

Consulta la hoja de branding en [`frontend/BRANDING.md`](frontend/BRANDING.md) (paleta URREA, referencia Grok/Starlink, reglas mobile-first). Tokens en `frontend/src/lib/branding.ts` y `globals.css`.

## Endpoints principales

| Módulo | Ejemplo |
|--------|---------|
| Core RH | `GET /api/core-rh/colaboradores` |
| Vacaciones | `GET /api/vacaciones/solicitudesausencia` |
| Reclutamiento | `GET /api/reclutamiento/vacantesreclutamiento` |
| Auditoría | `GET /api/auditoria/bitacoraeventos` |

Todos los recursos exponen CRUD básico: `GET`, `GET/{id}`, `POST`, `PUT/{id}`, `DELETE/{id}`.

## Connection string

Por defecto en `UrreaHub.Api/appsettings.json`:

```
Server=localhost,1433;Database=UrreaHub;User Id=sa;Password=UrreaHub2026!;TrustServerCertificate=True;
```

## Migraciones EF Core

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
dotnet ef migrations add NombreMigracion --project UrreaHub.Infrastructure --startup-project UrreaHub.Api
dotnet ef database update --project UrreaHub.Infrastructure --startup-project UrreaHub.Api
```

## Fase 1 — Portal colaborador + Vacaciones

### Credenciales demo (tras `./scripts/start.sh`)

| Usuario | Identificador | Rol | Password |
|---------|---------------|-----|----------|
| María López | `1003` o `maria.lopez@urrea.com` | Colaborador | `Urrea2026!` |
| Luis Martínez | `1002` o `luis.martinez@urrea.com` | Jefe | `Urrea2026!` |
| Patricia Ruiz | `1005` o `patricia.ruiz@urrea.com` | RH Admin | `Urrea2026!` |

### Rutas frontend

| Ruta | Descripción |
|------|-------------|
| http://localhost:3000/login | Inicio de sesión |
| http://localhost:3000/portal | Dashboard colaborador |
| http://localhost:3000/portal/vacaciones | Solicitudes e historial |
| http://localhost:3000/portal/aprobaciones | Cola de aprobación (jefes) |
| http://localhost:3000/rh/dashboard | Panel RH |

### Configuración (`UrreaHub.Api/appsettings.json`)

```json
"Jwt": { "Secret": "...", "ExpirationHours": 12 },
"NominaSync": { "Enabled": true, "Adapter": "Stub|Csv", "CsvPath": "Data/nomina.csv" },
"AzureAd": { "TenantId": "", "ClientId": "", "ClientSecret": "", "TeamsWebhookUrl": "" }
```

- **NominaSync.Adapter**: `Stub` (sin datos externos) o `Csv` (lee `Data/nomina.csv`)
- **AzureAd**: si se configura, envía notificaciones vía Microsoft Graph + Teams webhook; sin config usa log local

### Endpoints Fase 1

| Endpoint | Descripción |
|----------|-------------|
| `POST /api/auth/login` | Login (empleado / email / RFC + password) |
| `GET /api/portal/me` | Perfil del colaborador autenticado |
| `POST /api/vacaciones/solicitudes` | Crear solicitud de ausencia |
| `POST /api/vacaciones/solicitudes/{id}/aprobar` | Aprobar (jefe/RH) |
| `GET /api/rh/dashboard` | Dashboard RH |
| `GET /api/rh/reportes/ausencias?format=csv` | Exportar reporte |

## Próximos pasos sugeridos

- Integración real con sistema de nómina (adapter específico)
- SSO Microsoft Entra ID
- Módulos Fase 2 (Requisiciones, Reclutamiento, etc.)
- Carga de archivos (CV, documentos, evidencias)
