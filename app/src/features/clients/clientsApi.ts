import { httpClient } from "@/api/httpClient";
import { ClientDetails, RegisterClientRequest } from "@/features/clients/types";

export const clientsPath = "/api/clients";

export function registerClient(request: RegisterClientRequest): Promise<ClientDetails> {
  return httpClient.post<ClientDetails>(clientsPath, request);
}

export function getClient(clientId: string): Promise<ClientDetails> {
  return httpClient.get<ClientDetails>(`${clientsPath}/${encodeURIComponent(clientId)}`);
}
