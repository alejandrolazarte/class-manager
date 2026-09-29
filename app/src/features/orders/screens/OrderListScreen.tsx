import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { OrderCard } from "@/features/orders/components/OrderCard";
import { useOrders } from "@/features/orders/useOrders";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { Spinner } from "@/ui/Spinner";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

export function OrderListScreen() {
  const router = useRouter();
  const canManageOrders = useCan(permissions.ordersManage);
  const [awaitingPickupOnly, setAwaitingPickupOnly] = useState(false);
  const { data: orders = [], isPending, isError, refetch } = useOrders(null, awaitingPickupOnly);

  return (
    <ScrollScreen header={<ScreenHeader navigation="back" title={translate("orders.title")} />}>
      {canManageOrders ? (
        <Button
          icon="add"
          label={translate("orders.counterSale.open")}
          onPress={() => router.push(routes.newCounterSale)}
        />
      ) : null}
      <View className="flex-row flex-wrap gap-2">
        <Chip
          label={translate("orders.filter.all")}
          isSelected={!awaitingPickupOnly}
          onPress={() => setAwaitingPickupOnly(false)}
        />
        <Chip
          label={translate("orders.filter.awaitingPickup")}
          isSelected={awaitingPickupOnly}
          onPress={() => setAwaitingPickupOnly(true)}
        />
      </View>
      {isPending ? <Spinner /> : null}
      {isError ? (
        <Banner tone="error" message={translate("orders.loadError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={() => refetch()} />
        </Banner>
      ) : null}
      {!isPending && !isError && orders.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate(awaitingPickupOnly ? "orders.emptyAwaitingPickup" : "orders.empty")}
        </AppText>
      ) : null}
      {orders.map((order) => (
        <OrderCard key={order.id} order={order} canManage={canManageOrders} />
      ))}
    </ScrollScreen>
  );
}
