import { useMutation, useQueryClient } from "@tanstack/react-query";
import { clientQueryKeys } from "@/features/clients/clientQueryKeys";
import { updateClient } from "@/features/clients/clientsApi";
import { UpdateClientRequest } from "@/features/clients/types";
import { feeQueryKeys } from "@/features/fees/feeQueryKeys";
import { studentQueryKeys } from "@/features/students/studentQueryKeys";

export function useUpdateClient(clientId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: UpdateClientRequest) => updateClient(clientId, request),
    onSuccess: () =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: clientQueryKeys.all }),
        queryClient.invalidateQueries({ queryKey: studentQueryKeys.all }),
        queryClient.invalidateQueries({ queryKey: feeQueryKeys.all }),
      ]),
  });
}
