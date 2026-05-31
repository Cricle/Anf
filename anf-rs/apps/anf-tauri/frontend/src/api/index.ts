import { invoke } from "@tauri-apps/api/core";

export interface ComicSnapshot {
  target_url: string;
  name: string;
  author: string;
  image_uri: string;
  descript: string;
  sources: { target_url: string; name: string }[];
}

export interface SearchComicResult {
  support: boolean;
  snapshots: ComicSnapshot[];
  total: number | null;
}

export interface ComicChapter {
  target_url: string;
  title: string;
}

export interface ComicEntityTruck {
  comic_url: string;
  name: string;
  descript: string;
  image_url: string;
  chapters: ComicChapter[];
  create_time: number;
  update_time: number;
}

export interface ComicPage {
  name: string;
  target_url: string;
}

export interface WithPageChapter {
  target_url: string;
  title: string;
  pages: ComicPage[];
  create_time: number;
  update_time: number;
}

export interface BookshelfItem {
  url: string;
  name: string;
  image_url: string;
  descript: string;
  current_chapter: number;
  current_page: number;
  chapters_count: number;
}

export async function getProviders(): Promise<string[]> {
  return invoke("get_providers");
}

export async function search(keyword: string, skip = 0, take = 20): Promise<SearchComicResult> {
  return invoke("search", { keyword, skip, take });
}

export async function getEntity(url: string): Promise<ComicEntityTruck> {
  return invoke("get_entity", { url });
}

export async function getChapter(entityUrl: string, chapterUrl: string): Promise<WithPageChapter> {
  return invoke("get_chapter", { entityUrl, chapterUrl });
}

export async function getImage(entityUrl: string, url: string): Promise<number[]> {
  return invoke("get_image", { entityUrl, url });
}

export async function getProposal(engineName?: string, take = 20): Promise<ComicSnapshot[]> {
  return invoke("get_proposal", { engineName, take });
}

export async function getBookshelf(): Promise<BookshelfItem[]> {
  return invoke("get_bookshelf");
}

export async function addToBookshelf(item: Omit<BookshelfItem, "current_chapter" | "current_page">): Promise<void> {
  return invoke("add_to_bookshelf", {
    url: item.url, name: item.name, imageUrl: item.image_url,
    descript: item.descript, chaptersCount: item.chapters_count,
  });
}

export async function removeFromBookshelf(url: string): Promise<void> {
  return invoke("remove_from_bookshelf", { url });
}

export async function updateReadingProgress(url: string, chapter: number, page: number): Promise<void> {
  return invoke("update_reading_progress", { url, chapter, page });
}
