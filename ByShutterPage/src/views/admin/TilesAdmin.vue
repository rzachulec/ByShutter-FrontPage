<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { admin, photoUrl, type Tile } from '../../api'
import PhotoPicker from '../../components/PhotoPicker.vue'
import { useAction } from '../../composables/useAction'
import { usePhotoLibrary } from '../../composables/usePhotoLibrary'

const { photos, byId, reload: reloadPhotos } = usePhotoLibrary()
const tiles = ref<Tile[]>([])
const load = useAction()
const act = useAction()
const editing = ref<string | null>(null) // tile id, or null = creating
const form = reactive({ name: '', description: '', imageId: '' })

const reload = async () => {
  ;[tiles.value] = await Promise.all([admin.tiles(100, 0), reloadPhotos()])
}
function reset() {
  editing.value = null
  Object.assign(form, { name: '', description: '', imageId: '' })
}
function startEdit(t: Tile) {
  editing.value = t.id
  Object.assign(form, { name: t.name, description: t.description ?? '', imageId: t.imageId })
  window.scrollTo({ top: 0, behavior: 'smooth' })
}
async function save() {
  const done = await act.run(async () => {
    editing.value ? await admin.updateTile(editing.value, { ...form }) : await admin.createTile({ ...form })
    await reload()
    return true
  }, editing.value ? 'Tile saved.' : 'Tile created.')
  if (done) reset()
}
async function remove(t: Tile) {
  if (!confirm(`Delete tile "${t.name}"?`)) return
  await act.run(async () => { await admin.deleteTile(t.id); await reload() }, 'Tile deleted.')
}
onMounted(() => load.run(reload))
</script>

<template>
  <div class="grid gap-16 lg:grid-cols-[22rem_1fr]">
    <section>
      <h2 class="label border-b border-black pb-2">{{ editing ? 'Edit tile' : 'New tile' }}</h2>
      <form class="mt-6 space-y-6" @submit.prevent="save">
        <label class="block">
          <span class="label text-neutral-500">Name</span>
          <input v-model="form.name" class="field" required />
        </label>
        <label class="block">
          <span class="label text-neutral-500">Description</span>
          <textarea v-model="form.description" class="field" rows="2" />
        </label>
        <PhotoPicker v-model="form.imageId" :photos="photos" label="Tile photo" />
        <p v-if="act.error.value" class="text-sm">{{ act.error.value }}</p>
        <p v-if="act.ok.value" class="text-sm text-neutral-500">{{ act.ok.value }}</p>
        <div class="flex gap-2">
          <button class="btn btn-solid flex-1" :disabled="act.busy.value || !form.imageId">{{ editing ? 'Save' : 'Create tile' }}</button>
          <button v-if="editing" type="button" class="btn" @click="reset">Cancel</button>
        </div>
      </form>
    </section>

    <section>
      <h2 class="label border-b border-black pb-2">Tiles ({{ tiles.length }})</h2>
      <p v-if="load.error.value" class="mt-4 text-sm">{{ load.error.value }}</p>
      <p v-if="!tiles.length && !load.busy.value" class="mt-6 text-neutral-500">No tiles yet.</p>
      <ul class="mt-6 grid grid-cols-2 gap-x-3 gap-y-8 md:grid-cols-3 xl:grid-cols-4">
        <li v-for="t in tiles" :key="t.id">
          <div class="aspect-[3/4] overflow-hidden bg-neutral-100">
            <img v-if="byId.get(t.imageId)" :src="photoUrl(byId.get(t.imageId)!)" :alt="t.name" loading="lazy" class="size-full object-cover" />
          </div>
          <p class="mt-2 truncate text-sm font-medium">{{ t.name }}</p>
          <p v-if="t.description" class="truncate text-xs text-neutral-500">{{ t.description }}</p>
          <div class="mt-2 flex gap-4">
            <button type="button" class="link-danger" @click="startEdit(t)">Edit</button>
            <button type="button" class="link-danger" @click="remove(t)">Delete</button>
          </div>
        </li>
      </ul>
    </section>
  </div>
</template>
