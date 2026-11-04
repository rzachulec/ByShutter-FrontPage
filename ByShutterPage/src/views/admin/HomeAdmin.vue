<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { admin, photoUrl, type HomeProfile, type ProfileInput, type Tile } from '../../api'
import PhotoPicker from '../../components/PhotoPicker.vue'
import { useAction } from '../../composables/useAction'
import { usePhotoLibrary } from '../../composables/usePhotoLibrary'

// The API rejects a fifth tile on a profile (HomeProfile.AddPortfolioTile throws).
const MAX_TILES = 4
const { photos, byId, reload: reloadPhotos } = usePhotoLibrary()
const profiles = ref<HomeProfile[]>([])
const allTiles = ref<Tile[]>([])
const load = useAction()
const act = useAction()
const editing = ref<string | null>(null) // profile id, or null = creating
const empty = (): ProfileInput => ({ heroMessage: '', greetings: '', quote: '', aboutMe: '', heroImageId: '', profileImageId: '' })
const form = reactive<ProfileInput>(empty())

const reload = async () => {
  ;[profiles.value, allTiles.value] = await Promise.all([admin.profiles(), admin.tiles(100, 0), reloadPhotos()])
}
function reset() {
  editing.value = null
  Object.assign(form, empty())
}
function startEdit(p: HomeProfile) {
  editing.value = p.id
  const { heroMessage, greetings, quote, aboutMe, heroImageId, profileImageId } = p
  Object.assign(form, { heroMessage, greetings, quote, aboutMe, heroImageId, profileImageId })
  window.scrollTo({ top: 0, behavior: 'smooth' })
}
const valid = computed(() => form.heroImageId && form.profileImageId)

async function save() {
  const done = await act.run(async () => {
    editing.value ? await admin.updateProfile(editing.value, { ...form }) : await admin.createProfile({ ...form })
    await reload()
    return true
  }, editing.value ? 'Profile saved.' : 'Profile created.')
  if (done) reset()
}
async function remove(p: HomeProfile) {
  if (!confirm('Delete this home profile?')) return
  await act.run(async () => { await admin.deleteProfile(p.id); await reload() }, 'Profile deleted.')
}
const unassigned = (p: HomeProfile) => allTiles.value.filter((t) => !p.portfolioTiles.some((x) => x.id === t.id))
const assign = (p: HomeProfile, tileId: string) => act.run(async () => { await admin.assignTile(p.id, tileId); await reload() }, 'Tile assigned.')
const unassign = (p: HomeProfile, tileId: string) => act.run(async () => { await admin.unassignTile(p.id, tileId); await reload() }, 'Tile removed.')

onMounted(() => load.run(reload))
</script>

<template>
  <div class="grid gap-16 lg:grid-cols-[26rem_1fr]">
    <section>
      <h2 class="label border-b border-black pb-2">{{ editing ? 'Edit home profile' : 'New home profile' }}</h2>
      <form class="mt-6 space-y-6" @submit.prevent="save">
        <label class="block">
          <span class="label text-neutral-500">Hero message</span>
          <input v-model="form.heroMessage" class="field" required />
        </label>
        <label class="block">
          <span class="label text-neutral-500">Greetings</span>
          <input v-model="form.greetings" class="field" required />
        </label>
        <label class="block">
          <span class="label text-neutral-500">Quote</span>
          <input v-model="form.quote" class="field" required />
        </label>
        <label class="block">
          <span class="label text-neutral-500">About me</span>
          <textarea v-model="form.aboutMe" class="field" rows="5" required />
        </label>
        <PhotoPicker v-model="form.heroImageId" :photos="photos" label="Title (hero) photo" />
        <PhotoPicker v-model="form.profileImageId" :photos="photos" label="Profile photo" />
        <p v-if="act.error.value" class="text-sm">{{ act.error.value }}</p>
        <p v-if="act.ok.value" class="text-sm text-neutral-500">{{ act.ok.value }}</p>
        <div class="flex gap-2">
          <button class="btn btn-solid flex-1" :disabled="act.busy.value || !valid">{{ editing ? 'Save' : 'Create profile' }}</button>
          <button v-if="editing" type="button" class="btn" @click="reset">Cancel</button>
        </div>
      </form>
    </section>

    <section>
      <h2 class="label border-b border-black pb-2">Profiles ({{ profiles.length }})</h2>
      <p v-if="load.error.value" class="mt-4 text-sm">{{ load.error.value }}</p>
      <p v-if="profiles.length > 1" class="mt-4 text-sm text-neutral-500">The public home page uses the first profile.</p>
      <p v-if="!profiles.length && !load.busy.value" class="mt-6 text-neutral-500">No profile yet. Create one to publish the home page.</p>

      <article v-for="p in profiles" :key="p.id" class="mt-8 border-b border-black/15 pb-10">
        <div class="flex gap-5">
          <img v-if="byId.get(p.heroImageId)" :src="photoUrl(byId.get(p.heroImageId)!)" alt="" class="h-28 w-40 object-cover" />
          <div class="min-w-0">
            <p class="text-2xl font-light tracking-tight">{{ p.heroMessage }}</p>
            <p class="truncate text-sm text-neutral-500">{{ p.greetings }}</p>
            <div class="mt-3 flex gap-5">
              <button type="button" class="link-danger" @click="startEdit(p)">Edit</button>
              <button type="button" class="link-danger" @click="remove(p)">Delete</button>
              <RouterLink to="/home" class="link-danger">View site</RouterLink>
            </div>
          </div>
        </div>

        <h3 class="label mt-8">Portfolio tiles ({{ p.portfolioTiles.length }})</h3>
        <ul class="mt-3 grid grid-cols-3 gap-2 md:grid-cols-4">
          <li v-for="t in p.portfolioTiles" :key="t.id">
            <div class="aspect-[3/4] overflow-hidden bg-neutral-100">
              <img v-if="byId.get(t.imageId)" :src="photoUrl(byId.get(t.imageId)!)" :alt="t.name" loading="lazy" class="size-full object-cover" />
            </div>
            <p class="mt-1 truncate text-xs">{{ t.name }}</p>
            <button type="button" class="link-danger" @click="unassign(p, t.id)">Remove</button>
          </li>
        </ul>
        <div v-if="p.portfolioTiles.length < MAX_TILES && unassigned(p).length" class="mt-4 flex items-center gap-3">
          <select class="field !w-auto min-w-48" :id="`add-${p.id}`" @change="(e) => { const v = (e.target as HTMLSelectElement).value; if (v) assign(p, v); (e.target as HTMLSelectElement).value = '' }">
            <option value="">Add a tile…</option>
            <option v-for="t in unassigned(p)" :key="t.id" :value="t.id">{{ t.name }}</option>
          </select>
        </div>
        <p v-else-if="p.portfolioTiles.length >= MAX_TILES" class="mt-4 text-sm text-neutral-500">A profile shows at most {{ MAX_TILES }} tiles. Remove one to add another.</p>
        <p v-else-if="!allTiles.length" class="mt-4 text-sm text-neutral-500">Create tiles on the Tiles page first.</p>
      </article>
    </section>
  </div>
</template>
