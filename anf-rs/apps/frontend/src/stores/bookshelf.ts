import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { BookshelfItem } from '@/api/types'
import { getAdapter } from '@/api'

export const useBookshelfStore = defineStore('bookshelf', () => {
  const items = ref<BookshelfItem[]>([])
  const loading = ref(false)

  async function fetch() {
    loading.value = true
    try {
      const adapter = await getAdapter()
      items.value = await adapter.getBookshelf()
    } finally {
      loading.value = false
    }
  }

  async function add(item: Omit<BookshelfItem, 'current_chapter' | 'current_page'>) {
    const adapter = await getAdapter()
    await adapter.addToBookshelf(item)
    await fetch()
  }

  async function remove(url: string) {
    const adapter = await getAdapter()
    await adapter.removeFromBookshelf(url)
    await fetch()
  }

  return { items, loading, fetch, add, remove }
})
