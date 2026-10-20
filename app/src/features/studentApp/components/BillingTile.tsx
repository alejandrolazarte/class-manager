import { View } from "react-native";
import { feeStatusLabel, feeStatusTones } from "@/features/fees/components/feeStatus";
import { formatMoney } from "@/formatting/money";
import { StudentAppBilling } from "@/features/studentApp/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { ProgressBar } from "@/ui/ProgressBar";
import { StatusText } from "@/ui/StatusText";
import { TextLink } from "@/ui/TextLink";

interface BillingTileProps {
  billing: StudentAppBilling;
  currencyCode: string;
  onBuyClasses: () => void;
}

export function hasBillingTile(billing: StudentAppBilling): boolean {
  return billing.monthlyFee !== null || billing.classes !== null;
}

export function BillingTile({ billing, currencyCode, onBuyClasses }: BillingTileProps) {
  if (billing.monthlyFee !== null) {
    const { monthlyFee } = billing;
    const paidRatio =
      monthlyFee.fee === null || monthlyFee.fee === 0 ? 1 : monthlyFee.paid / monthlyFee.fee;
    return (
      <>
        <AppText variant="badge" tone="muted">
          {translate("student.fee.title")}
        </AppText>
        <AppText variant="headline">
          {monthlyFee.fee === null ? "—" : formatMoney(monthlyFee.fee, currencyCode)}
        </AppText>
        <StatusText
          label={feeStatusLabel(monthlyFee, currencyCode)}
          tone={feeStatusTones[monthlyFee.status]}
        />
        <View className="mt-1 flex-row">
          <ProgressBar ratio={paidRatio} tone="success" isThin />
        </View>
        <AppText variant="footnote" tone="muted">
          {translate("student.payAtSchool")}
        </AppText>
      </>
    );
  }
  if (billing.classes !== null) {
    const { classes } = billing;
    return (
      <>
        <AppText variant="badge" tone="muted">
          {translate("student.classes.title")}
        </AppText>
        <AppText variant="headline">
          {translateCount("student.classes.available", classes.availableClasses)}
        </AppText>
        {classes.unpaidClasses > 0 ? (
          <AppText variant="badge" tone="danger">
            {translateCount("student.classes.unpaid", classes.unpaidClasses)}
          </AppText>
        ) : null}
        <TextLink
          label={translate("student.classes.buyMore")}
          trailingIcon="next"
          onPress={onBuyClasses}
        />
      </>
    );
  }
  return null;
}
