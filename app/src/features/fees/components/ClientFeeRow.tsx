import { View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { feeStatusLabel, feeStatusTones } from "@/features/fees/components/feeStatus";
import { summarizeNames } from "@/features/fees/summarizeNames";
import { ClientFee } from "@/features/fees/types";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { StatusPill } from "@/ui/StatusPill";

interface ClientFeeRowProps {
  clientFee: ClientFee;
  onPress: (clientFee: ClientFee) => void;
}

export function ClientFeeRow({ clientFee, onPress }: ClientFeeRowProps) {
  const currencyCode = useBusinessCurrency();
  return (
    <Card
      onPress={() => onPress(clientFee)}
      accessibilityLabel={clientFee.clientFullName}
      className="flex-row items-center gap-3 rounded-[18px] px-3.5 py-[13px]"
    >
      <View className="min-w-0 flex-1 gap-0.5">
        <AppText variant="bodyStrong">{clientFee.clientFullName}</AppText>
        <AppText variant="caption" tone="subtle">
          {summarizeNames(clientFee.studentNames)}
        </AppText>
      </View>
      <StatusPill
        label={feeStatusLabel(clientFee, currencyCode)}
        tone={feeStatusTones[clientFee.status]}
      />
    </Card>
  );
}
