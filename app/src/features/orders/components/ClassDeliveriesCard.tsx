import { View } from "react-native";
import { useDeliverInClass } from "@/features/orders/useOrderMutations";
import { useClassDeliveries } from "@/features/orders/useOrders";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { useToast } from "@/ui/ToastProvider";

interface ClassDeliveriesCardProps {
  classGroupId: string;
}

export function ClassDeliveriesCard({ classGroupId }: ClassDeliveriesCardProps) {
  const { showToast } = useToast();
  const { data: deliveries = [] } = useClassDeliveries(classGroupId);
  const deliverInClassMutation = useDeliverInClass(classGroupId);

  if (deliveries.length === 0) {
    return null;
  }

  const deliver = async (orderId: string) => {
    try {
      await deliverInClassMutation.mutateAsync(orderId);
      showToast(translate("sessions.deliveries.delivered"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  return (
    <Card className="gap-3 p-4">
      <AppText variant="bodyStrong">{translate("sessions.deliveries.title")}</AppText>
      {deliveries.map((delivery) => (
        <View key={delivery.orderId} className="flex-row items-center justify-between gap-3">
          <View className="flex-1 gap-0.5">
            <AppText variant="bodyStrong">
              {delivery.clientFullName ?? translate("sessions.deliveries.noClient")}
            </AppText>
            <AppText variant="caption" tone="subtle">
              {delivery.lines
                .map((line) => (line.quantity > 1 ? `${line.quantity} × ${line.name}` : line.name))
                .join(", ")}
            </AppText>
          </View>
          <Button
            size="medium"
            variant="secondary"
            icon="delivered"
            label={translate("sessions.deliveries.deliver")}
            accessibilityLabel={`${translate("sessions.deliveries.deliver")} ${delivery.clientFullName ?? ""}`}
            onPress={() => deliver(delivery.orderId)}
            isLoading={deliverInClassMutation.isPending}
          />
        </View>
      ))}
    </Card>
  );
}
