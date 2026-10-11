import { useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { View } from "react-native";
import { studentAppQueryKeys } from "@/features/studentApp/studentAppQueryKeys";
import {
  giveGuardianConsentInApp,
  refuseGuardianConsentInApp,
} from "@/features/studentApp/studentAppApi";
import { firstNameOf } from "@/features/studentApp/studentAppSchedule";
import { StudentAppGuardianConsent } from "@/features/studentApp/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { TonalBlock } from "@/ui/TonalBlock";
import { useToast } from "@/ui/ToastProvider";

type GuardianAnswer = "authorized" | "refused";

interface GuardianConsentCardProps {
  consent: StudentAppGuardianConsent;
}

export function GuardianConsentCard({ consent }: GuardianConsentCardProps) {
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const [pendingAnswer, setPendingAnswer] = useState<GuardianAnswer | null>(null);
  const [hasFailed, setHasFailed] = useState(false);
  const firstName = firstNameOf(consent.studentFullName);

  const respond = async (answer: GuardianAnswer) => {
    setHasFailed(false);
    setPendingAnswer(answer);
    try {
      await (answer === "authorized"
        ? giveGuardianConsentInApp(consent.invitationId)
        : refuseGuardianConsentInApp(consent.invitationId));
      showToast(
        answer === "authorized"
          ? translate("guardianConsent.authorized", { email: consent.studentEmail })
          : translate("guardianConsent.refusedInApp", { name: firstName }),
      );
      await queryClient.invalidateQueries({ queryKey: studentAppQueryKeys.home() });
    } catch {
      setHasFailed(true);
    } finally {
      setPendingAnswer(null);
    }
  };

  return (
    <TonalBlock tone="warning">
      <AppText variant="bodyStrong">
        {translate("guardianConsent.cardTitle", { name: firstName })}
      </AppText>
      <AppText variant="caption" tone="warningSoft">
        {translate("guardianConsent.details", {
          name: consent.studentFullName,
          email: consent.studentEmail,
        })}
      </AppText>
      {hasFailed ? <Banner message={translate("common.unexpectedError")} /> : null}
      <View className="flex-row gap-2">
        <View className="flex-1">
          <Button
            size="medium"
            label={translate("guardianConsent.authorize")}
            onPress={() => void respond("authorized")}
            disabled={pendingAnswer === "refused"}
            isLoading={pendingAnswer === "authorized"}
          />
        </View>
        <View className="flex-1">
          <Button
            size="medium"
            variant="outline"
            label={translate("guardianConsent.refuse")}
            onPress={() => void respond("refused")}
            disabled={pendingAnswer === "authorized"}
            isLoading={pendingAnswer === "refused"}
          />
        </View>
      </View>
    </TonalBlock>
  );
}
