<script setup lang="ts">
// Step 5: HTML5 File uploader with drag & drop
// Шаг 5: Загрузка файлов с поддержкой drag & drop
import { ref } from 'vue'

const props = defineProps<{ modelValue: any }>()
const emit  = defineEmits(['update:modelValue', 'prev', 'submit'])

const isDragging = ref(false)
const files      = ref<File[]>([...( props.modelValue.documents ?? [])])

function onDrop(event: DragEvent) {
  isDragging.value = false
  const dropped = Array.from(event.dataTransfer?.files ?? [])
  addFiles(dropped)
}

function onFileInput(event: Event) {
  const input = event.target as HTMLInputElement
  addFiles(Array.from(input.files ?? []))
}

function addFiles(newFiles: File[]) {
  // Prevent duplicates by name / Предотвращаем дубликаты по имени
  const existing = files.value.map(f => f.name)
  files.value.push(...newFiles.filter(f => !existing.includes(f.name)))
  emit('update:modelValue', { ...props.modelValue, documents: files.value })
}

function removeFile(name: string) {
  files.value = files.value.filter(f => f.name !== name)
  emit('update:modelValue', { ...props.modelValue, documents: files.value })
}

function formatSize(bytes: number): string {
  return bytes < 1024 * 1024
    ? `${(bytes / 1024).toFixed(1)} KB`
    : `${(bytes / 1024 / 1024).toFixed(1)} MB`
}
</script>

<template>
  <div class="bg-white rounded-xl shadow p-6">
    <h2 class="text-lg font-semibold mb-4">Step 5: Project Documents</h2>

    <!-- Drag & drop zone / Зона для drag & drop -->
    <div
      class="border-2 border-dashed rounded-xl p-10 text-center transition-colors mb-4"
      :class="isDragging ? 'border-blue-500 bg-blue-50' : 'border-gray-300 hover:border-blue-400'"
      @dragover.prevent="isDragging = true"
      @dragleave="isDragging = false"
      @drop.prevent="onDrop"
    >
      <p class="text-gray-500 mb-3">Drag & drop files here, or</p>
      <label class="cursor-pointer bg-blue-700 text-white px-4 py-2 rounded-lg hover:bg-blue-800 text-sm">
        Browse Files
        <input type="file" multiple class="hidden" @change="onFileInput" />
      </label>
    </div>

    <!-- File list / Список файлов -->
    <ul v-if="files.length > 0" class="mb-6 space-y-2">
      <li
        v-for="file in files"
        :key="file.name"
        class="flex items-center justify-between bg-gray-50 rounded-lg px-4 py-2 text-sm"
      >
        <span class="text-gray-700">{{ file.name }}</span>
        <div class="flex items-center gap-3">
          <span class="text-gray-400">{{ formatSize(file.size) }}</span>
          <button class="text-red-500 hover:text-red-700" @click="removeFile(file.name)">Remove</button>
        </div>
      </li>
    </ul>

    <div class="flex justify-between">
      <button class="border px-6 py-2 rounded-lg hover:bg-gray-50" @click="emit('prev')">← Back</button>
      <button
        class="bg-green-600 text-white px-6 py-2 rounded-lg hover:bg-green-700 font-semibold"
        @click="emit('submit')"
      >
        ✓ Create Project
      </button>
    </div>
  </div>
</template>