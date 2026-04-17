<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useProjectsStore } from '@/stores/projects'
import { useEmployeesStore } from '@/stores/employees'
import { useAuthStore } from '@/stores/auth'
import SearchableEmployeePicker from '@/src/components/common/SearchableEmployeePicker.vue'
import type { Employee, ProjectDocument } from '@/types'

const route = useRoute()
const router = useRouter()
const projectsStore = useProjectsStore()
const employeesStore = useEmployeesStore()
const authStore = useAuthStore()

const isEdit = computed(() => !!route.params.id)
const projectId = computed(() => (route.params.id as string | undefined) ?? '')
const loading = ref(false)
const saving = ref(false)
const error = ref<string | null>(null)

const managers = ref<Employee[]>([])
const teamEmployees = ref<Employee[]>([])
const initialTeamIds = ref<string[]>([])
const selectedTeamIds = ref<string[]>([])
const selectedManagerIds = ref<string[]>([])
const files = ref<File[]>([])
const existingDocuments = ref<ProjectDocument[]>([])
const isDragging = ref(false)

const form = ref({
  name: '',
  customerCompany: '',
  executorCompany: '',
  startDate: new Date().toISOString().slice(0, 10),
  endDate: '',
  priority: 1,
  projectManagerId: '',
})

function onManagerChange(ids: string[]) {
  selectedManagerIds.value = ids
  form.value.projectManagerId = ids[0] ?? ''
}

function onTeamChange(ids: string[]) {
  selectedTeamIds.value = ids
}

onMounted(async () => {
  loading.value = true
  try {
    const [managersData, employeesData] = await Promise.all([
      employeesStore.fetchByRole('ProjectManager'),
      employeesStore.fetchByRole('Employee'),
    ])
    managers.value = managersData
    teamEmployees.value = employeesData
    if (!isEdit.value && managers.value.length > 0) {
      selectedManagerIds.value = [managers.value[0].id]
      form.value.projectManagerId = managers.value[0].id
    }

    if (isEdit.value && projectId.value) {
      await projectsStore.fetchById(projectId.value)
      const p = projectsStore.current
      if (!p) return
      
      // Check if current Project Manager can access this project
      if (authStore.isProjectManager && p.projectManager?.id !== authStore.user?.employeeId) {
        router.push('/projects')
        return
      }
      
      form.value = {
        name: p.name,
        customerCompany: p.customerCompany,
        executorCompany: p.executorCompany,
        startDate: p.startDate.slice(0, 10),
        endDate: p.endDate.slice(0, 10),
        priority: p.priority,
        projectManagerId: p.projectManager?.id ?? '',
      }
      selectedManagerIds.value = p.projectManager?.id ? [p.projectManager.id] : []
      initialTeamIds.value = p.employees.map((e) => e.id)
      selectedTeamIds.value = [...initialTeamIds.value]
      existingDocuments.value = [...(p.documents ?? [])]
    }
  } finally {
    loading.value = false
  }
})

function onFileInput(event: Event) {
  const input = event.target as HTMLInputElement
  addFiles(Array.from(input.files ?? []))
  input.value = ''
}

function onDrop(event: DragEvent) {
  isDragging.value = false
  const dropped = Array.from(event.dataTransfer?.files ?? [])
  addFiles(dropped)
}

function addFiles(newFiles: File[]) {
  const existing = new Set(files.value.map((f) => `${f.name}|${f.size}`))
  for (const file of newFiles) {
    const key = `${file.name}|${file.size}`
    if (!existing.has(key)) files.value.push(file)
  }
}

function removeFile(name: string) {
  files.value = files.value.filter((f) => f.name !== name)
}

async function save() {
  error.value = null
  if (!form.value.name.trim()) return (error.value = 'Project name is required.')
  if (!form.value.projectManagerId) return (error.value = 'Project manager is required.')
  if (!form.value.endDate) return (error.value = 'End date is required.')

  saving.value = true
  try {
    const dto = {
      name: form.value.name.trim(),
      customerCompany: form.value.customerCompany.trim(),
      executorCompany: form.value.executorCompany.trim(),
      startDate: form.value.startDate,
      endDate: form.value.endDate,
      priority: form.value.priority,
      projectManagerId: form.value.projectManagerId,
    }

    let id = projectId.value
    if (isEdit.value && id) {
      await projectsStore.update(id, dto)
    } else {
      const created = await projectsStore.create(dto)
      id = created.id
    }

    const toAdd = selectedTeamIds.value.filter((x) => !initialTeamIds.value.includes(x))
    const toRemove = initialTeamIds.value.filter((x) => !selectedTeamIds.value.includes(x))
    for (const employeeId of toAdd) await projectsStore.addEmployee(id, employeeId)
    for (const employeeId of toRemove) await projectsStore.removeEmployee(id, employeeId)

    await projectsStore.uploadDocuments(id, files.value)
    router.push(`/projects/${id}`)
  } catch (e: any) {
    error.value = e.response?.data?.message ?? 'Failed to save project'
  } finally {
    saving.value = false
  }
}

async function deleteExistingDocument(documentId: string) {
  if (!isEdit.value || !projectId.value) return
  await projectsStore.deleteDocument(projectId.value, documentId)
  existingDocuments.value = existingDocuments.value.filter((d) => d.id !== documentId)
}
</script>

<template>
  <div class="max-w-3xl mx-auto">
    <h1 class="text-2xl font-bold mb-6">{{ isEdit ? 'Edit Project' : 'Create Project' }}</h1>
    <div v-if="loading" class="text-gray-500">Loading...</div>
    <div v-else class="bg-white rounded-xl shadow p-6">
      <div v-if="error" class="mb-3 p-2 bg-red-50 text-red-600 rounded text-sm">{{ error }}</div>

      <div class="grid grid-cols-2 gap-4">
        <div class="col-span-2">
          <label class="block text-sm mb-1">Project Name *</label>
          <input v-model="form.name" class="w-full border rounded-lg px-3 py-2" />
        </div>
        <div>
          <label class="block text-sm mb-1">Customer Company *</label>
          <input v-model="form.customerCompany" class="w-full border rounded-lg px-3 py-2" />
        </div>
        <div>
          <label class="block text-sm mb-1">Executor Company *</label>
          <input v-model="form.executorCompany" class="w-full border rounded-lg px-3 py-2" />
        </div>
        <div>
          <label class="block text-sm mb-1">Start Date *</label>
          <input v-model="form.startDate" type="date" class="w-full border rounded-lg px-3 py-2" />
        </div>
        <div>
          <label class="block text-sm mb-1">End Date *</label>
          <input v-model="form.endDate" type="date" class="w-full border rounded-lg px-3 py-2" />
        </div>
        <div>
          <label class="block text-sm mb-1">Priority *</label>
          <input v-model.number="form.priority" type="number" min="1" class="w-full border rounded-lg px-3 py-2" />
        </div>
        <div>
          <SearchableEmployeePicker
            label="Project Manager *"
            :employees="managers"
            :selected-ids="selectedManagerIds"
            :multiple="false"
            @update:selected-ids="onManagerChange"
          />
        </div>
        <div class="col-span-2">
          <SearchableEmployeePicker
            label="Team Members"
            :employees="teamEmployees"
            :selected-ids="selectedTeamIds"
            :multiple="true"
            @update:selected-ids="onTeamChange"
          />
        </div>
        <div class="col-span-2">
          <label class="block text-sm mb-1">Project Documents</label>
          <ul v-if="existingDocuments.length > 0" class="mb-2 text-sm text-gray-600 space-y-1">
            <li v-for="doc in existingDocuments" :key="doc.id" class="flex items-center justify-between bg-gray-50 px-3 py-2 rounded">
              <button
                type="button"
                class="text-blue-600 hover:underline"
                @click="projectsStore.downloadDocument(projectId, doc.id, doc.fileName)"
              >
                {{ doc.fileName }}
              </button>
              <button type="button" class="text-red-500 hover:underline" @click="deleteExistingDocument(doc.id)">Delete</button>
            </li>
          </ul>
          <!-- Drag & drop zone / Зона drag & drop -->
          <div
            class="border-2 border-dashed rounded-xl p-6 text-center transition-colors mb-2"
            :class="isDragging ? 'border-blue-500 bg-blue-50' : 'border-gray-300 hover:border-blue-400'"
            @dragover.prevent="isDragging = true"
            @dragleave="isDragging = false"
            @drop.prevent="onDrop"
          >
            <p class="text-gray-500 mb-2 text-sm">Drag & drop files here, or</p>
            <label class="cursor-pointer bg-blue-700 text-white px-4 py-1.5 rounded-lg hover:bg-blue-800 text-sm">
              Browse Files
              <input type="file" multiple class="hidden" @change="onFileInput" />
            </label>
          </div>
          <ul v-if="files.length > 0" class="mt-2 text-sm text-gray-600 space-y-1">
            <li v-for="file in files" :key="file.name" class="flex justify-between">
              <span>{{ file.name }}</span>
              <button type="button" class="text-red-500 hover:underline" @click="removeFile(file.name)">Remove</button>
            </li>
          </ul>
        </div>
      </div>

      <div class="mt-6 flex gap-3">
        <button class="bg-blue-700 text-white px-5 py-2 rounded-lg hover:bg-blue-800" :disabled="saving" @click="save">
          {{ saving ? 'Saving...' : 'Save' }}
        </button>
        <button class="border px-5 py-2 rounded-lg hover:bg-gray-50" @click="router.push('/projects')">Cancel</button>
      </div>
    </div>
  </div>
</template>
