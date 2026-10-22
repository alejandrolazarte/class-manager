import { useMutation } from "@tanstack/react-query";
import Constants from "expo-constants";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Platform } from "react-native";
import { isNetworkError } from "@/api/httpClient";
import { sendAppFeedback } from "@/features/account/accountApi";
import { SendAppFeedbackRequest } from "@/features/account/types";
import { isFilled } from "@/forms/requiredFields";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";
import { ScreenFooter, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const messageMaxLength = 4000;

type SendFailure = "network" | "unexpected";

export function AppFeedbackScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const [message, setMessage] = useState("");
  const [failure, setFailure] = useState<SendFailure | null>(null);
  const feedbackMutation = useMutation({
    mutationFn: (request: SendAppFeedbackRequest) => sendAppFeedback(request),
  });

  const send = async () => {
    setFailure(null);
    try {
      await feedbackMutation.mutateAsync({
        message: message.trim(),
        platform: Platform.OS,
        appVersion: Constants.expoConfig?.version ?? null,
      });
    } catch (sendError) {
      setFailure(isNetworkError(sendError) ? "network" : "unexpected");
      return;
    }
    showToast(translate("appFeedback.sent"));
    router.back();
  };

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="close" title={translate("appFeedback.title")} />}
      footer={
        <ScreenFooter>
          <Button
            label={translate("appFeedback.submit")}
            onPress={send}
            disabled={!isFilled(message)}
            isLoading={feedbackMutation.isPending}
          />
        </ScreenFooter>
      }
    >
      {failure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={send} />
        </Banner>
      ) : null}
      {failure === "unexpected" ? <Banner message={translate("common.unexpectedError")} /> : null}
      <RequiredFieldsLegend />
      <AppText variant="body" tone="muted">
        {translate("appFeedback.hint")}
      </AppText>
      <TextField
        label={translate("appFeedback.message")}
        isRequired
        multiline
        maxLength={messageMaxLength}
        value={message}
        onChangeText={setMessage}
      />
    </ScrollScreen>
  );
}
