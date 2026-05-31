import type { ComicAdapter, SearchComicResult, ComicEntityTruck, WithPageChapter, ComicSnapshot, BookshelfItem } from './types'

const BASE = '/api/v1/reading'

interface EntityResult<T> {
  code: number
  msg?: string
  data?: T
}

async function get<T>(path: string, params: Record<string, string | number | undefined> = {}): Promise<T> {
  const qs = Object.entries(params)
    .filter(([, v]) => v !== undefined)
    .map(([k, v]) => `${k}=${encodeURIComponent(String(v))}`)
    .join('&')
  const url = qs ? `${BASE}${path}?${qs}` : `${BASE}${path}`
  const res = await fetch(url)
  const json: EntityResult<T> = await res.json()
  if (json.code !== 0 || json.data === undefined) {
    throw new Error(json.msg || 'request failed')
  }
  return json.data
}

export const httpAdapter: ComicAdapter = {
  getProviders(): Promise<string[]> {
    return get<string[]>('/get-providers')
  },

  search(keyword: string, skip = 0, take = 20): Promise<SearchComicResult> {
    return get<SearchComicResult>('/search', { keyword, skip, take })
  },

  getEntity(url: string): Promise<ComicEntityTruck> {
    return get<ComicEntityTruck>('/get-entity', { url })
  },

  getChapter(entityUrl: string, chapterUrl: string): Promise<WithPageChapter> {
    return get<WithPageChapter>('/get-chapter', { url: chapterUrl, entity_url: entityUrl })
  },

  async getImage(entityUrl: string, url: string): Promise<number[]> {
    const qs = `entity_url=${encodeURIComponent(entityUrl)}&url=${encodeURIComponent(url)}`
    const res = await fetch(`${BASE}/get-image?${qs}`)
    const buffer = await res.arrayBuffer()
    return Array.from(new Uint8Array(buffer))
  },

  getProposal(engineName?: string, take = 20): Promise<ComicSnapshot[]> {
    return get<ComicSnapshot[]>('/get-proposal', { engine: engineName, take })
  },

  getBookshelf(): Promise<BookshelfItem[]> {
    // Web mode has no bookshelf - return empty
    return Promise.resolve([])
  },

  addToBookshelf(): Promise<void> {
    return Promise.resolve()
  },

  removeFromBookshelf(): Promise<void> {
    return Promise.resolve()
  },

  updateReadingProgress(): Promise<void> {
    return Promise.resolve()
  },
}
