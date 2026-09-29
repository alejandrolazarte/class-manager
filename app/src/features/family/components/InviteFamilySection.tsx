import { useState } from "react";
import { View } from "react-native";
import { ClientDetails } from "@/features/clients/types";
import { inviteFamily } from "@/features/family/familyApi";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

interface InviteFamilySectionProps {
  client: ClientDetails;
}

const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export function InviteFamilySection({ client }: InviteFamilySectionProps) {
  const { showToast } = useToast();
  const [isOpen, setIsOpen] = useState(false);
  const [email, setEmail] = useState(client.email ?? "");
  const [emailError, setEmailError] = useState<string | undefined>();
  const [hasFailed, setHasFailed] = useState(false);
  const [isSending, setIsSending] = useState(false);

  const send = async () => {
    setHasFailed(false);
    const trimmedEmail = email.trim();
    if (!emailPattern.test(trimmedEmail)) {
      setEmailError(translate("family.invite.emailInvalid"));
      return;
    }
    setEmailError(undefined);
    setIsSending(true);
    try {
      await inviteFamily(client.id, { email: trimmedEmail });
      showToast(translate("family.invite.sent"));
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
        label={translate("family.invite.open")}
        onPress={() => setIsOpen(true)}
      />
    );
  }

  return (
    <View className="gap-2.5 rounded-2xl bg-muted p-3">
      {hasFailed ? <Banner message={translate("common.unexpectedError")} /> : null}
      <AppText variant="caption" tone="subtle">
        {translate("family.invite.hint")}
      </AppText>
      <TextField
        label={translate("family.invite.email")}
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
        label={translate("family.invite.send")}
        onPress={send}
        isLoading={isSending}
      />
    </View>
  );
}
