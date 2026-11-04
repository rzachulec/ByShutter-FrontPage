<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { signIn } from '../../composables/useAuth'
import { useAction } from '../../composables/useAction'

const route = useRoute()
const router = useRouter()
const username = ref('')
const password = ref('')
const { busy, error, run } = useAction()

async function submit() {
  const done = await run(async () => {
    try {
      await signIn(username.value, password.value)
    } catch (e) {
      throw new Error(e instanceof Error && e.message.startsWith('401') ? 'Wrong username or password.' : String(e))
    }
    return true
  })
  if (done) router.push((route.query.redirect as string) || '/config')
}
</script>

<template>
  <div class="mx-auto max-w-md px-5 pt-16 md:pt-28">
    <h1 class="text-4xl font-light tracking-tighter md:text-6xl">Sign in</h1>
    <form class="mt-12 space-y-8" @submit.prevent="submit">
      <label class="block">
        <span class="label text-neutral-500">Username</span>
        <input v-model="username" class="field" autocomplete="username" required autofocus />
      </label>
      <label class="block">
        <span class="label text-neutral-500">Password</span>
        <input v-model="password" type="password" class="field" autocomplete="current-password" required />
      </label>
      <p v-if="error" class="text-sm">{{ error }}</p>
      <button class="btn btn-solid w-full" :disabled="busy">{{ busy ? 'Signing in…' : 'Sign in' }}</button>
    </form>
  </div>
</template>
