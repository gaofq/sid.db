<template>
  <div>
    <div style="margin-bottom: 16px; display: flex; justify-content: space-between">
      <a-space>
        <a-input-search
          v-model:value="search"
          placeholder="搜索表名..."
          style="width: 250px"
          @search="fetchData"
        />
      </a-space>
      <a-button type="primary" @click="openCreate">
        <plus-outlined /> 添加表
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
        <template v-if="column.key === 'actions'">
          <a-space>
            <a-button type="link" size="small" @click="$router.push(`/tables/${record.id}/fields`)">
              管理字段
            </a-button>
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
      :title="editingId ? '编辑表' : '添加表'"
      @ok="handleSave"
      :confirm-loading="saving"
    >
      <a-form :model="form" layout="vertical">
        <a-form-item label="表名" required>
          <a-input v-model:value="form.name" placeholder="如: mporder" />
        </a-form-item>
        <a-form-item label="显示名称">
          <a-input v-model:value="form.displayName" placeholder="如: 客户订单表" />
        </a-form-item>
        <a-form-item label="Schema">
          <a-input v-model:value="form.schema" placeholder="dbo" />
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
import { dbTableApi, type DbTable } from '@/api/db-table';

const search = ref('');
const loading = ref(false);
const saving = ref(false);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const data = ref<DbTable[]>([]);
const pagination = reactive({ current: 1, pageSize: 10, total: 0 });

const form = reactive({
  name: '',
  displayName: '',
  schema: 'dbo',
  description: '',
});

const columns = [
  { title: '表名', dataIndex: 'name', key: 'name' },
  { title: '显示名称', dataIndex: 'displayName', key: 'displayName' },
  { title: 'Schema', dataIndex: 'schema', key: 'schema' },
  { title: '字段数', dataIndex: 'fieldCount', key: 'fieldCount', width: 100 },
  { title: '操作', key: 'actions', width: 260 },
];

async function fetchData() {
  loading.value = true;
  try {
    const res = await dbTableApi.getList({
      filter: search.value || undefined,
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
  form.displayName = '';
  form.schema = 'dbo';
  form.description = '';
  modalVisible.value = true;
}

function openEdit(record: DbTable) {
  editingId.value = record.id;
  form.name = record.name;
  form.displayName = record.displayName;
  form.schema = record.schema;
  form.description = record.description;
  modalVisible.value = true;
}

async function handleSave() {
  saving.value = true;
  try {
    if (editingId.value) {
      await dbTableApi.update(editingId.value, {
        name: form.name,
        displayName: form.displayName,
        schema: form.schema,
        description: form.description,
      });
      message.success('更新成功');
    } else {
      await dbTableApi.create(form);
      message.success('创建成功');
    }
    modalVisible.value = false;
    fetchData();
  } finally {
    saving.value = false;
  }
}

async function handleDelete(id: string) {
  await dbTableApi.delete(id);
  message.success('删除成功');
  fetchData();
}

onMounted(async () => {
  fetchData();
});
</script>
