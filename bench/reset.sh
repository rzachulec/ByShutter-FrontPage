#!/usr/bin/env bash
# Resets the ByShutter stack to a clean state and (re)builds the API image.
#   - recreates the containers: Postgres has no volume in compose.yaml, so the DB is emptied
#   - removes ONLY the benchmark files this suite generates in site/ (sample-*, originals/)
#   - starts Postgres first and waits for it (the API crashes at startup if Postgres is not
#     ready yet, because it runs migrations immediately and compose has no depends_on/healthcheck)
# It never touches other Docker projects, volumes or git-tracked files.
set -euo pipefail
cd "$(dirname "$0")/.."

docker compose down --remove-orphans

# guard: only delete generated, untracked benchmark files
if git ls-files --error-unmatch -- site/sample-* >/dev/null 2>&1 || [ -n "$(git ls-files site/originals)" ]; then
  echo "refusing to delete: git tracks files matching site/sample-* or site/originals" >&2
  exit 1
fi
rm -rf site/originals
rm -f site/sample-*

docker compose up -d postgres caddy
until docker exec postgres pg_isready -U byshutter >/dev/null 2>&1; do sleep 1; done

docker compose up -d --build byshutterbackend.web
until curl -fs http://localhost:5700/health >/dev/null; do sleep 1; done
echo "stack ready: API :5700, Caddy :5701/:5702, Postgres :5432 (empty DB)"
