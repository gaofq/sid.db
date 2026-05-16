import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import axios from 'axios';
import { getAntiForgeryToken } from '@/api/index';

export interface UserInfo {
  userName: string;
  email?: string;
  name?: string;
  surName?: string;
}

interface AppConfig {
  currentUser?: {
    isAuthenticated: boolean;
    userName: string;
    email: string;
    name: string;
    surName: string;
  };
}

const baseApi = axios.create({
  baseURL: '/api',
  timeout: 30000,
  headers: { 'Content-Type': 'application/json' },
});

// Copy interceptors from main api instance
baseApi.interceptors.request.use((config) => {
  config.headers = config.headers || {};
  const method = (config.method || 'get').toLowerCase();
  if (method !== 'get' && method !== 'head' && method !== 'options') {
    const token = getAntiForgeryToken();
    if (token) config.headers['RequestVerificationToken'] = token;
  }
  if (!config.headers['X-Requested-With']) {
    config.headers['X-Requested-With'] = 'XMLHttpRequest';
  }
  return config;
});

export const useAuthStore = defineStore('auth', () => {
  const user = ref<UserInfo | null>(null);
  const loading = ref(false);
  const loginError = ref('');

  const isAuthenticated = computed(() => !!user.value);
  const userName = computed(() => user.value?.userName ?? '');

  async function checkAuth(): Promise<boolean> {
    try {
      const res = await baseApi.get<AppConfig>('/abp/application-configuration');
      if (res.data?.currentUser?.isAuthenticated) {
        user.value = {
          userName: res.data.currentUser.userName,
          email: res.data.currentUser.email,
          name: res.data.currentUser.name,
          surName: res.data.currentUser.surName,
        };
        return true;
      }
      user.value = null;
      return false;
    } catch {
      user.value = null;
      return false;
    }
  }

  async function fetchAntiForgery(): Promise<string | null> {
    try {
      await baseApi.get('/abp/application-configuration');
    } catch {
      // ignore
    }
    return getAntiForgeryToken();
  }

  async function login(username: string, password: string): Promise<boolean> {
    loading.value = true;
    loginError.value = '';
    try {
      const token = await fetchAntiForgery();

      const headers: Record<string, string> = {};
      if (token) headers['RequestVerificationToken'] = token;

      await baseApi.post(
        '/account/login',
        { userNameOrEmailAddress: username, password, rememberMe: true },
        { headers },
      );

      const ok = await checkAuth();
      if (!ok) {
        loginError.value = '登录失败，请检查凭据';
      }
      return ok;
    } catch (err: any) {
      const msg = err?.response?.data?.error?.message || err?.message || '登录失败';
      loginError.value = msg;
      user.value = null;
      return false;
    } finally {
      loading.value = false;
    }
  }

  async function logout() {
    try {
      const token = getAntiForgeryToken();
      const headers: Record<string, string> = {};
      if (token) headers['RequestVerificationToken'] = token;
      await baseApi.get('/account/logout', { headers });
    } catch {
      // ignore
    } finally {
      user.value = null;
      loginError.value = '';
    }
  }

  return { user, loading, loginError, isAuthenticated, userName, checkAuth, login, logout };
});
