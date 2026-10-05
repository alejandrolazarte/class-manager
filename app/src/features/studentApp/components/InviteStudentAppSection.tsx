import { useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { View } from "react-native";
import { clientQueryKeys } from "@/features/clients/clientQueryKeys";
import { ClientDetails, StudentAppAccessStatus } from "@/features/clients/types";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { emailAddressPattern } from "@/forms/emailAddress";
import { translate } from "@/i18n/translate";
import { AppText, TextTone } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Icon } from "@/ui/Icon";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

interface InviteStudentAppSectionProps {
  client: ClientDetails;
}

const statusTones: Record<StudentAppAccessStatus, TextTone> = {
  NotInvited: "muted",
  Invited: "warning",
  Active: "success",
};

export function InviteStudentAppSection({ client }: InviteStudentAppSectionProps) {
  const { showToast } = useToast();
  const queryClient = useQueryClient();
  const { status, invitedEmail } = client.appAccess;
  const emailOnFile = (invitedEmail ?? client.email ?? "").trim();
  const [isAskingEmail, setIsAskingEmail] = useState(false);
  const [email, setEmail] = useState(emailOnFile);
  const [emailError, setEmailError] = useState<string | undefined>();
  const [hasFailed, setHasFailed] = useState(false);
  const [isSending, setIsSending] = useState(false);

  const sendTo = async (recipientEmail: string) => {
    setHasFailed(false);
    if (!emailAddressPattern.test(recipientEmail)) {
      setIsAskingEmail(true);
      setEmailError(translate("student.invite.emailInvalid"));
      return;
    }
    setEmailError(undefined);
    setIsSending(true);
    try {
      await inviteStudentApp(client.id, { email: recipientEmail });
      showToast(translate("student.invite.sent"));
      setIsAskingEmail(false);
      await queryClient.invalidateQueries({ queryKey: clientQueryKeys.detail(client.id) });
    } catch {
      setHasFailed(true);
    } finally {
      setIsSending(false);
    }
  };

  const invite = () => {
    if (emailOnFile.length === 0) {
      setIsAskingEmail(true);
      return;
    }
    void sendTo(emailOnFile);
  };

  return (
    <Card className="gap-3 p-3.5">
      <View className="flex-row items-center gap-3">
        <View className="h-10 w-10 items-center justify-center rounded-xl bg-primary-soft">
          <Icon name="studentApp" size="medium" tone="primary" />
        </View>
        <View className="min-w-0 flex-1 gap-0.5">
          <AppText variant="bodyStrong">{translate("student.app.title")}</AppText>
          <AppText variant="caption" tone={statusTones[status]}>
            {translate(`student.app.${status}`, { email: invitedEmail ?? "" })}
          </AppText>
        </View>
        {status === "Active" || isAskingEmail ? null : (
          <Button
            size="medium"
            label={translate(status === "Invited" ? "student.app.resend" : "student.app.invite")}
            accessibilityLabel={translate(
              status === "Invited"
                ? "student.app.resendAccessibility"
                : "student.app.inviteAccessibility",
            )}
            onPress={invite}
            isLoading={isSending}
          />
        )}
      </View>
      {hasFailed ? <Banner message={translate("common.unexpectedError")} /> : null}
      {isAskingEmail ? (
        <View className="gap-2.5 rounded-2xl bg-muted p-3">
          <AppText variant="caption" tone="subtle">
            {translate("student.invite.hint")}
          </AppText>
          <TextField
            label={translate("student.invite.email")}
            isRequired
            keyboardType="email-address"
            autoCapitalize="none"
            autoComplete="email"
            fieldSurface="background"
            value={email}
            onChangeText={setEmail}
            errorMessage={emailError}
          />
          <Button
            size="medium"
            label={translate("student.invite.send")}
            onPress={() => sendTo(email.trim())}
            disabled={email.trim().length === 0}
            isLoading={isSending}
          />
        </View>
      ) : null}
    </Card>
  );
}
