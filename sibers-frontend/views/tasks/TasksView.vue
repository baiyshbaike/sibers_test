<script setup lang="ts">
// #список_задач / #tasks_view
import { ref, onMounted } from 'vue'
import apiClient from '@/api/axiosInstance'
import { TaskStatus } from '@/types'
import type { TaskItem, TaskFilter } from '@/types'

const tasks   = ref<TaskItem[]>([])
const loading = ref(false)
const filter  = ref<TaskFilter>({ sortBy: 'priority', sortDescending: false })

const statusLabels: Record<TaskStatus, string> = {
  [TaskStatus.ToDo]:       'To Do',
  [TaskStatus.InProgress]: 'In Progress',
  [TaskStatus.Done]:       'Done',
}

const statusColors: Record<TaskStatus, string> = {
  [TaskStatus.ToDo]:       'bg-gray-100 text-gray-700',
  [TaskStatus.InProgress]: 'bg-yellow-100 text-yellow-800',
  [TaskStatus.Done]:       'bg-green-100 text-green-800',
}

async function fetchTasks() {
  loading.value = true
  try {
    const { data } = await apiClient.get<TaskItem[]>('/api/tasks', { params: filter.value })
    tasks.value = data
  } finally {
    loading.value = false
  }
}

onMounted(fetchTasks)
</script>

<template>
  <div>
    <h1 class="text-2xl font-bold mb-6 text-gray-800">Tasks</h1>

    <!-- Filters / Фильтры -->
    <div class="bg-white rounded-xl shadow p-4 mb-6 flex flex-wrap gap-4">
      <div>
        <label class="text-xs text-gray-500 block mb-1">Status</label>
        <select v-model.number="filter.status" class="border rounded px-2 py-1 text-sm">
          <option :value="undefined">All</option>
          <option :value="TaskStatus.ToDo">To Do</option>
          <option :value="TaskStatus.InProgress">In Progress</option>
          <option :value="TaskStatus.Done">Done</option>
        </select>
      </div>
      <div>
        <label class="text-xs text-gray-500 block mb-1">Sort by</label>
        <select v-model="filter.sortBy" class="border rounded px-2 py-1 text-sm">
          <option value="name">Name</option>
          <option value="priority">Priority</option>
          <option value="status">Status</option>
        </select>
      </div>
      <div class="flex items-end">
        <button class="bg-blue-700 text-white px-4 py-1.5 rounded-lg text-sm" @click="fetchTasks">
          Apply
        </button>
      </div>
    </div>

    <div v-if="loading" class="text-center py-10 text-gray-500">Loading...</div>

    <div v-else class="grid gap-3">
      <div
        v-for="task in tasks"
        :key="task.id"
        class="bg-white rounded-xl shadow p-4 flex items-center justify-between"
      >
        <div>
          <h3 class="font-medium text-gray-800">{{ task.name }}</h3>
          <p class="text-xs text-gray-400 mt-1">
            Project: {{ task.projectName }} · Priority: {{ task.priority }}
          </p>
          <p v-if="task.executor" class="text-xs text-gray-400">
            Executor: {{ task.executor.fullName }}
          </p>
        </div>
        <span
          class="text-xs font-semibold px-3 py-1 rounded-full"
          :class="statusColors[task.status]"
        >
          {{ statusLabels[task.status] }}
        </span>
      </div>
      <p v-if="tasks.length === 0" class="text-center text-gray-400 py-10">No tasks found.</p>
    </div>
  </div>
</template>