import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import apiClient from '@/api/axiosInstance'
import type { AuthResponse, LoginDto, RegisterDto } from '@/types'

// #хранилище_авторизации / #auth_store
export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const email = ref<string | null>(localStorage.getItem('email'))
  const role  = ref<string | null>(localStorage.getItem('role'))

  // Computed: is user logged in / Вычисляемое: авторизован ли пользователь
  const isAuthenticated = computed(() => !!token.value)
  const isSupervisor    = computed(() => role.value === 'Supervisor')
  const isProjectManager = computed(() => role.value === 'ProjectManager')
  const isEmployee      = computed(() => role.value === 'Employee')

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
    localStorage.removeItem('token')
    localStorage.removeItem('email')
    localStorage.removeItem('role')
  }

  // Save auth data to state and localStorage / Сохраняем данные авторизации
  function setAuth(data: AuthResponse): void {
    token.value = data.token
    email.value = data.email
    role.value  = data.role
    localStorage.setItem('token', data.token)
    localStorage.setItem('email', data.email)
    localStorage.setItem('role', data.role)
  }

  return { token, email, role, isAuthenticated, isSupervisor, isProjectManager, isEmployee, login, register, logout }
})