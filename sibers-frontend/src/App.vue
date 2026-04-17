<script setup lang="ts">
// #корневой_компонент / #root_component
import { RouterView } from 'vue-router'
import NavBar from '@/components/NavBar.vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()
</script>

<template>
  <div class="min-h-screen bg-gray-50">
    <NavBar v-if="authStore.isAuthenticated" />
    <main class="container mx-auto px-4 py-6">
      <RouterView />
    </main>
  </div>
</template>