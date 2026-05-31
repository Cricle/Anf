import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { ComicSnapshot } from '@/api/types'

export const useAppStore = defineStore('app', () => {
  const keyword = ref('')
  const searchResults = ref<ComicSnapshot[]>([])
  const searching = ref(false)

  function setKeyword(v: string) {
    keyword.value = v
  }

  function setSearchResults(v: ComicSnapshot[]) {
    searchResults.value = v
  }

  function setSearching(v: boolean) {
    searching.value = v
  }

  return { keyword, searchResults, searching, setKeyword, setSearchResults, setSearching }
})
