import { NativeStackNavigationProp, useNavigation, useRouter } from "expo-router";
import { useEffect } from "react";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { routes } from "@/navigation/routes";
import { LoadingScreen } from "@/ui/LoadingScreen";

const homeScreenName = "index";

type TodayStackNavigation = NativeStackNavigationProp<Record<string, undefined>>;

export function NotifiedOrdersScreen() {
  const router = useRouter();
  const navigation = useNavigation<TodayStackNavigation>();
  const canViewPayments = useCan(permissions.paymentsViewAll, permissions.paymentsViewOwn);

  useEffect(() => {
    if (!canViewPayments) {
      return;
    }
    const leaveForCollections = setTimeout(() => {
      navigation.reset({ index: 0, routes: [{ name: homeScreenName }] });
      router.navigate(routes.collections("orders"));
    });
    return () => clearTimeout(leaveForCollections);
  }, [canViewPayments, navigation, router]);

  return canViewPayments ? <LoadingScreen /> : <OrderListScreen />;
}
