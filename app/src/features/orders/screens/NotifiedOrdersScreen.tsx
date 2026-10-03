import { useRouter } from "expo-router";
import { useEffect } from "react";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { routes } from "@/navigation/routes";
import { LoadingScreen } from "@/ui/LoadingScreen";

export function NotifiedOrdersScreen() {
  const router = useRouter();
  const canViewPayments = useCan(permissions.paymentsViewAll, permissions.paymentsViewOwn);

  useEffect(() => {
    if (!canViewPayments) {
      return;
    }
    if (router.canDismiss()) {
      router.dismiss();
    }
    router.navigate(routes.collections("orders"));
  }, [canViewPayments, router]);

  return canViewPayments ? <LoadingScreen /> : <OrderListScreen />;
}
