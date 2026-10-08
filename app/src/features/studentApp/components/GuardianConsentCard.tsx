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
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
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
    <Card className="gap-3 p-4">
      <View className="flex-row items-center gap-3">
        <View className="h-10 w-10 items-center justify-center rounded-xl bg-warning-soft">
          <Icon name="studentApp" size="medium" tone="warning-soft-foreground" />
        </View>
        <AppText variant="bodyStrong" className="min-w-0 flex-1">
          {translate("guardianConsent.cardTitle", { name: firstName })}
        </AppText>
      </View>
      <AppText variant="body" tone="muted">
        {translate("guardianConsent.details", {
          name: consent.studentFullName,
          email: consent.studentEmail,
        })}
      </AppText>
      {hasFailed ? <Banner message={translate("common.unexpectedError")} /> : null}
      <Button
        label={translate("guardianConsent.authorize")}
        onPress={() => void respond("authorized")}
        disabled={pendingAnswer === "refused"}
        isLoading={pendingAnswer === "authorized"}
      />
      <Button
        variant="outline"
        label={translate("guardianConsent.refuse")}
        onPress={() => void respond("refused")}
        disabled={pendingAnswer === "authorized"}
        isLoading={pendingAnswer === "refused"}
      />
    </Card>
  );
}
