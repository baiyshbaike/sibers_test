<script setup lang="ts">
// #навигационная_панель / #navbar_component
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router   = useRouter()
const authStore = useAuthStore()

function logout() {
  authStore.logout()
  router.push('/login')
}
</script>

<template>
  <nav class="bg-blue-700 text-white shadow-md">
    <div class="container mx-auto px-4 py-3 flex items-center justify-between">
      <span class="text-xl font-bold tracking-wide">Sibers PM</span>

      <div class="flex items-center gap-6">
        <RouterLink to="/projects" class="hover:underline">Projects</RouterLink>
        <RouterLink to="/tasks"    class="hover:underline">Tasks</RouterLink>

        <!-- Only Supervisor sees employee management / Только Руководитель видит сотрудников -->
        <RouterLink
          v-if="authStore.isSupervisor"
          to="/employees"
          class="hover:underline"
        >
          Employees
        </RouterLink>

        <span class="text-sm opacity-75">{{ authStore.email }} ({{ authStore.role }})</span>
        <button
          class="bg-white text-blue-700 px-3 py-1 rounded hover:bg-blue-100 text-sm font-medium"
          @click="logout"
        >
          Logout
        </button>
      </div>
    </div>
  </nav>
</template>