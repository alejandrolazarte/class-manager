import { Redirect } from "expo-router";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { routes } from "@/navigation/routes";

export function NotifiedOrdersScreen() {
  const canViewPayments = useCan(permissions.paymentsViewAll, permissions.paymentsViewOwn);
  if (canViewPayments) {
    return <Redirect href={routes.collections("orders")} />;
  }
  return <OrderListScreen />;
}
