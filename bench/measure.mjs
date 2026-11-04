// Measures the running stack and writes bench/results/<label>.json
//
//   CHROME_PATH=/path/to/chrome node measure.mjs baseline
//
// Env: RUNS=5            Lighthouse runs per page x profile (report-1 asks for >= 5, median)
//      ONLY=4g/home      run a single "<profile>/<page>" combination
//      DUMP=/some/dir    also save the full Lighthouse JSON of every run (can be large)
//
// What is measured
//   1. Lighthouse (performance category), cold cache, *applied* network + CPU throttling that mirrors
//      the WebPageTest profiles named in report-1:  4G = 9 Mbps / 170 ms RTT (mobile emulation, 4x CPU),
//      Cable = 5 Mbps / 28 ms RTT (desktop).  A fresh Chrome is launched for every run.
//   2. "delivery": unthrottled localhost timing of every asset with curl (server-side cost only).
//   3. bytes on disk of the uploaded files, upload times recorded by seed.mjs, response headers.
import lighthouse from 'lighthouse';
import * as chromeLauncher from 'chrome-launcher';
import { execFileSync } from 'node:child_process';
import { mkdirSync, readFileSync, readdirSync, statSync, writeFileSync } from 'node:fs';
import { join } from 'node:path';
import { fileURLToPath } from 'node:url';

const label = process.argv[2];
if (!label) { console.error('usage: node measure.mjs <label>'); process.exit(1); }

const HERE = fileURLToPath(new URL('.', import.meta.url));
const SITE = join(HERE, '..', 'site');
const RUNS = Number(process.env.RUNS ?? 5);
const ONLY = process.env.ONLY;
const DUMP = process.env.DUMP;
const PAGE = 'http://localhost:5173';
const API = 'http://localhost:5700';
const IMAGES = 'https://localhost:5702';
const state = JSON.parse(readFileSync(join(HERE, '.state.json'), 'utf8'));

const PAGES = { home: '/home', album: `/albums/${state.albumId}` };
const PROFILES = {
  '4g': {
    desc: '4G, mobile emulation (9 Mbps down, 170 ms RTT, 4x CPU slowdown)',
    formFactor: 'mobile',
    screenEmulation: { mobile: true, width: 412, height: 823, deviceScaleFactor: 1.75, disabled: false },
    throttling: { rttMs: 170, throughputKbps: 9000, requestLatencyMs: 170, downloadThroughputKbps: 9000, uploadThroughputKbps: 9000, cpuSlowdownMultiplier: 4 },
  },
  cable: {
    desc: 'Cable, desktop (5 Mbps down, 28 ms RTT, no CPU slowdown)',
    formFactor: 'desktop',
    screenEmulation: { mobile: false, width: 1350, height: 940, deviceScaleFactor: 1, disabled: false },
    throttling: { rttMs: 28, throughputKbps: 5000, requestLatencyMs: 28, downloadThroughputKbps: 5000, uploadThroughputKbps: 1000, cpuSlowdownMultiplier: 1 },
  },
};

const median = (xs) => {
  const s = [...xs].sort((a, b) => a - b);
  const m = s.length >> 1;
  return s.length % 2 ? s[m] : (s[m - 1] + s[m]) / 2;
};
const round = (x, d = 0) => (x == null ? null : Math.round(x * 10 ** d) / 10 ** d);

function summarise(lhr) {
  const a = lhr.audits;
  const reqs = a['network-requests'].details.items;
  const byType = {};
  for (const r of reqs) {
    const t = r.resourceType ?? 'Other';
    byType[t] ??= { requests: 0, transferBytes: 0, resourceBytes: 0 };
    byType[t].requests += 1;
    byType[t].transferBytes += r.transferSize ?? 0;
    byType[t].resourceBytes += r.resourceSize ?? 0;
  }
  const host = (u) => new URL(u).host;
  const byHost = {};
  for (const r of reqs) {
    byHost[host(r.url)] ??= { requests: 0, transferBytes: 0 };
    byHost[host(r.url)].requests += 1;
    byHost[host(r.url)].transferBytes += r.transferSize ?? 0;
  }
  const images = reqs.filter((r) => r.resourceType === 'Image');
  const lcpParts = a['lcp-breakdown-insight']?.details?.items ?? [];
  const lcpTable = lcpParts.find((i) => i.type === 'table')?.items ?? [];
  const part = (name) => lcpTable.find((i) => i.subpart === name)?.duration ?? null;
  const lcpNode = lcpParts.find((i) => i.type === 'node');
  return {
    ttfbMs: round(a['server-response-time'].numericValue),
    fcpMs: round(a['first-contentful-paint'].numericValue),
    lcpMs: round(a['largest-contentful-paint'].numericValue),
    cls: round(a['cumulative-layout-shift'].numericValue, 4),
    loadMs: round(a['metrics'].details.items[0].observedLoad),
    fullyLoadedMs: round(Math.max(...reqs.map((r) => r.networkEndTime ?? 0))),
    speedIndexMs: round(a['speed-index'].numericValue),
    tbtMs: round(a['total-blocking-time'].numericValue),
    totalBytes: Math.round(a['total-byte-weight'].numericValue),
    requests: reqs.length,
    imageBytes: byType.Image?.transferBytes ?? 0,
    imageRequests: images.length,
    byType,
    byHost,
    // LCP split into phases (Lighthouse "LCP breakdown")
    lcpTtfbMs: round(part('timeToFirstByte')),
    lcpLoadDelayMs: round(part('resourceLoadDelay')),
    lcpLoadDurationMs: round(part('resourceLoadDuration')),
    lcpRenderDelayMs: round(part('elementRenderDelay')),
    // Lighthouse's own estimate of avoidable image bytes ("Improve image delivery")
    imageSavingsBytes: Math.round(a['image-delivery-insight']?.numericValue ?? 0),
    lcpElement: lcpNode ? (lcpNode.snippet ?? lcpNode.selector ?? '').slice(0, 200) : null,
    performanceScore: lhr.categories.performance.score,
  };
}

const NUMERIC = ['ttfbMs', 'fcpMs', 'lcpMs', 'cls', 'loadMs', 'fullyLoadedMs', 'speedIndexMs', 'tbtMs', 'totalBytes', 'requests', 'imageBytes', 'imageRequests', 'performanceScore', 'lcpTtfbMs', 'lcpLoadDelayMs', 'lcpLoadDurationMs', 'lcpRenderDelayMs', 'imageSavingsBytes'];

async function lighthouseRun(url, profile, tag) {
  const chrome = await chromeLauncher.launch({
    chromePath: process.env.CHROME_PATH,
    chromeFlags: ['--headless=new', '--ignore-certificate-errors', '--disable-gpu', '--no-first-run'],
  });
  try {
    const res = await lighthouse(url, {
      port: chrome.port,
      output: 'json',
      logLevel: 'error',
      onlyCategories: ['performance'],
      throttlingMethod: 'devtools',
      throttling: profile.throttling,
      formFactor: profile.formFactor,
      screenEmulation: profile.screenEmulation,
      maxWaitForFcp: 90_000,
      maxWaitForLoad: 240_000,
    });
    if (DUMP) { mkdirSync(DUMP, { recursive: true }); writeFileSync(join(DUMP, `${label}-${tag}.json`), res.report); }
    const lhr = res.lhr;
    if (lhr.runtimeError) throw new Error(`${lhr.runtimeError.code}: ${lhr.runtimeError.message}`);
    return { summary: summarise(lhr), version: lhr.lighthouseVersion, chrome: lhr.environment.hostUserAgent };
  } finally {
    await chrome.kill();
  }
}

// ---------------------------------------------------------------- 1. Lighthouse
const lh = {};
let lhVersion, chromeUA;
for (const [pName, profile] of Object.entries(PROFILES)) {
  for (const [pgName, path] of Object.entries(PAGES)) {
    const key = `${pName}/${pgName}`;
    if (ONLY && ONLY !== key) continue;
    const runs = [];
    for (let i = 1; i <= RUNS; i++) {
      const t0 = Date.now();
      const { summary, version, chrome } = await lighthouseRun(PAGE + path, profile, `${pName}-${pgName}-${i}`);
      lhVersion = version; chromeUA = chrome;
      runs.push(summary);
      console.log(`${label} ${key} run ${i}/${RUNS}: LCP ${summary.lcpMs} ms, load ${summary.loadMs} ms, ${(summary.totalBytes / 1e6).toFixed(2)} MB, ${summary.requests} req  (${((Date.now() - t0) / 1000).toFixed(0)} s)`);
    }
    const med = Object.fromEntries(NUMERIC.map((k) => [k, median(runs.map((r) => r[k]))]));
    lh[key] = { profile: profile.desc, page: path, runs, median: med };
  }
}

// ---------------------------------------------------------------- 2. delivery (unthrottled, curl)
const curl = (args) => execFileSync('curl', ['-sk', ...args], { encoding: 'utf8' });
const timeOne = (url, n = 10) => {
  const samples = [];
  for (let i = 0; i < n; i++) {
    const [bytes, ttfb, total, code] = curl(['-o', '/dev/null', '-w', '%{size_download} %{time_starttransfer} %{time_total} %{http_code}', url]).trim().split(' ');
    samples.push({ bytes: +bytes, ttfb: +ttfb * 1000, total: +total * 1000, code: +code });
  }
  return { bytes: samples[0].bytes, code: samples[0].code, ttfbMs: round(median(samples.map((s) => s.ttfb)), 1), totalMs: round(median(samples.map((s) => s.total)), 1) };
};
const headers = (url) => Object.fromEntries(
  curl(['-D', '-', '-o', '/dev/null', '-H', 'Accept-Encoding: gzip, br', url]).split(/\r?\n/).slice(1).filter(Boolean)
    .map((l) => { const i = l.indexOf(':'); return [l.slice(0, i).toLowerCase(), l.slice(i + 1).trim()]; })
    .filter(([k]) => ['content-type', 'content-length', 'content-encoding', 'cache-control', 'etag', 'last-modified', 'server'].includes(k)),
);

const photos = JSON.parse(curl([`${API}/api/photos/100/0`]));
const delivery = {
  api: {
    'home-profile': timeOne(`${API}/api/home-profile/1/0`),
    'photo-metadata': timeOne(`${API}/api/photos/${photos[0].id}`),
    albums: timeOne(`${API}/api/albums/5/0`),
    'album-images': timeOne(`${API}/api/albums/${state.albumId}/images/100/0`),
  },
  images: Object.fromEntries(photos.map((p) => [p.fileName, timeOne(`${IMAGES}/${encodeURIComponent(p.fileName)}`)])),
  headers: {
    image: headers(`${IMAGES}/${encodeURIComponent(photos[0].fileName)}`),
    apiJson: headers(`${API}/api/home-profile/1/0`),
  },
};
const imgTotals = Object.values(delivery.images);
delivery.imagesTotal = { bytes: imgTotals.reduce((s, x) => s + x.bytes, 0), totalMs: round(imgTotals.reduce((s, x) => s + x.totalMs, 0), 1) };

// ---------------------------------------------------------------- 3. storage + environment
const files = readdirSync(SITE).filter((f) => f.startsWith('sample-')).sort().map((f) => ({ file: f, bytes: statSync(join(SITE, f)).size }));
let originals = [];
try { originals = readdirSync(join(SITE, 'originals')).sort().map((f) => ({ file: `originals/${f}`, bytes: statSync(join(SITE, 'originals', f)).size })); } catch { /* none before the optimisation */ }
const sh = (cmd, args) => { try { return execFileSync(cmd, args, { encoding: 'utf8' }).trim(); } catch { return null; } };

const out = {
  label,
  date: new Date().toISOString(),
  env: {
    mac: sh('sysctl', ['-n', 'machdep.cpu.brand_string']),
    macos: sh('sw_vers', ['-productVersion']),
    docker: sh('docker', ['info', '--format', '{{.ServerVersion}} / {{.NCPU}} CPUs / {{.MemTotal}} bytes RAM']),
    apiImage: sh('docker', ['image', 'inspect', 'byshutterbackend.web', '--format', '{{.Id}} created {{.Created}}']),
    gitHead: sh('git', ['-C', HERE, 'rev-parse', '--short', 'HEAD']),
    node: process.version,
    lighthouse: lhVersion,
    chrome: chromeUA,
    runsPerCombination: RUNS,
  },
  profiles: Object.fromEntries(Object.entries(PROFILES).map(([k, v]) => [k, v.desc])),
  uploads: state.uploads,
  storage: { files, originals, totalBytes: [...files, ...originals].reduce((s, f) => s + f.bytes, 0) },
  delivery,
  lighthouse: lh,
};
mkdirSync(join(HERE, 'results'), { recursive: true });
const dest = join(HERE, 'results', ONLY ? `${label}-${ONLY.replace('/', '-')}.json` : `${label}.json`);
writeFileSync(dest, JSON.stringify(out, null, 2));
console.log(`\nwritten ${dest}`);
