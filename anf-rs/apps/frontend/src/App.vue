<script setup lang="ts">
import { useRouter, useRoute } from 'vue-router'
import { computed } from 'vue'
import { useAppStore } from '@/stores/app'

const router = useRouter()
const route = useRoute()
const app = useAppStore()

const isHome = computed(() => route.path === '/')
</script>

<template>
  <div class="app-layout">
    <va-navbar color="primary" class="app-navbar">
      <template #left>
        <va-navbar-item class="app-title" @click="router.push('/')">
          Anf
        </va-navbar-item>
      </template>
      <template #right>
        <va-navbar-item>
          <va-button
            preset="secondary"
            icon="home"
            @click="router.push('/')"
          />
        </va-navbar-item>
        <va-navbar-item>
          <va-button
            preset="secondary"
            icon="bookmark"
            @click="router.push('/bookshelf')"
          />
        </va-navbar-item>
        <va-navbar-item>
          <va-button
            preset="secondary"
            icon="cloud_download"
            @click="router.push('/downloads')"
          />
        </va-navbar-item>
        <va-navbar-item>
          <va-button
            preset="secondary"
            :icon="app.darkMode ? 'light_mode' : 'dark_mode'"
            @click="app.toggleDark()"
          />
        </va-navbar-item>
      </template>
    </va-navbar>
    <main class="app-main">
      <router-view />
    </main>
  </div>
</template>

<style scoped>
.app-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.app-navbar {
  flex-shrink: 0;
}

.app-title {
  font-size: 1.4rem;
  font-weight: 700;
  cursor: pointer;
  user-select: none;
}

.app-main {
  flex: 1;
  padding: 1.5rem;
  max-width: 1200px;
  margin: 0 auto;
  width: 100%;
  box-sizing: border-box;
}

@media (max-width: 640px) {
  .app-main {
    padding: 0.75rem;
  }
}
</style>
