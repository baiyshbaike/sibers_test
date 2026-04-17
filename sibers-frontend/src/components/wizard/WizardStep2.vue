<script setup lang="ts">
// Step 2: Customer and Executor companies / Шаг 2: Заказчик и Исполнитель
const props = defineProps<{ modelValue: any }>()
const emit  = defineEmits(['update:modelValue', 'next', 'prev'])

function update(field: string, value: string) {
  emit('update:modelValue', { ...props.modelValue, [field]: value })
}
</script>

<template>
  <div class="bg-white rounded-xl shadow p-6">
    <h2 class="text-lg font-semibold mb-4">Step 2: Companies</h2>

    <div class="mb-4">
      <label class="block text-sm font-medium text-gray-700 mb-1">Customer Company *</label>
      <input
        :value="props.modelValue.customerCompany"
        type="text"
        class="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500"
        @input="update('customerCompany', ($event.target as HTMLInputElement).value)"
      />
    </div>

    <div class="mb-6">
      <label class="block text-sm font-medium text-gray-700 mb-1">Executor Company *</label>
      <input
        :value="props.modelValue.executorCompany"
        type="text"
        class="w-full border rounded-lg px-3 py-2 focus:ring-2 focus:ring-blue-500"
        @input="update('executorCompany', ($event.target as HTMLInputElement).value)"
      />
    </div>

    <div class="flex justify-between">
      <button class="border px-6 py-2 rounded-lg hover:bg-gray-50" @click="emit('prev')">← Back</button>
      <button class="bg-blue-700 text-white px-6 py-2 rounded-lg hover:bg-blue-800" @click="emit('next')">Next →</button>
    </div>
  </div>
</template>