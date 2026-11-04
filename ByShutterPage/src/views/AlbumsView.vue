<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { api, photoUrl, type Album, type Photo } from '../api'
import AboutWidget from '../components/AboutWidget.vue'

// "Journal" layout: each album is a post (date header, title, cover, link), with a sidebar of
// widgets and Older/Newer pagination in the spirit of a classic blog.
const PAGE_SIZE = 5

interface Post {
  album: Album
  cover: Photo | null
  date: Date
}

const route = useRoute()
const page = computed(() => Math.max(0, Number(route.query.page ?? 0) || 0))
const posts = ref<Post[] | null>(null)
const error = ref<string | null>(null)

watch(
  page,
  async (p) => {
    posts.value = null
    error.value = null
    try {
      const albums = await api.albums(PAGE_SIZE, p)
      // One metadata request per album cover (the albums endpoint does not return file names).
      posts.value = await Promise.all(
        albums.map(async (album) => {
          const first = album.photos[0]
          const cover = first ? await api.photo(first.photoId) : null
          const date = new Date(album.date ?? first?.uploadedAt ?? Date.now())
          return { album, cover, date }
        }),
      )
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
    }
  },
  { immediate: true },
)

const fmtDay = new Intl.DateTimeFormat('en-GB', { day: 'numeric', month: 'long', year: 'numeric' })
const fmtMonth = new Intl.DateTimeFormat('en-GB', { month: 'long', year: 'numeric' })

const archive = computed(() => {
  const groups = new Map<string, Post[]>()
  for (const p of posts.value ?? []) {
    const key = fmtMonth.format(p.date)
    groups.set(key, [...(groups.get(key) ?? []), p])
  }
  return [...groups]
})
const hasOlder = computed(() => (posts.value?.length ?? 0) === PAGE_SIZE)
</script>

<template>
  <div class="mx-auto grid max-w-[110rem] gap-16 px-5 pb-8 pt-12 md:px-10 md:pt-20 lg:grid-cols-[minmax(0,1fr)_22rem] xl:gap-28">
    <!-- Posts -->
    <div>
      <h1 class="text-5xl font-light tracking-tighter md:text-7xl">Albums</h1>

      <p v-if="error" class="mt-12">Could not load albums: {{ error }}</p>
      <p v-else-if="posts && !posts.length" class="mt-12 text-neutral-500">Nothing here yet.</p>

      <article v-for="p in posts" :key="p.album.id" class="group mt-16 border-t border-black pt-5">
        <p class="label text-neutral-500">
          {{ fmtDay.format(p.date) }} <span class="mx-2">/</span> {{ p.album.photos.length }} photographs
        </p>
        <h2 class="mt-4 text-3xl font-light tracking-tight md:text-5xl">
          <RouterLink :to="`/albums/${p.album.id}`" class="hover:underline decoration-1 underline-offset-8">
            {{ p.album.name }}
          </RouterLink>
        </h2>
        <p v-if="p.album.description" class="mt-4 max-w-prose leading-relaxed text-neutral-600">
          {{ p.album.description }}
        </p>
        <RouterLink v-if="p.cover" :to="`/albums/${p.album.id}`" class="mt-8 block aspect-[16/10] overflow-hidden bg-neutral-100">
          <img
            :src="photoUrl(p.cover)"
            :alt="p.cover.imageAlt"
            loading="lazy"
            class="photo-mono size-full object-cover group-hover:scale-[1.02]"
          />
        </RouterLink>
        <RouterLink :to="`/albums/${p.album.id}`" class="label mt-6 inline-block border-b border-black pb-0.5">
          View album &rarr;
        </RouterLink>
      </article>

      <nav v-if="posts" class="label mt-16 flex justify-between border-t border-black pt-5" aria-label="Pagination">
        <RouterLink v-if="page > 0" :to="{ query: page > 1 ? { page: page - 1 } : {} }">&laquo; Newer posts</RouterLink>
        <span v-else />
        <RouterLink v-if="hasOlder" :to="{ query: { page: page + 1 } }">Older posts &raquo;</RouterLink>
      </nav>
    </div>

    <!-- Sidebar widgets -->
    <aside class="space-y-14 lg:pt-4">
      <AboutWidget compact />

      <section v-if="archive.length">
        <h2 class="label border-b border-black pb-2">Archive</h2>
        <ul class="mt-5 space-y-5 text-sm">
          <li v-for="[month, items] in archive" :key="month">
            <p class="font-medium">{{ month }} <span class="text-neutral-400">({{ items.length }})</span></p>
            <ul class="mt-2 space-y-1.5 border-l border-black/15 pl-4 text-neutral-600">
              <li v-for="p in items" :key="p.album.id">
                <RouterLink :to="`/albums/${p.album.id}`" class="hover:text-black hover:underline underline-offset-4">
                  {{ p.album.name }}
                </RouterLink>
              </li>
            </ul>
          </li>
        </ul>
      </section>
    </aside>
  </div>
</template>
