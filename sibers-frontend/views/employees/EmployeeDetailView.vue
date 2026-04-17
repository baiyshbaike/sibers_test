<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import apiClient from '@/api/axiosInstance'
import type { Employee } from '@/types'

const route = useRoute()
const loading = ref(false)
const employee = ref<Employee | null>(null)

onMounted(async () => {
  loading.value = true
  try {
    const { data } = await apiClient.get<Employee>(`/api/employees/${route.params.id}`)
    employee.value = data
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div>
    <div v-if="loading">Loading...</div>
    <div v-else-if="employee">
      <h1 class="text-2xl font-bold mb-2">{{ employee.fullName }}</h1>
      <p class="text-gray-600">{{ employee.email }}</p>
    </div>
    <div v-else class="text-gray-400">Employee not found.</div>
  </div>
</template>
