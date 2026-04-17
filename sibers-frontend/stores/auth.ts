import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import apiClient from '@/api/axiosInstance'
import type { AuthResponse, LoginDto, RegisterDto } from '@/types'

// #хранилище_авторизации / #auth_store
export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const email = ref<string | null>(localStorage.getItem('email'))
  const role  = ref<string | null>(localStorage.getItem('role'))
  const employeeId = ref<string | null>(localStorage.getItem('employee_id'))

  // Computed: is user logged in / Вычисляемое: авторизован ли пользователь
  const isAuthenticated = computed(() => !!token.value)
  const isSupervisor    = computed(() => role.value === 'Supervisor')
  const isProjectManager = computed(() => role.value === 'ProjectManager')
  const isEmployee      = computed(() => role.value === 'Employee')

  // Computed: user object for easy access / Вычисляемое: объект пользователя для удобного доступа
  const user = computed(() => ({
    email: email.value,
    role: role.value,
    employeeId: employeeId.value
  }))

  async function login(dto: LoginDto): Promise<void> {
    const { data } = await apiClient.post<AuthResponse>('/api/auth/login', dto)
    setAuth(data)
  }

  async function register(dto: RegisterDto): Promise<void> {
    const { data } = await apiClient.post<AuthResponse>('/api/auth/register', dto)
    setAuth(data)
  }

  function logout(): void {
    token.value = null
    email.value = null
    role.value  = null
    employeeId.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('email')
    localStorage.removeItem('role')
    localStorage.removeItem('employee_id')
  }

  // Save auth data to state and localStorage / Сохраняем данные авторизации
  function setAuth(data: AuthResponse): void {
    token.value = data.token
    email.value = data.email
    role.value  = data.role
    // Extract employee_id from JWT token payload
    if (data.token) {
      try {
        const payload = JSON.parse(atob(data.token.split('.')[1]))
        employeeId.value = payload.employee_id || null
        if (employeeId.value) {
          localStorage.setItem('employee_id', employeeId.value)
        }
      } catch (e) {
        console.error('Failed to parse JWT token:', e)
      }
    }
    localStorage.setItem('token', data.token)
    localStorage.setItem('email', data.email)
    localStorage.setItem('role', data.role)
  }

  return { token, email, role, employeeId, user, isAuthenticated, isSupervisor, isProjectManager, isEmployee, login, register, logout }
})