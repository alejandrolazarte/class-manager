import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  cancelStudentAppOrder,
  getStudentAppShop,
  listStudentAppOrders,
  placeStudentAppOrder,
} from "@/features/studentApp/studentAppApi";
import { studentAppQueryKeys } from "@/features/studentApp/studentAppQueryKeys";
import { StudentAppDelivery, PlaceStudentAppOrderLine } from "@/features/studentApp/types";

export function useStudentAppShop() {
  return useQuery({ queryKey: studentAppQueryKeys.shop(), queryFn: getStudentAppShop });
}

export function useStudentAppOrders() {
  return useQuery({ queryKey: studentAppQueryKeys.orders(), queryFn: listStudentAppOrders });
}

function useInvalidateStudentApp() {
  const queryClient = useQueryClient();
  return () => queryClient.invalidateQueries({ queryKey: studentAppQueryKeys.all });
}

export function usePlaceStudentAppOrder() {
  const invalidate = useInvalidateStudentApp();
  return useMutation({
    mutationFn: ({
      lines,
      delivery,
    }: {
      lines: PlaceStudentAppOrderLine[];
      delivery: StudentAppDelivery | null;
    }) => placeStudentAppOrder(lines, delivery),
    onSuccess: invalidate,
  });
}

export function useCancelStudentAppOrder() {
  const invalidate = useInvalidateStudentApp();
  return useMutation({
    mutationFn: (orderId: string) => cancelStudentAppOrder(orderId),
    onSuccess: invalidate,
  });
}
