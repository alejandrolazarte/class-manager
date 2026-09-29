import { Pressable, View } from "react-native";
import { feeStatusLabel, feeStatusTones } from "@/features/fees/components/feeStatus";
import { formatMoney } from "@/features/fees/money";
import { FamilyBilling } from "@/features/family/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { Icon, IconName } from "@/ui/Icon";
import { ProgressBar } from "@/ui/ProgressBar";
import { StatusPill } from "@/ui/StatusPill";

interface BillingTileProps {
  billing: FamilyBilling;
  currencyCode: string;
  onBuyClasses: () => void;
}

function TileTitle({ icon, title }: { icon: IconName; title: string }) {
  return (
    <View className="flex-row items-center gap-1.5">
      <Icon name={icon} tone="primary" />
      <AppText variant="overline" tone="subtle">
        {title}
      </AppText>
    </View>
  );
}

export function BillingTile({ billing, currencyCode, onBuyClasses }: BillingTileProps) {
  if (billing.monthlyFee !== null) {
    const { monthlyFee } = billing;
    const paidRatio =
      monthlyFee.fee === null || monthlyFee.fee === 0 ? 1 : monthlyFee.paid / monthlyFee.fee;
    return (
      <Card className="flex-1 gap-2 p-3.5">
        <TileTitle icon="fees" title={translate("family.fee.title")} />
        <AppText variant="headline">
          {monthlyFee.fee === null ? "—" : formatMoney(monthlyFee.fee, currencyCode)}
        </AppText>
        <StatusPill
          label={feeStatusLabel(monthlyFee, currencyCode)}
          tone={feeStatusTones[monthlyFee.status]}
        />
        <View className="flex-row">
          <ProgressBar ratio={paidRatio} tone="success" />
        </View>
        <AppText variant="caption" tone="subtle">
          {translate("family.payAtSchool")}
        </AppText>
      </Card>
    );
  }
  if (billing.classes !== null) {
    const { classes } = billing;
    return (
      <Card className="flex-1 gap-2 p-3.5">
        <TileTitle icon="classPacks" title={translate("family.classes.title")} />
        <AppText variant="headline">
          {translateCount("family.classes.available", classes.availableClasses)}
        </AppText>
        {classes.unpaidClasses > 0 ? (
          <AppText variant="label" tone="danger">
            {translateCount("family.classes.unpaid", classes.unpaidClasses)}
          </AppText>
        ) : null}
        <Pressable
          accessibilityRole="button"
          onPress={onBuyClasses}
          className="flex-row items-center gap-0.5 self-start active:opacity-70"
        >
          <AppText variant="link" tone="primary">
            {translate("family.classes.buyMore")}
          </AppText>
          <Icon name="next" size="small" tone="primary" />
        </Pressable>
      </Card>
    );
  }
  return null;
}
