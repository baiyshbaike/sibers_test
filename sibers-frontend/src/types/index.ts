// #типы / #types
// Shared TypeScript types matching backend DTOs

export interface Employee {
    id: string
    firstName: string
    lastName: string
    middleName?: string
    email: string
    fullName: string
  }
  
  export interface Project {
    id: string
    name: string
    customerCompany: string
    executorCompany: string
    startDate: string
    endDate: string
    priority: number
    projectManager?: Employee
    employees: Employee[]
  documents: ProjectDocument[]
  }

export interface ProjectDocument {
  id: string
  fileName: string
  contentType: string
  size: number
  uploadedAtUtc: string
}
  
  export interface TaskItem {
    id: string
    name: string
    comment?: string
    priority: number
    status: TaskStatus
    projectId: string
    projectName: string
    author?: Employee
    executor?: Employee
  }
  
  export enum TaskStatus {
    ToDo = 0,
    InProgress = 1,
    Done = 2,
  }
  
  export interface AuthResponse {
    token: string
    email: string
    role: string
    expiresAt: string
  }
  
  export interface LoginDto {
    email: string
    password: string
  }
  
  export interface RegisterDto {
    email: string
    password: string
    role: string
    employeeId?: string
  }
  
  // Filter types / Типы фильтров
  export interface ProjectFilter {
    startDateFrom?: string
    startDateTo?: string
    priority?: number
    sortBy?: string
    sortDescending?: boolean
  }
  
  export interface TaskFilter {
    projectId?: string
    status?: TaskStatus
    executorId?: string
    sortBy?: string
    sortDescending?: boolean
  }