import { computed, ref } from 'vue'
import { admin, type Photo } from '../api'

/** All uploaded photos (paged in blocks of 100), indexed by id. Used by the admin pickers. */
export function usePhotoLibrary() {
  const photos = ref<Photo[]>([])
  const byId = computed(() => new Map(photos.value.map((p) => [p.id, p])))

  async function reload() {
    const all: Photo[] = []
    for (let page = 0; ; page++) {
      const batch = await admin.photos(100, page)
      all.push(...batch)
      if (batch.length < 100) break
    }
    photos.value = all
  }
  return { photos, byId, reload }
}
