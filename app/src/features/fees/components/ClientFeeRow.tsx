import { Pressable, View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { summarizeNames } from "@/features/fees/summarizeNames";
import { formatMoney } from "@/features/fees/money";
import { ClientFee } from "@/features/fees/types";
import { translate } from "@/i18n/translate";
import { AppText, TextTone } from "@/ui/AppText";

interface ClientFeeRowProps {
  clientFee: ClientFee;
  onPress: (clientFee: ClientFee) => void;
}

const statusTones: Record<ClientFee["status"], TextTone> = {
  Unpaid: "danger",
  Partial: "warning",
  NoFee: "subtle",
  Paid: "success",
};

export function ClientFeeRow({ clientFee, onPress }: ClientFeeRowProps) {
  const currencyCode = useBusinessCurrency();
  const money = (amount: number) => formatMoney(amount, currencyCode);
  const statusLabel = {
    Unpaid: translate("fees.status.unpaid", { balance: money(clientFee.balance) }),
    Partial: translate("fees.status.partial", {
      paid: money(clientFee.paid),
      fee: money(clientFee.fee ?? 0),
    }),
    NoFee: translate("fees.status.noFee"),
    Paid: translate("fees.status.paid"),
  }[clientFee.status];
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={clientFee.clientFullName}
      onPress={() => onPress(clientFee)}
      className="flex-row items-center gap-3 border-b border-border-subtle bg-surface px-4 py-3"
    >
      <View className="flex-1 gap-1">
        <AppText variant="bodyStrong">{clientFee.clientFullName}</AppText>
        <AppText variant="caption" tone="muted">
          {summarizeNames(clientFee.studentNames)}
        </AppText>
      </View>
      <AppText variant="label" tone={statusTones[clientFee.status]}>
        {statusLabel}
      </AppText>
    </Pressable>
  );
}
