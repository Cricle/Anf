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
const proposalEngines = ref<string[]>([])
const selectedProposalEngine = ref<string>('')
const searchEngines = ref<string[]>([])
const error = ref('')

onMounted(async () => {
  const adapter = await getAdapter()

  // Load proposal engines
  try {
    proposalEngines.value = await adapter.getProviders()
    if (proposalEngines.value.length > 0) {
      selectedProposalEngine.value = proposalEngines.value[0]
    }
  } catch (e) {
    console.error(e)
  }

  // Load proposals
  await loadProposals()
})

async function loadProposals() {
  proposalLoading.value = true
  error.value = ''
  try {
    const adapter = await getAdapter()
    proposals.value = await adapter.getProposal(
      selectedProposalEngine.value || undefined,
      18
    )
  } catch (e: any) {
    error.value = e.message || 'Failed to load proposals'
  } finally {
    proposalLoading.value = false
  }
}

async function handleSearch() {
  if (!store.keyword.trim()) return
  store.setSearching(true)
  error.value = ''
  try {
    const adapter = await getAdapter()
    const result = await adapter.search(store.keyword)
    store.setSearchResults(result.snapshots)
    if (result.snapshots.length === 0) {
      error.value = 'No results found'
    }
  } catch (e: any) {
    error.value = e.message || 'Search failed'
  } finally {
    store.setSearching(false)
  }
}

function handleClick(snap: ComicSnapshot) {
  const url = snap.sources?.[0]?.target_url || snap.target_url
  router.push(`/comic?url=${encodeURIComponent(url)}`)
}

function clearSearch() {
  store.setSearchResults([])
  store.setKeyword('')
  error.value = ''
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
        <template #prepend>
          <va-icon name="search" />
        </template>
        <template #append>
          <va-button
            v-if="store.searchResults.length > 0"
            preset="plain"
            icon="close"
            @click="clearSearch"
          />
          <va-button
            :loading="store.searching"
            @click="handleSearch"
          >
            Search
          </va-button>
        </template>
      </va-input>
    </div>

    <!-- Error message -->
    <div v-if="error" class="home__error">
      <va-icon name="info" size="small" />
      <span>{{ error }}</span>
    </div>

    <!-- Search results -->
    <div v-if="store.searchResults.length > 0">
      <h3 class="section-title">Search Results ({{ store.searchResults.length }})</h3>
      <div class="comic-grid">
        <ComicCard
          v-for="(snap, i) in store.searchResults"
          :key="i"
          :snapshot="snap"
          @click="handleClick(snap)"
        />
      </div>
    </div>

    <!-- Proposals -->
    <div v-else>
      <div class="proposal-header">
        <h3 class="section-title">Proposals</h3>
        <div v-if="proposalEngines.length > 1" class="proposal-selector">
          <va-select
            v-model="selectedProposalEngine"
            :options="proposalEngines"
            @update:model-value="loadProposals"
            size="small"
            class="proposal-select"
          />
        </div>
      </div>
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
  margin-bottom: 1.5rem;
}

.search-input {
  width: 100%;
}

.home__error {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  margin-bottom: 1rem;
  background: var(--va-background-secondary);
  border-radius: 8px;
  color: var(--va-text-secondary);
  font-size: 0.9rem;
}

.section-title {
  color: var(--va-text-secondary);
  margin-bottom: 1rem;
}

.proposal-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.proposal-header .section-title {
  margin-bottom: 0;
}

.proposal-select {
  min-width: 150px;
}

.comic-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: 1rem;
}
</style>
