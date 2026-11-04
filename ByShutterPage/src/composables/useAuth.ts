import { ref } from 'vue'
import { admin } from '../api'

/** Signed-in username, or null. undefined = not checked yet. */
export const user = ref<string | null | undefined>(undefined)

export async function checkAuth(force = false): Promise<boolean> {
  if (user.value === undefined || force) {
    user.value = await admin.me().then((m) => m.username, () => null)
  }
  return !!user.value
}

export async function signIn(username: string, password: string) {
  await admin.login(username, password)
  await checkAuth(true)
}

export async function signOut() {
  await admin.logout().catch(() => {})
  user.value = null
}
