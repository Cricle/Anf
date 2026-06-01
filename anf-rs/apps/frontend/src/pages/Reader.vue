<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getAdapter } from '@/api'
import type { ComicPage } from '@/api'

const route = useRoute()
const router = useRouter()

const pages = ref<ComicPage[]>([])
const loading = ref(false)
const mode = ref<'single' | 'scroll'>('single')
const currentIndex = ref(0)

const chapterUrl = computed(() => (route.query.url as string) || '')
const entityUrl = computed(() => (route.query.entity as string) || '')
const chapterIndex = computed(() => parseInt(route.query.chapter as string) || 0)

const adapter = await getAdapter()

const imageUrls = computed(() =>
  pages.value.map(p => adapter.getImageUrl(entityUrl.value, p.target_url))
)

// Preload next images in single mode
const preloaded = new Set<string>()

function preloadImage(url: string) {
  if (!url || preloaded.has(url)) return
  preloaded.add(url)
  const img = new Image()
  img.src = url
}

watch(currentIndex, (idx) => {
  // Preload next 2 images
  for (let i = 1; i <= 2; i++) {
    const nextIdx = idx + i
    if (nextIdx < imageUrls.value.length) {
      preloadImage(imageUrls.value[nextIdx])
    }
  }
})

onMounted(async () => {
  if (!chapterUrl.value || !entityUrl.value) return
  loading.value = true
  try {
    const chapter = await adapter.getChapter(entityUrl.value, chapterUrl.value)
    pages.value = chapter.pages
    // Preload first 2 images
    for (let i = 0; i < Math.min(2, imageUrls.value.length); i++) {
      preloadImage(imageUrls.value[i])
    }
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
})

function handleKeydown(e: KeyboardEvent) {
  if (mode.value === 'single') {
    if (e.key === 'ArrowRight' || e.key === ' ') {
      e.preventDefault()
      next()
    } else if (e.key === 'ArrowLeft') {
      e.preventDefault()
      prev()
    }
  }
}

function prev() {
  if (currentIndex.value > 0) currentIndex.value--
}

function next() {
  if (currentIndex.value < pages.value.length - 1) currentIndex.value++
}

function goBack() {
  router.push(`/comic?url=${encodeURIComponent(entityUrl.value)}`)
}

onMounted(() => {
  window.addEventListener('keydown', handleKeydown)
})

onUnmounted(() => {
  window.removeEventListener('keydown', handleKeydown)
})
</script>

<template>
  <div class="reader">
    <div class="reader__toolbar">
      <va-button preset="secondary" icon="arrow_back" @click="goBack">Back</va-button>

      <div class="reader__mode-toggle">
        <va-button
          :preset="mode === 'single' ? 'primary' : 'secondary'"
          size="small"
          @click="mode = 'single'"
        >
          Single
        </va-button>
        <va-button
          :preset="mode === 'scroll' ? 'primary' : 'secondary'"
          size="small"
          @click="mode = 'scroll'"
        >
          Scroll
        </va-button>
      </div>

      <template v-if="mode === 'single'">
        <va-button
          :disabled="currentIndex <= 0"
          @click="prev"
          icon="chevron_left"
        />
        <span class="reader__page-info">{{ currentIndex + 1 }} / {{ pages.length }}</span>
        <va-button
          :disabled="currentIndex >= pages.length - 1"
          @click="next"
          icon="chevron_right"
        />
      </template>
      <span v-else class="reader__page-info">{{ pages.length }} pages</span>
    </div>

    <va-inner-loading :loading="loading">
      <div v-if="pages.length === 0 && !loading" class="reader__empty">
        No pages found
      </div>

      <!-- Single page mode -->
      <div v-if="mode === 'single' && pages.length > 0" class="reader__single">
        <img
          :src="imageUrls[currentIndex]"
          class="reader__image"
          @click="next"
        />
      </div>

      <!-- Scroll mode -->
      <div v-if="mode === 'scroll'" class="reader__scroll">
        <img
          v-for="(url, i) in imageUrls"
          :key="i"
          :src="url"
          class="reader__scroll-image"
          loading="lazy"
        />
      </div>
    </va-inner-loading>
  </div>
</template>

<style scoped>
.reader {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.reader__toolbar {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1rem;
  padding: 0.5rem 1rem;
  background: var(--va-background-secondary);
  border-radius: 8px;
  position: sticky;
  top: 0;
  z-index: 10;
}

.reader__mode-toggle {
  display: flex;
  gap: 0.25rem;
}

.reader__page-info {
  font-size: 0.9rem;
  min-width: 80px;
  text-align: center;
}

.reader__single {
  display: flex;
  justify-content: center;
}

.reader__image {
  max-width: 100%;
  max-height: 85vh;
  cursor: pointer;
  user-select: none;
}

.reader__scroll {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0;
}

.reader__scroll-image {
  max-width: 100%;
  display: block;
}

.reader__empty {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 400px;
  color: var(--va-text-secondary);
}

@media (max-width: 640px) {
  .reader__toolbar {
    flex-wrap: wrap;
    gap: 0.5rem;
    padding: 0.5rem;
  }
}
</style>
