import { useMutation, useQueryClient } from "@tanstack/react-query";
import { clientQueryKeys } from "@/features/clients/clientQueryKeys";
import { registerClient } from "@/features/clients/clientsApi";
import { RegisterClientRequest } from "@/features/clients/types";
import { studentQueryKeys } from "@/features/students/studentQueryKeys";

export function useRegisterClient() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: RegisterClientRequest) => registerClient(request),
    onSuccess: async (registeredClient) => {
      queryClient.setQueryData(clientQueryKeys.detail(registeredClient.id), registeredClient);
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: clientQueryKeys.all, refetchType: "active" }),
        queryClient.invalidateQueries({ queryKey: studentQueryKeys.all, refetchType: "active" }),
      ]);
    },
  });
}
