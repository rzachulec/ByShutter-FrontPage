<script setup lang="ts">
const year = new Date().getFullYear()
const toTop = () => window.scrollTo({ top: 0 })
</script>

<template>
  <footer class="mt-24 border-t border-black">
    <div class="mx-auto flex max-w-[110rem] flex-col gap-6 px-5 py-10 md:flex-row md:items-end md:justify-between md:px-10">
      <div>
        <p class="text-sm font-semibold uppercase tracking-[0.4em]">ByShutter</p>
        <p class="label mt-3 text-neutral-500">&copy; {{ year }} All photographs reserved</p>
      </div>
      <nav class="flex flex-wrap gap-x-8 gap-y-3" aria-label="Footer">
        <RouterLink to="/albums" class="label hover:underline underline-offset-8">Albums</RouterLink>
        <RouterLink to="/about-me" class="label hover:underline underline-offset-8">About</RouterLink>
        <RouterLink to="/contact" class="label hover:underline underline-offset-8">Contact</RouterLink>
        <RouterLink to="/config" class="label text-neutral-400 hover:text-black hover:underline underline-offset-8">Admin</RouterLink>
        <button type="button" class="label hover:underline underline-offset-8" @click="toTop">Back to top &uarr;</button>
      </nav>
    </div>
  </footer>
</template>
