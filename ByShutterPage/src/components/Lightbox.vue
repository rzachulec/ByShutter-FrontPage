<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { originalUrl, photoUrl, type Photo } from '../api'

// Full-screen viewer built on the native <dialog>. It shows the web-sized photo (already in the
// browser cache from the grid). The untouched original is only fetched if the visitor explicitly
// follows "View full image" - it is never part of the page's own loading.
const props = defineProps<{ photos: Photo[]; index: number | null }>()
const emit = defineEmits<{ close: []; 'update:index': [index: number] }>()

const dialog = ref<HTMLDialogElement>()
const photo = computed(() => (props.index === null ? null : (props.photos[props.index] ?? null)))
const original = computed(() => (photo.value ? originalUrl(photo.value) : null))

watch(
  () => props.index,
  (i) => {
    const d = dialog.value
    if (!d) return
    if (i !== null && !d.open) d.showModal()
    else if (i === null && d.open) d.close()
  },
  { flush: 'post' },
)

function step(delta: number) {
  if (props.index === null) return
  const n = props.photos.length
  emit('update:index', (props.index + delta + n) % n)
}

// ---- details panel -------------------------------------------------------------
// The API stores no resolution or byte size, so they are measured here: pixel size from the loaded
// <img>, byte size from a HEAD request (best effort: shown as "-" if the server hides it).
const showDetails = ref(false)
const natural = ref<{ w: number; h: number } | null>(null)
const bytes = reactive<Record<string, number | null>>({})

watch(photo, () => (natural.value = null))
const onLoad = (e: Event) => {
  const i = e.target as HTMLImageElement
  natural.value = { w: i.naturalWidth, h: i.naturalHeight }
}

async function measure(url: string) {
  if (url in bytes) return
  bytes[url] = null
  try {
    const n = Number((await fetch(url, { method: 'HEAD' })).headers.get('content-length'))
    bytes[url] = n > 0 ? n : null
  } catch {
    /* cross-origin without CORS: leave as unknown */
  }
}
watch(
  [showDetails, photo],
  () => {
    if (!showDetails.value || !photo.value) return
    measure(photoUrl(photo.value))
    if (original.value) measure(original.value)
  },
  { immediate: true },
)

const fmtBytes = (b?: number | null) => (b == null ? '-' : b >= 1e6 ? `${(b / 1e6).toFixed(2)} MB` : `${Math.round(b / 1e3)} kB`)
const fmtDate = (s: string) => new Date(s).toLocaleDateString('en-GB', { day: 'numeric', month: 'long', year: 'numeric' })
const ext = (n: string) => n.split('.').pop()?.toUpperCase() ?? ''

const rows = computed<[string, string][]>(() => {
  const p = photo.value
  if (!p) return []
  const out: [string, string][] = [
    ['Title', p.displayName || p.fileName],
    ['Uploaded', fmtDate(p.uploadedAt)],
    ['Shown as', `${ext(p.fileName)} · ${natural.value ? `${natural.value.w} × ${natural.value.h} px` : '-'} · ${fmtBytes(bytes[photoUrl(p)])}`],
  ]
  if (original.value) out.push(['Original', `${p.originalFileName} · ${fmtBytes(bytes[original.value])}`])
  if (p.description) out.push(['Description', p.description])
  out.push(['Alt text', p.imageAlt])
  return out
})
</script>

<template>
  <dialog
    ref="dialog"
    class="m-0 h-svh max-h-none w-screen max-w-none bg-black p-0 text-white open:flex open:flex-col"
    @close="emit('close')"
    @click.self="dialog?.close()"
    @keydown.left.prevent="step(-1)"
    @keydown.right.prevent="step(1)"
  >
    <div class="flex h-14 shrink-0 items-center justify-between px-5 md:px-10">
      <span class="label tabular-nums text-white/60" v-if="photo && index !== null">
        {{ String(index + 1).padStart(2, '0') }} / {{ String(photos.length).padStart(2, '0') }}
      </span>
      <button type="button" class="label -mr-2 px-2 py-2 hover:underline underline-offset-8" @click="dialog?.close()">
        Close &times;
      </button>
    </div>

    <!-- Image on the left, details panel on the right (desktop) or stacked above the buttons (mobile). -->
    <div class="flex min-h-0 flex-1 flex-col md:flex-row">
      <div class="relative flex min-h-0 flex-1 items-center justify-center md:px-20" @click.self="dialog?.close()">
        <button
          type="button"
          class="label absolute z-10 mix-blend-difference left-2 top-1/2 -translate-y-1/2 px-3 py-6 text-white/70 hover:text-white md:left-6"
          aria-label="Previous photo"
          @click="step(-1)"
        >
          &larr;
        </button>
        <img
          v-if="photo"
          :src="photoUrl(photo)"
          :alt="photo.imageAlt"
          class="size-full object-contain"
          @load="onLoad"
        />
        <button
          type="button"
          class="label absolute z-10 mix-blend-difference right-2 top-1/2 -translate-y-1/2 px-3 py-6 text-white/70 hover:text-white md:right-6"
          aria-label="Next photo"
          @click="step(1)"
        >
          &rarr;
        </button>
      </div>

      <aside
        v-if="showDetails && photo"
        id="photo-details"
        class="max-h-[38svh] shrink-0 overflow-y-auto border-t border-white/20 px-5 py-5 md:max-h-none md:w-80 md:border-l md:border-t-0 md:px-6"
        aria-label="Photo details"
      >
        <h3 class="label text-white/60">Details</h3>
        <dl class="mt-4 space-y-4 text-sm">
          <div v-for="[k, v] in rows" :key="k">
            <dt class="label text-white/50">{{ k }}</dt>
            <dd class="mt-1 break-words tabular-nums">{{ v }}</dd>
          </div>
        </dl>
      </aside>
    </div>

    <div class="flex h-20 shrink-0 items-center justify-between gap-4 px-5 md:px-10">
      <p class="min-w-0 truncate text-sm text-white/70">{{ photo?.displayName || photo?.imageAlt }}</p>
      <div class="flex shrink-0 gap-2">
        <button
          type="button"
          class="label border border-white px-4 py-2.5 transition-colors hover:bg-white hover:text-black"
          :class="showDetails && 'bg-white text-black'"
          :aria-expanded="showDetails"
          aria-controls="photo-details"
          @click="showDetails = !showDetails"
        >
          Details
        </button>
        <a
          v-if="original"
          :href="original"
          target="_blank"
          rel="noopener"
          class="label border border-white px-4 py-2.5 transition-colors hover:bg-white hover:text-black"
        >
          <span class="hidden sm:inline">View full image</span><span class="sm:hidden">Full</span> &nearr;
        </a>
      </div>
    </div>
  </dialog>
</template>
