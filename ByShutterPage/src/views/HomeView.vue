<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { api, photoUrl } from '../api'
import { useProfile } from '../composables/useProfile'
import AboutWidget from '../components/AboutWidget.vue'

const { data, error } = useProfile()

// A tile links to the album that contains its photo (first match if it is in several). The albums
// request runs after the profile has rendered, so it never delays the title photo.
const albumByPhoto = ref(new Map<string, string>())
watch(
  data,
  async (d) => {
    if (!d) return
    try {
      const map = new Map<string, string>()
      for (const album of await api.albums(100, 0)) {
        for (const p of album.photos) if (!map.has(p.photoId)) map.set(p.photoId, album.id)
      }
      albumByPhoto.value = map
    } catch {
      /* tiles keep pointing at the albums index */
    }
  },
  { immediate: true },
)
const tileLink = (photoId: string) => (albumByPhoto.value.has(photoId) ? `/albums/${albumByPhoto.value.get(photoId)}` : '/albums')

// Mirrors the structure described in raport-1: the title photo is a CSS background that is only
// set once the API has told us the file name, and the tile images are plain <img> (no lazy loading).
const heroStyle = computed(() => (data.value ? { backgroundImage: `url("${photoUrl(data.value.hero)}")` } : undefined))
</script>

<template>
  <p v-if="error" class="mx-auto max-w-[110rem] px-5 py-24 md:px-10">Could not load the page: {{ error }}</p>

  <template v-else>
    <!-- Title photo -->
    <section
      class="relative h-[calc(100svh-3.5rem)] min-h-[30rem] bg-neutral-950 bg-cover bg-center"
      :style="heroStyle"
      :aria-label="data?.hero.imageAlt"
      role="img"
    >
      <div class="absolute inset-0 bg-linear-to-t from-black/65 via-black/5 to-transparent" />
      <div class="absolute inset-x-0 bottom-0 mx-auto max-w-[110rem] px-5 pb-8 md:px-10 md:pb-12">
        <p class="label text-white/70">Photography</p>
        <h1 class="mt-4 max-w-5xl text-5xl font-light leading-[0.95] tracking-tighter text-white md:text-8xl">
          {{ data?.profile.heroMessage }}
        </h1>
      </div>
    </section>

    <!-- Selected work -->
    <section class="mx-auto max-w-[110rem] px-5 pt-20 md:px-10 md:pt-28">
      <div class="flex items-end justify-between border-b border-black pb-3">
        <h2 class="label">Selected work</h2>
        <RouterLink to="/albums" class="label hover:underline underline-offset-8">All albums &rarr;</RouterLink>
      </div>
      <ul class="mt-8 grid grid-cols-2 gap-3 md:gap-5 lg:grid-cols-4">
        <li v-for="(t, i) in data?.tiles" :key="t.tile.id">
          <RouterLink :to="tileLink(t.tile.imageId)" class="group block">
            <div class="aspect-[3/4] overflow-hidden bg-neutral-100">
              <img
                :src="photoUrl(t.photo)"
                :alt="t.photo.imageAlt"
                class="photo-mono size-full object-cover group-hover:scale-[1.03]"
              />
            </div>
            <div class="label mt-3 flex items-baseline justify-between gap-3">
              <span class="truncate">{{ t.tile.name }}</span>
              <span class="tabular-nums text-neutral-400">{{ String(i + 1).padStart(2, '0') }}</span>
            </div>
          </RouterLink>
        </li>
      </ul>
    </section>

    <!-- About -->
    <section class="mx-auto max-w-[110rem] px-5 pt-24 md:px-10 md:pt-36">
      <AboutWidget />
    </section>

    <!-- Quote + CTA -->
    <section class="mx-auto max-w-[110rem] px-5 pt-24 md:px-10 md:pt-36">
      <blockquote class="max-w-4xl text-3xl font-light leading-tight tracking-tight md:text-5xl">
        &ldquo;{{ data?.profile.quote }}&rdquo;
      </blockquote>
      <RouterLink
        to="/contact"
        class="mt-16 flex items-center justify-between border-y border-black py-8 text-4xl font-light tracking-tighter transition-colors hover:bg-black hover:text-white md:px-6 md:py-12 md:text-7xl"
      >
        <span>Get in touch</span><span>&rarr;</span>
      </RouterLink>
    </section>
  </template>
</template>
