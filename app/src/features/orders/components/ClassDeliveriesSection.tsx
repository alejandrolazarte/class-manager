import { View } from "react-native";
import { useDeliverInClass } from "@/features/orders/useOrderMutations";
import { useClassDeliveries } from "@/features/orders/useOrders";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { SectionTitle } from "@/ui/SectionTitle";
import { useToast } from "@/ui/ToastProvider";

interface ClassDeliveriesSectionProps {
  classGroupId: string;
}

export function ClassDeliveriesSection({ classGroupId }: ClassDeliveriesSectionProps) {
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
    <View className="mt-1">
      <SectionTitle title={translate("sessions.deliveries.title")} isOverline />
      <View className="mt-1.5 border-t border-border-subtle">
        {deliveries.map((delivery) => (
          <HairlineRow key={delivery.orderId} className="flex-row items-center gap-3 py-2.5">
            <View className="min-w-0 flex-1">
              <AppText variant="bodyStrong">
                {delivery.clientFullName ?? translate("sessions.deliveries.noClient")}
              </AppText>
              <AppText variant="footnote" tone="subtle">
                {delivery.lines
                  .map((line) =>
                    line.quantity > 1 ? `${line.quantity} × ${line.name}` : line.name,
                  )
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
          </HairlineRow>
        ))}
      </View>
    </View>
  );
}
