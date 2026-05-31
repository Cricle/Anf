<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useBookshelfStore } from '@/stores/bookshelf'
import ComicCard from '@/components/ComicCard.vue'
import type { ComicSnapshot } from '@/api/types'

const router = useRouter()
const bookshelf = useBookshelfStore()

onMounted(() => {
  bookshelf.fetch()
})

function toSnapshot(item: any): ComicSnapshot {
  return {
    target_url: item.url,
    name: item.name,
    author: '',
    image_uri: item.image_url,
    descript: item.descript,
    sources: [{ target_url: item.url, name: '' }],
  }
}

function handleClick(url: string) {
  router.push(`/comic?url=${encodeURIComponent(url)}`)
}

async function handleRemove(url: string, e: Event) {
  e.stopPropagation()
  await bookshelf.remove(url)
}
</script>

<template>
  <div class="bookshelf">
    <h2>Bookshelf</h2>
    <va-inner-loading :loading="bookshelf.loading">
      <div v-if="bookshelf.items.length === 0" class="bookshelf__empty">
        <va-icon name="bookmark_border" size="large" />
        <p>No comics in bookshelf</p>
      </div>
      <div v-else class="bookshelf__grid">
        <div v-for="item in bookshelf.items" :key="item.url" class="bookshelf__item">
          <ComicCard
            :snapshot="toSnapshot(item)"
            @click="handleClick(item.url)"
          />
          <va-button
            preset="plain"
            icon="delete"
            class="bookshelf__delete"
            @click="handleRemove(item.url, $event)"
          />
        </div>
      </div>
    </va-inner-loading>
  </div>
</template>

<style scoped>
.bookshelf__empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem;
  color: var(--va-text-secondary);
}

.bookshelf__grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: 1rem;
}

.bookshelf__item {
  position: relative;
}

.bookshelf__delete {
  position: absolute;
  top: 4px;
  right: 4px;
  z-index: 1;
}
</style>
