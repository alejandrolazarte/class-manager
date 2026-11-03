import { useRouter } from "expo-router";
import { useCallback, useState } from "react";
import { FlatList, View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { useClassPacks } from "@/features/classPacks/useClassPacks";
import { ClientFeeRow } from "@/features/fees/components/ClientFeeRow";
import {
  CollectionsSummary,
  CollectionsTotals,
} from "@/features/fees/components/CollectionsSummary";
import { addMonths, formatMonth, monthOf } from "@/features/fees/months";
import { ClientFee } from "@/features/fees/types";
import { useMonthlyFees } from "@/features/fees/useFees";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { OrderRow } from "@/features/orders/components/OrderRow";
import { Order, OrderFilter, OrderLineKind } from "@/features/orders/types";
import { useMonthOrders, useOrderSummary, usePendingOrders } from "@/features/orders/useOrders";
import { useCanSellAtCounter } from "@/features/orders/useOrderPermissions";
import { formatMoney } from "@/formatting/money";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { translate, TranslationKey } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { EmptyState } from "@/ui/EmptyState";
import { FloatingActionButton } from "@/ui/FloatingActionButton";
import { Screen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SectionTitle } from "@/ui/SectionTitle";
import { SegmentedControl } from "@/ui/SegmentedControl";
import { StepArrow } from "@/ui/StepArrow";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";

const collectionKinds = ["all", "plans", "classPacks", "products"] as const;
type CollectionKind = (typeof collectionKinds)[number];

type CollectionStatus = Exclude<OrderFilter, "all">;

type CollectionItem =
  | { type: "section"; key: string; title: string }
  | { type: "plan"; key: string; clientFee: ClientFee }
  | { type: "order"; key: string; order: Order };

interface CollectionsScreenProps {
  initialMonth?: string;
}

const debtorStatuses = new Set(["Unpaid", "Partial"]);

const kindLabelKeys: Record<CollectionKind, TranslationKey> = {
  all: "collections.kind.all",
  plans: "collections.kind.plans",
  classPacks: "collections.kind.classPacks",
  products: "collections.kind.products",
};

const statusLabelKeys: Record<CollectionStatus, TranslationKey> = {
  owing: "collections.status.owing",
  awaitingPickup: "collections.status.awaitingPickup",
};

const lineKindOf: Partial<Record<CollectionKind, OrderLineKind>> = {
  classPacks: "ClassPack",
  products: "Product",
};

function hasLineOf(order: Order, lineKind: OrderLineKind): boolean {
  return order.lines.some((line) => line.kind === lineKind);
}

function matchesStatus(order: Order, status: CollectionStatus | null): boolean {
  if (status === "owing") {
    return order.balance > 0;
  }
  if (status === "awaitingPickup") {
    return order.awaitsPickup;
  }
  return true;
}

function sumOf(amounts: readonly number[]): number {
  return amounts.reduce((total, amount) => total + amount, 0);
}

export function CollectionsScreen({ initialMonth }: CollectionsScreenProps) {
  const canViewPayments = useCan(permissions.paymentsViewAll, permissions.paymentsViewOwn);
  if (!canViewPayments) {
    return (
      <Screen header={<ScreenHeader title={translate("tabs.fees")} />}>
        <EmptyState
          icon="locked"
          iconTone="disabled-foreground"
          message={translate("fees.noAccess")}
        />
      </Screen>
    );
  }
  return <Collections initialMonth={initialMonth} />;
}

function Collections({ initialMonth }: CollectionsScreenProps) {
  const router = useRouter();
  const business = useCurrentBusiness();
  const canViewEveryPayment = useCan(permissions.paymentsViewAll);
  const canViewOrders = useCan(permissions.ordersViewAll, permissions.ordersViewOwn);
  const canSellAtCounter = useCanSellAtCounter();
  const canViewClassPacks = useCan(permissions.classPacksView);
  const { data: catalog } = useClassPacks(false, { enabled: canViewClassPacks });
  const [month, setMonth] = useState(initialMonth ?? monthOf());
  const [kind, setKind] = useState<CollectionKind>("all");
  const [status, setStatus] = useState<CollectionStatus | null>(null);
  const monthlyFees = useMonthlyFees(month);
  const showsOrders = canViewOrders && kind !== "plans";
  const monthOrders = useMonthOrders(month, { enabled: canViewOrders });
  const orderSummary = useOrderSummary(month, { enabled: canViewOrders });
  const pendingOrders = usePendingOrders({ enabled: canViewOrders });
  const { refetch: refetchMonthlyFees } = monthlyFees;
  const { refetch: refetchMonthOrders } = monthOrders;
  const { refetch: refetchOrderSummary } = orderSummary;
  const refetch = useCallback(() => {
    refetchMonthlyFees();
    if (canViewOrders) {
      refetchMonthOrders();
      refetchOrderSummary();
    }
  }, [refetchMonthlyFees, refetchMonthOrders, refetchOrderSummary, canViewOrders]);
  useRefetchOnFocus(refetch);
  const money = (amount: number) => formatMoney(amount, business.currencyCode);

  const allClients = monthlyFees.data?.clients ?? [];
  const debtors = allClients.filter((clientFee) => debtorStatuses.has(clientFee.status));
  const planRows =
    kind === "all" || kind === "plans"
      ? status === null
        ? allClients
        : status === "owing"
          ? debtors
          : []
      : [];
  const lineKind = lineKindOf[kind];
  const orderRows = showsOrders
    ? (monthOrders.data ?? [])
        .filter((order) => lineKind === undefined || hasLineOf(order, lineKind))
        .filter((order) => matchesStatus(order, status))
    : [];

  const items: CollectionItem[] = [];
  const hasBothSections = planRows.length > 0 && orderRows.length > 0;
  if (hasBothSections) {
    items.push({
      type: "section",
      key: "plans-section",
      title: translate("fees.month.monthlyFeeClientsTitle"),
    });
  }
  [...planRows]
    .sort((first, second) => second.balance - first.balance)
    .forEach((clientFee) =>
      items.push({ type: "plan", key: `plan-${clientFee.clientId}`, clientFee }),
    );
  if (hasBothSections) {
    items.push({ type: "section", key: "orders-section", title: translate("orders.title") });
  }
  orderRows.forEach((order) => items.push({ type: "order", key: `order-${order.id}`, order }));

  const totals: CollectionsTotals | null = monthlyFees.data
    ? {
        month,
        plansCollected: sumOf(allClients.map((clientFee) => clientFee.paid)),
        classPacksCollected: orderSummary.data?.classPacksCollected ?? 0,
        productsCollected: orderSummary.data?.productsCollected ?? 0,
        owed: sumOf(debtors.map((debtor) => debtor.balance)) + (orderSummary.data?.unpaid ?? 0),
        debtorCount: debtors.length + (canViewOrders ? pendingOrders.owing.length : 0),
        paidByClassPackClients:
          monthlyFees.data.totalPaid - sumOf(allClients.map((clientFee) => clientFee.paid)),
      }
    : null;

  const kindOptions = collectionKinds.map((collectionKind) => ({
    value: collectionKind,
    label: translate(kindLabelKeys[collectionKind]),
  }));
  const statusOptions: CollectionStatus[] = canViewOrders ? ["owing", "awaitingPickup"] : ["owing"];
  const isPending = monthlyFees.isPending || (showsOrders && monthOrders.isPending);
  const isError = monthlyFees.isError || (showsOrders && monthOrders.isError);

  const emptyState =
    status === "owing" ? (
      <EmptyState
        icon="celebration"
        iconTone="success"
        message={translate("fees.month.nobodyOwes")}
      />
    ) : status === "awaitingPickup" ? (
      <EmptyState icon="products" message={translate("orders.emptyFilter.awaitingPickup")} />
    ) : kind === "plans" ? (
      <EmptyState icon="fees" message={translate("fees.month.empty")} />
    ) : (
      <EmptyState icon="fees" message={translate("collections.empty")} />
    );

  const header = (
    <View className="gap-4 pb-1">
      <ScreenHeader
        eyebrow={translate(canViewEveryPayment ? "fees.month.title" : "fees.month.ownTitle")}
        title={formatMonth(month)}
        accessory={
          <View className="flex-row gap-1">
            <StepArrow
              direction="previous"
              label={translate("fees.month.previous")}
              onPress={() => setMonth(addMonths(month, -1))}
            />
            <StepArrow
              direction="next"
              label={translate("fees.month.next")}
              onPress={() => setMonth(addMonths(month, 1))}
            />
          </View>
        }
      />
      <View className="gap-4 px-5">
        {catalog?.length === 0 ? (
          <Banner tone="warning" message={translate("fees.month.noPlansYet")}>
            <Button
              variant="secondary"
              size="medium"
              label={translate("fees.month.goToPlans")}
              onPress={() => router.push(routes.feesNewPlan)}
            />
          </Banner>
        ) : null}
        {totals ? (
          <CollectionsSummary
            totals={totals}
            awaitingPickupCount={canViewOrders ? pendingOrders.awaitingPickup.length : null}
            money={money}
          />
        ) : null}
        {canViewOrders ? (
          <SegmentedControl options={kindOptions} selectedValue={kind} onChange={setKind} />
        ) : null}
        <View className="flex-row flex-wrap gap-2">
          {statusOptions.map((statusOption) => (
            <Chip
              key={statusOption}
              label={translate(statusLabelKeys[statusOption])}
              isSelected={status === statusOption}
              onPress={() => setStatus(status === statusOption ? null : statusOption)}
            />
          ))}
        </View>
        {isError ? (
          <Banner message={translate("common.unexpectedError")}>
            <Button
              variant="secondary"
              size="medium"
              label={translate("common.retry")}
              onPress={refetch}
            />
          </Banner>
        ) : null}
      </View>
    </View>
  );

  return (
    <Screen
      overlay={
        canViewOrders && canSellAtCounter ? (
          <FloatingActionButton
            label={translate("orders.counterSale.open")}
            onPress={() => router.push(routes.newCounterSale("fees"))}
          />
        ) : undefined
      }
    >
      <FlatList
        data={isPending ? [] : items}
        keyExtractor={(item) => item.key}
        ListHeaderComponent={header}
        renderItem={({ item, index }) =>
          item.type === "section" ? (
            <View className="px-5 pt-5">
              <SectionTitle title={item.title} isOverline />
            </View>
          ) : (
            <AnimatedListItem index={index} className="px-5">
              {item.type === "plan" ? (
                <ClientFeeRow
                  clientFee={item.clientFee}
                  onPress={() =>
                    router.push(routes.recordPayment("fees", item.clientFee.clientId, month))
                  }
                />
              ) : (
                <OrderRow order={item.order} />
              )}
            </AnimatedListItem>
          )
        }
        ListEmptyComponent={
          isPending ? <SkeletonList className="px-5" /> : isError ? null : emptyState
        }
        refreshing={monthlyFees.isRefetching}
        onRefresh={refetch}
        contentContainerClassName="w-full max-w-2xl self-center pb-28"
      />
    </Screen>
  );
}
