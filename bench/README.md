# ByShutter benchmark harness

Scripts behind `Docs/raport-2`. They measure the running stack (Caddy + API + Postgres + `ByShutterPage`)
before and after an optimisation, using the same data and the same procedure.

## Prerequisites

- Docker Desktop, Node 22+ and a Chrome/Chromium binary (`CHROME_PATH=/path/to/chrome`).
- `cd bench && npm ci` (Lighthouse, chrome-launcher).
- `cd ByShutterPage && npm ci && npm run build` (the client under test).

## Procedure

```bash
# 1. clean slate: recreates the containers (empty DB), removes generated site/sample-* and site/originals,
#    rebuilds + starts the API after Postgres is ready
bench/reset.sh

# 2. serve the built client on :5173 (the origin the API's CORS policy allows)
(cd ByShutterPage && npx vite preview)

# 3. upload the large sample photos from site/ and create a home profile, 4 tiles and an album
node bench/seed.mjs

# 4. measure (Lighthouse x 5 runs per page x network profile, plus unthrottled curl timings)
CHROME_PATH=... node bench/measure.mjs <label>        # -> bench/results/<label>.json

# 5. compare two labels (Typst table fragments, or --md for a quick overview)
node bench/summarize.mjs baseline optimised --md
```

## What is measured

| Where | What |
|---|---|
| Lighthouse (applied throttling, cold cache, fresh Chrome per run) | TTFB, FCP, LCP (+ phase breakdown), CLS, Speed Index, page weight, requests, fully-loaded time |
| Profiles | **4G**: mobile emulation, 9 Mbps / 170 ms RTT, 4x CPU slowdown. **Cable**: desktop, 5 Mbps / 28 ms RTT |
| Pages | `/home` and `/albums/<id>` of the client |
| curl (unthrottled localhost) | per-asset bytes and time-to-last-byte, API response times, response headers |

`load` (the browser's load event) is recorded but is not meaningful for this SPA: it fires before the app has
fetched its data and images. "Fully loaded" (last network activity) is the comparable figure.

## Notes

- `seed.mjs` uploads the originals from `site/` under new names (`sample-NN.*`), so git-tracked files are never overwritten.
- WebPageTest (named in report-1) cannot reach `localhost` and was not used; see the report for the rationale.
- The measurement results are committed in `results/`; raw Lighthouse JSON is not (it is large). Use `DUMP=<dir>` to keep it.
