<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useEmployeesStore } from '@/stores/employees'

const router = useRouter()
const store = useEmployeesStore()
onMounted(() => store.fetchAll())

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
        @click="router.push('/employees/upsert')"
      >
        + New Employee
      </button>
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
            <td class="px-4 py-3 text-right space-x-3">
              <button class="text-blue-600 hover:underline text-xs" @click="router.push(`/employees/${emp.id}`)">View</button>
              <button class="text-blue-600 hover:underline text-xs" @click="router.push(`/employees/upsert/${emp.id}`)">Edit</button>
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