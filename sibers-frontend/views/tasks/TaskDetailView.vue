<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute } from 'vue-router'
import apiClient from '@/api/axiosInstance'
import type { TaskItem } from '@/types'
import { TaskStatus } from '@/types'

const route = useRoute()
const loading = ref(false)
const task = ref<TaskItem | null>(null)

const statusLabel: Record<TaskStatus, string> = {
  [TaskStatus.ToDo]: 'To Do',
  [TaskStatus.InProgress]: 'In Progress',
  [TaskStatus.Done]: 'Done',
}

onMounted(async () => {
  loading.value = true
  try {
    const { data } = await apiClient.get<TaskItem>(`/api/tasks/${route.params.id}`)
    task.value = data
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div>
    <div v-if="loading">Loading...</div>
    <div v-else-if="task">
      <h1 class="text-2xl font-bold mb-2">{{ task.name }}</h1>
      <p class="text-gray-600 mb-1">Project: {{ task.projectName }}</p>
      <p class="text-gray-600 mb-1">Status: {{ statusLabel[task.status] }}</p>
      <p class="text-gray-600 mb-1">Priority: {{ task.priority }}</p>
      <p v-if="task.author" class="text-gray-600 mb-1">Author: {{ task.author.fullName }}</p>
      <p v-if="task.executor" class="text-gray-600 mb-1">Executor: {{ task.executor.fullName }}</p>
      <p v-if="task.comment" class="text-gray-600 mt-4">{{ task.comment }}</p>
    </div>
    <div v-else class="text-gray-400">Task not found.</div>
  </div>
</template>
