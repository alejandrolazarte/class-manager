import { useMutation, useQueryClient } from "@tanstack/react-query";
import { businessQueryKeys } from "@/features/business/businessQueryKeys";
import { classPackQueryKeys } from "@/features/classPacks/classPackQueryKeys";
import { clientQueryKeys } from "@/features/clients/clientQueryKeys";
import { feeQueryKeys } from "@/features/fees/feeQueryKeys";
import {
  deletePayment,
  recordPayment,
  setClientBillingPlan,
  setDefaultMonthlyFee,
} from "@/features/fees/feesApi";
import { RecordPaymentRequest, SetBillingPlanRequest } from "@/features/fees/types";

function useInvalidateFees() {
  const queryClient = useQueryClient();
  return () => queryClient.invalidateQueries({ queryKey: feeQueryKeys.all });
}

export function useRecordPayment(clientId: string) {
  const invalidateFees = useInvalidateFees();
  return useMutation({
    mutationFn: (request: RecordPaymentRequest) => recordPayment(clientId, request),
    onSuccess: invalidateFees,
  });
}

export function useDeletePayment() {
  const invalidateFees = useInvalidateFees();
  return useMutation({
    mutationFn: (paymentId: string) => deletePayment(paymentId),
    onSuccess: invalidateFees,
  });
}

export function useSetDefaultMonthlyFee() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ amount, effectiveFrom }: { amount: number | null; effectiveFrom: string }) =>
      setDefaultMonthlyFee(amount, effectiveFrom),
    onSuccess: () =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: businessQueryKeys.all }),
        queryClient.invalidateQueries({ queryKey: feeQueryKeys.all }),
      ]),
  });
}

export function useSetClientBillingPlan(clientId: string) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: SetBillingPlanRequest) => setClientBillingPlan(clientId, request),
    onSuccess: () =>
      Promise.all([
        queryClient.invalidateQueries({ queryKey: clientQueryKeys.detail(clientId) }),
        queryClient.invalidateQueries({ queryKey: feeQueryKeys.all }),
        queryClient.invalidateQueries({ queryKey: classPackQueryKeys.all }),
      ]),
  });
}
