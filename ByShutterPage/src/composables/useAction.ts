import { ref } from 'vue'

/** Runs an async action, tracking busy state and a status message for the UI. */
export function useAction() {
  const busy = ref(false)
  const ok = ref<string | null>(null)
  const error = ref<string | null>(null)

  async function run<T>(fn: () => Promise<T>, success?: string): Promise<T | undefined> {
    busy.value = true
    ok.value = error.value = null
    try {
      const r = await fn()
      if (success) ok.value = success
      return r
    } catch (e) {
      error.value = e instanceof Error ? e.message : String(e)
    } finally {
      busy.value = false
    }
  }
  return { busy, ok, error, run }
}
