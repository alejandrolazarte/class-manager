import { View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { feeStatusLabel, feeStatusTones } from "@/features/fees/components/feeStatus";
import { summarizeNames } from "@/features/fees/summarizeNames";
import { ClientFee } from "@/features/fees/types";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { StatusPill } from "@/ui/StatusPill";
import { StatusText } from "@/ui/StatusText";

interface ClientFeeRowProps {
  clientFee: ClientFee;
  onPress: (clientFee: ClientFee) => void;
}

const owingStatuses = new Set(["Unpaid", "Partial"]);

export function ClientFeeRow({ clientFee, onPress }: ClientFeeRowProps) {
  const currencyCode = useBusinessCurrency();
  const canRecordPayments = useCan(permissions.paymentsRecord);
  const canCollect = canRecordPayments && owingStatuses.has(clientFee.status);
  return (
    <View className="border-b border-border-subtle pb-1">
      <HairlineRow
        hasDivider={false}
        onPress={() => onPress(clientFee)}
        accessibilityLabel={clientFee.clientFullName}
      >
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
      <View className="min-h-8 flex-row items-center justify-between pb-2">
        <StatusPill
          label={translate("collections.tag.plan")}
          tone="neutral"
          icon="recurringFee"
          isSmall
        />
        {canCollect ? (
          <Button
            size="small"
            icon="paid"
            label={translate("orders.collect")}
            onPress={() => onPress(clientFee)}
          />
        ) : null}
      </View>
    </View>
  );
}
