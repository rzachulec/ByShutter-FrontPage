<script setup lang="ts">
import TopBar from './components/TopBar.vue'
import SiteFooter from './components/SiteFooter.vue'
</script>

<template>
  <TopBar />
  <main class="min-h-[70svh]">
    <RouterView />
  </main>
  <SiteFooter />
</template>
