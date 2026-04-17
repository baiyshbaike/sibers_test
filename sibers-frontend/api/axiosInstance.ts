import axios from 'axios'

// #экземпляр_axios / #axios_instance
// Configured axios instance with base URL and JWT interceptor
const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:5050',
  headers: {
    'Content-Type': 'application/json',
  },
})

// Add JWT token to every request if available
// Добавляем JWT токен к каждому запросу если он есть
apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Handle 401 — redirect to login / Обрабатываем 401 — перенаправляем на логин
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default apiClient