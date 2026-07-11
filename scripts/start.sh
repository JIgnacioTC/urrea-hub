#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
RUN_DIR="$ROOT_DIR/.run"
LOG_DIR="$RUN_DIR/logs"
API_PID_FILE="$RUN_DIR/api.pid"
FRONTEND_PID_FILE="$RUN_DIR/frontend.pid"

export PATH="$PATH:$HOME/.dotnet/tools"

mkdir -p "$LOG_DIR"

require_command() {
  if ! command -v "$1" >/dev/null 2>&1; then
    echo "Error: no se encontró '$1'. Instálalo antes de continuar." >&2
    exit 1
  fi
}

is_running() {
  local pid_file="$1"
  if [[ -f "$pid_file" ]]; then
    local pid
    pid="$(cat "$pid_file")"
    if kill -0 "$pid" >/dev/null 2>&1; then
      return 0
    fi
    rm -f "$pid_file"
  fi
  return 1
}

wait_for_url() {
  local url="$1"
  local label="$2"
  local attempts="${3:-60}"

  echo "Esperando $label..."
  for ((i = 1; i <= attempts; i++)); do
    if curl -fsS "$url" >/dev/null 2>&1; then
      echo "$label listo."
      return 0
    fi
    sleep 2
  done

  echo "Error: $label no respondió a tiempo ($url)." >&2
  if [[ "$label" == "API" ]]; then
    print_api_log_tail
  fi
  exit 1
}

wait_for_postgres_healthy() {
  local attempts="${1:-90}"
  echo "Esperando PostgreSQL (healthcheck)..."
  for ((i = 1; i <= attempts; i++)); do
    local status
    status="$(docker inspect --format='{{if .State.Health}}{{.State.Health.Status}}{{else}}unknown{{end}}' urrea-hub-postgres 2>/dev/null || echo "missing")"
    if [[ "$status" == "healthy" ]]; then
      echo "PostgreSQL listo."
      return 0
    fi
    sleep 2
  done
  echo "Error: PostgreSQL no alcanzó estado healthy a tiempo." >&2
  echo "Revisa: docker logs urrea-hub-postgres" >&2
  exit 1
}

print_api_log_tail() {
  if [[ -f "$LOG_DIR/api.log" ]]; then
    echo "--- Últimas líneas de api.log ---" >&2
    tail -n 30 "$LOG_DIR/api.log" >&2
  fi
}

echo "==> URREA Hub - inicio"
echo "Proyecto: $ROOT_DIR"

require_command colima
require_command docker
require_command dotnet
require_command npm
require_command curl

if ! colima status >/dev/null 2>&1; then
  echo "==> Iniciando Colima..."
  colima start --cpu 2 --memory 4 --disk 20
else
  echo "==> Colima ya está activo"
fi

echo "==> Levantando PostgreSQL..."
cd "$ROOT_DIR"
docker compose up -d
wait_for_postgres_healthy 60

stop_if_running() {
  local name="$1"
  local pid_file="$2"
  if [[ -f "$pid_file" ]]; then
    local pid
    pid="$(cat "$pid_file")"
    if kill -0 "$pid" >/dev/null 2>&1; then
      echo "==> Deteniendo $name (PID $pid) para aplicar cambios..."
      kill "$pid" >/dev/null 2>&1 || true
      for _ in {1..10}; do
        kill -0 "$pid" >/dev/null 2>&1 || break
        sleep 1
      done
      kill -9 "$pid" >/dev/null 2>&1 || true
    fi
    rm -f "$pid_file"
  fi
}

api_sources_changed() {
  [[ ! -f "$API_PID_FILE" ]] && return 1
  find "$ROOT_DIR/UrreaHub.Api" "$ROOT_DIR/UrreaHub.Application" "$ROOT_DIR/UrreaHub.Infrastructure" "$ROOT_DIR/UrreaHub.Domain" \
    -name "*.cs" -newer "$API_PID_FILE" 2>/dev/null | grep -q .
}

if is_running "$API_PID_FILE" && api_sources_changed; then
  stop_if_running "API" "$API_PID_FILE"
fi

if is_running "$API_PID_FILE"; then
  echo "==> API ya está corriendo (PID $(cat "$API_PID_FILE"))"
else
  echo "==> Iniciando API..."
  cd "$ROOT_DIR/UrreaHub.Api"
  nohup dotnet run --launch-profile http >"$LOG_DIR/api.log" 2>&1 &
  echo $! >"$API_PID_FILE"
  wait_for_url "http://localhost:5018/api/health" "API" 90
fi

if is_running "$FRONTEND_PID_FILE"; then
  echo "==> Frontend ya está corriendo (PID $(cat "$FRONTEND_PID_FILE"))"
else
  echo "==> Iniciando frontend..."
  cd "$ROOT_DIR/frontend"
  nohup npm run dev -- --port 3000 >"$LOG_DIR/frontend.log" 2>&1 &
  echo $! >"$FRONTEND_PID_FILE"
  wait_for_url "http://localhost:3000" "Frontend"
fi

cat <<EOF

URREA Hub está arriba.

  Frontend:  http://localhost:3000
  API:       http://localhost:5018
  Swagger:   http://localhost:5018/swagger
  Postgres:  localhost:5432

Logs:
  $LOG_DIR/api.log
  $LOG_DIR/frontend.log

Para detener:
  ./scripts/stop.sh

EOF
