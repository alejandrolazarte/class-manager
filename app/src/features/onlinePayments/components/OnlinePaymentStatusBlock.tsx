import { onlinePaymentStatusLabel } from "@/features/onlinePayments/onlinePaymentLabels";
import { goToStripePage, stripeDashboardUrl } from "@/features/onlinePayments/stripeRedirect";
import { OnlinePayments, OnlinePaymentStatus } from "@/features/onlinePayments/types";
import { TranslationKey, translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { TonalBlock } from "@/ui/TonalBlock";

const explanationKeys: Record<OnlinePaymentStatus, TranslationKey> = {
  NotConnected: "onlinePayments.explanation.notConnected",
  Pending: "onlinePayments.explanation.pending",
  Restricted: "onlinePayments.explanation.restricted",
  Active: "onlinePayments.explanation.active",
};

const onboardingLabelKeys: Record<Exclude<OnlinePaymentStatus, "Active">, TranslationKey> = {
  NotConnected: "onlinePayments.connect",
  Pending: "onlinePayments.continueOnboarding",
  Restricted: "onlinePayments.reviewInStripe",
};

interface OnlinePaymentStatusBlockProps {
  onlinePayments: OnlinePayments;
  isWeb: boolean;
  isOpeningStripe: boolean;
  onOpenOnboarding: () => void;
  onStopUsing: () => void;
}

export function OnlinePaymentStatusBlock({
  onlinePayments,
  isWeb,
  isOpeningStripe,
  onOpenOnboarding,
  onStopUsing,
}: OnlinePaymentStatusBlockProps) {
  const { status, account } = onlinePayments;
  const sharedBranchNames = account && account.branchNames.length > 1 ? account.branchNames : null;

  return (
    <TonalBlock tone={status === "Active" ? "success" : "muted"}>
      <AppText variant="eyebrow" tone="accent">
        {translate("onlinePayments.stripeAccount")}
      </AppText>
      <AppText variant="headline">{onlinePaymentStatusLabel(status)}</AppText>
      <AppText variant="body" tone="muted">
        {translate(explanationKeys[status])}
      </AppText>
      {sharedBranchNames ? (
        <AppText variant="body">
          {translate("onlinePayments.sharedWith", { branches: sharedBranchNames.join(", ") })}
        </AppText>
      ) : null}
      {isWeb && status !== "Active" ? (
        <Button
          variant={status === "Restricted" ? "secondary" : "primary"}
          size="medium"
          label={translate(onboardingLabelKeys[status])}
          isLoading={isOpeningStripe}
          onPress={onOpenOnboarding}
        />
      ) : null}
      {isWeb && status === "Active" ? (
        <Button
          variant="secondary"
          size="medium"
          label={translate("onlinePayments.openStripe")}
          onPress={() => goToStripePage(stripeDashboardUrl)}
        />
      ) : null}
      {status !== "NotConnected" ? (
        <Button
          variant="dangerGhost"
          size="medium"
          label={translate("onlinePayments.stopUsing")}
          onPress={onStopUsing}
        />
      ) : null}
    </TonalBlock>
  );
}
