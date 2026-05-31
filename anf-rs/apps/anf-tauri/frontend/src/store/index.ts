import { create } from "zustand";
import type { ComicSnapshot, ComicEntityTruck, WithPageChapter, BookshelfItem } from "../api";

interface AppState {
  // Search
  keyword: string;
  setKeyword: (k: string) => void;
  searchResults: ComicSnapshot[];
  setSearchResults: (r: ComicSnapshot[]) => void;
  searching: boolean;
  setSearching: (v: boolean) => void;

  // Current comic
  currentComic: ComicEntityTruck | null;
  setCurrentComic: (c: ComicEntityTruck | null) => void;

  // Current chapter
  currentChapter: WithPageChapter | null;
  setCurrentChapter: (c: WithPageChapter | null) => void;
  currentChapterIndex: number;
  setCurrentChapterIndex: (i: number) => void;

  // Bookshelf
  bookshelf: BookshelfItem[];
  setBookshelf: (items: BookshelfItem[]) => void;

  // Page
  currentPageIndex: number;
  setCurrentPageIndex: (i: number) => void;
}

export const useStore = create<AppState>((set) => ({
  keyword: "",
  setKeyword: (keyword) => set({ keyword }),
  searchResults: [],
  setSearchResults: (searchResults) => set({ searchResults }),
  searching: false,
  setSearching: (searching) => set({ searching }),

  currentComic: null,
  setCurrentComic: (currentComic) => set({ currentComic }),

  currentChapter: null,
  setCurrentChapter: (currentChapter) => set({ currentChapter }),
  currentChapterIndex: 0,
  setCurrentChapterIndex: (currentChapterIndex) => set({ currentChapterIndex }),

  bookshelf: [],
  setBookshelf: (bookshelf) => set({ bookshelf }),

  currentPageIndex: 0,
  setCurrentPageIndex: (currentPageIndex) => set({ currentPageIndex }),
}));
