import { useMutation, useQueryClient } from "@tanstack/react-query";
import { businessQueryKeys } from "@/features/business/businessQueryKeys";
import { classPackQueryKeys } from "@/features/classPacks/classPackQueryKeys";
import { clientQueryKeys } from "@/features/clients/clientQueryKeys";
import { feeQueryKeys } from "@/features/fees/feeQueryKeys";
import {
  deleteClientBillingPlanChange,
  deleteDefaultMonthlyFeeChange,
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

function useInvalidateDefaultFee() {
  const queryClient = useQueryClient();
  return () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: businessQueryKeys.all }),
      queryClient.invalidateQueries({ queryKey: feeQueryKeys.all }),
    ]);
}

function useInvalidateClientBillingPlan(clientId: string) {
  const queryClient = useQueryClient();
  return () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: clientQueryKeys.detail(clientId) }),
      queryClient.invalidateQueries({ queryKey: feeQueryKeys.all }),
      queryClient.invalidateQueries({ queryKey: classPackQueryKeys.all }),
    ]);
}

export function useSetDefaultMonthlyFee() {
  const invalidateDefaultFee = useInvalidateDefaultFee();
  return useMutation({
    mutationFn: ({ amount, effectiveFrom }: { amount: number | null; effectiveFrom: string }) =>
      setDefaultMonthlyFee(amount, effectiveFrom),
    onSuccess: invalidateDefaultFee,
  });
}

export function useDeleteDefaultMonthlyFeeChange() {
  const invalidateDefaultFee = useInvalidateDefaultFee();
  return useMutation({
    mutationFn: (month: string) => deleteDefaultMonthlyFeeChange(month),
    onSuccess: invalidateDefaultFee,
  });
}

export function useSetClientBillingPlan(clientId: string) {
  const invalidateClientBillingPlan = useInvalidateClientBillingPlan(clientId);
  return useMutation({
    mutationFn: (request: SetBillingPlanRequest) => setClientBillingPlan(clientId, request),
    onSuccess: invalidateClientBillingPlan,
  });
}

export function useDeleteClientBillingPlanChange(clientId: string) {
  const invalidateClientBillingPlan = useInvalidateClientBillingPlan(clientId);
  return useMutation({
    mutationFn: (month: string) => deleteClientBillingPlanChange(clientId, month),
    onSuccess: invalidateClientBillingPlan,
  });
}
