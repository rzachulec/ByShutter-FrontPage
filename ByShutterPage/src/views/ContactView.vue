<script setup lang="ts">
import { CONTACT_EMAIL } from '../api'
</script>

<template>
  <div class="mx-auto max-w-[110rem] px-5 pt-12 md:px-10 md:pt-20">
    <h1 class="text-5xl font-light tracking-tighter md:text-8xl">Contact</h1>
    <div class="mt-12 border-t border-black pt-10">
      <p class="max-w-xl text-lg leading-relaxed text-neutral-600">
        Portraits, editorial and commissions. Tell me about your project, the place and the date, and I will get back to you.
      </p>
      <a
        :href="`mailto:${CONTACT_EMAIL}`"
        class="mt-12 block break-all border-y border-black py-8 text-3xl font-light tracking-tighter transition-colors hover:bg-black hover:text-white md:px-6 md:py-12 md:text-6xl"
      >
        {{ CONTACT_EMAIL }}
      </a>
    </div>
  </div>
</template>
