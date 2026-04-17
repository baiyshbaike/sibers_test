<script setup lang="ts">
// Step 3: Select project manager with AJAX search
// Шаг 3: Выбор руководителя с AJAX поиском
import { ref } from 'vue'
import { useEmployeesStore } from '@/stores/employees'
import type { Employee } from '@/types'

const props = defineProps<{ modelValue: any }>()
const emit  = defineEmits(['update:modelValue', 'next', 'prev'])

const employeesStore = useEmployeesStore()

const searchQuery    = ref('')
const searchResults  = ref<Employee[]>([])
const selectedManager = ref<Employee | null>(null)
let debounceTimer: ReturnType<typeof setTimeout>

// AJAX search with debounce / AJAX поиск с задержкой
async function onSearch() {
  clearTimeout(debounceTimer)
  if (searchQuery.value.length < 2) {
    searchResults.value = []
    return
  }
  debounceTimer = setTimeout(async () => {
    searchResults.value = await employeesStore.search(searchQuery.value)
  }, 300)
}

function selectManager(employee: Employee) {
  selectedManager.value = employee
  searchQuery.value     = employee.fullName
  searchResults.value   = []
  emit('update:modelValue', { ...props.modelValue, projectManagerId: employee.id })
}
</script>

<template>
  <div class="bg-white rounded-xl shadow p-6">
    <h2 class="text-lg font-semibold mb-4">Step 3: Project Manager</h2>

    <div class="mb-6 relative">
      <label class="block text-sm font-medium text-gray-700 mb-1">
        Search manager (type name or email)
      </label>
      <input
        v-model="searchQuery"
        type="text"
        class="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500"
        placeholder="Start typing..."
        @input="onSearch"
      />

      <!-- Dropdown results / Выпадающий список -->
      <ul
        v-if="searchResults.length > 0"
        class="absolute z-10 w-full bg-white border rounded-lg shadow-lg mt-1 max-h-48 overflow-y-auto"
      >
        <li
          v-for="emp in searchResults"
          :key="emp.id"
          class="px-4 py-2 hover:bg-blue-50 cursor-pointer text-sm"
          @click="selectManager(emp)"
        >
          {{ emp.fullName }} — {{ emp.email }}
        </li>
      </ul>

      <p v-if="selectedManager" class="text-green-600 text-sm mt-2">
        ✓ Selected: {{ selectedManager.fullName }}
      </p>
    </div>

    <div class="flex justify-between">
      <button class="border px-6 py-2 rounded-lg hover:bg-gray-50" @click="emit('prev')">← Back</button>
      <button class="bg-blue-700 text-white px-6 py-2 rounded-lg hover:bg-blue-800" @click="emit('next')">Next →</button>
    </div>
  </div>
</template>