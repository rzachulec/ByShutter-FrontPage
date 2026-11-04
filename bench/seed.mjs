// Seeds a freshly reset ByShutter stack with the large sample photos from ../site/
// and builds the content the stand-in page needs (home profile + 4 tiles + 1 album).
//
// Usage:  node seed.mjs            (API at http://localhost:5700)
//         API=http://host:port node seed.mjs
//
// Originals are uploaded under new names (sample-NN.*) so the git-tracked files
// in ../site/ are never overwritten.
import { openAsBlob } from 'node:fs';
import { writeFile } from 'node:fs/promises';
import { performance } from 'node:perf_hooks';

const API = process.env.API ?? 'http://localhost:5700';
const SITE = new URL('../site/', import.meta.url);
const STATE = new URL('./.state.json', import.meta.url);

const USERNAME = process.env.BS_USER ?? 'admin';
const PASSWORD = process.env.BS_PASS ?? 'admin';

// [file in ../site/, name to upload as]
const SAMPLES = [
  ['IMG_2081.jpeg', 'sample-01.jpeg'],
  ['IMG_2082.jpeg', 'sample-02.jpeg'],
  ['IMG_2083.jpeg', 'sample-03.jpeg'],
  ['IMG_2084.jpeg', 'sample-04.jpeg'],
  ['IMG_2085.jpeg', 'sample-05.jpeg'],
  ['IMG_2086.jpeg', 'sample-06.jpeg'],
  ['pexels-cmrcn-24861721.jpg', 'sample-07.jpg'],
];

let cookie = '';

async function call(method, path, { json, form, expect } = {}) {
  const headers = {};
  if (cookie) headers.cookie = cookie;
  let body;
  if (json !== undefined) {
    headers['content-type'] = 'application/json';
    body = JSON.stringify(json);
  } else if (form) {
    body = form;
  }
  const res = await fetch(API + path, { method, headers, body });
  if (expect && !expect.includes(res.status)) {
    throw new Error(`${method} ${path} -> ${res.status}: ${(await res.text()).slice(0, 300)}`);
  }
  return res;
}

// --- login ---------------------------------------------------------------
const login = await call('POST', '/api/auth/login', {
  json: { username: USERNAME, password: PASSWORD },
  expect: [204],
});
cookie = login.headers.getSetCookie().map((c) => c.split(';')[0]).join('; ');
if (!cookie) throw new Error('login returned no session cookie');

// --- guard: only seed an empty database ------------------------------------
const existing = await (await call('GET', '/api/photos/100/0', { expect: [200] })).json();
if (existing.length > 0) {
  console.error(`Database already has ${existing.length} photos. Run bench/reset.sh first.`);
  process.exit(1);
}

// --- upload ----------------------------------------------------------------
const uploads = [];
for (const [i, [src, name]] of SAMPLES.entries()) {
  const blob = await openAsBlob(new URL(src, SITE));
  const n = String(i + 1).padStart(2, '0');
  const form = new FormData();
  form.set('Photo', blob, name);
  form.set('FileName', name);
  form.set('ImageAlt', `Sample photo ${n}`);
  form.set('DisplayName', `Sample ${n}`);
  form.set('Description', `Benchmark sample ${n} (${src})`);

  const t0 = performance.now();
  await call('POST', '/api/photos', { form, expect: [204] });
  const ms = performance.now() - t0;
  uploads.push({ name, sourceBytes: blob.size, uploadMs: Math.round(ms) });
  console.log(`uploaded ${name}  ${(blob.size / 1e6).toFixed(2)} MB  ${Math.round(ms)} ms`);
}

// --- map display names -> photo ids ------------------------------------------
const photos = await (await call('GET', '/api/photos/100/0', { expect: [200] })).json();
const byDisplay = new Map(photos.map((p) => [p.displayName, p]));
const photo = (i) => {
  const p = byDisplay.get(`Sample ${String(i).padStart(2, '0')}`);
  if (!p) throw new Error(`photo ${i} missing from API`);
  return p;
};

// --- home profile: hero = 01, profile = 02, tiles = 03..06 -------------------------
const profileId = await (
  await call('POST', '/api/home-profile', {
    json: {
      heroMessage: 'Light, place, time.',
      greetings: 'Hello, I am a photographer.',
      quote: 'A photograph is a secret about a secret.',
      aboutMe: 'Benchmark profile text. Landscapes, travel and everyday scenes shot on location.',
      heroImageId: photo(1).id,
      profileImageId: photo(2).id,
    },
    expect: [201],
  })
).json();

for (const i of [3, 4, 5, 6]) {
  const tileId = await (
    await call('POST', '/api/portfolio-tiles', {
      json: { name: `Tile ${i - 2}`, imageId: photo(i).id, description: `Portfolio tile ${i - 2}` },
      expect: [201],
    })
  ).json();
  await call('PUT', `/api/home-profile/${profileId}/portfolio-tiles/${tileId}`, { expect: [204] });
}

// --- album with all seven photos ----------------------------------------------
await call('POST', '/api/albums', { json: { name: 'Benchmark album', description: 'All sample photos' }, expect: [204] });
const albums = await (await call('GET', '/api/albums/10/0', { expect: [200] })).json();
const album = albums.find((a) => a.name === 'Benchmark album');
if (!album) throw new Error('album not found after creation');
await call('PUT', `/api/albums/${album.id}/add-images`, {
  json: { imageIds: SAMPLES.map((_, i) => photo(i + 1).id) },
  expect: [200, 204],
});

await writeFile(STATE, JSON.stringify({ profileId, albumId: album.id, uploads }, null, 2));
console.log(`\nprofile ${profileId}\nalbum   ${album.id}\nstate written to bench/.state.json`);
