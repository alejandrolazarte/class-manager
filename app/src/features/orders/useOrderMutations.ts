import { useMutation, useQueryClient } from "@tanstack/react-query";
import { classPackQueryKeys } from "@/features/classPacks/classPackQueryKeys";
import { feeQueryKeys } from "@/features/fees/feeQueryKeys";
import { orderQueryKeys } from "@/features/orders/orderQueryKeys";
import { PaymentMethod } from "@/features/fees/types";
import {
  cancelOrder,
  confirmOrderPayment,
  createCounterSale,
  deliverInClass,
  markOrderDelivered,
  markOrderReady,
  refundOrder,
} from "@/features/orders/ordersApi";
import { CreateCounterSaleRequest, RefundLine } from "@/features/orders/types";
import { productQueryKeys } from "@/features/products/productQueryKeys";

function useInvalidateOrders() {
  const queryClient = useQueryClient();
  return () =>
    Promise.all(
      [orderQueryKeys.all, productQueryKeys.all, classPackQueryKeys.all, feeQueryKeys.all].map(
        (queryKey) => queryClient.invalidateQueries({ queryKey }),
      ),
    );
}

export function useCreateCounterSale() {
  const invalidate = useInvalidateOrders();
  return useMutation({
    mutationFn: (request: CreateCounterSaleRequest) => createCounterSale(request),
    onSuccess: invalidate,
  });
}

export function useMarkOrderDelivered() {
  const invalidate = useInvalidateOrders();
  return useMutation({
    mutationFn: (orderId: string) => markOrderDelivered(orderId),
    onSuccess: invalidate,
  });
}

export function useRefundOrder() {
  const invalidate = useInvalidateOrders();
  return useMutation({
    mutationFn: ({ orderId, lines }: { orderId: string; lines: RefundLine[] }) =>
      refundOrder(orderId, lines),
    onSuccess: invalidate,
  });
}

export function useConfirmOrderPayment() {
  const invalidate = useInvalidateOrders();
  return useMutation({
    mutationFn: ({
      orderId,
      method,
      isReady,
    }: {
      orderId: string;
      method: PaymentMethod;
      isReady: boolean;
    }) => confirmOrderPayment(orderId, method, isReady),
    onSuccess: invalidate,
  });
}

export function useCancelOrder() {
  const invalidate = useInvalidateOrders();
  return useMutation({
    mutationFn: (orderId: string) => cancelOrder(orderId),
    onSuccess: invalidate,
  });
}

export function useMarkOrderReady() {
  const invalidate = useInvalidateOrders();
  return useMutation({
    mutationFn: (orderId: string) => markOrderReady(orderId),
    onSuccess: invalidate,
  });
}

export function useDeliverInClass(classGroupId: string) {
  const invalidate = useInvalidateOrders();
  return useMutation({
    mutationFn: (orderId: string) => deliverInClass(classGroupId, orderId),
    onSuccess: invalidate,
  });
}
