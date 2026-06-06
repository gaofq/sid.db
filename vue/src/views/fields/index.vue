<template>
  <div>
    <a-page-header title="字段管理" @back="() => $router.back()">
      <template #extra>
        <a-space>
          <a-button type="primary" @click="addNewRow">
            <plus-outlined /> 添加字段
          </a-button>
          <a-button
            type="primary"
            :disabled="selectedIds.length === 0"
            @click="previewSql"
          >
            <eye-outlined /> 预览SQL
          </a-button>
          <a-button
            type="primary"
            danger
            :disabled="selectedIds.length === 0"
            @click="confirmExecute"
          >
            <play-circle-outlined /> 执行SQL
          </a-button>
          <a-dropdown>
            <template #overlay>
              <a-menu>
                <a-menu-item @click="previewCreateTableSql">
                  <eye-outlined /> 预览建表SQL
                </a-menu-item>
                <a-menu-item @click="confirmCreateTable">
                  <play-circle-outlined /> 执行建表SQL
                </a-menu-item>
              </a-menu>
            </template>
            <a-button>
              建表 <down-outlined />
            </a-button>
          </a-dropdown>
        </a-space>
      </template>
    </a-page-header>

    <div style="margin-bottom: 16px; display: flex; gap: 12px; align-items: center; flex-wrap: wrap">
      <span style="color: #666; font-size: 13px">创建时间:</span>
      <a-range-picker
        v-model:value="dateRange"
        :placeholder="['开始日期', '结束日期']"
        format="YYYY-MM-DD"
        :allow-clear="true"
        @change="onDateChange"
      />
      <span style="color: #666; font-size: 13px">修改时间:</span>
      <a-range-picker
        v-model:value="modifyDateRange"
        :placeholder="['开始日期', '结束日期']"
        format="YYYY-MM-DD"
        :allow-clear="true"
        @change="onModifyDateChange"
      />
    </div>

    <a-table
      :columns="columns"
      :data-source="tableData"
      :loading="loading"
      :pagination="pagination"
      :row-selection="rowSelection"
      @change="onPageChange"
      row-key="key"
    >
      <template #bodyCell="{ column, record }">
        <template v-if="column.key === 'name'">
          <template v-if="record.isNew || record.isEditing">
            <a-input v-model:value="record.name" placeholder="字段名" />
          </template>
          <template v-else>
            {{ record.name }}
          </template>
        </template>
        
        <template v-if="column.key === 'sqlType'">
          <template v-if="record.isNew || record.isEditing">
            <a-select
              v-model:value="record.sqlType"
              style="width: 100%"
              placeholder="选择或输入SQL类型"
              :allow-clear="true"
              show-search
              filter-option
            >
              <a-select-option v-for="type in sqlTypes" :key="type" :value="type">
                {{ type }}
              </a-select-option>
            </a-select>
          </template>
          <template v-else>
            {{ record.sqlType }}
          </template>
        </template>
        
        <template v-if="column.key === 'isNullable'">
          <template v-if="record.isNew || record.isEditing">
            <a-switch v-model:checked="record.isNullable" />
          </template>
          <template v-else>
            <a-tag :color="record.isNullable ? 'blue' : 'red'">
              {{ record.isNullable ? 'NULL' : 'NOT NULL' }}
            </a-tag>
          </template>
        </template>
        
        <template v-if="column.key === 'defaultValue'">
          <template v-if="record.isNew || record.isEditing">
            <a-input v-model:value="record.defaultValue" placeholder="默认值" />
          </template>
          <template v-else>
            {{ record.defaultValue }}
          </template>
        </template>
        
        <template v-if="column.key === 'description'">
          <template v-if="record.isNew || record.isEditing">
            <a-input v-model:value="record.description" placeholder="描述" />
          </template>
          <template v-else>
            {{ record.description }}
          </template>
        </template>
        
        <template v-if="column.key === 'sortOrder'">
          <template v-if="record.isNew || record.isEditing">
            <a-input-number v-model:value="record.sortOrder" :min="0" style="width: 100%" />
          </template>
          <template v-else>
            {{ record.sortOrder }}
          </template>
        </template>
        
        <template v-if="column.key === 'executionStatus'">
          <a-tag v-if="record.executionStatus === 1" color="green">已执行</a-tag>
          <a-tag v-else-if="record.executionStatus === 2" color="red">失败</a-tag>
          <a-tag v-else color="default">待执行</a-tag>
        </template>

        <template v-if="column.key === 'creationTime'">
          <span v-if="!record.isNew">{{ formatTime(record.creationTime) }}</span>
        </template>

        <template v-if="column.key === 'lastModificationTime'">
          <span v-if="!record.isNew && record.lastModificationTime">{{ formatTime(record.lastModificationTime) }}</span>
        </template>
        
        <template v-if="column.key === 'actions'">
          <template v-if="record.isNew || record.isEditing">
            <a-space>
              <a-button type="link" size="small" @click="saveRow(record)">
                <check-outlined /> 保存
              </a-button>
              <a-button type="link" size="small" @click="cancelEdit(record)">
                <close-outlined /> 取消
              </a-button>
            </a-space>
          </template>
          <template v-else>
            <a-space>
              <a-button type="link" size="small" @click="startEdit(record)">
                <edit-outlined /> 编辑
              </a-button>
              <a-popconfirm title="确认删除?" @confirm="handleDelete(record)">
                <a-button type="link" size="small" danger>
                  <delete-outlined /> 删除
                </a-button>
              </a-popconfirm>
            </a-space>
          </template>
        </template>
      </template>
    </a-table>

    <!-- SQL Preview Modal -->
    <a-modal
      v-model:open="sqlModalVisible"
      title="SQL预览"
      width="800px"
      :footer="null"
    >
      <div style="margin-bottom: 8px; display: flex; justify-content: space-between">
        <span>表: {{ sqlResult.tableName }}</span>
        <a-button type="primary" @click="copySql">复制SQL</a-button>
      </div>
      <pre class="sql-preview">{{ sqlResult.sqlScript }}</pre>
    </a-modal>

    <!-- Execution Result Modal -->
    <a-modal
      v-model:open="execResultVisible"
      title="执行结果"
      :footer="null"
    >
      <div v-for="r in execResults" :key="r.fieldId" style="margin-bottom: 8px">
        <a-tag :color="r.success ? 'green' : 'red'">{{ r.success ? '成功' : '失败' }}</a-tag>
        <span>{{ r.fieldName }}: {{ r.message }}</span>
      </div>
    </a-modal>

    <!-- Create Table SQL Preview Modal -->
    <a-modal
      v-model:open="createTableSqlVisible"
      title="建表SQL预览"
      width="800px"
      :footer="null"
    >
      <div style="margin-bottom: 8px; display: flex; justify-content: space-between">
        <span>表: {{ createTableSqlResult.tableName }}</span>
        <a-button type="primary" @click="copyCreateTableSql">复制SQL</a-button>
      </div>
      <pre class="sql-preview">{{ createTableSqlResult.sqlScript }}</pre>
    </a-modal>

    <!-- Create Table Execution Result Modal -->
    <a-modal
      v-model:open="createTableExecVisible"
      title="建表结果"
      :footer="null"
    >
      <a-result
        :status="createTableExecResult?.success ? 'success' : 'error'"
        :title="createTableExecResult?.success ? '建表成功' : '建表失败'"
        :sub-title="createTableExecResult?.message"
      />
    </a-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { message, Modal } from 'ant-design-vue';
import {
  PlusOutlined, EyeOutlined, PlayCircleOutlined, EditOutlined, DeleteOutlined,
  CheckOutlined, CloseOutlined, DownOutlined
} from '@ant-design/icons-vue';
import { dbFieldApi, type DbField, type GeneratedSqlResult, type FieldExecutionResult, type TableExecuteSqlResult } from '@/api/db-field';
import { targetDatabaseApi } from '@/api/target-database';
import { dbTableApi } from '@/api/db-table';

// SQL Server 常用数据类型
const sqlTypes = [
  'bit',
  'tinyint',
  'smallint',
  'int',
  'bigint',
  'decimal(18,0)',
  'decimal(18,2)',
  'numeric(18,0)',
  'numeric(18,2)',
  'float',
  'real',
  'money',
  'smallmoney',
  'datetime',
  'datetime2',
  'smalldatetime',
  'date',
  'time',
  'timestamp',
  'char(1)',
  'char(10)',
  'char(50)',
  'char(100)',
  'varchar(10)',
  'varchar(50)',
  'varchar(100)',
  'varchar(255)',
  'varchar(500)',
  'varchar(1000)',
  'varchar(max)',
  'text',
  'nchar(1)',
  'nchar(10)',
  'nchar(50)',
  'nchar(100)',
  'nvarchar(10)',
  'nvarchar(50)',
  'nvarchar(100)',
  'nvarchar(255)',
  'nvarchar(500)',
  'nvarchar(1000)',
  'nvarchar(max)',
  'ntext',
  'binary(50)',
  'varbinary(50)',
  'varbinary(max)',
  'image',
  'uniqueidentifier',
  'xml',
  'json',
];

const route = useRoute();
const tableId = route.params.tableId as string;

const loading = ref(false);
const saving = ref<string | null>(null);
const sqlModalVisible = ref(false);
const execResultVisible = ref(false);
const data = ref<DbField[]>([]);
const editingRows = ref<Map<string, any>>(new Map());
const selectedIds = ref<string[]>([]);
const sqlResult = ref<GeneratedSqlResult>({ sqlScript: '', tableName: '', statements: [] });
const execResults = ref<FieldExecutionResult[]>([]);
const targetDatabaseId = ref<string>('');
const createTableSqlVisible = ref(false);
const createTableExecVisible = ref(false);
const createTableSqlResult = ref<GeneratedSqlResult>({ sqlScript: '', tableName: '', statements: [] });
const createTableExecResult = ref<TableExecuteSqlResult | null>(null);
const dateRange = ref<any>(null);
const modifyDateRange = ref<any>(null);
const pagination = reactive({ current: 1, pageSize: 20, total: 0 });

interface TableRow extends Partial<DbField> {
  key: string;
  isNew?: boolean;
  isEditing?: boolean;
  originalData?: Partial<DbField>;
}

const tableData = computed<TableRow[]>(() => {
  const result: TableRow[] = [];
  
  // Add new rows first
  editingRows.value.forEach((row, key) => {
    if (row.isNew) {
      result.push(row);
    }
  });
  
  // Add existing rows
  data.value.forEach(item => {
    const editingRow = editingRows.value.get(item.id);
    if (editingRow) {
      result.push(editingRow);
    } else {
      result.push({ ...item, key: item.id });
    }
  });
  
  return result;
});

const columns = [
  { title: '字段名', dataIndex: 'name', key: 'name' },
  { title: 'SQL类型', dataIndex: 'sqlType', key: 'sqlType', width: 220 },
  { title: '允许NULL', key: 'isNullable', width: 100 },
  { title: '默认值', dataIndex: 'defaultValue', key: 'defaultValue' },
  { title: '描述', dataIndex: 'description', key: 'description' },
  { title: '排序', dataIndex: 'sortOrder', key: 'sortOrder', width: 80 },
  { title: '状态', key: 'executionStatus', width: 90 },
  { title: '创建时间', key: 'creationTime', width: 170 },
  { title: '修改时间', key: 'lastModificationTime', width: 170 },
  { title: '操作', key: 'actions', width: 180 },
];

const rowSelection = {
  selectedRowKeys: computed(() => {
    const keys: string[] = [];
    // Only select existing rows (not new ones)
    selectedIds.value.forEach(id => {
      if (data.value.some(item => item.id === id)) {
        keys.push(id);
      }
    });
    return keys;
  }),
  onChange: (keys: string[]) => {
    selectedIds.value = keys.filter(key => data.value.some(item => item.id === key));
  },
};

async function fetchData() {
  loading.value = true;
  try {
    const table = await dbTableApi.get(tableId);
    targetDatabaseId.value = table.data.targetDatabaseId;

    const params: any = {
      dbTableId: tableId,
      maxResultCount: pagination.pageSize,
      skipCount: (pagination.current - 1) * pagination.pageSize,
    };

    if (dateRange.value && dateRange.value[0]) {
      params.creationTimeMin = formatDate(dateRange.value[0]);
    }
    if (dateRange.value && dateRange.value[1]) {
      params.creationTimeMax = formatDate(dateRange.value[1]);
    }

    if (modifyDateRange.value && modifyDateRange.value[0]) {
      params.lastModificationTimeMin = formatDate(modifyDateRange.value[0]);
    }
    if (modifyDateRange.value && modifyDateRange.value[1]) {
      params.lastModificationTimeMax = formatDate(modifyDateRange.value[1]);
    }

    const res = await dbFieldApi.getList(params);
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

function addNewRow() {
  const key = `new_${Date.now()}`;
  const maxSort = Math.max(...data.value.map(d => d.sortOrder), 0);
  editingRows.value.set(key, {
    key,
    isNew: true,
    isEditing: true,
    name: '',
    sqlType: '',
    isNullable: true,
    defaultValue: '',
    description: '',
    sortOrder: maxSort + 1,
    executionStatus: 0,
  });
}

function startEdit(record: TableRow) {
  editingRows.value.set(record.key, {
    ...record,
    isEditing: true,
    originalData: { ...record },
  });
}

function cancelEdit(record: TableRow) {
  if (record.isNew) {
    editingRows.value.delete(record.key);
  } else {
    const original = record.originalData;
    if (original) {
      editingRows.value.delete(record.key);
    }
  }
}

async function saveRow(record: TableRow) {
  if (!record.name || !record.sqlType) {
    message.warning('请填写字段名和SQL类型');
    return;
  }
  
  saving.value = record.key;
  try {
    if (record.isNew) {
      await dbFieldApi.create({
        dbTableId: tableId,
        name: record.name!,
        sqlType: record.sqlType!,
        isNullable: record.isNullable ?? true,
        defaultValue: record.defaultValue,
        description: record.description,
        sortOrder: record.sortOrder ?? 0,
      });
      message.success('创建成功');
      editingRows.value.delete(record.key);
    } else {
      await dbFieldApi.update(record.id!, {
        name: record.name!,
        sqlType: record.sqlType!,
        isNullable: record.isNullable ?? true,
        defaultValue: record.defaultValue,
        description: record.description,
        sortOrder: record.sortOrder ?? 0,
      });
      message.success('更新成功');
      editingRows.value.delete(record.key);
    }
    fetchData();
  } finally {
    saving.value = null;
  }
}

async function handleDelete(record: TableRow) {
  if (record.isNew) {
    editingRows.value.delete(record.key);
    return;
  }
  await dbFieldApi.delete(record.id!);
  message.success('删除成功');
  fetchData();
}

async function previewSql() {
  const res = await dbFieldApi.previewSql(selectedIds.value);
  sqlResult.value = res.data;
  sqlModalVisible.value = true;
}

async function copySql() {
  await copyToClipboard(sqlResult.value.sqlScript);
}

async function copyCreateTableSql() {
  await copyToClipboard(createTableSqlResult.value.sqlScript);
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

async function previewCreateTableSql() {
  const res = await dbFieldApi.previewCreateTableSql(tableId);
  createTableSqlResult.value = res.data;
  createTableSqlVisible.value = true;
}

async function confirmCreateTable() {
  Modal.confirm({
    title: '确认执行建表？',
    content: '执行建表将删除已存在的同名表（DROP IF EXISTS），确认继续？',
    okText: '确认执行',
    cancelText: '取消',
    okType: 'danger',
    onOk: handleCreateTable,
  });
}

async function handleCreateTable() {
  try {
    const res = await dbFieldApi.executeCreateTableSql(tableId, targetDatabaseId.value);
    createTableExecResult.value = res.data;
    createTableExecVisible.value = true;
  } catch (e: any) {
    message.error(e?.response?.data?.error?.message || '建表失败');
  }
}

async function confirmExecute() {
  try {
    const res = await dbFieldApi.executeSql(targetDatabaseId.value, selectedIds.value);
    execResults.value = res.data.results;
    execResultVisible.value = true;
    fetchData(); // refresh status
  } catch (e: any) {
    message.error(e?.response?.data?.error?.message || '执行失败');
  }
}

function onDateChange() {
  pagination.current = 1;
  fetchData();
}

function onModifyDateChange() {
  pagination.current = 1;
  fetchData();
}

function formatTime(time: string | undefined) {
  if (!time) return '';
  const d = new Date(time);
  const y = d.getFullYear();
  const m = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  const h = String(d.getHours()).padStart(2, '0');
  const min = String(d.getMinutes()).padStart(2, '0');
  const s = String(d.getSeconds()).padStart(2, '0');
  return `${y}-${m}-${day} ${h}:${min}:${s}`;
}

function formatDate(date: any) {
  const d = date.toDate ? date.toDate() : new Date(date);
  return d.toISOString().slice(0, 10);
}

onMounted(fetchData);
</script>

<style scoped>
.sql-preview {
  max-height: 500px;
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
