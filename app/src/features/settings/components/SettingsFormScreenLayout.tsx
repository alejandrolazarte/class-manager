import { PropsWithChildren } from "react";
import { KeyboardAvoidingView, Platform, ScrollView } from "react-native";
import { SubmissionFailure } from "@/features/settings/submissionFailure";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";

interface SettingsFormScreenLayoutProps extends PropsWithChildren {
  submissionFailure: SubmissionFailure | null;
  onRetry: () => void;
}

export function SettingsFormScreenLayout({
  submissionFailure,
  onRetry,
  children,
}: SettingsFormScreenLayoutProps) {
  return (
    <KeyboardAvoidingView
      className="flex-1 bg-gray-50"
      behavior={Platform.OS === "ios" ? "padding" : undefined}
    >
      <ScrollView contentContainerClassName="gap-4 p-4 pb-12" keyboardShouldPersistTaps="handled">
        {submissionFailure === "network" ? (
          <Banner message={translate("common.networkError")}>
            <Button variant="secondary" label={translate("common.retry")} onPress={onRetry} />
          </Banner>
        ) : null}
        {submissionFailure === "unexpected" ? (
          <Banner message={translate("common.unexpectedError")} />
        ) : null}
        {children}
      </ScrollView>
    </KeyboardAvoidingView>
  );
}
