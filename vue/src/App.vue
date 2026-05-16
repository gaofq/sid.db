<template>
  <template v-if="route.meta.public">
    <router-view />
  </template>
  <a-layout v-else style="min-height: 100vh">
    <a-layout-sider v-model:collapsed="collapsed" collapsible>
      <div class="logo">
        <span v-if="!collapsed">数据库字段管理器</span>
        <span v-else>DB</span>
      </div>
      <a-menu
        v-model:selectedKeys="selectedKeys"
        theme="dark"
        mode="inline"
        @click="onMenuClick"
      >
        <a-menu-item key="/databases">
          <database-outlined />
          <span>目标数据库</span>
        </a-menu-item>
        <a-menu-item key="/tables">
          <table-outlined />
          <span>数据表管理</span>
        </a-menu-item>
        <a-menu-item key="/execution-logs">
          <history-outlined />
          <span>执行日志</span>
        </a-menu-item>
      </a-menu>
    </a-layout-sider>
    <a-layout>
      <a-layout-header style="background: #fff; padding: 0 24px; display: flex; justify-content: space-between; align-items: center">
        <span style="font-weight: bold">{{ currentTitle }}</span>
        <a-dropdown>
          <a-space style="cursor: pointer">
            <a-avatar size="small">
              <template #icon><user-outlined /></template>
            </a-avatar>
            <span>{{ authStore.userName }}</span>
          </a-space>
          <template #overlay>
            <a-menu @click="onUserMenuClick">
              <a-menu-item key="logout">
                <logout-outlined />
                <span>退出登录</span>
              </a-menu-item>
            </a-menu>
          </template>
        </a-dropdown>
      </a-layout-header>
      <a-layout-content style="margin: 16px">
        <router-view />
      </a-layout-content>
    </a-layout>
  </a-layout>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { DatabaseOutlined, TableOutlined, HistoryOutlined, UserOutlined, LogoutOutlined } from '@ant-design/icons-vue';
import { useAuthStore } from '@/stores/auth';

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();
const collapsed = ref(false);
const selectedKeys = ref<string[]>([route.path]);

const currentTitle = computed(() => (route.meta?.title as string) || '数据库字段管理器');

watch(() => route.path, (path) => {
  selectedKeys.value = [path];
});

function onMenuClick({ key }: { key: string }) {
  router.push(key);
}

async function onUserMenuClick({ key }: { key: string }) {
  if (key === 'logout') {
    await authStore.logout();
    router.push('/login');
  }
}
</script>

<style>
.logo {
  height: 64px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #fff;
  font-size: 18px;
  font-weight: bold;
  overflow: hidden;
  white-space: nowrap;
}
</style>
