#!/usr/bin/env bash
# Starts everything: Docker (if needed), Postgres, Caddy, the API, and the Vue client (vite dev on :5173).
# Safe to re-run: already-running parts are left alone, the API image is rebuilt only if the code changed.
set -euo pipefail
cd "$(dirname "$0")"

step() { printf '\n\033[1m▸ %s\033[0m\n' "$*"; }
# wait_for "<what>" <seconds> <command...>
wait_for() {
  local what=$1 limit=$2; shift 2
  for ((i = 0; i < limit; i++)); do "$@" >/dev/null 2>&1 && return 0; sleep 1; done
  echo "timed out waiting for $what" >&2; exit 1
}

if ! docker info >/dev/null 2>&1; then
  step "Starting Docker Desktop"
  command -v open >/dev/null && open -a Docker || { echo "Docker is not running, start it and retry" >&2; exit 1; }
  wait_for "Docker" 180 docker info
fi

step "Postgres + Caddy"
docker compose up -d postgres caddy
# the API runs migrations at startup and crashes if Postgres is not accepting connections yet
wait_for "Postgres" 60 docker exec postgres pg_isready -U byshutter

step "API (builds only if the code changed)"
docker compose up -d --build byshutterbackend.web
wait_for "API health" 90 curl -fs http://localhost:5700/health

step "Client"
if lsof -nP -iTCP:5173 -sTCP:LISTEN >/dev/null 2>&1; then
  echo "something already listens on :5173, leaving it (run ./down.sh first to restart it)"
else
  [ -d ByShutterPage/node_modules ] || (cd ByShutterPage && npm ci --no-audit --no-fund)
  mkdir -p .run
  (cd ByShutterPage && nohup npm run dev > ../.run/client.log 2>&1 &)
  wait_for "client" 60 curl -fs -o /dev/null http://localhost:5173/
fi

cat <<MSG

Everything is up
  site      http://localhost:5173
  admin     http://localhost:5173/config      (admin / admin)
  API       http://localhost:5700/swagger
  photos    https://localhost:5702            (self-signed; the client reaches it via /media)
  client log: .run/client.log        stop everything: ./down.sh
MSG
