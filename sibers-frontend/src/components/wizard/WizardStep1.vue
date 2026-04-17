<script setup lang="ts">
// Step 1: Project name, dates, priority / Шаг 1: название, даты, приоритет
const props  = defineProps<{ modelValue: any }>()
const emit   = defineEmits(['update:modelValue', 'next'])

function update(field: string, value: any) {
  emit('update:modelValue', { ...props.modelValue, [field]: value })
}
</script>

<template>
  <div class="bg-white rounded-xl shadow p-6">
    <h2 class="text-lg font-semibold mb-4">Step 1: Project Info</h2>

    <div class="mb-4">
      <label class="block text-sm font-medium text-gray-700 mb-1">Project Name *</label>
      <input
        :value="props.modelValue.name"
        type="text"
        class="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500"
        @input="update('name', ($event.target as HTMLInputElement).value)"
      />
    </div>

    <div class="grid grid-cols-2 gap-4 mb-4">
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">Start Date *</label>
        <input
          :value="props.modelValue.startDate"
          type="date"
          class="w-full border rounded-lg px-3 py-2"
          @input="update('startDate', ($event.target as HTMLInputElement).value)"
        />
      </div>
      <div>
        <label class="block text-sm font-medium text-gray-700 mb-1">End Date *</label>
        <input
          :value="props.modelValue.endDate"
          type="date"
          class="w-full border rounded-lg px-3 py-2"
          @input="update('endDate', ($event.target as HTMLInputElement).value)"
        />
      </div>
    </div>

    <div class="mb-6">
      <label class="block text-sm font-medium text-gray-700 mb-1">Priority *</label>
      <input
        :value="props.modelValue.priority"
        type="number"
        min="1"
        class="w-full border rounded-lg px-3 py-2"
        @input="update('priority', Number(($event.target as HTMLInputElement).value))"
      />
    </div>

    <div class="flex justify-end">
      <button
        class="bg-blue-700 text-white px-6 py-2 rounded-lg hover:bg-blue-800"
        @click="emit('next')"
      >
        Next →
      </button>
    </div>
  </div>
</template>