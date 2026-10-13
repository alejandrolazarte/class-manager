import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { acceptLegalTerms } from "@/features/account/accountApi";
import { accountQueryKeys } from "@/features/account/accountQueryKeys";
import { MyAccount } from "@/features/account/types";
import { useSession } from "@/features/authentication/useSession";
import { LegalTermsConsent } from "@/features/legal/components/LegalTermsConsent";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

export function LegalTermsUpdateScreen() {
  const queryClient = useQueryClient();
  const { signOut } = useSession();
  const [isAccepted, setIsAccepted] = useState(false);
  const acceptance = useMutation({
    mutationFn: acceptLegalTerms,
    onSuccess: () =>
      queryClient.setQueryData<MyAccount>(accountQueryKeys.myAccount, (account) =>
        account ? { ...account, hasAcceptedCurrentLegalTerms: true } : account,
      ),
  });

  return (
    <ScrollScreen header={<ScreenHeader title={translate("legal.update.title")} />}>
      {acceptance.isError ? <Banner message={translate("common.unexpectedError")} /> : null}
      <AppText variant="lead" tone="muted">
        {translate("legal.update.message")}
      </AppText>
      <LegalTermsConsent isAccepted={isAccepted} onAcceptedChange={setIsAccepted} />
      <Button
        label={translate("legal.update.submit")}
        onPress={() => acceptance.mutate()}
        disabled={!isAccepted}
        isLoading={acceptance.isPending}
      />
      <Button
        variant="ghost"
        label={translate("legal.update.signOut")}
        onPress={() => void signOut()}
        disabled={acceptance.isPending}
      />
    </ScrollScreen>
  );
}
