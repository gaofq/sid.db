import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/login/index.vue'),
    meta: { title: '登录', public: true },
  },
  {
    path: '/',
    redirect: '/databases',
  },
  {
    path: '/databases',
    name: 'Databases',
    component: () => import('@/views/databases/index.vue'),
    meta: { title: '目标数据库' },
  },
  {
    path: '/tables',
    name: 'Tables',
    component: () => import('@/views/tables/index.vue'),
    meta: { title: '数据表管理' },
  },
  {
    path: '/tables/:tableId/fields',
    name: 'Fields',
    component: () => import('@/views/fields/index.vue'),
    meta: { title: '字段管理' },
  },
  {
    path: '/execution-logs',
    name: 'ExecutionLogs',
    component: () => import('@/views/execution-logs/index.vue'),
    meta: { title: '执行日志' },
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// Pinia must be installed before we can use useAuthStore inside the guard
let authInitialized = false;

router.beforeEach(async (to) => {
  // Lazy-load auth store to avoid circular dependency
  const auth = useAuthStore();

  // Public routes don't need auth
  if (to.meta.public) {
    return true;
  }

  // Check auth on first navigation
  if (!authInitialized) {
    await auth.checkAuth();
    authInitialized = true;
  }

  if (!auth.isAuthenticated) {
    return { name: 'Login', query: { redirect: to.fullPath } };
  }

  return true;
});

export default router;
