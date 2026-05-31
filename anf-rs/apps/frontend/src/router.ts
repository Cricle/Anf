import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', component: () => import('./pages/HomePage.vue') },
    { path: '/comic', component: () => import('./pages/ComicDetail.vue') },
    { path: '/reader', component: () => import('./pages/Reader.vue') },
    { path: '/bookshelf', component: () => import('./pages/Bookshelf.vue') },
  ],
})

export default router
