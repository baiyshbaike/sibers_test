<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useProjectsStore } from '@/stores/projects'
import { useAuthStore } from '@/stores/auth'
import type { ProjectFilter } from '@/types'

const router = useRouter()
const projectsStore = useProjectsStore()
const authStore = useAuthStore()

const filter = ref<ProjectFilter>({
  sortBy: 'name',
  sortDescending: false,
})

onMounted(() => projectsStore.fetchAll(filter.value))

function applyFilter() {
  projectsStore.fetchAll(filter.value)
}

async function deleteProject(id: string) {
  if (!confirm('Delete this project?')) return
  await projectsStore.remove(id)
}
</script>

<template>
  <div>
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-800">Projects</h1>
      <RouterLink
        v-if="!authStore.isEmployee"
        to="/projects/upsert"
        class="bg-blue-700 text-white px-4 py-2 rounded-lg hover:bg-blue-800 text-sm font-medium"
      >
        + New Project
      </RouterLink>
    </div>

    <!-- Filters / Фильтры -->
    <div class="bg-white rounded-xl shadow p-4 mb-6 flex flex-wrap gap-4">
      <div>
        <label class="text-xs text-gray-500 block mb-1">Start date from</label>
        <input v-model="filter.startDateFrom" type="date" class="border rounded px-2 py-1 text-sm" />
      </div>
      <div>
        <label class="text-xs text-gray-500 block mb-1">Start date to</label>
        <input v-model="filter.startDateTo" type="date" class="border rounded px-2 py-1 text-sm" />
      </div>
      <div>
        <label class="text-xs text-gray-500 block mb-1">Priority</label>
        <input v-model.number="filter.priority" type="number" min="1" class="border rounded px-2 py-1 text-sm w-20" />
      </div>
      <div>
        <label class="text-xs text-gray-500 block mb-1">Sort by</label>
        <select v-model="filter.sortBy" class="border rounded px-2 py-1 text-sm">
          <option value="name">Name</option>
          <option value="startDate">Start Date</option>
          <option value="endDate">End Date</option>
          <option value="priority">Priority</option>
        </select>
      </div>
      <div class="flex items-end">
        <label class="flex items-center gap-1 text-sm cursor-pointer">
          <input v-model="filter.sortDescending" type="checkbox" />
          Descending
        </label>
      </div>
      <div class="flex items-end">
        <button
          class="bg-blue-700 text-white px-4 py-1.5 rounded-lg text-sm hover:bg-blue-800"
          @click="applyFilter"
        >
          Apply
        </button>
      </div>
    </div>

    <div v-if="projectsStore.loading" class="text-center py-10 text-gray-500">Loading...</div>

    <!-- Error / Ошибка -->
    <div v-else-if="projectsStore.error" class="text-red-600 text-center py-10">
      {{ projectsStore.error }}
    </div>

    <!-- Projects list / Список проектов -->
    <div v-else class="grid gap-4">
      <div
        v-for="project in projectsStore.projects"
        :key="project.id"
        class="bg-white rounded-xl shadow p-5 flex items-center justify-between hover:shadow-md transition"
      >
        <div>
          <h2 class="font-semibold text-gray-800 text-lg">{{ project.name }}</h2>
          <p class="text-sm text-gray-500">
            {{ project.customerCompany }} → {{ project.executorCompany }}
          </p>
          <p class="text-xs text-gray-400 mt-1">
            {{ project.startDate.slice(0, 10) }} — {{ project.endDate.slice(0, 10) }}
            · Priority: {{ project.priority }}
          </p>
          <p class="text-xs text-gray-400">
            Manager: {{ project.projectManager?.fullName ?? '—' }}
          </p>
        </div>
        <div class="flex gap-2">
          <button
            class="text-blue-600 hover:underline text-sm"
            @click="router.push(`/projects/${project.id}`)"
          >
            View
          </button>
          <button
            v-if="!authStore.isEmployee"
            class="text-blue-600 hover:underline text-sm"
            @click="router.push(`/projects/upsert/${project.id}`)"
          >
            Edit
          </button>
          <button
            v-if="!authStore.isEmployee"
            class="text-red-500 hover:underline text-sm"
            @click="deleteProject(project.id)"
          >
            Delete
          </button>
        </div>
      </div>

      <p v-if="projectsStore.projects.length === 0" class="text-center text-gray-400 py-10">
        No projects found.
      </p>
    </div>
  </div>
</template>