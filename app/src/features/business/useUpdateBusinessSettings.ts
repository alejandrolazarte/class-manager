import { useMutation, useQueryClient } from "@tanstack/react-query";
import { updateBusinessSettings } from "@/features/business/businessApi";
import { businessQueryKeys } from "@/features/business/businessQueryKeys";
import { UpdateBusinessSettingsRequest } from "@/features/business/types";

export function useUpdateBusinessSettings() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: UpdateBusinessSettingsRequest) => updateBusinessSettings(request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: businessQueryKeys.all }),
  });
}
