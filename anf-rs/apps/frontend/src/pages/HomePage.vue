<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { getAdapter, type ComicSnapshot } from '@/api'
import { useAppStore } from '@/stores/app'
import ComicCard from '@/components/ComicCard.vue'

const router = useRouter()
const store = useAppStore()
const proposals = ref<ComicSnapshot[]>([])
const proposalLoading = ref(false)

onMounted(async () => {
  proposalLoading.value = true
  try {
    const adapter = await getAdapter()
    proposals.value = await adapter.getProposal(undefined, 12)
  } catch (e) {
    console.error(e)
  } finally {
    proposalLoading.value = false
  }
})

async function handleSearch() {
  if (!store.keyword.trim()) return
  store.setSearching(true)
  try {
    const adapter = await getAdapter()
    const result = await adapter.search(store.keyword)
    store.setSearchResults(result.snapshots)
  } catch (e) {
    console.error(e)
  } finally {
    store.setSearching(false)
  }
}

function handleClick(snap: ComicSnapshot) {
  const url = snap.sources[0]?.target_url || snap.target_url
  router.push(`/comic?url=${encodeURIComponent(url)}`)
}
</script>

<template>
  <div class="home">
    <div class="search-bar">
      <va-input
        v-model="store.keyword"
        placeholder="Search comics or paste a URL..."
        @keyup.enter="handleSearch"
        class="search-input"
      >
        <template #append>
          <va-button
            :loading="store.searching"
            icon="search"
            @click="handleSearch"
          />
        </template>
      </va-input>
    </div>

    <div v-if="store.searchResults.length > 0">
      <h3 class="section-title">Search Results</h3>
      <div class="comic-grid">
        <ComicCard
          v-for="(snap, i) in store.searchResults"
          :key="i"
          :snapshot="snap"
          @click="handleClick(snap)"
        />
      </div>
    </div>

    <div v-else>
      <h3 class="section-title">Proposals</h3>
      <va-inner-loading :loading="proposalLoading">
        <div class="comic-grid">
          <ComicCard
            v-for="(snap, i) in proposals"
            :key="i"
            :snapshot="snap"
            @click="handleClick(snap)"
          />
        </div>
      </va-inner-loading>
    </div>
  </div>
</template>

<style scoped>
.home {
  max-width: 1000px;
  margin: 0 auto;
}

.search-bar {
  margin-bottom: 2rem;
}

.search-input {
  width: 100%;
}

.section-title {
  color: var(--va-text-secondary);
  margin-bottom: 1rem;
}

.comic-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: 1rem;
}
</style>
