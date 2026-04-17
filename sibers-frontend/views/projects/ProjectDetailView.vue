<script setup lang="ts">
// #детали_проекта / #project_detail_view
import { onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { useProjectsStore } from '@/stores/projects'

const route         = useRoute()
const projectsStore = useProjectsStore()

onMounted(() => projectsStore.fetchById(route.params.id as string))
</script>

<template>
  <div>
    <div v-if="projectsStore.loading">Loading...</div>
    <div v-else-if="projectsStore.current">
      <h1 class="text-2xl font-bold mb-2">{{ projectsStore.current.name }}</h1>
      <p class="text-gray-600 mb-1">Customer: {{ projectsStore.current.customerCompany }}</p>
      <p class="text-gray-600 mb-1">Executor: {{ projectsStore.current.executorCompany }}</p>
      <p class="text-gray-600 mb-1">Manager: {{ projectsStore.current.projectManager?.fullName }}</p>
      <p class="text-gray-600 mb-4">
        {{ projectsStore.current.startDate.slice(0,10) }} — {{ projectsStore.current.endDate.slice(0,10) }}
        · Priority: {{ projectsStore.current.priority }}
      </p>

      <h2 class="font-semibold mb-2">Team ({{ projectsStore.current.employees.length }})</h2>
      <ul class="list-disc pl-5 text-sm text-gray-700">
        <li v-for="emp in projectsStore.current.employees" :key="emp.id">
          {{ emp.fullName }} — {{ emp.email }}
        </li>
      </ul>

      <h2 class="font-semibold mt-6 mb-2">Documents ({{ projectsStore.current.documents?.length ?? 0 }})</h2>
      <ul v-if="(projectsStore.current.documents?.length ?? 0) > 0" class="space-y-2">
        <li
          v-for="doc in projectsStore.current.documents"
          :key="doc.id"
          class="flex items-center justify-between bg-gray-50 px-3 py-2 rounded text-sm"
        >
          <span>{{ doc.fileName }}</span>
          <a
            class="text-blue-600 hover:underline"
            :href="projectsStore.getDocumentDownloadUrl(projectsStore.current.id, doc.id)"
            target="_blank"
            rel="noopener noreferrer"
          >
            Download
          </a>
        </li>
      </ul>
      <p v-else class="text-sm text-gray-400">No documents uploaded.</p>
    </div>
    <div v-else class="text-gray-400">Project not found.</div>
  </div>
</template>