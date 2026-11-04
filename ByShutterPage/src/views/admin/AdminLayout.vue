<script setup lang="ts">
import { useRouter } from 'vue-router'
import { signOut, user } from '../../composables/useAuth'

const router = useRouter()
const tabs = [
  { to: '/config/photos', text: 'Photos' },
  { to: '/config/albums', text: 'Albums' },
  { to: '/config/tiles', text: 'Tiles' },
  { to: '/config/home', text: 'Home page' },
]

async function logout() {
  await signOut()
  router.push('/config/login')
}
</script>

<template>
  <div class="mx-auto max-w-[110rem] px-5 pt-10 md:px-10 md:pt-16">
    <div class="flex flex-wrap items-end justify-between gap-4">
      <h1 class="text-4xl font-light tracking-tighter md:text-6xl">Admin</h1>
      <p class="label text-neutral-500">
        {{ user }} <button type="button" class="ml-4 text-black underline underline-offset-4" @click="logout">Sign out</button>
      </p>
    </div>
    <nav class="mt-8 flex flex-wrap gap-x-8 gap-y-3 border-y border-black py-4" aria-label="Admin">
      <RouterLink v-for="t in tabs" :key="t.to" :to="t.to" class="label border-b border-transparent pb-0.5 hover:border-black" active-class="!border-black">
        {{ t.text }}
      </RouterLink>
    </nav>
    <div class="pt-10">
      <RouterView />
    </div>
  </div>
</template>
