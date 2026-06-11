import api from './index';

export interface DbTable {
  id: string;
  name: string;
  displayName: string;
  schema: string;
  description: string;
  fieldCount: number;
}

export const dbTableApi = {
  getList: (params?: any) => api.get('/db-table', { params }),
  get: (id: string) => api.get(`/db-table/${id}`),
  create: (data: any) => api.post('/db-table', data),
  update: (id: string, data: any) => api.put(`/db-table/${id}`, data),
  delete: (id: string) => api.delete(`/db-table/${id}`),
};
