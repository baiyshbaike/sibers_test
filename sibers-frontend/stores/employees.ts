import { defineStore } from 'pinia'
import { ref } from 'vue'
import apiClient from '@/api/axiosInstance'
import type { Employee } from '@/types'

// #хранилище_сотрудников / #employees_store
export const useEmployeesStore = defineStore('employees', () => {
  const employees = ref<Employee[]>([])
  const loading   = ref(false)
  const error     = ref<string | null>(null)

  async function fetchAll(): Promise<void> {
    loading.value = true
    error.value   = null
    try {
      const { data } = await apiClient.get<Employee[]>('/api/employees')
      employees.value = data
    } catch (e: any) {
      error.value = e.response?.data?.message ?? 'Failed to load employees'
    } finally {
      loading.value = false
    }
  }

  async function search(query: string): Promise<Employee[]> {
    const { data } = await apiClient.get<Employee[]>('/api/employees/search', {
      params: { q: query },
    })
    return data
  }

  async function create(dto: Omit<Employee, 'id' | 'fullName'>): Promise<void> {
    await apiClient.post('/api/employees', dto)
    await fetchAll()
  }

  async function update(id: string, dto: Omit<Employee, 'id' | 'fullName'>): Promise<void> {
    await apiClient.put(`/api/employees/${id}`, dto)
    await fetchAll()
  }

  async function remove(id: string): Promise<void> {
    await apiClient.delete(`/api/employees/${id}`)
    await fetchAll()
  }

  return { employees, loading, error, fetchAll, search, create, update, remove }
})