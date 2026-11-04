<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { admin, originalUrl, photoUrl, type Photo } from '../../api'
import { useAction } from '../../composables/useAction'
import { usePhotoLibrary } from '../../composables/usePhotoLibrary'

const { photos, reload } = usePhotoLibrary()
const load = useAction()
const up = useAction()
const edit = useAction()

// ---- upload
const file = ref<File | null>(null)
const form = reactive({ fileName: '', displayName: '', imageAlt: '', description: '' })
const preview = computed(() => (file.value ? URL.createObjectURL(file.value) : null))

// Upload limit enforced by the API (Kestrel MaxRequestBodySize in Program.cs).
const MAX_UPLOAD_BYTES = 100 * 1024 * 1024
const dims = ref<{ w: number; h: number } | null>(null)
const tooBig = computed(() => (file.value?.size ?? 0) > MAX_UPLOAD_BYTES)
const fmtSize = (b: number) => (b >= 1e6 ? `${(b / 1e6).toFixed(2)} MB` : `${(b / 1e3).toFixed(0)} kB`)
const info = computed(() => {
  const f = file.value
  if (!f) return []
  return [
    ['Resolution', dims.value ? `${dims.value.w} × ${dims.value.h} px` : 'unknown (browser cannot decode this format)'],
    ['Megapixels', dims.value ? `${((dims.value.w * dims.value.h) / 1e6).toFixed(1)} MP` : '-'],
    ['File size', fmtSize(f.size)],
    ['Type', f.type || 'unknown'],
    ['Last modified', new Date(f.lastModified).toLocaleDateString('en-GB')],
  ]
})
// The site stores a WebP, so the stored name is "<stem>.webp" and must fit the 32-char column.
const storedName = computed(() => form.fileName.replace(/\.[^./\\]+$/, '') + '.webp')
const nameTooLong = computed(() => storedName.value.length > 32)

function onFile(e: Event) {
  const f = (e.target as HTMLInputElement).files?.[0] ?? null
  file.value = f
  dims.value = null
  if (f) {
    createImageBitmap(f).then((b) => { dims.value = { w: b.width, h: b.height }; b.close() }, () => {})
    form.fileName = f.name
    form.displayName ||= f.name.replace(/\.[^.]+$/, '').slice(0, 32)
    form.imageAlt ||= form.displayName
  }
}

async function upload() {
  if (!file.value) return
  const fd = new FormData()
  fd.set('Photo', file.value, form.fileName)
  fd.set('FileName', form.fileName)
  fd.set('ImageAlt', form.imageAlt)
  fd.set('DisplayName', form.displayName)
  fd.set('Description', form.description)
  const done = await up.run(async () => {
    await admin.uploadPhoto(fd)
    await reload()
    return true
  }, 'Uploaded.')
  if (done) {
    file.value = null
    Object.assign(form, { fileName: '', displayName: '', imageAlt: '', description: '' })
  }
}

// ---- edit / delete
const editing = ref<string | null>(null)
const draft = reactive({ displayName: '', description: '', photoAlt: '' })

function startEdit(p: Photo) {
  editing.value = p.id
  Object.assign(draft, { displayName: p.displayName ?? '', description: p.description ?? '', photoAlt: p.imageAlt })
}
async function save(p: Photo) {
  const done = await edit.run(async () => {
    await admin.updatePhoto(p.id, { ...draft })
    await reload()
    return true
  }, 'Saved.')
  if (done) editing.value = null
}
async function remove(p: Photo) {
  if (!confirm(`Delete "${p.displayName || p.fileName}"? It is also removed from albums.`)) return
  await edit.run(async () => {
    await admin.deletePhoto(p.id)
    await reload()
  }, 'Deleted.')
}

onMounted(() => load.run(reload))
</script>

<template>
  <div class="grid gap-16 lg:grid-cols-[22rem_1fr]">
    <section>
      <h2 class="label border-b border-black pb-2">Upload photo</h2>
      <form class="mt-6 space-y-6" @submit.prevent="upload">
        <label class="block">
          <span class="label text-neutral-500">Image file</span>
          <input type="file" accept="image/*" class="mt-2 block w-full text-sm" required @change="onFile" />
        </label>
        <div v-if="preview">
          <img :src="preview" alt="Preview" class="max-h-56 w-auto" />
          <dl class="mt-3 grid grid-cols-[auto_1fr] gap-x-4 gap-y-1 border-t border-black/15 pt-3 text-sm">
            <template v-for="[k, v] in info" :key="k">
              <dt class="label text-neutral-500">{{ k }}</dt>
              <dd class="tabular-nums">{{ v }}</dd>
            </template>
          </dl>
          <p v-if="tooBig" class="mt-2 text-sm font-medium">Too large: the upload limit is 100 MB.</p>
        </div>
        <p v-else class="text-xs text-neutral-500">Maximum upload size: 100 MB.</p>
        <label class="block">
          <span class="label text-neutral-500">File name</span>
          <input v-model="form.fileName" class="field" required />
          <span class="mt-1 block text-xs" :class="nameTooLong ? 'font-medium text-black' : 'text-neutral-500'">
            Stored as {{ storedName }}<template v-if="nameTooLong"> - too long, max 32 characters</template>
          </span>
        </label>
        <label class="block">
          <span class="label text-neutral-500">Display name</span>
          <input v-model="form.displayName" class="field" maxlength="32" />
        </label>
        <label class="block">
          <span class="label text-neutral-500">Alt text</span>
          <input v-model="form.imageAlt" class="field" required />
        </label>
        <label class="block">
          <span class="label text-neutral-500">Description</span>
          <textarea v-model="form.description" class="field" rows="3" maxlength="256" />
        </label>
        <p v-if="up.error.value" class="text-sm">{{ up.error.value }}</p>
        <p v-if="up.ok.value" class="text-sm text-neutral-500">{{ up.ok.value }}</p>
        <button class="btn btn-solid w-full" :disabled="up.busy.value || !file || nameTooLong || tooBig">
          {{ up.busy.value ? 'Uploading…' : 'Upload' }}
        </button>
      </form>
    </section>

    <section>
      <h2 class="label border-b border-black pb-2">Photos ({{ photos.length }})</h2>
      <p v-if="load.error.value || edit.error.value" class="mt-4 text-sm">{{ load.error.value || edit.error.value }}</p>
      <p v-if="edit.ok.value" class="mt-4 text-sm text-neutral-500">{{ edit.ok.value }}</p>
      <p v-if="!photos.length && !load.busy.value" class="mt-6 text-neutral-500">No photos yet.</p>
      <ul class="mt-6 grid grid-cols-2 gap-x-3 gap-y-8 md:grid-cols-3 xl:grid-cols-4">
        <li v-for="p in photos" :key="p.id">
          <div class="aspect-[4/5] overflow-hidden bg-neutral-100">
            <img :src="photoUrl(p)" :alt="p.imageAlt" loading="lazy" class="size-full object-cover" />
          </div>
          <template v-if="editing !== p.id">
            <p class="mt-2 truncate text-sm font-medium">{{ p.displayName || p.fileName }}</p>
            <p class="truncate text-xs text-neutral-500">{{ p.fileName }}</p>
            <div class="mt-2 flex flex-wrap gap-x-4 gap-y-1">
              <button type="button" class="link-danger" @click="startEdit(p)">Edit</button>
              <button type="button" class="link-danger" @click="remove(p)">Delete</button>
              <a v-if="originalUrl(p)" :href="originalUrl(p)!" target="_blank" rel="noopener" class="link-danger">Original</a>
            </div>
          </template>
          <form v-else class="mt-3 space-y-3" @submit.prevent="save(p)">
            <input v-model="draft.displayName" class="field" maxlength="32" placeholder="Display name" />
            <input v-model="draft.photoAlt" class="field" placeholder="Alt text" required />
            <textarea v-model="draft.description" class="field" rows="2" maxlength="256" placeholder="Description" />
            <div class="flex gap-2">
              <button class="btn btn-solid" :disabled="edit.busy.value">Save</button>
              <button type="button" class="btn" @click="editing = null">Cancel</button>
            </div>
          </form>
        </li>
      </ul>
    </section>
  </div>
</template>
