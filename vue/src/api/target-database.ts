import api from './index';

export interface TargetDatabase {
  id: string;
  name: string;
  connectionString: string;
  description: string;
  tableCount: number;
}

export const targetDatabaseApi = {
  getList: (params?: any) => api.get('/target-database', { params }),
  get: (id: string) => api.get(`/target-database/${id}`),
  create: (data: any) => api.post('/target-database', data),
  update: (id: string, data: any) => api.put(`/target-database/${id}`, data),
  delete: (id: string) => api.delete(`/target-database/${id}`),
  testConnection: (id: string) => api.post(`/target-database/${id}/test-connection`),
};
