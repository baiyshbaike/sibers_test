import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

// #маршрутизатор / #router
// Vue Router with route guards for authentication and role-based access
const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/login',
      name: 'Login',
      component: () => import('@/views/LoginView.vue'),
      meta: { requiresGuest: true },
    },
    {
      path: '/',
      name: 'Home',
      redirect: '/projects',
    },
    {
      path: '/projects',
      name: 'Projects',
      component: () => import('@/views/projects/ProjectsView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/projects/upsert',
      name: 'ProjectCreate',
      component: () => import('@/views/projects/ProjectUpsertView.vue'),
      meta: { requiresAuth: true, roles: ['Supervisor', 'ProjectManager'] },
    },
    {
      path: '/projects/upsert/:id',
      name: 'ProjectEdit',
      component: () => import('@/views/projects/ProjectUpsertView.vue'),
      meta: { requiresAuth: true, roles: ['Supervisor', 'ProjectManager'] },
    },
    {
      path: '/projects/:id',
      name: 'ProjectDetail',
      component: () => import('@/views/projects/ProjectDetailView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/employees',
      name: 'Employees',
      component: () => import('@/views/employees/EmployeesView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/employees/upsert',
      name: 'EmployeeCreate',
      component: () => import('@/views/employees/EmployeeUpsertView.vue'),
      meta: { requiresAuth: true, roles: ['Supervisor'] },
    },
    {
      path: '/employees/upsert/:id',
      name: 'EmployeeEdit',
      component: () => import('@/views/employees/EmployeeUpsertView.vue'),
      meta: { requiresAuth: true, roles: ['Supervisor'] },
    },
    {
      path: '/employees/:id',
      name: 'EmployeeDetail',
      component: () => import('@/views/employees/EmployeeDetailView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/tasks',
      name: 'Tasks',
      component: () => import('@/views/tasks/TasksView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/tasks/upsert',
      name: 'TaskCreate',
      component: () => import('@/views/tasks/TaskUpsertView.vue'),
      meta: { requiresAuth: true, roles: ['Supervisor', 'ProjectManager'] },
    },
    {
      path: '/tasks/upsert/:id',
      name: 'TaskEdit',
      component: () => import('@/views/tasks/TaskUpsertView.vue'),
      meta: { requiresAuth: true, roles: ['Supervisor', 'ProjectManager'] },
    },
    {
      path: '/tasks/:id',
      name: 'TaskDetail',
      component: () => import('@/views/tasks/TaskDetailView.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/:pathMatch(.*)*',
      name: 'NotFound',
      component: () => import('@/views/NotFoundView.vue'),
    },
  ],
})

// Navigation guard / Охранник навигации
router.beforeEach((to, _from, next) => {
  const authStore = useAuthStore()

  // Redirect unauthenticated users to login / Перенаправляем неавторизованных на логин
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return next('/login')
  }

  // Redirect authenticated users away from login / Авторизованных перенаправляем с логина
  if (to.meta.requiresGuest && authStore.isAuthenticated) {
    return next('/projects')
  }

  // Role-based access check / Проверка доступа по роли
  if (to.meta.roles && Array.isArray(to.meta.roles)) {
    const userRole = authStore.role
    if (!userRole || !(to.meta.roles as string[]).includes(userRole)) {
      return next('/projects') // Redirect to safe page / Перенаправляем на безопасную страницу
    }
  }

  next()
})

export default router