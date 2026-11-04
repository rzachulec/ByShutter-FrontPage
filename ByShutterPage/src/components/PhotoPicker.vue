<script setup lang="ts">
import { computed, ref } from 'vue'
import { photoUrl, type Photo } from '../api'

const props = defineProps<{ modelValue: string; photos: Photo[]; label: string }>()
const emit = defineEmits<{ 'update:modelValue': [id: string] }>()

const dialog = ref<HTMLDialogElement>()
const current = computed(() => props.photos.find((p) => p.id === props.modelValue))

function pick(p: Photo) {
  emit('update:modelValue', p.id)
  dialog.value?.close()
}
</script>

<template>
  <div>
    <span class="label text-neutral-500">{{ label }}</span>
    <button type="button" class="mt-2 flex items-center gap-4 text-left" @click="dialog?.showModal()">
      <span class="block h-24 w-20 shrink-0 overflow-hidden bg-neutral-100">
        <img v-if="current" :src="photoUrl(current)" :alt="current.imageAlt" class="size-full object-cover" />
      </span>
      <span class="label underline underline-offset-4">{{ current ? 'Change photo' : 'Choose photo' }}</span>
    </button>

    <dialog ref="dialog" class="m-auto max-h-[88svh] w-[min(70rem,94vw)] p-5 backdrop:bg-black/60" @click.self="dialog?.close()">
      <div class="flex items-center justify-between border-b border-black pb-3">
        <h3 class="label">{{ label }}</h3>
        <button type="button" class="label" @click="dialog?.close()">Close &times;</button>
      </div>
      <p v-if="!photos.length" class="py-10 text-neutral-500">No photos uploaded yet.</p>
      <ul class="mt-4 grid grid-cols-3 gap-2 sm:grid-cols-4 md:grid-cols-6">
        <li v-for="p in photos" :key="p.id">
          <button
            type="button"
            class="block w-full outline-offset-2"
            :class="p.id === modelValue ? 'outline-2 outline-black' : ''"
            :title="p.displayName || p.fileName"
            @click="pick(p)"
          >
            <span class="block aspect-[4/5] overflow-hidden bg-neutral-100">
              <img :src="photoUrl(p)" :alt="p.imageAlt" loading="lazy" class="size-full object-cover" />
            </span>
          </button>
        </li>
      </ul>
    </dialog>
  </div>
</template>
