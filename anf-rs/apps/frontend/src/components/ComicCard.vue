<script setup lang="ts">
import type { ComicSnapshot } from '@/api/types'

defineProps<{
  snapshot: ComicSnapshot
  wide?: boolean
}>()

defineEmits<{
  click: []
}>()
</script>

<template>
  <va-card
    class="comic-card"
    :class="{ 'comic-card--wide': wide }"
    @click="$emit('click')"
    hoverable
  >
    <va-image
      v-if="snapshot.image_uri"
      :src="snapshot.image_uri"
      :ratio="wide ? 3 / 4 : 2 / 3"
      class="comic-card__image"
    >
      <template #error>
        <div class="comic-card__placeholder">
          <va-icon name="image" size="large" />
        </div>
      </template>
    </va-image>
    <div v-else class="comic-card__placeholder">
      <va-icon name="image" size="large" />
    </div>
    <va-card-content>
      <div class="comic-card__title">{{ snapshot.name }}</div>
      <div v-if="snapshot.author" class="comic-card__author">{{ snapshot.author }}</div>
    </va-card-content>
  </va-card>
</template>

<style scoped>
.comic-card {
  cursor: pointer;
  min-width: 150px;
}

.comic-card--wide {
  min-width: 200px;
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
}
</style>
