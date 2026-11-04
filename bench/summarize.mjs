// Compares two result files and prints Typst table fragments (Polish number format) for the report,
// so numbers never have to be copied by hand.
//
//   node summarize.mjs baseline optimised          -> prints fragments to stdout
//   node summarize.mjs baseline optimised --md     -> prints a plain Markdown overview instead
import { readFileSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
import { join } from 'node:path';

const [aName, bName] = process.argv.slice(2).filter((a) => !a.startsWith('--'));
const MD = process.argv.includes('--md');
const dir = join(fileURLToPath(new URL('.', import.meta.url)), 'results');
const A = JSON.parse(readFileSync(join(dir, `${aName}.json`), 'utf8'));
const B = JSON.parse(readFileSync(join(dir, `${bName}.json`), 'utf8'));

const NBSP = ' ';
const num = (x, d = 0) => x.toFixed(d).replace('.', ',').replace(/\B(?=(\d{3})+(?!\d))/g, NBSP);
const sec = (ms) => `${num(ms / 1000, ms >= 10000 ? 1 : 2)}${NBSP}s`;
const msf = (ms) => `${num(ms)}${NBSP}ms`;
const time = (ms) => (ms >= 1000 ? sec(ms) : msf(ms));
const mb = (b) => `${num(b / 1e6, 2)}${NBSP}MB`;
const kb = (b) => `${num(b / 1e3, b >= 1e4 ? 0 : 1)}${NBSP}kB`;
const size = (b) => (b >= 1e6 ? mb(b) : kb(b));
const cnt = (x) => num(x);
const sc = (x) => num(x * 100);
const cls = (x) => num(x, 3);
const delta = (a, b) => {
  if (a === 0) return b === 0 ? 'bez zmian' : '—';
  const p = ((b - a) / a) * 100;
  if (Math.abs(p) < 1) return 'bez zmian';
  return `${p < 0 ? '−' : '+'}${num(Math.abs(p), Math.abs(p) >= 10 ? 0 : 1)}${NBSP}%`;
};
const deltaAbs = (a, b, f) => {
  const d = b - a;
  if (Math.abs(d) < 0.0005) return 'bez zmian';
  return `${d < 0 ? '−' : '+'}${f(Math.abs(d))}`;
};

const ROWS = [
  ['TTFB', 'ttfbMs', time],
  ['FCP', 'fcpMs', time],
  ['LCP', 'lcpMs', time],
  ['Fully loaded (ostatnie żądanie sieciowe)', 'fullyLoadedMs', time],
  ['Speed Index', 'speedIndexMs', time],
  ['CLS', 'cls', cls, 'abs'],
  ['Rozmiar strony (transfer)', 'totalBytes', mb],
  ['w tym obrazy', 'imageBytes', mb],
  ['Liczba żądań', 'requests', cnt],
  ['Wynik wydajności Lighthouse (0–100)', 'performanceScore', sc, 'abs0'],
];

const out = [];
const emit = (title, body) => out.push(`// ===== ${title} =====\n${body}\n`);
const row = (cells) => cells.map((c) => `[${c}]`).join(', ');

// ---- Lighthouse medians: one table per page x profile
for (const key of Object.keys(A.lighthouse)) {
  const a = A.lighthouse[key].median;
  const b = B.lighthouse[key].median;
  const body = ROWS.map(([label, k, f, mode]) => {
    const d = mode === 'abs' ? deltaAbs(a[k], b[k], f) : mode === 'abs0' ? deltaAbs(a[k], b[k], (x) => num(x * 100)) : delta(a[k], b[k]);
    return row([label, f(a[k]), f(b[k]), d]);
  }).join(',\n    ');
  emit(`LH ${key}`, `    ${row(['*Metryka*', '*Przed*', '*Po*', '*Zmiana*'])},\n    ${body},`);
}

// ---- LCP phases (home page)
for (const key of Object.keys(A.lighthouse).filter((k) => k.endsWith('/home'))) {
  const a = A.lighthouse[key].median;
  const b = B.lighthouse[key].median;
  const P = [['Czas do pierwszego bajta (TTFB)', 'lcpTtfbMs'], ['Opóźnienie rozpoczęcia pobierania obrazu', 'lcpLoadDelayMs'], ['Czas pobierania obrazu', 'lcpLoadDurationMs'], ['Opóźnienie renderowania', 'lcpRenderDelayMs']];
  const body = P.map(([l, k]) => row([l, time(a[k]), time(b[k]), delta(a[k], b[k])])).join(',\n    ');
  emit(`LCP phases ${key}`, `    ${row(['*Faza LCP*', '*Przed*', '*Po*', '*Zmiana*'])},\n    ${body},`);
}

// ---- Delivery per file (unthrottled localhost)
{
  const names = Object.keys(A.delivery.images);
  const bNames = Object.keys(B.delivery.images);
  const bFor = (i) => B.delivery.images[bNames[i]];
  const body = names.map((n, i) => {
    const x = A.delivery.images[n];
    const y = bFor(i);
    return row([`\`${bNames[i]}\``, size(x.bytes), size(y.bytes), delta(x.bytes, y.bytes), msf(x.totalMs), msf(y.totalMs)]);
  }).join(',\n    ');
  const ta = A.delivery.imagesTotal, tb = B.delivery.imagesTotal;
  const total = row(['*Razem*', `*${size(ta.bytes)}*`, `*${size(tb.bytes)}*`, `*${delta(ta.bytes, tb.bytes)}*`, `*${msf(ta.totalMs)}*`, `*${msf(tb.totalMs)}*`]);
  emit('Delivery images', `    ${row(['*Plik*', '*Rozmiar przed*', '*Rozmiar po*', '*Zmiana*', '*Czas pobrania przed*', '*Czas pobrania po*'])},\n    ${body},\n    ${total},`);
}

// ---- Upload + storage
{
  const ua = A.uploads, ub = B.uploads;
  const body = ua.map((x, i) => row([`\`${ub[i].name}\``, size(x.sourceBytes), msf(x.uploadMs), msf(ub[i].uploadMs), delta(x.uploadMs, ub[i].uploadMs)])).join(',\n    ');
  const sum = (u) => u.reduce((s, x) => s + x.uploadMs, 0);
  const tot = row(['*Razem*', `*${size(ua.reduce((s, x) => s + x.sourceBytes, 0))}*`, `*${msf(sum(ua))}*`, `*${msf(sum(ub))}*`, `*${delta(sum(ua), sum(ub))}*`]);
  emit('Upload', `    ${row(['*Plik*', '*Rozmiar wgrywanego pliku*', '*Czas wgrania przed*', '*Czas wgrania po*', '*Zmiana*'])},\n    ${body},\n    ${tot},`);
  emit('Storage', `before total: ${size(A.storage.totalBytes)} (${A.storage.files.length} files)\nafter total:  ${size(B.storage.totalBytes)} (${B.storage.files.length} web files + ${B.storage.originals.length} originals)\nweb files only after: ${size(B.storage.files.reduce((s, f) => s + f.bytes, 0))}`);
}

// ---- API (unthrottled)
{
  const body = Object.keys(A.delivery.api).map((k) => row([`\`${k}\``, msf(A.delivery.api[k].totalMs), msf(B.delivery.api[k].totalMs), `${A.delivery.api[k].bytes}${NBSP}B → ${B.delivery.api[k].bytes}${NBSP}B`])).join(',\n    ');
  emit('API', `    ${row(['*Endpoint*', '*Przed*', '*Po*', '*Rozmiar odpowiedzi*'])},\n    ${body},`);
}

if (MD) {
  // quick human-readable overview
  for (const key of Object.keys(A.lighthouse)) {
    const a = A.lighthouse[key].median, b = B.lighthouse[key].median;
    console.log(`\n## ${key}  (${A.lighthouse[key].profile})`);
    for (const [label, k, f] of ROWS) console.log(`${label.padEnd(44)} ${String(f(a[k])).padStart(12)} -> ${String(f(b[k])).padStart(12)}   ${delta(a[k], b[k])}`);
  }
} else {
  console.log(out.join('\n'));
}
