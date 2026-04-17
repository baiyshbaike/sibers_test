<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import apiClient from '@/api/axiosInstance'
import { TaskStatus } from '@/types'
import { useAuthStore } from '@/stores/auth'
import type { TaskFilter, TaskItem } from '@/types'

const router = useRouter()
const authStore = useAuthStore()
const tasks = ref<TaskItem[]>([])
const loading = ref(false)
const error = ref<string | null>(null)
const statusDraft = ref<Record<string, TaskStatus>>({})
const filter = ref<TaskFilter>({ sortBy: 'priority', sortDescending: false })

const canManageTasks = computed(() => authStore.isSupervisor || authStore.isProjectManager)
const canChangeOwnStatus = computed(() => authStore.isEmployee)

const statusLabels: Record<TaskStatus, string> = {
  [TaskStatus.ToDo]: 'To Do',
  [TaskStatus.InProgress]: 'In Progress',
  [TaskStatus.Done]: 'Done',
}
const statusColors: Record<TaskStatus, string> = {
  [TaskStatus.ToDo]: 'bg-gray-100 text-gray-700',
  [TaskStatus.InProgress]: 'bg-yellow-100 text-yellow-800',
  [TaskStatus.Done]: 'bg-green-100 text-green-800',
}

async function fetchTasks() {
  loading.value = true
  error.value = null
  try {
    const { data } = await apiClient.get<TaskItem[]>('/api/tasks', { params: filter.value })
    tasks.value = data
    statusDraft.value = {}
    for (const task of data) statusDraft.value[task.id] = task.status
  } catch (e: any) {
    error.value = e.response?.data?.message ?? 'Failed to load tasks'
  } finally {
    loading.value = false
  }
}

async function deleteTask(id: string) {
  if (!confirm('Delete this task?')) return
  await apiClient.delete(`/api/tasks/${id}`)
  await fetchTasks()
}

async function updateOwnStatus(task: TaskItem) {
  await apiClient.put(`/api/tasks/${task.id}`, {
    name: task.name,
    comment: task.comment ?? null,
    priority: task.priority,
    status: statusDraft.value[task.id],
    executorId: task.executor?.id ?? null,
  })
  await fetchTasks()
}

onMounted(fetchTasks)
</script>

<template>
  <div>
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-800">Tasks</h1>
      <button v-if="canManageTasks" class="bg-blue-700 text-white px-4 py-2 rounded-lg hover:bg-blue-800 text-sm font-medium" @click="router.push('/tasks/upsert')">
        + New Task
      </button>
    </div>

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

    <div v-if="error" class="mb-4 p-2 bg-red-50 text-red-600 rounded text-sm">{{ error }}</div>
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

        <div class="flex items-center gap-3">
          <span class="text-xs font-semibold px-3 py-1 rounded-full" :class="statusColors[task.status]">
            {{ statusLabels[task.status] }}
          </span>

          <button class="text-blue-600 hover:underline text-sm" @click="router.push(`/tasks/${task.id}`)">View</button>

          <div v-if="canChangeOwnStatus" class="flex items-center gap-2">
            <select v-model.number="statusDraft[task.id]" class="border rounded px-2 py-1 text-sm">
              <option :value="TaskStatus.ToDo">To Do</option>
              <option :value="TaskStatus.InProgress">In Progress</option>
              <option :value="TaskStatus.Done">Done</option>
            </select>
            <button class="text-blue-600 hover:underline text-sm" @click="updateOwnStatus(task)">Save</button>
          </div>

          <div v-if="canManageTasks" class="flex gap-2">
            <button class="text-blue-600 hover:underline text-sm" @click="router.push(`/tasks/upsert/${task.id}`)">Edit</button>
            <button class="text-red-500 hover:underline text-sm" @click="deleteTask(task.id)">Delete</button>
          </div>
        </div>
      </div>

      <p v-if="tasks.length === 0" class="text-center text-gray-400 py-10">No tasks found.</p>
    </div>
  </div>
</template>