#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
RUN_DIR="$ROOT_DIR/.run"
API_PID_FILE="$RUN_DIR/api.pid"
FRONTEND_PID_FILE="$RUN_DIR/frontend.pid"

stop_process() {
  local name="$1"
  local pid_file="$2"

  if [[ ! -f "$pid_file" ]]; then
    echo "==> $name no estaba registrado como activo"
    return 0
  fi

  local pid
  pid="$(cat "$pid_file")"

  if kill -0 "$pid" >/dev/null 2>&1; then
    echo "==> Deteniendo $name (PID $pid)..."
    kill "$pid" >/dev/null 2>&1 || true

    for _ in {1..10}; do
      if ! kill -0 "$pid" >/dev/null 2>&1; then
        break
      fi
      sleep 1
    done

    if kill -0 "$pid" >/dev/null 2>&1; then
      kill -9 "$pid" >/dev/null 2>&1 || true
    fi
  else
    echo "==> $name ya no estaba corriendo"
  fi

  rm -f "$pid_file"
}

echo "==> URREA Hub - detención"

stop_process "API" "$API_PID_FILE"
stop_process "Frontend" "$FRONTEND_PID_FILE"

if command -v docker >/dev/null 2>&1; then
  echo "==> Deteniendo contenedor SQL..."
  cd "$ROOT_DIR"
  docker compose stop >/dev/null 2>&1 || true
fi

echo
echo "Servicios de aplicación detenidos."
echo "Colima sigue activo. Para apagarlo: colima stop"
