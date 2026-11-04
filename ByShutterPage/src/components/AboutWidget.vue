<script setup lang="ts">
import { photoUrl } from '../api'
import { useProfile } from '../composables/useProfile'

// Blogspot-style "About me" widget: small portrait, a few lines, link to the full page.
defineProps<{ compact?: boolean }>()
const { data } = useProfile()
</script>

<template>
  <section v-if="data" class="group">
    <h2 class="label border-b border-black pb-2">About me</h2>
    <div class="mt-5" :class="compact ? 'space-y-4' : 'grid gap-10 md:grid-cols-[minmax(0,22rem)_1fr]'">
      <div class="aspect-[4/5] overflow-hidden bg-neutral-100" :class="compact && 'w-40'">
        <img
          :src="photoUrl(data.me)"
          :alt="data.me.imageAlt"
          class="photo-mono size-full object-cover"
        />
      </div>
      <div>
        <p :class="compact ? 'text-lg font-light leading-snug' : 'text-3xl font-light leading-tight tracking-tight md:text-4xl'">
          {{ data.profile.greetings }}
        </p>
        <p
          class="mt-4 leading-relaxed text-neutral-600"
          :class="compact ? 'line-clamp-4 text-sm' : 'max-w-xl text-base md:text-lg'"
        >
          {{ data.profile.aboutMe }}
        </p>
        <RouterLink to="/about-me" class="label mt-5 inline-block border-b border-black pb-0.5">
          View my complete profile &rarr;
        </RouterLink>
      </div>
    </div>
  </section>
</template>
