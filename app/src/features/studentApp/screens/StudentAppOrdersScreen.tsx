import { useState } from "react";
import { ScrollView } from "react-native";
import { StudentAppOrderCard } from "@/features/studentApp/components/StudentAppOrderCard";
import {
  StudentAppOrderFilter,
  studentAppOrderFilters,
  isOrderOpen,
  matchesOrderFilter,
} from "@/features/studentApp/studentAppOrderStage";
import { useStudentAppHome } from "@/features/studentApp/useStudentAppHome";
import { useStudentAppOrders } from "@/features/studentApp/useStudentAppShop";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { translate, TranslationKey } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Chip } from "@/ui/Chip";
import { EmptyState } from "@/ui/EmptyState";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";

const filterLabels: Record<StudentAppOrderFilter, TranslationKey> = {
  open: "student.orders.filter.open",
  past: "student.orders.filter.past",
  all: "student.orders.filter.all",
};

export function StudentAppOrdersScreen() {
  const { data: orders = [], isPending, isError, refetch } = useStudentAppOrders();
  useRefetchOnFocus(refetch);
  const { data: home, isPending: isHomePending } = useStudentAppHome();
  const [chosenFilter, setChosenFilter] = useState<StudentAppOrderFilter | null>(null);

  const filter = chosenFilter ?? (orders.some(isOrderOpen) ? "open" : "all");
  const visibleOrders = orders.filter((order) => matchesOrderFilter(order, filter));
  const isLoading = isPending || isHomePending;

  return (
    <ScrollScreen
      header={
        <ScreenHeader
          navigation="back"
          eyebrow={translate("student.shop.title")}
          title={translate("student.orders.title")}
        />
      }
    >
      {isLoading ? <Spinner className="mt-6" /> : null}
      {isError ? (
        <Banner message={translate("student.loadError")}>
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
          {studentAppOrderFilters.map((orderFilter) => (
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
              orders.length === 0 ? "student.orders.empty" : "student.orders.emptyFilter",
            )}
          />
        </Card>
      ) : null}
      {home === undefined
        ? null
        : visibleOrders.map((order) => (
            <StudentAppOrderCard key={order.id} order={order} currencyCode={home.currencyCode} />
          ))}
    </ScrollScreen>
  );
}
