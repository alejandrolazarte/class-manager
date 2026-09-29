import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  cancelFamilyOrder,
  getFamilyShop,
  listFamilyOrders,
  placeFamilyOrder,
} from "@/features/family/familyApi";
import { familyQueryKeys } from "@/features/family/familyQueryKeys";
import { PlaceFamilyOrderLine } from "@/features/family/types";

export function useFamilyShop() {
  return useQuery({ queryKey: familyQueryKeys.shop(), queryFn: getFamilyShop });
}

export function useFamilyOrders() {
  return useQuery({ queryKey: familyQueryKeys.orders(), queryFn: listFamilyOrders });
}

function useInvalidateFamily() {
  const queryClient = useQueryClient();
  return () => queryClient.invalidateQueries({ queryKey: familyQueryKeys.all });
}

export function usePlaceFamilyOrder() {
  const invalidate = useInvalidateFamily();
  return useMutation({
    mutationFn: (lines: PlaceFamilyOrderLine[]) => placeFamilyOrder(lines),
    onSuccess: invalidate,
  });
}

export function useCancelFamilyOrder() {
  const invalidate = useInvalidateFamily();
  return useMutation({
    mutationFn: (orderId: string) => cancelFamilyOrder(orderId),
    onSuccess: invalidate,
  });
}
