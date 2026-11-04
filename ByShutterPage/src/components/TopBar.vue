<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRoute } from 'vue-router'

const links = [
  { to: '/home', text: 'Home' },
  { to: '/albums', text: 'Albums' },
  { to: '/about-me', text: 'About' },
  { to: '/contact', text: 'Contact' },
]

const open = ref(false)
const route = useRoute()
watch(() => route.fullPath, () => (open.value = false))
</script>

<template>
  <header class="sticky top-0 z-50 border-b border-black/10 bg-white/60 backdrop-blur-xl backdrop-saturate-150">
    <div class="mx-auto flex h-14 max-w-[110rem] items-center justify-between px-5 md:px-10">
      <RouterLink to="/home" class="text-sm font-semibold uppercase tracking-[0.4em]">ByShutter</RouterLink>

      <nav class="hidden items-center gap-9 md:flex" aria-label="Main">
        <RouterLink
          v-for="l in links"
          :key="l.to"
          :to="l.to"
          class="label border-b border-transparent pb-0.5 transition-colors hover:border-black"
          active-class="!border-black"
        >
          {{ l.text }}
        </RouterLink>
      </nav>

      <button
        type="button"
        class="-mr-2 grid size-10 place-items-center md:hidden"
        :aria-expanded="open"
        aria-label="Toggle menu"
        @click="open = !open"
      >
        <span class="relative block h-2.5 w-6">
          <span class="absolute inset-x-0 h-px bg-black transition-transform" :class="open ? 'top-1/2 rotate-45' : 'top-0'" />
          <span class="absolute inset-x-0 h-px bg-black transition-transform" :class="open ? 'top-1/2 -rotate-45' : 'bottom-0'" />
        </span>
      </button>
    </div>

    <nav v-if="open" class="flex flex-col gap-5 border-t border-black/10 px-5 py-6 md:hidden" aria-label="Mobile">
      <RouterLink v-for="l in links" :key="l.to" :to="l.to" class="label" active-class="underline underline-offset-8">
        {{ l.text }}
      </RouterLink>
    </nav>
  </header>
</template>
