<template>
  <div>
    <div style="margin-bottom: 16px">
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
        <template v-if="column.key === 'isSuccess'">
          <a-tag :color="record.isSuccess ? 'green' : 'red'">
            {{ record.isSuccess ? '成功' : '失败' }}
          </a-tag>
        </template>
        <template v-if="column.key === 'executedAt'">
          {{ formatTime(record.executedAt) }}
        </template>
        <template v-if="column.key === 'sqlScript'">
          <a-button type="link" size="small" @click="showSql(record.sqlScript)">查看SQL</a-button>
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
import { ref, reactive, onMounted } from 'vue';
import dayjs from 'dayjs';
import { sqlExecutionLogApi } from '@/api/db-field';
import { targetDatabaseApi, type TargetDatabase } from '@/api/target-database';

const loading = ref(false);
const sqlVisible = ref(false);
const currentSql = ref('');
const filterDbId = ref<string | undefined>();
const data = ref<any[]>([]);
const databases = ref<TargetDatabase[]>([]);
const pagination = reactive({ current: 1, pageSize: 15, total: 0 });

const columns = [
  { title: '数据库', dataIndex: 'targetDatabaseName', key: 'targetDatabaseName' },
  { title: '字段', dataIndex: 'dbFieldName', key: 'dbFieldName' },
  { title: '结果', key: 'isSuccess', width: 80 },
  { title: '错误信息', dataIndex: 'errorMessage', key: 'errorMessage' },
  { title: 'SQL', key: 'sqlScript', width: 100 },
  { title: '时间', key: 'executedAt', width: 170 },
];

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
