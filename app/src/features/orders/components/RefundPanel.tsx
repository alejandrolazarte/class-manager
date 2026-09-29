import { useState } from "react";
import { View } from "react-native";
import { Order, OrderLine } from "@/features/orders/types";
import { useRefundOrder } from "@/features/orders/useOrderMutations";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { TextField } from "@/ui/TextField";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { useToast } from "@/ui/ToastProvider";

interface RefundPanelProps {
  order: Order;
  onDone: () => void;
}

const wholeNumberPattern = /^\d+$/;

function remainingUnits(line: OrderLine): number {
  return line.quantity - line.refundedQuantity;
}

export function RefundPanel({ order, onDone }: RefundPanelProps) {
  const { showToast } = useToast();
  const refundOrderMutation = useRefundOrder();
  const refundableLines = order.lines.filter((line) => remainingUnits(line) > 0);
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

  const confirm = async () => {
    if (selectedLine === undefined) {
      return;
    }
    const isProduct = selectedLine.kind === "Product";
    if (isProduct && !wholeNumberPattern.test(quantityText.trim())) {
      setHasFailed(true);
      return;
    }
    setHasFailed(false);
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
    }
  };

  if (refundableLines.length === 0) {
    return (
      <AppText variant="caption" tone="subtle">
        {translate("orders.refund.nothingLeft")}
      </AppText>
    );
  }

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
        onPress={confirm}
        isLoading={refundOrderMutation.isPending}
      />
    </View>
  );
}
