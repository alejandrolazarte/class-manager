import { useMutation, useQueryClient } from "@tanstack/react-query";
import { classPackQueryKeys } from "@/features/classPacks/classPackQueryKeys";
import { clientQueryKeys } from "@/features/clients/clientQueryKeys";
import { feeQueryKeys } from "@/features/fees/feeQueryKeys";
import {
  deletePayment,
  deleteStudentPlanChange,
  restorePayment,
  recordPayment,
  setStudentPlan,
} from "@/features/fees/feesApi";
import { RecordPaymentRequest, SetStudentPlanRequest } from "@/features/fees/types";

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

export function useRestorePayment() {
  const invalidateFees = useInvalidateFees();
  return useMutation({
    mutationFn: (paymentId: string) => restorePayment(paymentId),
    onSuccess: invalidateFees,
  });
}

function useInvalidateFamilyPlans(clientId: string) {
  const queryClient = useQueryClient();
  return () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: clientQueryKeys.detail(clientId) }),
      queryClient.invalidateQueries({ queryKey: feeQueryKeys.all }),
      queryClient.invalidateQueries({ queryKey: classPackQueryKeys.all }),
    ]);
}

export function useSetStudentPlan(clientId: string) {
  const invalidateFamilyPlans = useInvalidateFamilyPlans(clientId);
  return useMutation({
    mutationFn: ({ studentId, request }: { studentId: string; request: SetStudentPlanRequest }) =>
      setStudentPlan(studentId, request),
    onSuccess: invalidateFamilyPlans,
  });
}

export function useDeleteStudentPlanChange(clientId: string) {
  const invalidateFamilyPlans = useInvalidateFamilyPlans(clientId);
  return useMutation({
    mutationFn: ({ studentId, month }: { studentId: string; month: string }) =>
      deleteStudentPlanChange(studentId, month),
    onSuccess: invalidateFamilyPlans,
  });
}
