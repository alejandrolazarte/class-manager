import { httpClient } from "@/api/httpClient";
import {
  Client,
  ClientDetails,
  RegisterClientRequest,
  UpdateClientRequest,
} from "@/features/clients/types";

export const clientsPath = "/api/clients";

export function registerClient(request: RegisterClientRequest): Promise<ClientDetails> {
  return httpClient.post<ClientDetails>(clientsPath, request);
}

export function getClient(clientId: string): Promise<ClientDetails> {
  return httpClient.get<ClientDetails>(`${clientsPath}/${encodeURIComponent(clientId)}`);
}

export function updateClient(clientId: string, request: UpdateClientRequest): Promise<Client> {
  return httpClient.put<Client>(`${clientsPath}/${encodeURIComponent(clientId)}`, request);
}
