<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getAdapter, type ComicEntityTruck } from '@/api'
import { useBookshelfStore } from '@/stores/bookshelf'

const route = useRoute()
const router = useRouter()
const bookshelf = useBookshelfStore()

const entity = ref<ComicEntityTruck | null>(null)
const loading = ref(false)

const comicUrl = computed(() => (route.query.url as string) || '')

onMounted(async () => {
  if (!comicUrl.value) return
  loading.value = true
  try {
    const adapter = await getAdapter()
    entity.value = await adapter.getEntity(comicUrl.value)
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
})

function readChapter(chapterUrl: string, index: number) {
  router.push(`/reader?url=${encodeURIComponent(chapterUrl)}&entity=${encodeURIComponent(comicUrl.value)}&chapter=${index}`)
}

async function addToBookshelf() {
  if (!entity.value) return
  await bookshelf.add({
    url: comicUrl.value,
    name: entity.value.name,
    image_url: entity.value.image_url,
    descript: entity.value.descript,
    chapters_count: entity.value.chapters.length,
  })
}
</script>

<template>
  <div class="detail">
    <va-inner-loading :loading="loading">
      <div v-if="entity" class="detail__content">
        <div class="detail__header">
          <va-image
            v-if="entity.image_url"
            :src="entity.image_url"
            :ratio="3 / 4"
            class="detail__cover"
          />
          <div class="detail__info">
            <h2>{{ entity.name }}</h2>
            <p v-if="entity.descript" class="detail__desc">{{ entity.descript }}</p>
            <va-button @click="addToBookshelf" icon="bookmark" class="detail__btn">
              Add to Bookshelf
            </va-button>
          </div>
        </div>

        <h3 class="detail__chapters-title">Chapters ({{ entity.chapters.length }})</h3>
        <va-list class="detail__chapters">
          <va-list-item
            v-for="(ch, i) in entity.chapters"
            :key="ch.target_url"
            @click="readChapter(ch.target_url, i)"
          >
            <va-list-item-section>
              <va-list-item-label>{{ ch.title }}</va-list-item-label>
            </va-list-item-section>
          </va-list-item>
        </va-list>
      </div>
    </va-inner-loading>
  </div>
</template>

<style scoped>
.detail__content {
  max-width: 900px;
  margin: 0 auto;
}

.detail__header {
  display: flex;
  gap: 2rem;
  margin-bottom: 2rem;
}

.detail__cover {
  width: 200px;
  flex-shrink: 0;
  border-radius: 8px;
  overflow: hidden;
}

.detail__info {
  flex: 1;
}

.detail__desc {
  color: var(--va-text-secondary);
  margin: 1rem 0;
  line-height: 1.6;
}

.detail__btn {
  margin-top: 1rem;
}

.detail__chapters-title {
  margin-bottom: 1rem;
}

.detail__chapters {
  max-height: 500px;
  overflow-y: auto;
}
</style>
