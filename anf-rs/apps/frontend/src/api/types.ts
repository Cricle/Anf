export interface ComicSnapshot {
  target_url: string
  name: string
  author: string
  image_uri: string
  descript: string
  sources: { target_url: string; name: string }[]
}

export interface SearchComicResult {
  support: boolean
  snapshots: ComicSnapshot[]
  total: number | null
}

export interface ComicChapter {
  target_url: string
  title: string
}

export interface ComicEntityTruck {
  comic_url: string
  name: string
  descript: string
  image_url: string
  chapters: ComicChapter[]
  create_time: number
  update_time: number
}

export interface ComicPage {
  name: string
  target_url: string
}

export interface WithPageChapter {
  target_url: string
  title: string
  pages: ComicPage[]
  create_time: number
  update_time: number
}

export interface BookshelfItem {
  url: string
  name: string
  image_url: string
  descript: string
  current_chapter: number
  current_page: number
  chapters_count: number
}

export type DownloadStatus = 'preparing' | 'downloading' | 'completed' | 'failed' | 'cancelled'

export interface DownloadInfo {
  url: string
  name: string
  status: DownloadStatus
  total_pages: number
  downloaded_pages: number
  current_chapter: string
  error: string | null
}

export interface ComicAdapter {
  getProviders(): Promise<string[]>
  search(keyword: string, skip?: number, take?: number): Promise<SearchComicResult>
  getEntity(url: string): Promise<ComicEntityTruck>
  getChapter(entityUrl: string, chapterUrl: string): Promise<WithPageChapter>
  getImageUrl(entityUrl: string, url: string): string
  getProposal(engineName?: string, take?: number): Promise<ComicSnapshot[]>
  getBookshelf(): Promise<BookshelfItem[]>
  addToBookshelf(item: Omit<BookshelfItem, 'current_chapter' | 'current_page'>): Promise<void>
  removeFromBookshelf(url: string): Promise<void>
  updateReadingProgress(url: string, chapter: number, page: number): Promise<void>
  listDownloads(): Promise<DownloadInfo[]>
  startDownload(url: string, name: string): Promise<string>
  getDownloadStatus(url: string): Promise<DownloadInfo | null>
  cancelDownload(url: string): Promise<void>
  exportPdfUrl(name: string): string
}
