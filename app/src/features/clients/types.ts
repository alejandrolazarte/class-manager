export interface Client {
  id: string;
  fullName: string;
  phoneNumber: string;
  email: string | null;
  notes: string | null;
  createdAt: string;
}

export interface RegisterClientRequest {
  fullName: string;
  phoneNumber: string;
  email?: string;
  notes?: string;
}

export interface SearchClientsRequest {
  search: string;
  limit: number;
}
