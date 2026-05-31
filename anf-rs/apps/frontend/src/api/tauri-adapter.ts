import { invoke } from '@tauri-apps/api/core'
import type { ComicAdapter, SearchComicResult, ComicEntityTruck, WithPageChapter, ComicSnapshot, BookshelfItem } from './types'

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

  getImage(entityUrl: string, url: string): Promise<number[]> {
    return invoke('get_image', { entityUrl, url })
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
}
