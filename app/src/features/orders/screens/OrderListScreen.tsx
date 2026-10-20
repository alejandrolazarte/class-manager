import { useRouter } from "expo-router";
import { ReactNode, useState } from "react";
import { View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { formatMoney } from "@/formatting/money";
import { monthOf } from "@/features/fees/months";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { OrderRow } from "@/features/orders/components/OrderRow";
import { OrderMonthSummary } from "@/features/orders/components/OrderMonthSummary";
import { OrderFilter, orderFilters } from "@/features/orders/types";
import { useOrders, useOrderSummary, usePendingOrders } from "@/features/orders/useOrders";
import { translate, TranslationKey } from "@/i18n/translate";
import { orderTabs, routes } from "@/navigation/routes";
import { useCurrentTab } from "@/navigation/useCurrentTab";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ChipGroup } from "@/ui/ChipGroup";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";

interface OrderListScreenProps {
  viewSwitcher?: ReactNode;
  month?: string;
}

export function OrderListScreen({ viewSwitcher, month = monthOf() }: OrderListScreenProps) {
  const router = useRouter();
  const business = useCurrentBusiness();
  const orderTab = useCurrentTab(orderTabs);
  const canManageOrders = useCan(permissions.ordersManage);
  const isInCollections = viewSwitcher !== undefined;
  const [filter, setFilter] = useState<OrderFilter>("all");
  const { data: orders = [], isPending, isError, refetch } = useOrders(null, filter);
  const pendingOrders = usePendingOrders({ enabled: isInCollections });
  const { data: orderSummary } = useOrderSummary(month, { enabled: isInCollections });
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
      {isInCollections && orderSummary ? (
        <OrderMonthSummary
          orderSummary={orderSummary}
          pendingOrders={pendingOrders}
          money={(amount) => formatMoney(amount, business.currencyCode)}
        />
      ) : null}
      {canManageOrders && !isInCollections ? (
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
              <OrderRow order={order} canManage={canManageOrders} />
            </AnimatedListItem>
          ))}
        </View>
      ) : null}
    </ScrollScreen>
  );
}
