<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getAdapter } from '@/api'
import type { ComicEntityTruck } from '@/api'
import { useBookshelfStore } from '@/stores/bookshelf'

const route = useRoute()
const router = useRouter()
const bookshelf = useBookshelfStore()

const entity = ref<ComicEntityTruck | null>(null)
const loading = ref(false)
const error = ref('')
const downloading = ref(false)

const comicUrl = computed(() => (route.query.url as string) || '')

onMounted(async () => {
  if (!comicUrl.value) return
  loading.value = true
  error.value = ''
  try {
    const adapter = await getAdapter()
    entity.value = await adapter.getEntity(comicUrl.value)
  } catch (e: any) {
    error.value = e.message || 'Failed to load comic'
  } finally {
    loading.value = false
  }
})

function readChapter(chapterUrl: string, index: number) {
  router.push(`/reader?url=${encodeURIComponent(chapterUrl)}&entity=${encodeURIComponent(comicUrl.value)}&chapter=${index}`)
}

function readFromBeginning() {
  if (!entity.value || entity.value.chapters.length === 0) return
  readChapter(entity.value.chapters[0].target_url, 0)
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

async function startDownload() {
  if (!entity.value) return
  downloading.value = true
  try {
    const adapter = await getAdapter()
    await adapter.startDownload(comicUrl.value, entity.value.name)
  } catch (e: any) {
    console.error('download failed:', e)
  } finally {
    downloading.value = false
  }
}

function goBack() {
  router.back()
}
</script>

<template>
  <div class="detail">
    <va-button preset="secondary" icon="arrow_back" class="detail__back" @click="goBack">Back</va-button>

    <va-inner-loading :loading="loading">
      <div v-if="error" class="detail__error">
        <va-icon name="error_outline" size="large" />
        <p>{{ error }}</p>
        <va-button @click="$router.push('/')">Go Home</va-button>
      </div>

      <div v-else-if="entity" class="detail__content">
        <div class="detail__header">
          <va-image
            v-if="entity.image_url"
            :src="entity.image_url"
            :ratio="3 / 4"
            class="detail__cover"
          >
            <template #error>
              <div class="detail__cover-placeholder">
                <va-icon name="image" size="large" />
              </div>
            </template>
          </va-image>
          <div class="detail__info">
            <h2 class="detail__name">{{ entity.name }}</h2>
            <p v-if="entity.descript" class="detail__desc">{{ entity.descript }}</p>
            <div class="detail__actions">
              <va-button @click="readFromBeginning" icon="play_arrow" class="detail__btn">
                Read from beginning
              </va-button>
              <va-button @click="addToBookshelf" preset="secondary" icon="bookmark" class="detail__btn">
                Add to Bookshelf
              </va-button>
              <va-button
                @click="startDownload"
                preset="secondary"
                icon="download"
                class="detail__btn"
                :loading="downloading"
              >
                Download
              </va-button>
            </div>
          </div>
        </div>

        <div class="detail__chapters-section">
          <h3 class="detail__chapters-title">Chapters ({{ entity.chapters.length }})</h3>
          <va-list class="detail__chapters">
            <va-list-item
              v-for="(ch, i) in entity.chapters"
              :key="ch.target_url"
              class="detail__chapter-item"
              @click="readChapter(ch.target_url, i)"
            >
              <va-list-item-section>
                <va-list-item-label>
                  <span class="detail__chapter-index">{{ i + 1 }}</span>
                  {{ ch.title }}
                </va-list-item-label>
              </va-list-item-section>
            </va-list-item>
          </va-list>
        </div>
      </div>
    </va-inner-loading>
  </div>
</template>

<style scoped>
.detail {
  max-width: 900px;
  margin: 0 auto;
}

.detail__back {
  margin-bottom: 1rem;
}

.detail__error {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  padding: 4rem;
  color: var(--va-text-secondary);
}

.detail__content {
  display: flex;
  flex-direction: column;
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

.detail__cover-placeholder {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 267px;
  background: var(--va-background-secondary);
}

.detail__info {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.detail__name {
  margin: 0 0 0.5rem;
}

.detail__desc {
  color: var(--va-text-secondary);
  margin: 0 0 1rem;
  line-height: 1.6;
  flex: 1;
}

.detail__actions {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.detail__btn {
  flex-shrink: 0;
}

.detail__chapters-section {
  margin-top: 1rem;
}

.detail__chapters-title {
  margin-bottom: 0.75rem;
}

.detail__chapters {
  max-height: 600px;
  overflow-y: auto;
  border: 1px solid var(--va-background-secondary);
  border-radius: 8px;
}

.detail__chapter-item {
  cursor: pointer;
}

.detail__chapter-item:hover {
  background: var(--va-background-secondary);
}

.detail__chapter-index {
  display: inline-block;
  width: 2.5rem;
  text-align: right;
  margin-right: 0.75rem;
  color: var(--va-text-secondary);
  font-size: 0.85rem;
}

@media (max-width: 640px) {
  .detail__header {
    flex-direction: column;
    gap: 1rem;
  }

  .detail__cover {
    width: 140px;
  }

  .detail__cover-placeholder {
    height: 187px;
  }

  .detail__actions {
    flex-direction: column;
  }
}
</style>
