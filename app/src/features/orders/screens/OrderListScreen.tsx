import { useRouter } from "expo-router";
import { ReactNode, useState } from "react";
import { View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { formatMoney } from "@/features/fees/money";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { OrderCard } from "@/features/orders/components/OrderCard";
import { PendingOrdersSummaryCard } from "@/features/orders/components/PendingOrdersSummaryCard";
import { OrderFilter, orderFilters } from "@/features/orders/types";
import { useOrders, usePendingOrders } from "@/features/orders/useOrders";
import { translate, TranslationKey } from "@/i18n/translate";
import { orderTabs, routes } from "@/navigation/routes";
import { useCurrentTab } from "@/navigation/useCurrentTab";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { Spinner } from "@/ui/Spinner";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

interface OrderListScreenProps {
  viewSwitcher?: ReactNode;
}

export function OrderListScreen({ viewSwitcher }: OrderListScreenProps) {
  const router = useRouter();
  const business = useCurrentBusiness();
  const orderTab = useCurrentTab(orderTabs);
  const canManageOrders = useCan(permissions.ordersManage);
  const isInCollections = viewSwitcher !== undefined;
  const [filter, setFilter] = useState<OrderFilter>("all");
  const { data: orders = [], isPending, isError, refetch } = useOrders(null, filter);
  const pendingOrders = usePendingOrders({ enabled: isInCollections });
  const openCounterSale = () => router.push(routes.newCounterSale(orderTab));

  return (
    <ScrollScreen
      hasFloatingAction={isInCollections && canManageOrders}
      header={
        isInCollections ? (
          <ScreenHeader eyebrow={translate("collections.shop")} title={translate("orders.title")} />
        ) : (
          <ScreenHeader navigation="back" title={translate("orders.title")} />
        )
      }
      overlay={
        isInCollections && canManageOrders ? (
          <FloatingActionButton
            label={translate("orders.counterSale.open")}
            onPress={openCounterSale}
          />
        ) : undefined
      }
    >
      {viewSwitcher}
      {isInCollections ? (
        <PendingOrdersSummaryCard
          pendingOrders={pendingOrders}
          money={(amount) => formatMoney(amount, business.currencyCode)}
        />
      ) : null}
      {canManageOrders && !isInCollections ? (
        <Button icon="add" label={translate("orders.counterSale.open")} onPress={openCounterSale} />
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
