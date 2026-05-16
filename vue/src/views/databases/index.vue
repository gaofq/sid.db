<template>
  <div>
    <div style="margin-bottom: 16px; display: flex; justify-content: space-between">
      <a-input-search
        v-model:value="search"
        placeholder="搜索数据库名称..."
        style="width: 300px"
        @search="fetchData"
      />
      <a-button type="primary" @click="openCreate">
        <plus-outlined /> 添加数据库
      </a-button>
    </div>

    <a-table
      :columns="columns"
      :data-source="data"
      :loading="loading"
      :pagination="pagination"
      @change="onPageChange"
      row-key="id"
    >
      <template #bodyCell="{ column, record }">
        <template v-if="column.key === 'connectionString'">
          <span>{{ maskConnection(record.connectionString) }}</span>
        </template>
        <template v-if="column.key === 'actions'">
          <a-space>
            <a-button type="link" size="small" @click="testConnection(record.id)">测试连接</a-button>
            <a-button type="link" size="small" @click="openEdit(record)">编辑</a-button>
            <a-popconfirm title="确认删除?" @confirm="handleDelete(record.id)">
              <a-button type="link" size="small" danger>删除</a-button>
            </a-popconfirm>
          </a-space>
        </template>
      </template>
    </a-table>

    <a-modal
      v-model:open="modalVisible"
      :title="editingId ? '编辑数据库' : '添加数据库'"
      @ok="handleSave"
      :confirm-loading="saving"
    >
      <a-form :model="form" layout="vertical">
        <a-form-item label="名称" required>
          <a-input v-model:value="form.name" placeholder="如: 生产环境订单库" />
        </a-form-item>
        <a-form-item label="连接字符串" required>
          <a-input-password v-model:value="form.connectionString" placeholder="Server=...;Database=...;..." />
        </a-form-item>
        <a-form-item label="描述">
          <a-textarea v-model:value="form.description" placeholder="可选描述" :rows="2" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { message } from 'ant-design-vue';
import { PlusOutlined } from '@ant-design/icons-vue';
import { targetDatabaseApi, type TargetDatabase } from '@/api/target-database';

const search = ref('');
const loading = ref(false);
const saving = ref(false);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const data = ref<TargetDatabase[]>([]);
const pagination = reactive({ current: 1, pageSize: 10, total: 0 });

const form = reactive({
  name: '',
  connectionString: '',
  description: '',
});

const columns = [
  { title: '名称', dataIndex: 'name', key: 'name' },
  { title: '连接字符串', key: 'connectionString' },
  { title: '表数量', dataIndex: 'tableCount', key: 'tableCount', width: 100 },
  { title: '操作', key: 'actions', width: 260 },
];

function maskConnection(conn: string) {
  if (!conn) return '';
  return conn.replace(/Password=([^;]+)/, 'Password=****');
}

async function fetchData() {
  loading.value = true;
  try {
    const res = await targetDatabaseApi.getList({
      maxResultCount: pagination.pageSize,
      skipCount: (pagination.current - 1) * pagination.pageSize,
    });
    data.value = res.data.items;
    pagination.total = res.data.totalCount;
  } finally {
    loading.value = false;
  }
}

function onPageChange(pag: any) {
  pagination.current = pag.current;
  pagination.pageSize = pag.pageSize;
  fetchData();
}

function openCreate() {
  editingId.value = null;
  form.name = '';
  form.connectionString = '';
  form.description = '';
  modalVisible.value = true;
}

function openEdit(record: TargetDatabase) {
  editingId.value = record.id;
  form.name = record.name;
  form.connectionString = record.connectionString;
  form.description = record.description;
  modalVisible.value = true;
}

async function handleSave() {
  saving.value = true;
  try {
    if (editingId.value) {
      await targetDatabaseApi.update(editingId.value, form);
      message.success('更新成功');
    } else {
      await targetDatabaseApi.create(form);
      message.success('创建成功');
    }
    modalVisible.value = false;
    fetchData();
  } finally {
    saving.value = false;
  }
}

async function handleDelete(id: string) {
  await targetDatabaseApi.delete(id);
  message.success('删除成功');
  fetchData();
}

async function testConnection(id: string) {
  const res = await targetDatabaseApi.testConnection(id);
  if (res.data) {
    message.success('连接成功');
  } else {
    message.error('连接失败');
  }
}

onMounted(fetchData);
</script>
