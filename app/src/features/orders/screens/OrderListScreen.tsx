import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { OrderCard } from "@/features/orders/components/OrderCard";
import { OrderFilter, orderFilters } from "@/features/orders/types";
import { useOrders } from "@/features/orders/useOrders";
import { translate, TranslationKey } from "@/i18n/translate";
import { orderTabs, routes } from "@/navigation/routes";
import { useCurrentTab } from "@/navigation/useCurrentTab";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { Spinner } from "@/ui/Spinner";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

export function OrderListScreen() {
  const router = useRouter();
  const orderTab = useCurrentTab(orderTabs);
  const canManageOrders = useCan(permissions.ordersManage);
  const [filter, setFilter] = useState<OrderFilter>("all");
  const { data: orders = [], isPending, isError, refetch } = useOrders(null, filter);

  return (
    <ScrollScreen header={<ScreenHeader navigation="back" title={translate("orders.title")} />}>
      {canManageOrders ? (
        <Button
          icon="add"
          label={translate("orders.counterSale.open")}
          onPress={() => router.push(routes.newCounterSale(orderTab))}
        />
      ) : null}
      <View className="flex-row flex-wrap gap-2">
        {orderFilters.map((orderFilter) => (
          <Chip
            key={orderFilter}
            label={translate(`orders.filter.${orderFilter}` as TranslationKey)}
            isSelected={filter === orderFilter}
            onPress={() => setFilter(orderFilter)}
          />
        ))}
      </View>
      {isPending ? <Spinner /> : null}
      {isError ? (
        <Banner tone="error" message={translate("orders.loadError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={() => refetch()} />
        </Banner>
      ) : null}
      {!isPending && !isError && orders.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate(`orders.emptyFilter.${filter}` as TranslationKey)}
        </AppText>
      ) : null}
      {orders.map((order) => (
        <OrderCard key={order.id} order={order} canManage={canManageOrders} />
      ))}
    </ScrollScreen>
  );
}
