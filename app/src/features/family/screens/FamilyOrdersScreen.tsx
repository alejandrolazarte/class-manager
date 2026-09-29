import { View } from "react-native";
import { FamilyOrder } from "@/features/family/types";
import { useCancelFamilyOrder, useFamilyOrders } from "@/features/family/useFamilyShop";
import { useFamilyHome } from "@/features/family/useFamilyHome";
import { formatMoney } from "@/features/fees/money";
import { formatLongDate } from "@/features/sessions/dates";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";
import { useToast } from "@/ui/ToastProvider";

const isoDateLength = 10;

function statusKey(order: FamilyOrder): TranslationKey {
  if (order.awaitsPickup) {
    return "family.orders.status.awaitingPickup";
  }
  return `family.orders.status.${order.status}` as TranslationKey;
}

function FamilyOrderCard({ order, currencyCode }: { order: FamilyOrder; currencyCode: string }) {
  const { showToast } = useToast();
  const cancelFamilyOrderMutation = useCancelFamilyOrder();

  const cancel = async () => {
    try {
      await cancelFamilyOrderMutation.mutateAsync(order.id);
      showToast(translate("family.orders.cancelled"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  return (
    <Card className="gap-2 px-4 py-3.5">
      <View className="flex-row items-start justify-between gap-3">
        <View className="flex-1 gap-0.5">
          <AppText variant="bodyStrong">{translate(statusKey(order))}</AppText>
          <AppText variant="caption" tone="subtle">
            {formatLongDate(order.createdAt.slice(0, isoDateLength))}
          </AppText>
        </View>
        <AppText variant="bodyStrong">{formatMoney(order.total, currencyCode)}</AppText>
      </View>
      {order.lines.map((line) => (
        <AppText key={line.id} variant="body" tone="muted">
          {line.quantity > 1 ? `${line.quantity} × ${line.name}` : line.name}
        </AppText>
      ))}
      {order.status === "Requested" ? (
        <>
          <AppText variant="caption" tone="subtle">
            {translate("family.orders.payBefore", {
              date: formatLongDate(order.expiresAt.slice(0, isoDateLength)),
            })}
          </AppText>
          <Button
            size="medium"
            variant="dangerOutline"
            label={translate("family.orders.cancel")}
            onPress={cancel}
            isLoading={cancelFamilyOrderMutation.isPending}
          />
        </>
      ) : null}
    </Card>
  );
}

export function FamilyOrdersScreen() {
  const { data: orders = [], isPending, isError, refetch } = useFamilyOrders();
  const { data: home, isPending: isHomePending } = useFamilyHome();

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="back" title={translate("family.orders.title")} />}
    >
      {isPending || isHomePending ? <Spinner className="mt-6" /> : null}
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
      {!isPending && !isError && orders.length === 0 ? (
        <AppText variant="body" tone="muted">
          {translate("family.orders.empty")}
        </AppText>
      ) : null}
      {home === undefined
        ? null
        : orders.map((order) => (
            <FamilyOrderCard key={order.id} order={order} currencyCode={home.currencyCode} />
          ))}
    </ScrollScreen>
  );
}
