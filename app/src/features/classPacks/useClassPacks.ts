import { useQuery } from "@tanstack/react-query";
import { classPackQueryKeys } from "@/features/classPacks/classPackQueryKeys";
import { getClassBalance, listClassPacks } from "@/features/classPacks/classPacksApi";

export function useClassPacks(includeInactive: boolean, { enabled = true } = {}) {
  return useQuery({
    queryKey: classPackQueryKeys.catalog(includeInactive),
    queryFn: () => listClassPacks(includeInactive),
    enabled,
  });
}

export function useClassBalance(clientId: string | undefined) {
  return useQuery({
    queryKey: classPackQueryKeys.balance(clientId ?? ""),
    queryFn: () => getClassBalance(clientId ?? ""),
    enabled: clientId !== undefined && clientId.length > 0,
  });
}
