<script setup lang="ts">
// Step 4: Select project employees with AJAX search
// Шаг 4: Выбор исполнителей с AJAX поиском
import { ref } from 'vue'
import { useEmployeesStore } from '@/stores/employees'
import type { Employee } from '@/types'

const props = defineProps<{ modelValue: any }>()
const emit  = defineEmits(['update:modelValue', 'next', 'prev'])

const employeesStore = useEmployeesStore()

const searchQuery   = ref('')
const searchResults = ref<Employee[]>([])
const selected      = ref<Employee[]>([])
let debounceTimer: ReturnType<typeof setTimeout>

async function onSearch() {
  clearTimeout(debounceTimer)
  if (searchQuery.value.length < 2) { searchResults.value = []; return }
  debounceTimer = setTimeout(async () => {
    const results = await employeesStore.search(searchQuery.value)
    // Filter out already selected / Исключаем уже выбранных
    searchResults.value = results.filter((r: Employee) => !selected.value.find((s: Employee) => s.id === r.id))
  }, 300)
}

function addEmployee(employee: Employee) {
  selected.value.push(employee)
  searchQuery.value   = ''
  searchResults.value = []
  emit('update:modelValue', { ...props.modelValue, employeeIds: selected.value.map((e: Employee) => e.id) })
}

function removeEmployee(id: string) {
  selected.value = selected.value.filter((e: Employee) => e.id !== id)
  emit('update:modelValue', { ...props.modelValue, employeeIds: selected.value.map((e: Employee) => e.id) })
}
</script>

<template>
  <div class="bg-white rounded-xl shadow p-6">
    <h2 class="text-lg font-semibold mb-4">Step 4: Team Members</h2>

    <!-- Selected employees / Выбранные сотрудники -->
    <div v-if="selected.length > 0" class="mb-4 flex flex-wrap gap-2">
      <span
        v-for="emp in selected"
        :key="emp.id"
        class="bg-blue-100 text-blue-800 text-sm px-3 py-1 rounded-full flex items-center gap-1"
      >
        {{ emp.fullName }}
        <button class="ml-1 text-blue-500 hover:text-red-600" @click="removeEmployee(emp.id)">×</button>
      </span>
    </div>

    <div class="relative mb-6">
      <input
        v-model="searchQuery"
        type="text"
        class="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500"
        placeholder="Search and add employees..."
        @input="onSearch"
      />
      <ul
        v-if="searchResults.length > 0"
        class="absolute z-10 w-full bg-white border rounded-lg shadow-lg mt-1 max-h-48 overflow-y-auto"
      >
        <li
          v-for="emp in searchResults"
          :key="emp.id"
          class="px-4 py-2 hover:bg-blue-50 cursor-pointer text-sm"
          @click="addEmployee(emp)"
        >
          {{ emp.fullName }} — {{ emp.email }}
        </li>
      </ul>
    </div>

    <div class="flex justify-between">
      <button class="border px-6 py-2 rounded-lg hover:bg-gray-50" @click="emit('prev')">← Back</button>
      <button class="bg-blue-700 text-white px-6 py-2 rounded-lg hover:bg-blue-800" @click="emit('next')">Next →</button>
    </div>
  </div>
</template>