<script setup lang="ts">
import { computed, ref } from 'vue'
import type { Employee } from '@/types'
import { employeesForPicker } from '@/src/utils/employeePicker'

const props = withDefaults(defineProps<{
  label: string
  employees: Employee[]
  selectedIds: string[]
  multiple?: boolean
  placeholder?: string
}>(), {
  multiple: false,
  placeholder: 'Click or type to filter…',
})

const emit = defineEmits<{
  (e: 'update:selectedIds', value: string[]): void
}>()

const query = ref('')
const listOpen = ref(false)
let blurTimer: ReturnType<typeof setTimeout> | undefined

const selectedSet = computed(() => new Set(props.selectedIds))
const selectedItems = computed(() =>
  props.selectedIds
    .map((id) => props.employees.find((e) => e.id === id))
    .filter(Boolean) as Employee[],
)
const visibleList = computed(() =>
  employeesForPicker(props.employees, query.value, props.multiple ? selectedSet.value : new Set<string>()),
)

function open() {
  listOpen.value = true
}

function closeSoon() {
  clearTimeout(blurTimer)
  blurTimer = setTimeout(() => {
    listOpen.value = false
  }, 200)
}

function select(emp: Employee) {
  if (props.multiple) {
    if (!selectedSet.value.has(emp.id)) {
      emit('update:selectedIds', [...props.selectedIds, emp.id])
    }
    query.value = ''
    listOpen.value = true
    return
  }

  emit('update:selectedIds', [emp.id])
  query.value = ''
  listOpen.value = false
}

function remove(id: string) {
  emit('update:selectedIds', props.selectedIds.filter((x) => x !== id))
}

function clearSingle() {
  emit('update:selectedIds', [])
}
</script>

<template>
  <div class="relative">
    <label class="block text-sm mb-1">{{ label }}</label>

    <div v-if="multiple && selectedItems.length > 0" class="mb-2 flex flex-wrap gap-2">
      <span
        v-for="emp in selectedItems"
        :key="emp.id"
        class="bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded-full flex items-center gap-1"
      >
        {{ emp.fullName }}
        <button type="button" class="text-blue-600 hover:text-red-600" @click="remove(emp.id)">×</button>
      </span>
    </div>

    <input
      v-model="query"
      class="w-full border rounded-lg px-3 py-2"
      :placeholder="placeholder"
      autocomplete="off"
      @focus="open"
      @blur="closeSoon"
    />

    <ul
      v-if="listOpen && visibleList.length > 0"
      class="absolute z-20 w-full bg-white border rounded-lg shadow-lg mt-1 max-h-48 overflow-y-auto"
    >
      <li
        v-for="emp in visibleList"
        :key="emp.id"
        class="px-3 py-2 hover:bg-blue-50 cursor-pointer text-sm"
        @mousedown.prevent="select(emp)"
      >
        {{ emp.fullName }} — {{ emp.email }}
      </li>
    </ul>

    <div v-if="!multiple && selectedItems[0]" class="mt-2 flex items-center gap-3">
      <span class="text-green-600 text-sm">✓ Selected: {{ selectedItems[0].fullName }}</span>
      <button type="button" class="text-red-600 text-sm hover:underline" @click="clearSingle">Remove</button>
    </div>
  </div>
</template>
