<template>
  <div class="login-container">
    <div class="login-card">
      <h2>数据库字段管理器</h2>
      <p class="subtitle">请登录您的账户</p>
      <a-form
        :model="form"
        layout="vertical"
        @finish="handleLogin"
        autocomplete="off"
      >
        <a-form-item label="用户名" name="username" :rules="[{ required: true, message: '请输入用户名' }]">
          <a-input
            v-model:value="form.username"
            placeholder="请输入用户名"
            size="large"
            :disabled="loading"
          >
            <template #prefix><user-outlined /></template>
          </a-input>
        </a-form-item>
        <a-form-item label="密码" name="password" :rules="[{ required: true, message: '请输入密码' }]">
          <a-input-password
            v-model:value="form.password"
            placeholder="请输入密码"
            size="large"
            :disabled="loading"
            @keyup.enter="handleLogin"
          >
            <template #prefix><lock-outlined /></template>
          </a-input-password>
        </a-form-item>
        <a-form-item>
          <a-button
            type="primary"
            html-type="submit"
            size="large"
            block
            :loading="loading"
          >
            登 录
          </a-button>
        </a-form-item>
        <div v-if="authStore.loginError" class="error-msg">
          {{ authStore.loginError }}
        </div>
      </a-form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { UserOutlined, LockOutlined } from '@ant-design/icons-vue';
import { useAuthStore } from '@/stores/auth';

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const loading = ref(false);

const form = reactive({
  username: 'admin',
  password: '1q2w3E*',
});

async function handleLogin() {
  if (!form.username || !form.password) return;
  loading.value = true;
  try {
    const ok = await authStore.login(form.username, form.password);
    if (ok) {
      const redirect = (route.query.redirect as string) || '/databases';
      router.push(redirect);
    }
  } finally {
    loading.value = false;
  }
}
</script>

<style scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f0f2f5;
}
.login-card {
  width: 400px;
  padding: 40px;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
}
.login-card h2 {
  text-align: center;
  margin-bottom: 4px;
  color: #1a1a2e;
}
.subtitle {
  text-align: center;
  color: #999;
  margin-bottom: 32px;
}
.error-msg {
  color: #ff4d4f;
  text-align: center;
  margin-top: -8px;
}
</style>
