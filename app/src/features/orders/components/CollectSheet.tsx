import { View } from "react-native";
import { PaymentPanel } from "@/features/orders/components/PaymentPanel";
import { orderClientLabel } from "@/features/orders/orderLabels";
import { Order } from "@/features/orders/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { BottomSheet } from "@/ui/BottomSheet";

interface CollectSheetProps {
  order: Order;
  onClose: () => void;
}

export function CollectSheet({ order, onClose }: CollectSheetProps) {
  return (
    <BottomSheet onClose={onClose}>
      <View className="gap-1">
        <AppText variant="heading">{translate("orders.collect")}</AppText>
        <AppText variant="footnote" tone="subtle">
          {[orderClientLabel(order), translate("orders.number", { number: order.number })].join(
            " · ",
          )}
        </AppText>
      </View>
      <PaymentPanel order={order} onDone={onClose} />
    </BottomSheet>
  );
}
