<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()
</script>

<template>
  <va-app-layout>
    <template #content>
      <div class="app-layout">
        <va-navbar color="primary" class="app-navbar">
          <template #left>
            <va-navbar-item class="app-title" @click="router.push('/')">
              Anf
            </va-navbar-item>
          </template>
          <template #right>
            <va-navbar-item>
              <va-button preset="secondary" @click="router.push('/')">Home</va-button>
            </va-navbar-item>
            <va-navbar-item>
              <va-button preset="secondary" @click="router.push('/bookshelf')">Bookshelf</va-button>
            </va-navbar-item>
          </template>
        </va-navbar>
        <main class="app-main">
          <router-view />
        </main>
      </div>
    </template>
  </va-app-layout>
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
}

.app-main {
  flex: 1;
  padding: 1.5rem;
  max-width: 1200px;
  margin: 0 auto;
  width: 100%;
}
</style>
