import { useState } from "react";
import { MonthlyFeesScreen } from "@/features/fees/screens/MonthlyFeesScreen";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { OrderListScreen } from "@/features/orders/screens/OrderListScreen";
import { usePendingOrders } from "@/features/orders/useOrders";
import { translate } from "@/i18n/translate";
import { SegmentedControl } from "@/ui/SegmentedControl";

export type CollectionsView = "fees" | "orders";

interface CollectionsScreenProps {
  initialMonth?: string;
  initialView?: CollectionsView;
}

export function CollectionsScreen({ initialMonth, initialView = "fees" }: CollectionsScreenProps) {
  const canViewOrders = useCan(permissions.ordersViewAll, permissions.ordersViewOwn);
  const [view, setView] = useState<CollectionsView>(canViewOrders ? initialView : "fees");
  const pendingOrders = usePendingOrders({ enabled: canViewOrders });

  if (!canViewOrders) {
    return <MonthlyFeesScreen initialMonth={initialMonth} />;
  }

  const viewSwitcher = (
    <SegmentedControl
      options={[
        { value: "fees" as const, label: translate("collections.fees") },
        {
          value: "orders" as const,
          label:
            pendingOrders.count > 0
              ? translate("collections.ordersWithCount", { count: pendingOrders.count })
              : translate("collections.orders"),
          accessibilityLabel: translate("collections.orders"),
        },
      ]}
      selectedValue={view}
      onChange={setView}
    />
  );

  return view === "orders" ? (
    <OrderListScreen viewSwitcher={viewSwitcher} />
  ) : (
    <MonthlyFeesScreen initialMonth={initialMonth} viewSwitcher={viewSwitcher} />
  );
}
