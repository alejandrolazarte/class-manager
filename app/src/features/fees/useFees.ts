import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { feeQueryKeys } from "@/features/fees/feeQueryKeys";
import { listClientPayments, listMonthlyFees } from "@/features/fees/feesApi";

export function useMonthlyFees(month: string, { enabled = true } = {}) {
  return useQuery({
    queryKey: feeQueryKeys.month(month),
    queryFn: () => listMonthlyFees(month),
    placeholderData: keepPreviousData,
    enabled,
  });
}

export function useClientPayments(clientId: string) {
  return useQuery({
    queryKey: feeQueryKeys.clientPayments(clientId),
    queryFn: () => listClientPayments(clientId),
  });
}
