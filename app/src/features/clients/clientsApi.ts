import { httpClient } from "@/api/httpClient";
import { Client, RegisterClientRequest, SearchClientsRequest } from "@/features/clients/types";

const clientsPath = "/api/clients";

export function registerClient(request: RegisterClientRequest): Promise<Client> {
  return httpClient.post<Client>(clientsPath, request);
}

export function getClient(clientId: string): Promise<Client> {
  return httpClient.get<Client>(`${clientsPath}/${encodeURIComponent(clientId)}`);
}

export function searchClients({ search, limit }: SearchClientsRequest): Promise<Client[]> {
  return httpClient.get<Client[]>(clientsPath, { search, limit });
}
