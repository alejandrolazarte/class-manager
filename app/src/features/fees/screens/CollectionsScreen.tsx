import { useState } from "react";
import { monthOf } from "@/features/fees/months";
import { MonthlyFeesScreen } from "@/features/fees/screens/MonthlyFeesScreen";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { usePendingOrders } from "@/features/orders/useOrders";
import { translate } from "@/i18n/translate";
import { CollectionsView } from "@/navigation/routes";
import { View } from "react-native";
import { Card } from "@/ui/Card";
import { EmptyState } from "@/ui/EmptyState";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Tabs } from "@/ui/Tabs";

interface CollectionsScreenProps {
  initialMonth?: string;
  initialView?: CollectionsView;
}

export function CollectionsScreen({ initialMonth, initialView = "fees" }: CollectionsScreenProps) {
  const canViewPayments = useCan(permissions.paymentsViewAll, permissions.paymentsViewOwn);
  const canViewOrders = useCan(permissions.ordersViewAll, permissions.ordersViewOwn);
  const [view, setView] = useState<CollectionsView>(canViewOrders ? initialView : "fees");
  const pendingOrders = usePendingOrders({ enabled: canViewOrders });

  if (!canViewPayments) {
    return (
      <ScrollScreen header={<ScreenHeader title={translate("tabs.fees")} />}>
        <Card>
          <EmptyState
            icon="locked"
            iconTone="disabled-foreground"
            message={translate("fees.noAccess")}
          />
        </Card>
      </ScrollScreen>
    );
  }

  if (!canViewOrders) {
    return <MonthlyFeesScreen initialMonth={initialMonth} />;
  }

  const viewSwitcher = (
    <View className="-mx-5">
      <Tabs
        options={[
          { value: "fees" as const, label: translate("collections.fees"), icon: "recurringFee" },
          {
            value: "orders" as const,
            label:
              pendingOrders.count > 0
                ? translate("collections.ordersWithCount", { count: pendingOrders.count })
                : translate("collections.orders"),
            accessibilityLabel: translate("collections.orders"),
            icon: "products",
          },
        ]}
        selectedValue={view}
        onChange={setView}
      />
    </View>
  );

  return view === "orders" ? (
    <OrderListScreen viewSwitcher={viewSwitcher} month={initialMonth ?? monthOf()} />
  ) : (
    <MonthlyFeesScreen initialMonth={initialMonth} viewSwitcher={viewSwitcher} />
  );
}
