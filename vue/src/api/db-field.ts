import api from './index';

export interface DbField {
  id: string;
  dbTableId: string;
  tableName: string;
  name: string;
  sqlType: string;
  isNullable: boolean;
  maxLength?: number;
  defaultValue?: string;
  description?: string;
  sortOrder: number;
  executionStatus: number;
  executedAt?: string;
  creationTime: string;
  lastModificationTime?: string;
  targetDatabaseId?: string;
  targetDatabaseName?: string;
}

export interface GeneratedSqlResult {
  sqlScript: string;
  tableName: string;
  statements: GeneratedSqlStatement[];
}

export interface GeneratedSqlStatement {
  fieldId: string;
  fieldName: string;
  alterTableSql: string;
  extendedPropertySql?: string;
}

export interface ExecuteSqlResult {
  allSuccess: boolean;
  results: FieldExecutionResult[];
}

export interface FieldExecutionResult {
  fieldId: string;
  fieldName: string;
  success: boolean;
  message: string;
}

export interface TableExecuteSqlResult {
  success: boolean;
  message: string;
}

export const dbFieldApi = {
  getList: (params?: any) => api.get('/db-field', { params }),
  get: (id: string) => api.get(`/db-field/${id}`),
  create: (data: any) => api.post('/db-field', data),
  update: (id: string, data: any) => api.put(`/db-field/${id}`, data),
  delete: (id: string) => api.delete(`/db-field/${id}`),
  batchCreate: (data: { dbTableId: string; fields: any[] }) => api.post('/db-field/batch-create', data),
  previewSql: (fieldIds: string[]) => api.post('/sql-generation/preview', { fieldIds }),
  executeSql: (targetDatabaseId: string, fieldIds: string[]) =>
    api.post('/sql-execution/execute', { targetDatabaseId, fieldIds }),
  previewCreateTableSql: (tableId: string) => api.post('/sql-generation/preview-create-table', { tableId }),
  executeCreateTableSql: (tableId: string, targetDatabaseId: string) =>
    api.post('/sql-execution/execute-create-table', { tableId, targetDatabaseId }),
};

export const sqlExecutionLogApi = {
  getList: (params?: any) => api.get('/sql-execution-log', { params }),
};
