<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { useRoute } from 'vue-router'
import { admin, api, photoUrl, type Album, type Photo } from '../../api'
import { useAction } from '../../composables/useAction'

const id = computed(() => useRoute().params.id as string)
const album = ref<Album | null>(null)
const inAlbum = ref<Photo[]>([])
const available = ref<Photo[]>([])
const toRemove = ref(new Set<string>())
const toAdd = ref(new Set<string>())
const load = useAction()
const act = useAction()
const meta = useAction()
let formFilled = false
const form = reactive({ name: '', description: '', date: '' })
const dirty = computed(
  () => !!album.value && (form.name !== album.value.name || form.description !== (album.value.description ?? '') || form.date !== (album.value.date?.slice(0, 10) ?? '')),
)
function fillForm() {
  if (!album.value) return
  form.name = album.value.name
  form.description = album.value.description ?? ''
  form.date = album.value.date?.slice(0, 10) ?? ''
}
async function saveDetails() {
  await meta.run(async () => {
    await admin.updateAlbum(id.value, { name: form.name.trim(), description: form.description, date: form.date ? `${form.date}T00:00:00Z` : null })
    album.value = await api.album(id.value)
    fillForm()
  }, 'Album details saved.')
}

async function reload() {
  ;[album.value, inAlbum.value, available.value] = await Promise.all([
    api.album(id.value),
    api.albumPhotos(id.value),
    admin.availablePhotos(id.value, 0, 100),
  ])
  toRemove.value = new Set()
  toAdd.value = new Set()
  // First load always fills the form; later reloads (photos added/removed) keep unsaved edits.
  if (!formFilled || !dirty.value) fillForm()
  formFilled = true
}
function toggle(set: Set<string>, pid: string) {
  set.has(pid) ? set.delete(pid) : set.add(pid)
}
const addSelected = () => act.run(async () => { await admin.addToAlbum(id.value, [...toAdd.value]); await reload() }, 'Photos added.')
const removeSelected = () => act.run(async () => { await admin.removeFromAlbum(id.value, [...toRemove.value]); await reload() }, 'Photos removed.')

onMounted(() => load.run(reload))
</script>

<template>
  <div>
    <RouterLink to="/config/albums" class="label hover:underline underline-offset-8">&larr; Albums</RouterLink>
    <h2 class="mt-6 text-4xl font-light tracking-tighter md:text-6xl">{{ album?.name }}</h2>

    <form class="mt-10 grid max-w-4xl gap-6 border-t border-black pt-6 md:grid-cols-2" @submit.prevent="saveDetails">
      <h3 class="label md:col-span-2">Album details</h3>
      <label class="block">
        <span class="label text-neutral-500">Title</span>
        <input v-model="form.name" class="field" maxlength="32" required />
      </label>
      <label class="block">
        <span class="label text-neutral-500">Date</span>
        <input v-model="form.date" type="date" class="field" />
        <span class="mt-1 block text-xs text-neutral-500">Leave empty to use the date of the first photo.</span>
      </label>
      <label class="block md:col-span-2">
        <span class="label text-neutral-500">Description</span>
        <textarea v-model="form.description" class="field" rows="3" maxlength="256" />
      </label>
      <div class="flex flex-wrap items-center gap-4 md:col-span-2">
        <button class="btn btn-solid" :disabled="meta.busy.value || !dirty || !form.name.trim()">Save details</button>
        <button v-if="form.date" type="button" class="link-danger" @click="form.date = ''">Clear date</button>
        <p v-if="meta.error.value" class="text-sm">{{ meta.error.value }}</p>
        <p v-if="meta.ok.value && !dirty" class="text-sm text-neutral-500">{{ meta.ok.value }}</p>
      </div>
    </form>
    <p v-if="load.error.value || act.error.value" class="mt-4 text-sm">{{ load.error.value || act.error.value }}</p>
    <p v-if="act.ok.value" class="mt-4 text-sm text-neutral-500">{{ act.ok.value }}</p>

    <div class="mt-10 grid gap-14 xl:grid-cols-2">
      <section v-for="s in [
        { title: 'In this album', photos: inAlbum, sel: toRemove, action: removeSelected, label: 'Remove selected' },
        { title: 'Available photos', photos: available, sel: toAdd, action: addSelected, label: 'Add selected' },
      ]" :key="s.title">
        <div class="flex items-center justify-between border-b border-black pb-2">
          <h3 class="label">{{ s.title }} ({{ s.photos.length }})</h3>
          <button type="button" class="btn" :disabled="!s.sel.size || act.busy.value" @click="s.action()">{{ s.label }} ({{ s.sel.size }})</button>
        </div>
        <p v-if="!s.photos.length" class="mt-6 text-neutral-500">Nothing here.</p>
        <ul class="mt-4 grid grid-cols-3 gap-2 md:grid-cols-4">
          <li v-for="p in s.photos" :key="p.id">
            <button type="button" class="relative block w-full" :aria-pressed="s.sel.has(p.id)" :title="p.displayName || p.fileName" @click="toggle(s.sel, p.id)">
              <span class="block aspect-[4/5] overflow-hidden bg-neutral-100" :class="s.sel.has(p.id) ? 'outline-4 -outline-offset-4 outline-black' : ''">
                <img :src="photoUrl(p)" :alt="p.imageAlt" loading="lazy" class="size-full object-cover" :class="s.sel.has(p.id) ? 'opacity-60' : ''" />
              </span>
              <span v-if="s.sel.has(p.id)" class="label absolute right-2 top-2 bg-black px-1.5 py-0.5 text-white">&check;</span>
            </button>
          </li>
        </ul>
      </section>
    </div>
  </div>
</template>
