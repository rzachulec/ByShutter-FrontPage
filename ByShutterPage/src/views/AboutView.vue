<script setup lang="ts">
import { photoUrl } from '../api'
import { useProfile } from '../composables/useProfile'

const { data, error } = useProfile()
</script>

<template>
  <div class="mx-auto max-w-[110rem] px-5 pt-12 md:px-10 md:pt-20">
    <h1 class="text-5xl font-light tracking-tighter md:text-8xl">About</h1>
    <p v-if="error" class="mt-12">Could not load this page: {{ error }}</p>

    <div v-else-if="data" class="mt-12 grid gap-12 border-t border-black pt-10 md:grid-cols-[minmax(0,32rem)_1fr] lg:gap-24">
      <div class="group aspect-[4/5] overflow-hidden bg-neutral-100">
        <img :src="photoUrl(data.me)" :alt="data.me.imageAlt" class="photo-mono size-full object-cover" />
      </div>
      <div class="max-w-2xl">
        <p class="text-3xl font-light leading-tight tracking-tight md:text-5xl">{{ data.profile.greetings }}</p>
        <p class="mt-10 whitespace-pre-line leading-relaxed text-neutral-700">{{ data.profile.aboutMe }}</p>
        <blockquote class="mt-12 border-l border-black pl-6 text-xl font-light italic leading-snug">
          {{ data.profile.quote }}
        </blockquote>
        <RouterLink to="/contact" class="label mt-12 inline-block border-b border-black pb-0.5">Get in touch &rarr;</RouterLink>
      </div>
    </div>
  </div>
</template>
