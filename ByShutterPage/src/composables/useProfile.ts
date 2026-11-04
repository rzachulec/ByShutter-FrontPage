import { ref, type Ref } from 'vue'
import { api, type HomeProfile, type Photo, type PortfolioTile } from '../api'

export interface ProfileData {
  profile: HomeProfile
  hero: Photo
  me: Photo
  tiles: { tile: PortfolioTile; photo: Photo }[]
}

// The home-profile endpoint only returns photo ids (its `path` is a server directory,
// not a file name), so every photo needs its own metadata request. Loading chain:
//   GET home-profile  ->  GET photo x (2 + tiles)  ->  image files from Caddy.
// Shared between pages so navigating Home -> About does not repeat the requests.
let cached: Promise<ProfileData> | null = null

function load(): Promise<ProfileData> {
  cached ??= (async () => {
    const [profile] = await api.homeProfiles()
    if (!profile) throw new Error('No home profile has been configured yet.')
    const ids = [profile.heroImageId, profile.profileImageId, ...profile.portfolioTiles.map((t) => t.imageId)]
    const [hero, me, ...photos] = await Promise.all(ids.map((id) => api.photo(id)))
    return { profile, hero, me, tiles: profile.portfolioTiles.map((tile, i) => ({ tile, photo: photos[i] })) }
  })()
  cached.catch(() => (cached = null))
  return cached
}

export function useProfile(): { data: Ref<ProfileData | null>; error: Ref<string | null> } {
  const data = ref<ProfileData | null>(null)
  const error = ref<string | null>(null)
  load().then(
    (d) => (data.value = d),
    (e) => (error.value = e instanceof Error ? e.message : String(e)),
  )
  return { data, error }
}
