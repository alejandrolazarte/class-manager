import { View } from "react-native";
import { feeStatusLabel, feeStatusTones } from "@/features/fees/components/feeStatus";
import { formatMoney } from "@/formatting/money";
import { StudentAppBilling } from "@/features/studentApp/types";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Icon } from "@/ui/Icon";
import { ProgressBar } from "@/ui/ProgressBar";
import { StatusText } from "@/ui/StatusText";

interface BillingTileProps {
  billing: StudentAppBilling;
  currencyCode: string;
}

export function hasBillingTile(billing: StudentAppBilling): boolean {
  return billing.monthlyFee !== null || billing.classes !== null;
}

export function BillingTile({ billing, currencyCode }: BillingTileProps) {
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
        <View className="flex-row items-center justify-between gap-2">
          <AppText variant="badge" tone="muted" className="min-w-0 flex-1">
            {translate("student.classes.title")}
          </AppText>
          <Icon name="next" size="small" tone="subtle-foreground" />
        </View>
        <AppText variant="headline">
          {translateCount("student.classes.available", classes.availableClasses)}
        </AppText>
        {classes.unpaidClasses > 0 ? (
          <AppText variant="badge" tone="danger">
            {translateCount("student.classes.unpaid", classes.unpaidClasses)}
          </AppText>
        ) : null}
      </>
    );
  }
  return null;
}
