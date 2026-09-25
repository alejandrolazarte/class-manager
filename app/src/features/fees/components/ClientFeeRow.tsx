import { Pressable, Text, View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { summarizeNames } from "@/features/fees/summarizeNames";
import { formatMoney } from "@/features/fees/money";
import { ClientFee } from "@/features/fees/types";
import { translate } from "@/i18n/translate";

interface ClientFeeRowProps {
  clientFee: ClientFee;
  onPress: (clientFee: ClientFee) => void;
}

const statusClassNames = {
  Unpaid: "text-red-600",
  Partial: "text-amber-700",
  NoFee: "text-gray-500",
  Paid: "text-green-700",
} as const;

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
      className="flex-row items-center gap-3 border-b border-gray-100 bg-white px-4 py-3"
    >
      <View className="flex-1 gap-1">
        <Text className="text-base font-semibold text-gray-900">{clientFee.clientFullName}</Text>
        <Text className="text-sm text-gray-600">{summarizeNames(clientFee.studentNames)}</Text>
      </View>
      <Text className={`text-sm font-medium ${statusClassNames[clientFee.status]}`}>
        {statusLabel}
      </Text>
    </Pressable>
  );
}
