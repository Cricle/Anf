import { defineStore } from 'pinia'
import { ref, watch } from 'vue'
import type { ComicSnapshot } from '@/api/types'

export const useAppStore = defineStore('app', () => {
  const keyword = ref('')
  const searchResults = ref<ComicSnapshot[]>([])
  const searching = ref(false)
  const darkMode = ref(localStorage.getItem('anf-dark') === 'true')

  watch(darkMode, (v) => {
    localStorage.setItem('anf-dark', String(v))
    document.documentElement.classList.toggle('dark', v)
  }, { immediate: true })

  function setKeyword(v: string) {
    keyword.value = v
  }

  function setSearchResults(v: ComicSnapshot[]) {
    searchResults.value = v
  }

  function setSearching(v: boolean) {
    searching.value = v
  }

  function toggleDark() {
    darkMode.value = !darkMode.value
  }

  return { keyword, searchResults, searching, darkMode, setKeyword, setSearchResults, setSearching, toggleDark }
})
