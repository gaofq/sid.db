<template>
  <div>
    <div style="margin-bottom: 16px; display: flex; justify-content: space-between; align-items: center">
      <a-select
        v-model:value="filterDbId"
        placeholder="筛选目标数据库"
        style="width: 250px"
        allow-clear
        @change="fetchData"
      >
        <a-select-option
          v-for="db in databases"
          :key="db.id"
          :value="db.id"
        >{{ db.name }}</a-select-option>
      </a-select>
      <a-space>
        <a-button
          :disabled="selectedIds.length === 0"
          @click="copySelectedSql"
        >
          <copy-outlined /> 复制选中SQL ({{ selectedIds.length }})
        </a-button>
        <a-button @click="copyAllSql">
          <copy-outlined /> 复制全部SQL
        </a-button>
      </a-space>
    </div>

    <a-table
      :columns="columns"
      :data-source="data"
      :loading="loading"
      :pagination="pagination"
      :row-selection="rowSelection"
      @change="onPageChange"
      row-key="id"
    >
      <template #bodyCell="{ column, record }">
        <template v-if="column.key === 'isSuccess'">
          <a-tag :color="record.isSuccess ? 'green' : 'red'">
            {{ record.isSuccess ? '成功' : '失败' }}
          </a-tag>
        </template>
        <template v-if="column.key === 'executedAt'">
          {{ formatTime(record.executedAt) }}
        </template>
        <template v-if="column.key === 'sqlScript'">
          <a-space>
            <a-button type="link" size="small" @click="showSql(record.sqlScript)">查看</a-button>
            <a-button type="link" size="small" @click="copySql(record.sqlScript)">复制</a-button>
          </a-space>
        </template>
      </template>
    </a-table>

    <a-modal
      v-model:open="sqlVisible"
      title="SQL详情"
      width="700px"
      :footer="null"
    >
      <pre class="sql-preview">{{ currentSql }}</pre>
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue';
import { message } from 'ant-design-vue';
import { CopyOutlined } from '@ant-design/icons-vue';
import dayjs from 'dayjs';
import { sqlExecutionLogApi } from '@/api/db-field';
import { targetDatabaseApi, type TargetDatabase } from '@/api/target-database';

const loading = ref(false);
const sqlVisible = ref(false);
const currentSql = ref('');
const filterDbId = ref<string | undefined>();
const data = ref<any[]>([]);
const databases = ref<TargetDatabase[]>([]);
const selectedIds = ref<string[]>([]);
const pagination = reactive({ current: 1, pageSize: 15, total: 0 });

const columns = [
  { title: '数据库', dataIndex: 'targetDatabaseName', key: 'targetDatabaseName' },
  { title: '字段', dataIndex: 'dbFieldName', key: 'dbFieldName' },
  { title: '结果', key: 'isSuccess', width: 80 },
  { title: '错误信息', dataIndex: 'errorMessage', key: 'errorMessage' },
  { title: 'SQL', key: 'sqlScript', width: 130 },
  { title: '时间', key: 'executedAt', width: 170 },
];

const rowSelection = {
  selectedRowKeys: computed(() => selectedIds.value),
  onChange: (keys: string[]) => {
    selectedIds.value = keys;
  },
};

async function fetchDatabases() {
  const res = await targetDatabaseApi.getList({ maxResultCount: 1000 });
  databases.value = res.data.items;
}

async function fetchData() {
  loading.value = true;
  try {
    const res = await sqlExecutionLogApi.getList({
      targetDatabaseId: filterDbId.value,
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

function showSql(sql: string) {
  currentSql.value = sql;
  sqlVisible.value = true;
}

function formatTime(time: string) {
  return time ? dayjs(time).format('YYYY-MM-DD HH:mm:ss') : '';
}

async function copyToClipboard(text: string) {
  try {
    await navigator.clipboard.writeText(text);
    message.success('已复制到剪贴板');
  } catch {
    const textarea = document.createElement('textarea');
    textarea.value = text;
    textarea.style.position = 'fixed';
    textarea.style.left = '-9999px';
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand('copy');
    document.body.removeChild(textarea);
    message.success('已复制到剪贴板');
  }
}

function copySql(sql: string) {
  copyToClipboard(sql);
}

function copySelectedSql() {
  const selected = data.value.filter(item => selectedIds.value.includes(item.id));
  if (selected.length === 0) {
    message.warning('请先选择要复制的记录');
    return;
  }
  const sqlText = selected.map(item => item.sqlScript).join('\n\n');
  copyToClipboard(sqlText);
}

function copyAllSql() {
  if (data.value.length === 0) {
    message.warning('当前没有可复制的记录');
    return;
  }
  const sqlText = data.value.map(item => item.sqlScript).join('\n\n');
  copyToClipboard(sqlText);
}

onMounted(async () => {
  await fetchDatabases();
  fetchData();
});
</script>

<style scoped>
.sql-preview {
  max-height: 400px;
  overflow: auto;
  background: #1e1e1e;
  color: #d4d4d4;
  padding: 16px;
  border-radius: 6px;
  font-family: 'Consolas', 'Courier New', monospace;
  font-size: 13px;
  line-height: 1.5;
  white-space: pre-wrap;
  word-break: break-all;
}
</style>
