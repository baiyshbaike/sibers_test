import { defineStore } from 'pinia'
import { ref } from 'vue'
import apiClient from '@/api/axiosInstance'
import type { Project, ProjectFilter } from '@/types'

// #хранилище_проектов / #projects_store
export const useProjectsStore = defineStore('projects', () => {
  const projects = ref<Project[]>([])
  const current  = ref<Project | null>(null)
  const loading  = ref(false)
  const error    = ref<string | null>(null)

  async function fetchAll(filter?: ProjectFilter): Promise<void> {
    loading.value = true
    error.value   = null
    try {
      const { data } = await apiClient.get<Project[]>('/api/projects', { params: filter })
      projects.value = data
    } catch (e: any) {
      error.value = e.response?.data?.message ?? 'Failed to load projects'
    } finally {
      loading.value = false
    }
  }

  async function fetchById(id: string): Promise<void> {
    loading.value = true
    try {
      const { data } = await apiClient.get<Project>(`/api/projects/${id}`)
      current.value = data
    } finally {
      loading.value = false
    }
  }

  async function create(dto: any): Promise<Project> {
    const { data } = await apiClient.post<Project>('/api/projects', dto)
    await fetchAll()
    return data
  }

  async function update(id: string, dto: any): Promise<void> {
    await apiClient.put(`/api/projects/${id}`, dto)
    await fetchAll()
  }

  async function remove(id: string): Promise<void> {
    await apiClient.delete(`/api/projects/${id}`)
    await fetchAll()
  }

  async function addEmployee(projectId: string, employeeId: string): Promise<void> {
    await apiClient.post(`/api/projects/${projectId}/employees/${employeeId}`)
    await fetchById(projectId)
  }

  async function removeEmployee(projectId: string, employeeId: string): Promise<void> {
    await apiClient.delete(`/api/projects/${projectId}/employees/${employeeId}`)
    await fetchById(projectId)
  }

  async function uploadDocuments(projectId: string, files: File[]): Promise<void> {
    if (files.length === 0) return
    const form = new FormData()
    for (const file of files) form.append('files', file)
    await apiClient.post(`/api/projects/${projectId}/documents`, form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    await fetchById(projectId)
  }

  function getDocumentDownloadUrl(projectId: string, documentId: string): string {
    return `${apiClient.defaults.baseURL}/api/projects/${projectId}/documents/${documentId}/download`
  }

  // Download document via axios (includes JWT token) and trigger browser save
  // Скачивание документов через axios (включает JWT-токен) с сохранением в браузере
  async function downloadDocument(projectId: string, documentId: string, fileName: string): Promise<void> {
    const { data } = await apiClient.get(`/api/projects/${projectId}/documents/${documentId}/download`, {
      responseType: 'blob',
    })
    const url = window.URL.createObjectURL(new Blob([data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', fileName)
    document.body.appendChild(link)
    link.click()
    link.remove()
    window.URL.revokeObjectURL(url)
  }

  async function deleteDocument(projectId: string, documentId: string): Promise<void> {
    await apiClient.delete(`/api/projects/${projectId}/documents/${documentId}`)
    await fetchById(projectId)
  }

  return {
    projects,
    current,
    loading,
    error,
    fetchAll,
    fetchById,
    create,
    update,
    remove,
    addEmployee,
    removeEmployee,
    uploadDocuments,
    deleteDocument,
    getDocumentDownloadUrl,
    downloadDocument,
  }
})