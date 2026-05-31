<script setup lang="ts">
import type { ComicSnapshot } from '@/api/types'

defineProps<{
  snapshot: ComicSnapshot
}>()

defineEmits<{
  click: []
}>()
</script>

<template>
  <va-card
    class="comic-card"
    @click="$emit('click')"
    hoverable
  >
    <va-image
      v-if="snapshot.image_uri"
      :src="snapshot.image_uri"
      :ratio="2 / 3"
      class="comic-card__image"
    >
      <template #error>
        <div class="comic-card__placeholder">
          <va-icon name="image" size="large" />
        </div>
      </template>
      <template #loading>
        <div class="comic-card__placeholder">
          <va-spinner size="small" />
        </div>
      </template>
    </va-image>
    <div v-else class="comic-card__placeholder">
      <va-icon name="image" size="large" />
    </div>
    <va-card-content>
      <div class="comic-card__title" :title="snapshot.name">{{ snapshot.name }}</div>
      <div v-if="snapshot.author" class="comic-card__author">{{ snapshot.author }}</div>
    </va-card-content>
  </va-card>
</template>

<style scoped>
.comic-card {
  cursor: pointer;
  min-width: 150px;
  transition: transform 0.15s ease;
}

.comic-card:hover {
  transform: translateY(-2px);
}

.comic-card__image {
  width: 100%;
}

.comic-card__placeholder {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 200px;
  background: var(--va-background-secondary);
}

.comic-card__title {
  font-weight: 600;
  font-size: 0.9rem;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.comic-card__author {
  font-size: 0.8rem;
  color: var(--va-text-secondary);
  margin-top: 0.25rem;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>
