import { useState } from "react";
import { StudentAppOrderCard } from "@/features/studentApp/components/StudentAppOrderCard";
import {
  StudentAppOrderFilter,
  studentAppOrderFilters,
  isOrderOpen,
  matchesOrderFilter,
} from "@/features/studentApp/studentAppOrderStage";
import { useStudentAppOrders } from "@/features/studentApp/useStudentAppShop";
import { useRefetchOnFocus } from "@/hooks/useRefetchOnFocus";
import { translate, TranslationKey } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { ChipGroup } from "@/ui/ChipGroup";
import { EmptyState } from "@/ui/EmptyState";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonList } from "@/ui/motion/SkeletonList";
import { AnimatedListItem } from "@/ui/motion/AnimatedListItem";

const filterLabels: Record<StudentAppOrderFilter, TranslationKey> = {
  open: "student.orders.filter.open",
  past: "student.orders.filter.past",
  all: "student.orders.filter.all",
};

export function StudentAppOrdersScreen() {
  const { data: orders = [], isPending, isError, refetch } = useStudentAppOrders();
  useRefetchOnFocus(refetch);
  const [chosenFilter, setChosenFilter] = useState<StudentAppOrderFilter | null>(null);

  const filter = chosenFilter ?? (orders.some(isOrderOpen) ? "open" : "all");
  const visibleOrders = orders.filter((order) => matchesOrderFilter(order, filter));

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
      {isPending ? <SkeletonList /> : null}
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
      {!isPending && !isError && orders.length > 0 ? (
        <ChipGroup
          isScrollable
          options={studentAppOrderFilters.map((orderFilter) => ({
            value: orderFilter,
            label: translate(filterLabels[orderFilter]),
            count:
              orderFilter === "all"
                ? undefined
                : orders.filter((order) => matchesOrderFilter(order, orderFilter)).length,
          }))}
          selectedValue={filter}
          onChange={setChosenFilter}
        />
      ) : null}
      {!isPending && !isError && visibleOrders.length === 0 ? (
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
      {visibleOrders.map((order, index) => (
        <AnimatedListItem key={order.id} index={index}>
          <StudentAppOrderCard order={order} />
        </AnimatedListItem>
      ))}
    </ScrollScreen>
  );
}
