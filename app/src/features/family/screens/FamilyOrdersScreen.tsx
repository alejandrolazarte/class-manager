import { useState } from "react";
import { ScrollView } from "react-native";
import { FamilyOrderCard } from "@/features/family/components/FamilyOrderCard";
import {
  FamilyOrderFilter,
  familyOrderFilters,
  isOrderOpen,
  matchesOrderFilter,
} from "@/features/family/familyOrderStage";
import { useFamilyHome } from "@/features/family/useFamilyHome";
import { useFamilyOrders } from "@/features/family/useFamilyShop";
import { translate, TranslationKey } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Chip } from "@/ui/Chip";
import { EmptyState } from "@/ui/EmptyState";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";

const filterLabels: Record<FamilyOrderFilter, TranslationKey> = {
  open: "family.orders.filter.open",
  past: "family.orders.filter.past",
  all: "family.orders.filter.all",
};

export function FamilyOrdersScreen() {
  const { data: orders = [], isPending, isError, refetch } = useFamilyOrders();
  const { data: home, isPending: isHomePending } = useFamilyHome();
  const [chosenFilter, setChosenFilter] = useState<FamilyOrderFilter | null>(null);

  const filter = chosenFilter ?? (orders.some(isOrderOpen) ? "open" : "all");
  const visibleOrders = orders.filter((order) => matchesOrderFilter(order, filter));
  const isLoading = isPending || isHomePending;

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          eyebrow={translate("family.shop.title")}
          title={translate("family.orders.title")}
        />
      }
    >
      {isLoading ? <Spinner className="mt-6" /> : null}
      {isError ? (
        <Banner message={translate("family.loadError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={() => refetch()}
          />
        </Banner>
      ) : null}
      {!isLoading && !isError && orders.length > 0 ? (
        <ScrollView
          horizontal
          showsHorizontalScrollIndicator={false}
          className="-mx-5"
          contentContainerClassName="gap-2 px-5"
        >
          {familyOrderFilters.map((orderFilter) => (
            <Chip
              key={orderFilter}
              label={translate(filterLabels[orderFilter])}
              count={
                orderFilter === "all"
                  ? undefined
                  : orders.filter((order) => matchesOrderFilter(order, orderFilter)).length
              }
              isSelected={filter === orderFilter}
              onPress={() => setChosenFilter(orderFilter)}
            />
          ))}
        </ScrollView>
      ) : null}
      {!isLoading && !isError && visibleOrders.length === 0 ? (
        <Card>
          <EmptyState
            icon="orders"
            iconTone="disabled-foreground"
            message={translate(
              orders.length === 0 ? "family.orders.empty" : "family.orders.emptyFilter",
            )}
          />
        </Card>
      ) : null}
      {home === undefined
        ? null
        : visibleOrders.map((order) => (
            <FamilyOrderCard key={order.id} order={order} currencyCode={home.currencyCode} />
          ))}
    </ScrollScreen>
  );
}
