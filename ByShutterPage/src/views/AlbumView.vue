<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { api, photoUrl, type Album, type Photo } from '../api'
import Lightbox from '../components/Lightbox.vue'

const route = useRoute()
const album = ref<Album | null>(null)
const photos = ref<Photo[]>([])
const error = ref<string | null>(null)
const open = ref<number | null>(null)
const shownDate = computed(() => {
  const d = album.value?.date ?? photos.value[0]?.uploadedAt
  return d ? new Date(d).toLocaleDateString('en-GB', { day: 'numeric', month: 'long', year: 'numeric' }) : ''
})

watch(
  () => route.params.id as string,
  async (id) => {
    album.value = null
    photos.value = []
    error.value = null
    try {
      ;[album.value, photos.value] = await Promise.all([api.album(id), api.albumPhotos(id)])
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
    }
  },
  { immediate: true },
)
</script>

<template>
  <div class="mx-auto max-w-[110rem] px-5 pt-12 md:px-10 md:pt-20">
    <RouterLink to="/albums" class="label hover:underline underline-offset-8">&larr; All albums</RouterLink>

    <p v-if="error" class="mt-12">Could not load this album: {{ error }}</p>

    <template v-else>
      <header class="mt-10 grid gap-6 border-b border-black pb-10 md:grid-cols-[1fr_minmax(0,28rem)] md:items-end">
        <h1 class="text-5xl font-light tracking-tighter md:text-8xl">{{ album?.name }}</h1>
        <div>
          <p class="leading-relaxed text-neutral-600">{{ album?.description }}</p>
          <p v-if="album" class="label mt-4 text-neutral-500">
            <template v-if="shownDate">{{ shownDate }} <span class="mx-2">/</span></template>{{ photos.length }} photographs
          </p>
        </div>
      </header>

      <ul class="mt-8 grid grid-cols-2 gap-2 md:grid-cols-3 md:gap-4 xl:grid-cols-4">
        <li v-for="(p, i) in photos" :key="p.id">
          <button type="button" class="group block w-full cursor-zoom-in" :aria-label="`Open ${p.displayName || p.imageAlt}`" @click="open = i">
            <div class="aspect-[4/5] overflow-hidden bg-neutral-100">
              <img
                :src="photoUrl(p)"
                :alt="p.imageAlt"
                loading="lazy"
                class="photo-mono size-full object-cover group-hover:scale-[1.03]"
              />
            </div>
          </button>
        </li>
      </ul>
    </template>

    <Lightbox :photos="photos" :index="open" @update:index="open = $event" @close="open = null" />
  </div>
</template>
