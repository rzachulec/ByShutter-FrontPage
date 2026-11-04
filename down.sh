#!/usr/bin/env bash
# Stops the client and the backend containers.
#   ./down.sh          stop only: containers and the database are kept, ./up.sh resumes where you left off
#   ./down.sh --wipe   remove the containers too. Postgres has no volume, so this EMPTIES THE DATABASE
#                      (files in site/ are not touched)
set -euo pipefail
cd "$(dirname "$0")"

step() { printf '\n\033[1m▸ %s\033[0m\n' "$*"; }
WIPE=0
case "${1:-}" in "") ;; --wipe) WIPE=1 ;; *) echo "usage: $0 [--wipe]" >&2; exit 1 ;; esac

step "Client"
pids=$(lsof -nP -tiTCP:5173 -sTCP:LISTEN 2>/dev/null || true)
if [ -z "$pids" ]; then echo "not running"; fi
for p in $pids; do
  if ps -o command= -p "$p" | grep -q vite; then kill "$p" && echo "stopped vite (pid $p)"
  else echo "port 5173 is held by a non-vite process ($p), leaving it"; fi
done

step "Backend"
if ! docker info >/dev/null 2>&1; then
  echo "Docker is not running, nothing to stop"
elif [ "$WIPE" = 1 ]; then
  docker compose down --remove-orphans
  echo "containers removed, database is now empty"
else
  docker compose stop
  echo "stopped (data kept; ./up.sh resumes)"
fi
