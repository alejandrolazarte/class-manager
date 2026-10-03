import { useMutation, useQueryClient } from "@tanstack/react-query";
import { classPackQueryKeys } from "@/features/classPacks/classPackQueryKeys";
import {
  createClassPack,
  deleteClassPackPurchase,
  sellClassPack,
  setClassPackActive,
  updateClassPack,
} from "@/features/classPacks/classPacksApi";
import { SaveClassPackRequest, SellClassPackRequest } from "@/features/classPacks/types";
import { feeQueryKeys } from "@/features/fees/feeQueryKeys";
import { orderQueryKeys } from "@/features/orders/orderQueryKeys";

function useInvalidateClassPacksAndFees() {
  const queryClient = useQueryClient();
  return () =>
    Promise.all([
      queryClient.invalidateQueries({ queryKey: classPackQueryKeys.all }),
      queryClient.invalidateQueries({ queryKey: feeQueryKeys.all }),
    ]);
}

export function useSaveClassPack() {
  const invalidate = useInvalidateClassPacksAndFees();
  return useMutation({
    mutationFn: ({
      classPackId,
      request,
    }: {
      classPackId?: string;
      request: SaveClassPackRequest;
    }) =>
      classPackId === undefined ? createClassPack(request) : updateClassPack(classPackId, request),
    onSuccess: invalidate,
  });
}

export function useSetClassPackActive() {
  const invalidate = useInvalidateClassPacksAndFees();
  return useMutation({
    mutationFn: ({ classPackId, isActive }: { classPackId: string; isActive: boolean }) =>
      setClassPackActive(classPackId, isActive),
    onSuccess: invalidate,
  });
}

function useInvalidateClassPackSales() {
  const queryClient = useQueryClient();
  const invalidateClassPacksAndFees = useInvalidateClassPacksAndFees();
  return () =>
    Promise.all([
      invalidateClassPacksAndFees(),
      queryClient.invalidateQueries({ queryKey: orderQueryKeys.all }),
    ]);
}

export function useSellClassPack(clientId: string) {
  const invalidate = useInvalidateClassPackSales();
  return useMutation({
    mutationFn: (request: SellClassPackRequest) => sellClassPack(clientId, request),
    onSuccess: invalidate,
  });
}

export function useDeleteClassPackPurchase() {
  const invalidate = useInvalidateClassPackSales();
  return useMutation({
    mutationFn: (purchaseId: string) => deleteClassPackPurchase(purchaseId),
    onSuccess: invalidate,
  });
}
