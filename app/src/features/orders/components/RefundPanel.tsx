import { useState } from "react";
import { View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { refundableLinesOf, remainingUnits } from "@/features/orders/refunds";
import { Order, OrderLine } from "@/features/orders/types";
import { useRefundOrder } from "@/features/orders/useOrderMutations";
import { formatMoney } from "@/formatting/money";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { TextField } from "@/ui/TextField";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { useToast } from "@/ui/ToastProvider";
import { isFilled } from "@/forms/requiredFields";

interface RefundPanelProps {
  order: Order;
  onDone: () => void;
}

const wholeNumberPattern = /^\d+$/;

export function RefundPanel({ order, onDone }: RefundPanelProps) {
  const { showToast } = useToast();
  const business = useCurrentBusiness();
  const refundOrderMutation = useRefundOrder();
  const [isConfirming, setIsConfirming] = useState(false);
  const refundableLines = refundableLinesOf(order);
  const [lineId, setLineId] = useState(refundableLines[0]?.id ?? "");
  const [quantityText, setQuantityText] = useState(
    String(refundableLines[0] ? remainingUnits(refundableLines[0]) : ""),
  );
  const [returnToStock, setReturnToStock] = useState(true);
  const [hasFailed, setHasFailed] = useState(false);
  const selectedLine = refundableLines.find((line) => line.id === lineId);

  const selectLine = (line: OrderLine) => {
    setLineId(line.id);
    setQuantityText(String(remainingUnits(line)));
    setHasFailed(false);
  };

  const askToConfirm = () => {
    if (selectedLine === undefined) {
      return;
    }
    if (selectedLine.kind === "Product" && !wholeNumberPattern.test(quantityText.trim())) {
      setHasFailed(true);
      return;
    }
    setHasFailed(false);
    setIsConfirming(true);
  };

  const refundMessage = () => {
    if (selectedLine?.kind !== "Product") {
      return translate("orders.refund.unusedClasses");
    }
    return translate("orders.cancelConfirm.refund", {
      amount: formatMoney(selectedLine.unitPrice * Number(quantityText.trim()), order.currencyCode),
      businessName: business.name,
    });
  };

  const confirm = async () => {
    if (selectedLine === undefined) {
      return;
    }
    const isProduct = selectedLine.kind === "Product";
    try {
      await refundOrderMutation.mutateAsync({
        orderId: order.id,
        lines: [
          {
            lineId: selectedLine.id,
            quantity: isProduct ? Number(quantityText.trim()) : null,
            returnToStock: isProduct && returnToStock,
          },
        ],
      });
      showToast(translate("orders.refund.saved"));
      onDone();
    } catch {
      setHasFailed(true);
    } finally {
      setIsConfirming(false);
    }
  };

  return (
    <View className="gap-2.5">
      <View className="flex-row flex-wrap gap-2">
        {refundableLines.map((line) => (
          <Chip
            key={line.id}
            label={line.name}
            isSelected={lineId === line.id}
            onPress={() => selectLine(line)}
          />
        ))}
      </View>
      {selectedLine?.kind === "Product" ? (
        <>
          <TextField
            label={translate("orders.refund.quantity")}
            isRequired
            keyboardType="number-pad"
            value={quantityText}
            onChangeText={setQuantityText}
          />
          <ToggleSwitch
            label={translate("orders.refund.returnToStock")}
            value={returnToStock}
            onValueChange={setReturnToStock}
          />
        </>
      ) : (
        <AppText variant="caption" tone="subtle">
          {translate("orders.refund.unusedClasses")}
        </AppText>
      )}
      {hasFailed ? <Banner tone="warning" message={translate("orders.refund.failed")} /> : null}
      <Button
        size="medium"
        variant="dangerOutline"
        label={translate("orders.refund.confirm")}
        onPress={askToConfirm}
        disabled={selectedLine?.kind === "Product" && !isFilled(quantityText)}
      />
      {isConfirming ? (
        <ConfirmDialog
          title={translate("orders.refund.confirmQuestion")}
          message={refundMessage()}
          confirmLabel={translate("orders.refund.confirm")}
          onCancel={() => setIsConfirming(false)}
          onConfirm={confirm}
          isConfirming={refundOrderMutation.isPending}
        />
      ) : null}
    </View>
  );
}
