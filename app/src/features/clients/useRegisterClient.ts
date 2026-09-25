import { useMutation, useQueryClient } from "@tanstack/react-query";
import { clientQueryKeys } from "@/features/clients/clientQueryKeys";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientRequest } from "@/features/clients/types";

export function useRegisterClient() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: RegisterClientRequest) => registerClient(request),
    onSuccess: async (registeredClient) => {
      queryClient.setQueryData(clientQueryKeys.detail(registeredClient.id), registeredClient);
      await queryClient.invalidateQueries({ queryKey: clientQueryKeys.all, refetchType: "active" });
    },
  });
}
