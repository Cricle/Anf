import { invoke } from '@tauri-apps/api/core'
import type { ComicAdapter, SearchComicResult, ComicEntityTruck, WithPageChapter, ComicSnapshot, BookshelfItem, DownloadInfo } from './types'

// Cache for Tauri image blob URLs
const imageUrlCache = new Map<string, string>()

export const tauriAdapter: ComicAdapter = {
  getProviders(): Promise<string[]> {
    return invoke('get_providers')
  },

  search(keyword: string, skip = 0, take = 20): Promise<SearchComicResult> {
    return invoke('search', { keyword, skip, take })
  },

  getEntity(url: string): Promise<ComicEntityTruck> {
    return invoke('get_entity', { url })
  },

  getChapter(entityUrl: string, chapterUrl: string): Promise<WithPageChapter> {
    return invoke('get_chapter', { entityUrl, chapterUrl })
  },

  getImageUrl(entityUrl: string, url: string): string {
    const key = `${entityUrl}|${url}`
    // Return a placeholder; actual loading happens async
    if (imageUrlCache.has(key)) {
      return imageUrlCache.get(key)!
    }
    // Start async load, return empty for now
    invoke<number[]>('get_image', { entityUrl, url }).then(data => {
      const blob = new Blob([new Uint8Array(data)], { type: 'image/png' })
      const blobUrl = URL.createObjectURL(blob)
      imageUrlCache.set(key, blobUrl)
      // Dispatch event so components can react
      window.dispatchEvent(new CustomEvent('tauri-image-loaded', { detail: { key, url: blobUrl } }))
    }).catch(console.error)
    return ''
  },

  getProposal(engineName?: string, take = 20): Promise<ComicSnapshot[]> {
    return invoke('get_proposal', { engineName, take })
  },

  getBookshelf(): Promise<BookshelfItem[]> {
    return invoke('get_bookshelf')
  },

  addToBookshelf(item): Promise<void> {
    return invoke('add_to_bookshelf', {
      url: item.url,
      name: item.name,
      imageUrl: item.image_url,
      descript: item.descript,
      chaptersCount: item.chapters_count,
    })
  },

  removeFromBookshelf(url: string): Promise<void> {
    return invoke('remove_from_bookshelf', { url })
  },

  updateReadingProgress(url: string, chapter: number, page: number): Promise<void> {
    return invoke('update_reading_progress', { url, chapter, page })
  },

  listDownloads(): Promise<DownloadInfo[]> {
    return Promise.resolve([])
  },

  startDownload(): Promise<string> {
    return Promise.resolve('ok')
  },

  getDownloadStatus(): Promise<DownloadInfo | null> {
    return Promise.resolve(null)
  },

  cancelDownload(): Promise<void> {
    return Promise.resolve()
  },

  exportPdfUrl(): string {
    return ''
  },
}
