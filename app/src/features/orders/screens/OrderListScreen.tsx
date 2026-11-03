import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { OrderRow } from "@/features/orders/components/OrderRow";
import { OrderFilter, orderFilters } from "@/features/orders/types";
import { useOrders } from "@/features/orders/useOrders";
import { useCanSellAtCounter } from "@/features/orders/useOrderPermissions";
import { translate, TranslationKey } from "@/i18n/translate";
import { orderTabs, routes } from "@/navigation/routes";
import { useCurrentTab } from "@/navigation/useCurrentTab";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ChipGroup } from "@/ui/ChipGroup";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";

export function OrderListScreen() {
  const router = useRouter();
  const orderTab = useCurrentTab(orderTabs);
  const canSellAtCounter = useCanSellAtCounter();
  const [filter, setFilter] = useState<OrderFilter>("all");
  const { data: orders = [], isPending, isError, refetch } = useOrders(null, filter);
  const openCounterSale = () => router.push(routes.newCounterSale(orderTab));

  return (
    <ScrollScreen header={<ScreenHeader navigation="back" title={translate("orders.title")} />}>
      {canSellAtCounter ? (
        <Button icon="add" label={translate("orders.counterSale.open")} onPress={openCounterSale} />
      ) : null}
      <ChipGroup
        options={orderFilters.map((orderFilter) => ({
          value: orderFilter,
          label: translate(`orders.filter.${orderFilter}` as TranslationKey),
        }))}
        selectedValue={filter}
        onChange={setFilter}
      />
      {isPending ? <SkeletonList /> : null}
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
      {orders.length > 0 ? (
        <View>
          {orders.map((order, index) => (
            <AnimatedListItem key={order.id} index={index}>
              <OrderRow order={order} />
            </AnimatedListItem>
          ))}
        </View>
      ) : null}
    </ScrollScreen>
  );
}
