<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { getAdapter } from '@/api'
import type { ComicAdapter, DownloadInfo } from '@/api'

let adapter: ComicAdapter | null = null

const downloads = ref<DownloadInfo[]>([])
const loading = ref(false)
let timer: ReturnType<typeof setInterval> | null = null

async function refresh() {
  try {
    if (!adapter) adapter = await getAdapter()
    downloads.value = await adapter.listDownloads()
  } catch {
    // ignore
  }
}

async function cancelDownload(url: string) {
  try {
    if (!adapter) adapter = await getAdapter()
    await adapter.cancelDownload(url)
    await refresh()
  } catch {
    // ignore
  }
}

function exportPdf(name: string) {
  if (!adapter) return
  window.open(adapter.exportPdfUrl(name), '_blank')
}

function statusColor(status: string) {
  switch (status) {
    case 'completed': return 'success'
    case 'failed': return 'danger'
    case 'cancelled': return 'warning'
    default: return 'info'
  }
}

function statusIcon(status: string) {
  switch (status) {
    case 'preparing': return 'hourglass_empty'
    case 'downloading': return 'downloading'
    case 'completed': return 'check_circle'
    case 'failed': return 'error'
    case 'cancelled': return 'cancel'
    default: return 'help'
  }
}

function progress(download: DownloadInfo) {
  if (download.total_pages === 0) return 0
  return Math.round((download.downloaded_pages / download.total_pages) * 100)
}

onMounted(() => {
  refresh()
  timer = setInterval(refresh, 2000)
})

onUnmounted(() => {
  if (timer) clearInterval(timer)
})
</script>

<template>
  <div class="downloads">
    <h2>Downloads</h2>

    <va-inner-loading :loading="loading">
      <div v-if="downloads.length === 0" class="downloads__empty">
        <va-icon name="cloud_download" size="large" />
        <p>No active downloads</p>
      </div>

      <va-list v-else class="downloads__list">
        <va-list-item
          v-for="dl in downloads"
          :key="dl.url"
          class="downloads__item"
        >
          <va-list-item-section>
            <va-list-item-label>
              <va-icon :name="statusIcon(dl.status)" :color="statusColor(dl.status)" size="small" class="downloads__status-icon" />
              {{ dl.name }}
            </va-list-item-label>
            <va-list-item-label caption>
              <template v-if="dl.status === 'downloading'">
                {{ dl.current_chapter }} — {{ dl.downloaded_pages }}/{{ dl.total_pages }} pages
              </template>
              <template v-else-if="dl.status === 'failed'">
                {{ dl.error || 'Unknown error' }}
              </template>
              <template v-else-if="dl.status === 'completed'">
                {{ dl.total_pages }} pages downloaded
              </template>
              <template v-else-if="dl.status === 'preparing'">
                Preparing...
              </template>
            </va-list-item-label>

            <va-progress-bar
              v-if="dl.status === 'downloading' || dl.status === 'preparing'"
              :model-value="progress(dl)"
              :indeterminate="dl.status === 'preparing'"
              class="downloads__progress"
            />
          </va-list-item-section>

          <va-list-item-section side>
            <div class="downloads__actions">
              <va-button
                v-if="dl.status === 'completed'"
                preset="secondary"
                icon="picture_as_pdf"
                size="small"
                @click="exportPdf(dl.name)"
              />
              <va-button
                v-if="dl.status === 'downloading' || dl.status === 'preparing'"
                preset="secondary"
                icon="close"
                size="small"
                @click="cancelDownload(dl.url)"
              />
            </div>
          </va-list-item-section>
        </va-list-item>
      </va-list>
    </va-inner-loading>
  </div>
</template>

<style scoped>
.downloads {
  max-width: 800px;
  margin: 0 auto;
}

.downloads__empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  padding: 4rem;
  color: var(--va-text-secondary);
}

.downloads__list {
  border: 1px solid var(--va-background-secondary);
  border-radius: 8px;
}

.downloads__item {
  padding: 1rem;
}

.downloads__status-icon {
  margin-right: 0.5rem;
}

.downloads__progress {
  margin-top: 0.5rem;
}
</style>
