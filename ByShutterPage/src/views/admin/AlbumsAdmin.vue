<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import { admin, api, type Album } from '../../api'
import { useAction } from '../../composables/useAction'

const albums = ref<Album[]>([])
const load = useAction()
const create = useAction()
const form = reactive({ name: '', description: '' })

const reload = async () => (albums.value = await api.albums(100, 0))

async function add() {
  const done = await create.run(async () => {
    await admin.createAlbum(form.name, form.description)
    await reload()
    return true
  }, 'Album created.')
  if (done) Object.assign(form, { name: '', description: '' })
}
async function remove(a: Album) {
  if (!confirm(`Delete album "${a.name}"? The photos stay in the library.`)) return
  await create.run(async () => {
    await admin.deleteAlbum(a.id)
    await reload()
  }, 'Album deleted.')
}
onMounted(() => load.run(reload))
</script>

<template>
  <div class="grid gap-16 lg:grid-cols-[22rem_1fr]">
    <section>
      <h2 class="label border-b border-black pb-2">New album</h2>
      <form class="mt-6 space-y-6" @submit.prevent="add">
        <label class="block">
          <span class="label text-neutral-500">Name</span>
          <input v-model="form.name" class="field" maxlength="32" required />
        </label>
        <label class="block">
          <span class="label text-neutral-500">Description</span>
          <textarea v-model="form.description" class="field" rows="3" maxlength="256" />
        </label>
        <p v-if="create.error.value" class="text-sm">{{ create.error.value }}</p>
        <p v-if="create.ok.value" class="text-sm text-neutral-500">{{ create.ok.value }}</p>
        <button class="btn btn-solid w-full" :disabled="create.busy.value">Create album</button>
      </form>
    </section>

    <section>
      <h2 class="label border-b border-black pb-2">Albums ({{ albums.length }})</h2>
      <p v-if="load.error.value" class="mt-4 text-sm">{{ load.error.value }}</p>
      <p v-if="!albums.length && !load.busy.value" class="mt-6 text-neutral-500">No albums yet.</p>
      <ul>
        <li v-for="a in albums" :key="a.id" class="flex flex-wrap items-baseline justify-between gap-4 border-b border-black/15 py-5">
          <div class="min-w-0">
            <p class="text-2xl font-light tracking-tight">{{ a.name }}</p>
            <p class="text-sm text-neutral-500">{{ a.photos.length }} photos<template v-if="a.description"> · {{ a.description }}</template></p>
          </div>
          <div class="flex gap-5">
            <RouterLink :to="`/config/albums/${a.id}`" class="link-danger">Manage</RouterLink>
            <RouterLink :to="`/albums/${a.id}`" class="link-danger">View</RouterLink>
            <button type="button" class="link-danger" @click="remove(a)">Delete</button>
          </div>
        </li>
      </ul>
    </section>
  </div>
</template>
