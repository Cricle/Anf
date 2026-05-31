<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import { getAdapter, type ComicPage } from '@/api'

const route = useRoute()

const pages = ref<ComicPage[]>([])
const loading = ref(false)
const currentIndex = ref(0)
const imageSrc = ref('')

const chapterUrl = computed(() => (route.query.url as string) || '')
const entityUrl = computed(() => (route.query.entity as string) || '')

onMounted(async () => {
  if (!chapterUrl.value || !entityUrl.value) return
  loading.value = true
  try {
    const adapter = await getAdapter()
    const chapter = await adapter.getChapter(entityUrl.value, chapterUrl.value)
    pages.value = chapter.pages
    if (pages.value.length > 0) {
      await loadImage(0)
    }
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
})

async function loadImage(index: number) {
  if (index < 0 || index >= pages.value.length) return
  currentIndex.value = index
  loading.value = true
  try {
    const adapter = await getAdapter()
    const data = await adapter.getImage(entityUrl.value, pages.value[index].target_url)
    const blob = new Blob([new Uint8Array(data)], { type: 'image/png' })
    imageSrc.value = URL.createObjectURL(blob)
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
}

function prev() {
  loadImage(currentIndex.value - 1)
}

function next() {
  loadImage(currentIndex.value + 1)
}
</script>

<template>
  <div class="reader">
    <div class="reader__toolbar">
      <va-button :disabled="currentIndex <= 0" @click="prev" icon="chevron_left" />
      <span class="reader__page-info">{{ currentIndex + 1 }} / {{ pages.length }}</span>
      <va-button :disabled="currentIndex >= pages.length - 1" @click="next" icon="chevron_right" />
    </div>
    <va-inner-loading :loading="loading">
      <div class="reader__image-container">
        <img v-if="imageSrc" :src="imageSrc" class="reader__image" @click="next" />
        <div v-else class="reader__empty">No pages</div>
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
  gap: 1rem;
  margin-bottom: 1rem;
}

.reader__page-info {
  font-size: 1rem;
  min-width: 80px;
  text-align: center;
}

.reader__image-container {
  display: flex;
  justify-content: center;
  min-height: 400px;
}

.reader__image {
  max-width: 100%;
  max-height: 80vh;
  cursor: pointer;
}

.reader__empty {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 400px;
  color: var(--va-text-secondary);
}
</style>
