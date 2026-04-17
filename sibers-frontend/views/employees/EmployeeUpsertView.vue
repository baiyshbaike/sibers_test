<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import apiClient from '@/api/axiosInstance'
import { useEmployeesStore } from '@/stores/employees'
import type { Employee } from '@/types'

const route = useRoute()
const router = useRouter()
const employeesStore = useEmployeesStore()

const isEdit = computed(() => !!route.params.id)
const employeeId = computed(() => (route.params.id as string | undefined) ?? '')
const loading = ref(false)
const saving = ref(false)
const error = ref<string | null>(null)

const form = ref({
  firstName: '',
  lastName: '',
  middleName: '',
  email: '',
})

onMounted(async () => {
  if (!isEdit.value || !employeeId.value) return
  loading.value = true
  try {
    const { data } = await apiClient.get<Employee>(`/api/employees/${employeeId.value}`)
    form.value = {
      firstName: data.firstName,
      lastName: data.lastName,
      middleName: data.middleName ?? '',
      email: data.email,
    }
  } finally {
    loading.value = false
  }
})

async function save() {
  error.value = null
  if (!form.value.firstName.trim() || !form.value.lastName.trim() || !form.value.email.trim()) {
    error.value = 'First name, last name, and email are required.'
    return
  }

  saving.value = true
  try {
    if (isEdit.value && employeeId.value) {
      await employeesStore.update(employeeId.value, form.value)
    } else {
      await employeesStore.create(form.value)
    }
    router.push('/employees')
  } catch (e: any) {
    error.value = e.response?.data?.message ?? 'Failed to save employee'
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <div class="max-w-2xl mx-auto">
    <h1 class="text-2xl font-bold mb-6">{{ isEdit ? 'Edit Employee' : 'Create Employee' }}</h1>
    <div v-if="loading" class="text-gray-500">Loading...</div>
    <div v-else class="bg-white rounded-xl shadow p-6">
      <div v-if="error" class="mb-3 p-2 bg-red-50 text-red-600 rounded text-sm">{{ error }}</div>
      <div class="grid grid-cols-2 gap-4">
        <div>
          <label class="block text-sm mb-1">First Name *</label>
          <input v-model="form.firstName" class="w-full border rounded-lg px-3 py-2" />
        </div>
        <div>
          <label class="block text-sm mb-1">Last Name *</label>
          <input v-model="form.lastName" class="w-full border rounded-lg px-3 py-2" />
        </div>
        <div>
          <label class="block text-sm mb-1">Middle Name</label>
          <input v-model="form.middleName" class="w-full border rounded-lg px-3 py-2" />
        </div>
        <div>
          <label class="block text-sm mb-1">Email *</label>
          <input v-model="form.email" type="email" class="w-full border rounded-lg px-3 py-2" />
        </div>
      </div>
      <div class="mt-6 flex gap-3">
        <button class="bg-blue-700 text-white px-5 py-2 rounded-lg hover:bg-blue-800" :disabled="saving" @click="save">
          {{ saving ? 'Saving...' : 'Save' }}
        </button>
        <button class="border px-5 py-2 rounded-lg hover:bg-gray-50" @click="router.push('/employees')">Cancel</button>
      </div>
    </div>
  </div>
</template>
