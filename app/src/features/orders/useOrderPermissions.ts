import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { Order } from "@/features/orders/types";
import { featureCodes } from "@/features/subscriptions/subscriptionCodes";
import { useFeature } from "@/features/subscriptions/useSubscription";

export function useCanManageOrder(order: Order): boolean {
  const canManageOrders = useCan(permissions.ordersManage);
  const planIncludesOrder = useFeature(
    order.lines.some((line) => line.kind === "Product")
      ? featureCodes.shop
      : featureCodes.classPacks,
  );
  return canManageOrders && planIncludesOrder;
}

export function useCanSellAtCounter(): boolean {
  const canManageOrders = useCan(permissions.ordersManage);
  const planIncludesShop = useFeature(featureCodes.shop);
  return canManageOrders && planIncludesShop;
}
