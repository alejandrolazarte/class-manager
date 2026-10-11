import { View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { feeStatusLabel, feeStatusTones } from "@/features/fees/components/feeStatus";
import { summarizeNames } from "@/features/fees/summarizeNames";
import { ClientFee } from "@/features/fees/types";
import { AppText } from "@/ui/AppText";
import { HairlineRow } from "@/ui/HairlineRow";
import { StatusText } from "@/ui/StatusText";

interface ClientFeeRowProps {
  clientFee: ClientFee;
  onPress: (clientFee: ClientFee) => void;
}

export function ClientFeeRow({ clientFee, onPress }: ClientFeeRowProps) {
  const currencyCode = useBusinessCurrency();
  return (
    <HairlineRow onPress={() => onPress(clientFee)} accessibilityLabel={clientFee.clientFullName}>
      <View className="min-w-0 flex-1">
        <AppText variant="bodyStrong" numberOfLines={1}>
          {clientFee.clientFullName}
        </AppText>
        <AppText variant="caption" tone="subtle">
          {summarizeNames(clientFee.studentNames)}
        </AppText>
      </View>
      <StatusText
        label={feeStatusLabel(clientFee, currencyCode)}
        tone={feeStatusTones[clientFee.status]}
      />
    </HairlineRow>
  );
}
