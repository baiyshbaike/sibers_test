<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import apiClient from '@/api/axiosInstance'
import { useAuthStore } from '@/stores/auth'
import SearchableEmployeePicker from '@/src/components/common/SearchableEmployeePicker.vue'
import type { Employee, Project, TaskItem } from '@/types'
import { TaskStatus } from '@/types'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const isEdit = computed(() => !!route.params.id)
const taskId = computed(() => (route.params.id as string | undefined) ?? '')
const loading = ref(false)
const saving = ref(false)
const error = ref<string | null>(null)

const projects = ref<Project[]>([])
const managers = ref<Employee[]>([])
const allEmployees = ref<Employee[]>([])
const executors = ref<Employee[]>([])
const selectedAuthorIds = ref<string[]>([])
const selectedExecutorIds = ref<string[]>([])

const form = ref({
  name: '',
  comment: '',
  priority: 1,
  status: TaskStatus.ToDo,
  projectId: '',
  authorId: '',
  executorId: '',
})

const canPickAuthor = computed(() => authStore.isSupervisor)
const authorLabel = computed(() =>
  managers.value.find((m) => m.id === form.value.authorId)?.fullName ?? '—',
)

onMounted(async () => {
  loading.value = true
  try {
    const [projectRes, managerRes, employeeRes, executorRes] = await Promise.all([
      apiClient.get<Project[]>('/api/projects'),
      apiClient.get<Employee[]>('/api/employees/by-role/ProjectManager'),
      apiClient.get<Employee[]>('/api/employees'),
      apiClient.get<Employee[]>('/api/employees/by-role/Employee'),
    ])
    projects.value = projectRes.data
    managers.value = managerRes.data
    allEmployees.value = employeeRes.data
    executors.value = executorRes.data

    const currentEmployee = allEmployees.value.find((e) => e.email === authStore.email)
    if (authStore.isProjectManager && currentEmployee) {
      form.value.authorId = currentEmployee.id
      selectedAuthorIds.value = [currentEmployee.id]
    } else if (authStore.isSupervisor) {
      form.value.authorId = managers.value[0]?.id ?? ''
      selectedAuthorIds.value = form.value.authorId ? [form.value.authorId] : []
    }
    form.value.projectId = projects.value[0]?.id ?? ''

    if (isEdit.value && taskId.value) {
      const { data } = await apiClient.get<TaskItem>(`/api/tasks/${taskId.value}`)
      form.value = {
        name: data.name,
        comment: data.comment ?? '',
        priority: data.priority,
        status: data.status,
        projectId: data.projectId,
        authorId: data.author?.id ?? form.value.authorId,
        executorId: data.executor?.id ?? '',
      }
      selectedAuthorIds.value = form.value.authorId ? [form.value.authorId] : []
      selectedExecutorIds.value = form.value.executorId ? [form.value.executorId] : []
    }
  } finally {
    loading.value = false
  }
})

function onAuthorChange(ids: string[]) {
  selectedAuthorIds.value = ids
  form.value.authorId = ids[0] ?? ''
}

function onExecutorChange(ids: string[]) {
  selectedExecutorIds.value = ids
  form.value.executorId = ids[0] ?? ''
}

async function save() {
  error.value = null
  if (!form.value.name.trim()) return (error.value = 'Task name is required.')
  if (!form.value.projectId) return (error.value = 'Project is required.')
  if (!form.value.authorId) return (error.value = 'Author is required.')

  saving.value = true
  try {
    if (isEdit.value && taskId.value) {
      await apiClient.put(`/api/tasks/${taskId.value}`, {
        name: form.value.name.trim(),
        comment: form.value.comment.trim() || null,
        priority: form.value.priority,
        status: form.value.status,
        executorId: form.value.executorId || null,
      })
    } else {
      await apiClient.post('/api/tasks', {
        name: form.value.name.trim(),
        comment: form.value.comment.trim() || null,
        priority: form.value.priority,
        status: form.value.status,
        projectId: form.value.projectId,
        authorId: form.value.authorId,
        executorId: form.value.executorId || null,
      })
    }
    router.push('/tasks')
  } catch (e: any) {
    error.value = e.response?.data?.message ?? 'Failed to save task'
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div class="max-w-3xl mx-auto">
    <h1 class="text-2xl font-bold mb-6">{{ isEdit ? 'Edit Task' : 'Create Task' }}</h1>
    <div v-if="loading" class="text-gray-500">Loading...</div>
    <div v-else class="bg-white rounded-xl shadow p-6">
      <div v-if="error" class="mb-3 p-2 bg-red-50 text-red-600 rounded text-sm">{{ error }}</div>
      <div class="grid grid-cols-2 gap-4">
        <div class="col-span-2">
          <label class="block text-sm mb-1">Task Name *</label>
          <input v-model="form.name" class="w-full border rounded-lg px-3 py-2" />
        </div>
        <div class="col-span-2">
          <label class="block text-sm mb-1">Comment</label>
          <textarea v-model="form.comment" rows="3" class="w-full border rounded-lg px-3 py-2" />
        </div>
        <div>
          <label class="block text-sm mb-1">Priority *</label>
          <input v-model.number="form.priority" type="number" min="1" class="w-full border rounded-lg px-3 py-2" />
        </div>
        <div>
          <label class="block text-sm mb-1">Status *</label>
          <select v-model.number="form.status" class="w-full border rounded-lg px-3 py-2">
            <option :value="TaskStatus.ToDo">To Do</option>
            <option :value="TaskStatus.InProgress">In Progress</option>
            <option :value="TaskStatus.Done">Done</option>
          </select>
        </div>
        <div>
          <label class="block text-sm mb-1">Project *</label>
          <select v-model="form.projectId" class="w-full border rounded-lg px-3 py-2">
            <option disabled value="">Select project</option>
            <option v-for="p in projects" :key="p.id" :value="p.id">{{ p.name }}</option>
          </select>
        </div>
        <div>
          <SearchableEmployeePicker
            v-if="canPickAuthor"
            label="Author (Project Manager) *"
            :employees="managers"
            :selected-ids="selectedAuthorIds"
            :multiple="false"
            @update:selected-ids="onAuthorChange"
          />
          <div v-else>
            <label class="block text-sm mb-1">Author (Project Manager) *</label>
            <input
              :value="authorLabel"
              class="w-full border rounded-lg px-3 py-2 bg-gray-50 text-gray-600"
              disabled
            />
          </div>
        </div>
        <div>
          <SearchableEmployeePicker
            label="Executor"
            :employees="executors"
            :selected-ids="selectedExecutorIds"
            :multiple="false"
            @update:selected-ids="onExecutorChange"
          />
        </div>
      </div>
      <div class="mt-6 flex gap-3">
        <button class="bg-blue-700 text-white px-5 py-2 rounded-lg hover:bg-blue-800" :disabled="saving" @click="save">
          {{ saving ? 'Saving...' : 'Save' }}
        </button>
        <button class="border px-5 py-2 rounded-lg hover:bg-gray-50" @click="router.push('/tasks')">Cancel</button>
      </div>
    </div>
  </div>
</template>
