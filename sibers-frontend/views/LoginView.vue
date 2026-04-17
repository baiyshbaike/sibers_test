<script setup lang="ts">
// #представление_входа / #login_view
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router    = useRouter()
const authStore = useAuthStore()

const email    = ref('')
const password = ref('')
const error    = ref<string | null>(null)
const loading  = ref(false)

async function handleLogin() {
  error.value   = null
  loading.value = true
  try {
    await authStore.login({ email: email.value, password: password.value })
    router.push('/projects')
  } catch (e: any) {
    error.value = e.response?.data?.message ?? 'Login failed. Check credentials.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="flex items-center justify-center min-h-screen">
    <div class="bg-white shadow-lg rounded-xl p-8 w-full max-w-md">
      <h1 class="text-2xl font-bold mb-6 text-center text-blue-700">Sibers Project Manager</h1>

      <div v-if="error" class="mb-4 p-3 bg-red-100 text-red-700 rounded-lg text-sm">
        {{ error }}
      </div>

      <div class="mb-4">
        <label class="block text-sm font-medium text-gray-700 mb-1">Email</label>
        <input
          v-model="email"
          type="email"
          class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          placeholder="your@email.com"
          @keyup.enter="handleLogin"
        />
      </div>

      <div class="mb-6">
        <label class="block text-sm font-medium text-gray-700 mb-1">Password</label>
        <input
          v-model="password"
          type="password"
          class="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          @keyup.enter="handleLogin"
        />
      </div>

      <button
        class="w-full bg-blue-700 text-white py-2 rounded-lg font-semibold hover:bg-blue-800 disabled:opacity-50 transition"
        :disabled="loading"
        @click="handleLogin"
      >
        {{ loading ? 'Logging in...' : 'Login' }}
      </button>
    </div>
  </div>
</template>