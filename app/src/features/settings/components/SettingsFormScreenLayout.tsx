import { PropsWithChildren } from "react";
import { SubmissionFailure } from "@/features/settings/submissionFailure";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";

interface SettingsFormScreenLayoutProps extends PropsWithChildren {
  title: string;
  eyebrow?: string;
  subtitle?: string;
  submissionFailure: SubmissionFailure | null;
  onRetry: () => void;
}

export function SettingsFormScreenLayout({
  title,
  eyebrow,
  subtitle,
  submissionFailure,
  onRetry,
  children,
}: SettingsFormScreenLayoutProps) {
  return (
    <ScrollScreen
      header={
        <ScreenHeader navigation="close" title={title} eyebrow={eyebrow} subtitle={subtitle} />
      }
    >
      {submissionFailure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button
            variant="secondary"
            size="medium"
            label={translate("common.retry")}
            onPress={onRetry}
          />
        </Banner>
      ) : null}
      {submissionFailure === "unexpected" ? (
        <Banner message={translate("common.unexpectedError")} />
      ) : null}
      {children}
    </ScrollScreen>
  );
}
