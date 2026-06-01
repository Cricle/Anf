import type { ComicAdapter, SearchComicResult, ComicEntityTruck, WithPageChapter, ComicSnapshot, BookshelfItem, DownloadInfo } from './types'

const BASE = '/api/v1/reading'
const DL_BASE = '/api/v1/download'

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

  getImageUrl(entityUrl: string, url: string): string {
    return `${BASE}/get-image?entity_url=${encodeURIComponent(entityUrl)}&url=${encodeURIComponent(url)}`
  },

  getProposal(engineName?: string, take = 20): Promise<ComicSnapshot[]> {
    return get<ComicSnapshot[]>('/get-proposal', { engine: engineName, take })
  },

  getBookshelf(): Promise<BookshelfItem[]> {
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

  async listDownloads(): Promise<DownloadInfo[]> {
    const res = await fetch(`${DL_BASE}/list`)
    const json = await res.json()
    if (json.code !== 0) throw new Error(json.msg || 'request failed')
    return json.data ?? []
  },

  async startDownload(url: string, name: string): Promise<string> {
    const res = await fetch(`${DL_BASE}/start?url=${encodeURIComponent(url)}&name=${encodeURIComponent(name)}`)
    const json = await res.json()
    if (json.code !== 0) throw new Error(json.msg || 'request failed')
    return json.data
  },

  async getDownloadStatus(url: string): Promise<DownloadInfo | null> {
    const res = await fetch(`${DL_BASE}/status?url=${encodeURIComponent(url)}`)
    const json = await res.json()
    if (json.code !== 0) throw new Error(json.msg || 'request failed')
    return json.data ?? null
  },

  async cancelDownload(url: string): Promise<void> {
    const res = await fetch(`${DL_BASE}/cancel?url=${encodeURIComponent(url)}`)
    const json = await res.json()
    if (json.code !== 0) throw new Error(json.msg || 'request failed')
  },

  exportPdfUrl(name: string): string {
    return `${DL_BASE}/export-pdf?name=${encodeURIComponent(name)}`
  },
}
