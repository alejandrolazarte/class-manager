import { useLocalSearchParams } from "expo-router";
import { useEffect, useRef, useState } from "react";
import { Platform } from "react-native";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { BrandAccountList } from "@/features/onlinePayments/components/BrandAccountList";
import { OnlinePaymentStatusBlock } from "@/features/onlinePayments/components/OnlinePaymentStatusBlock";
import { OnlinePaymentAccount } from "@/features/onlinePayments/types";
import {
  useOnlinePaymentActions,
  useOnlinePayments,
} from "@/features/onlinePayments/useOnlinePayments";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonDetail } from "@/ui/motion/SkeletonDetail";
import { TonalBlock } from "@/ui/TonalBlock";

export const onboardingReturned = "return";
export const onboardingExpired = "refresh";

export function OnlinePaymentsScreen() {
  const { onboarding: onboardingReturn } = useLocalSearchParams<{ onboarding?: string }>();
  const onlinePayments = useOnlinePayments();
  const actions = useOnlinePaymentActions();
  const canShare = useCan(permissions.onlinePaymentsShare);
  const isWeb = Platform.OS === "web";
  const [isConfirmingStop, setIsConfirmingStop] = useState(false);
  const [accountToChoose, setAccountToChoose] = useState<OnlinePaymentAccount | null>(null);
  const hasRefreshedAfterOnboarding = useRef(false);
  const refreshAfterOnboarding = actions.refresh.mutate;

  useEffect(() => {
    if (onboardingReturn !== onboardingReturned || hasRefreshedAfterOnboarding.current) {
      return;
    }
    hasRefreshedAfterOnboarding.current = true;
    refreshAfterOnboarding();
  }, [onboardingReturn, refreshAfterOnboarding]);

  const header = <ScreenHeader navigation="back" title={translate("onlinePayments.title")} />;
  const data = onlinePayments.data;
  if (onlinePayments.isPending) {
    return (
      <ScrollScreen header={header}>
        <SkeletonDetail />
      </ScrollScreen>
    );
  }
  if (!data) {
    return (
      <ScrollScreen header={header}>
        <Banner message={translate("common.unexpectedError")} />
      </ScrollScreen>
    );
  }

  const stopUsingAccount = () =>
    actions.stopUsing.mutate(undefined, { onSettled: () => setIsConfirmingStop(false) });
  const chooseAccount = (account: OnlinePaymentAccount) =>
    actions.chooseAccount.mutate(account.id, { onSettled: () => setAccountToChoose(null) });

  return (
    <ScrollScreen header={header}>
      {onboardingReturn === onboardingReturned && actions.refresh.isPending ? (
        <Banner tone="info" message={translate("onlinePayments.checkingStripe")} />
      ) : null}
      {onboardingReturn === onboardingExpired ? (
        <Banner tone="warning" message={translate("onlinePayments.linkExpired")} />
      ) : null}
      {data.isAvailable ? (
        <OnlinePaymentStatusBlock
          onlinePayments={data}
          isWeb={isWeb}
          isOpeningStripe={actions.onboarding.isPending}
          onOpenOnboarding={() => actions.onboarding.mutate()}
          onStopUsing={() => setIsConfirmingStop(true)}
        />
      ) : (
        <TonalBlock tone="muted">
          <AppText variant="body" tone="muted">
            {translate("onlinePayments.notAvailable")}
          </AppText>
        </TonalBlock>
      )}
      {!isWeb && data.isAvailable ? (
        <AppText variant="body" tone="muted">
          {translate("onlinePayments.manageOnWeb")}
        </AppText>
      ) : null}
      {canShare && data.isAvailable && data.brandAccounts.length > 0 ? (
        <BrandAccountList
          accounts={data.brandAccounts}
          onChoose={setAccountToChoose}
          choosingAccountId={actions.chooseAccount.isPending ? (accountToChoose?.id ?? null) : null}
        />
      ) : null}
      {isConfirmingStop ? (
        <ConfirmDialog
          title={translate("onlinePayments.stopQuestion")}
          message={translate("onlinePayments.stopMessage")}
          confirmLabel={translate("onlinePayments.stopConfirm")}
          onCancel={() => setIsConfirmingStop(false)}
          onConfirm={stopUsingAccount}
          isConfirming={actions.stopUsing.isPending}
        />
      ) : null}
      {accountToChoose ? (
        <ConfirmDialog
          title={translate("onlinePayments.chooseQuestion", {
            branches: accountToChoose.branchNames.join(", "),
          })}
          message={translate("onlinePayments.chooseMessage")}
          confirmLabel={translate("onlinePayments.chooseConfirm")}
          onCancel={() => setAccountToChoose(null)}
          onConfirm={() => chooseAccount(accountToChoose)}
          isConfirming={actions.chooseAccount.isPending}
        />
      ) : null}
    </ScrollScreen>
  );
}
