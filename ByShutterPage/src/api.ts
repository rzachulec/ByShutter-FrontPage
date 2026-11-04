// Thin client for the ByShutter API plus URL helpers for the Caddy file server.
const API = import.meta.env.VITE_API_URL ?? 'http://localhost:5700'
const IMAGES = import.meta.env.VITE_IMAGES_URL ?? 'https://localhost:5702'

export interface Photo {
  id: string
  fileName: string
  /** Name of the untouched upload under /originals/. Absent for photos stored before originals were kept. */
  originalFileName?: string | null
  imageAlt: string
  uploadedAt: string
  displayName?: string | null
  description?: string | null
}

export interface PortfolioTile {
  id: string
  name: string
  description?: string | null
  imageId: string
}

export interface HomeProfile {
  id: string
  heroMessage: string
  greetings: string
  quote: string
  aboutMe: string
  heroImageId: string
  profileImageId: string
  portfolioTiles: PortfolioTile[]
}

export interface Album {
  id: string
  name: string
  description: string
  /** Date chosen for the album; null/absent means "use the first photo's date". */
  date?: string | null
  photos: { albumId: string; photoId: string; uploadedAt: string }[]
}

async function get<T>(path: string): Promise<T> {
  const res = await fetch(API + path)
  if (!res.ok) throw new Error(`${res.status} ${res.statusText} - ${path}`)
  return res.json() as Promise<T>
}

export class ApiError extends Error {
  constructor(public status: number, message: string) {
    super(message)
  }
}

/** Authenticated request (session cookie) for the admin area. */
async function send<T = void>(method: string, path: string, body?: unknown): Promise<T> {
  const init: RequestInit = { method, credentials: 'include' }
  if (body instanceof FormData) init.body = body
  else if (body !== undefined) {
    init.headers = { 'content-type': 'application/json' }
    init.body = JSON.stringify(body)
  }
  const res = await fetch(API + path, init)
  if (!res.ok) {
    const detail = (await res.text().catch(() => '')).slice(0, 200)
    throw new ApiError(res.status, `${res.status} ${res.statusText}${detail ? ` - ${detail}` : ''}`)
  }
  if (res.status === 204) return undefined as T
  return (res.headers.get('content-type')?.includes('json') ? res.json() : res.text()) as Promise<T>
}

export interface Tile {
  id: string
  name: string
  description?: string | null
  imageId: string
}

export interface ProfileInput {
  heroMessage: string
  greetings: string
  quote: string
  aboutMe: string
  heroImageId: string
  profileImageId: string
}

export const admin = {
  login: (username: string, password: string) => send('POST', '/api/auth/login', { username, password }),
  logout: () => send('POST', '/api/auth/logout'),
  me: () => send<{ username: string }>('GET', '/api/auth/me'),

  photos: (pageSize: number, page: number) => send<Photo[]>('GET', `/api/photos/${pageSize}/${page}`),
  uploadPhoto: (form: FormData) => send('POST', '/api/photos', form),
  updatePhoto: (id: string, v: { displayName: string; description: string; photoAlt: string }) =>
    send('PUT', `/api/photos/${id}`, v),
  deletePhoto: (id: string) => send('DELETE', `/api/photos/${id}`),

  createAlbum: (name: string, description: string) => send('POST', '/api/albums', { name, description }),
  updateAlbum: (id: string, v: { name: string; description: string; date: string | null }) => send('PUT', `/api/albums/${id}`, v),
  deleteAlbum: (id: string) => send('DELETE', `/api/albums/${id}`),
  availablePhotos: (albumId: string, page: number, pageSize: number) =>
    send<Photo[]>('GET', `/api/photos/${albumId}/avaiable-photos?page=${page}&pageSize=${pageSize}`),
  addToAlbum: (id: string, imageIds: string[]) => send('PUT', `/api/albums/${id}/add-images`, { imageIds }),
  removeFromAlbum: (id: string, imageIds: string[]) => send('DELETE', `/api/albums/${id}/images`, { imageIds }),

  tiles: (pageSize: number, page: number) => send<Tile[]>('GET', `/api/portfolio-tiles/${pageSize}/${page}`),
  createTile: (v: { name: string; imageId: string; description: string }) => send<string>('POST', '/api/portfolio-tiles', v),
  updateTile: (id: string, v: { name: string; imageId: string; description: string }) => send('PUT', `/api/portfolio-tiles/${id}`, v),
  deleteTile: (id: string) => send('DELETE', `/api/portfolio-tiles/${id}`),

  profiles: () => send<HomeProfile[]>('GET', '/api/home-profile/20/0'),
  createProfile: (v: ProfileInput) => send<string>('POST', '/api/home-profile', v),
  updateProfile: (id: string, v: ProfileInput) => send('PUT', `/api/home-profile/${id}`, v),
  deleteProfile: (id: string) => send('DELETE', `/api/home-profile/${id}`),
  assignTile: (profileId: string, tileId: string) => send('PUT', `/api/home-profile/${profileId}/portfolio-tiles/${tileId}`),
  unassignTile: (profileId: string, tileId: string) => send('DELETE', `/api/home-profile/${profileId}/portfolio-tiles/${tileId}`),
}

export const api = {
  homeProfiles: () => get<HomeProfile[]>('/api/home-profile/1/0'),
  photo: (id: string) => get<Photo>(`/api/photos/${id}`),
  albums: (pageSize: number, page: number) => get<Album[]>(`/api/albums/${pageSize}/${page}`),
  album: (id: string) => get<Album>(`/api/albums/${id}`),
  albumPhotos: (id: string, pageSize = 100, page = 0) =>
    get<Photo[]>(`/api/albums/${id}/images/${pageSize}/${page}`),
}

export const photoUrl = (p: Pick<Photo, 'fileName'>) => `${IMAGES}/${encodeURIComponent(p.fileName)}`

export const originalUrl = (p: Pick<Photo, 'originalFileName'>) =>
  p.originalFileName ? `${IMAGES}/originals/${encodeURIComponent(p.originalFileName)}` : null

export const CONTACT_EMAIL = import.meta.env.VITE_CONTACT_EMAIL ?? 'hello@byshutter.example'
