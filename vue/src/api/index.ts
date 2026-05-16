import axios from 'axios';

const api = axios.create({
  baseURL: '/api/app',
  timeout: 30000,
  headers: { 'Content-Type': 'application/json' },
});

function getAntiForgeryToken(): string | null {
  const entries = document.cookie.split(';');
  for (const entry of entries) {
    const trimmed = entry.trim();
    if (trimmed.startsWith('.AspNetCore.Antiforgery.')) {
      return decodeURIComponent(trimmed.split('=')[1]);
    }
  }
  return null;
}

api.interceptors.request.use((config) => {
  config.headers = config.headers || {};

  // Anti-forgery token for mutating requests
  const method = (config.method || 'get').toLowerCase();
  if (method !== 'get' && method !== 'head' && method !== 'options') {
    const token = getAntiForgeryToken();
    if (token) {
      config.headers['RequestVerificationToken'] = token;
    }
  }

  // ABP requires this header for AJAX detection
  if (!config.headers['X-Requested-With']) {
    config.headers['X-Requested-With'] = 'XMLHttpRequest';
  }

  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Clear auth state on 401, but don't redirect from config fetch itself
      const url = error.config?.url ?? '';
      if (!url.includes('/abp/application-configuration') && !url.includes('/account/login')) {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  },
);

export default api;
export { getAntiForgeryToken };
