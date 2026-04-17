<script setup lang="ts">
// #список_сотрудников / #employees_view
import { onMounted, ref } from 'vue'
import { useEmployeesStore } from '@/stores/employees'

const store = useEmployeesStore()
onMounted(() => store.fetchAll())

const showForm = ref(false)
const form     = ref({ firstName: '', lastName: '', middleName: '', email: '' })
const editId   = ref<string | null>(null)
const error    = ref<string | null>(null)

function openCreate() {
  form.value  = { firstName: '', lastName: '', middleName: '', email: '' }
  editId.value = null
  showForm.value = true
  error.value = null
}

async function saveEmployee() {
  error.value = null
  try {
    if (editId.value) {
      await store.update(editId.value, form.value)
    } else {
      await store.create(form.value)
    }
    showForm.value = false
  } catch (e: any) {
    error.value = e.response?.data?.message ?? 'Error saving employee'
  }
}

async function deleteEmployee(id: string) {
  if (!confirm('Delete this employee?')) return
  await store.remove(id)
}
</script>

<template>
  <div>
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-800">Employees</h1>
      <button
        class="bg-blue-700 text-white px-4 py-2 rounded-lg hover:bg-blue-800 text-sm"
        @click="openCreate"
      >
        + New Employee
      </button>
    </div>

    <!-- Form / Форма -->
    <div v-if="showForm" class="bg-white rounded-xl shadow p-6 mb-6">
      <h2 class="font-semibold mb-4">{{ editId ? 'Edit' : 'Create' }} Employee</h2>
      <div v-if="error" class="mb-3 p-2 bg-red-50 text-red-600 rounded text-sm">{{ error }}</div>
      <div class="grid grid-cols-2 gap-4 mb-4">
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
      <div class="flex gap-3">
        <button class="bg-blue-700 text-white px-5 py-2 rounded-lg hover:bg-blue-800" @click="saveEmployee">Save</button>
        <button class="border px-5 py-2 rounded-lg hover:bg-gray-50" @click="showForm = false">Cancel</button>
      </div>
    </div>

    <!-- Table / Таблица -->
    <div class="bg-white rounded-xl shadow overflow-hidden">
      <table class="w-full text-sm">
        <thead class="bg-gray-50 text-gray-600">
          <tr>
            <th class="text-left px-4 py-3">Full Name</th>
            <th class="text-left px-4 py-3">Email</th>
            <th class="px-4 py-3"></th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="emp in store.employees"
            :key="emp.id"
            class="border-t hover:bg-gray-50"
          >
            <td class="px-4 py-3">{{ emp.fullName }}</td>
            <td class="px-4 py-3 text-gray-500">{{ emp.email }}</td>
            <td class="px-4 py-3 text-right">
              <button class="text-red-500 hover:underline text-xs" @click="deleteEmployee(emp.id)">Delete</button>
            </td>
          </tr>
          <tr v-if="store.employees.length === 0">
            <td colspan="3" class="text-center py-8 text-gray-400">No employees found.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>