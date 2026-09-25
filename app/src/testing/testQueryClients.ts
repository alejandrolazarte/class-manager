import { QueryClient } from "@tanstack/react-query";

const createdQueryClients: QueryClient[] = [];

export function createTestQueryClient(): QueryClient {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false, gcTime: Infinity },
      mutations: { retry: false, gcTime: Infinity },
    },
  });
  createdQueryClients.push(queryClient);
  return queryClient;
}

export function clearTestQueryClients(): void {
  createdQueryClients.splice(0).forEach((queryClient) => queryClient.clear());
}
