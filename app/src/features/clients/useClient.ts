import { useQuery } from "@tanstack/react-query";
import { clientQueryKeys } from "@/features/clients/clientQueryKeys";
import { getClient } from "@/features/clients/clientsApi";

export function useClient(clientId: string | undefined) {
  return useQuery({
    queryKey: clientQueryKeys.detail(clientId ?? ""),
    queryFn: () => getClient(clientId ?? ""),
    enabled: clientId !== undefined && clientId.length > 0,
  });
}
