import { useState } from "react";
import { View } from "react-native";
import { ClientDetails } from "@/features/clients/types";
import { inviteStudentApp } from "@/features/studentApp/studentAppApi";
import { emailAddressPattern } from "@/forms/emailAddress";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

interface InviteStudentAppSectionProps {
  client: ClientDetails;
}

export function InviteStudentAppSection({ client }: InviteStudentAppSectionProps) {
  const { showToast } = useToast();
  const [isOpen, setIsOpen] = useState(false);
  const [email, setEmail] = useState(client.email ?? "");
  const [emailError, setEmailError] = useState<string | undefined>();
  const [hasFailed, setHasFailed] = useState(false);
  const [isSending, setIsSending] = useState(false);

  const send = async () => {
    setHasFailed(false);
    const trimmedEmail = email.trim();
    if (!emailAddressPattern.test(trimmedEmail)) {
      setEmailError(translate("student.invite.emailInvalid"));
      return;
    }
    setEmailError(undefined);
    setIsSending(true);
    try {
      await inviteStudentApp(client.id, { email: trimmedEmail });
      showToast(translate("student.invite.sent"));
      setIsOpen(false);
    } catch {
      setHasFailed(true);
    } finally {
      setIsSending(false);
    }
  };

  if (!isOpen) {
    return (
      <Button
        variant="secondary"
        size="medium"
        icon="enroll"
        label={translate("student.invite.open")}
        onPress={() => setIsOpen(true)}
      />
    );
  }

  return (
    <View className="gap-2.5 rounded-2xl bg-muted p-3">
      {hasFailed ? <Banner message={translate("common.unexpectedError")} /> : null}
      <AppText variant="caption" tone="subtle">
        {translate("student.invite.hint")}
      </AppText>
      <TextField
        label={translate("student.invite.email")}
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
        onPress={send}
        isLoading={isSending}
      />
    </View>
  );
}
