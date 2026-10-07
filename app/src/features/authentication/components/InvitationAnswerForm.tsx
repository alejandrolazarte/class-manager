import { useState } from "react";
import { CheckedInvitation } from "@/features/authentication/types";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { PasswordField } from "@/ui/PasswordField";
import { TextField } from "@/ui/TextField";

export interface InvitationAnswerTexts {
  newAccountSubtitle: string;
  existingAccount: string;
  fullName: string;
  password: string;
  passwordHint: string;
  createAccount: string;
}

export interface InvitationFieldErrors {
  fullName?: string;
  password?: string;
}

interface InvitationAnswerFormProps {
  invitation: CheckedInvitation;
  texts: InvitationAnswerTexts;
  fieldErrors: InvitationFieldErrors;
  isAccepting: boolean;
  isDeclining: boolean;
  onAccept: (fullName: string, password: string) => void;
  onDecline: () => void;
}

export function InvitationAnswerForm({
  invitation,
  texts,
  fieldErrors,
  isAccepting,
  isDeclining,
  onAccept,
  onDecline,
}: InvitationAnswerFormProps) {
  const [fullName, setFullName] = useState("");
  const [password, setPassword] = useState("");
  const accept = () => onAccept(fullName.trim(), password);
  const isFilled = invitation.hasAccount || (password.length > 0 && fullName.trim().length > 0);

  return (
    <>
      {invitation.hasAccount ? (
        <AppText variant="body" tone="muted">
          {texts.existingAccount}
        </AppText>
      ) : (
        <>
          <AppText variant="body" tone="muted">
            {texts.newAccountSubtitle}
          </AppText>
          <TextField
            label={texts.fullName}
            autoCapitalize="words"
            autoComplete="name"
            value={fullName}
            onChangeText={setFullName}
            errorMessage={fieldErrors.fullName}
          />
          <PasswordField
            label={texts.password}
            autoComplete="new-password"
            value={password}
            onChangeText={setPassword}
            onSubmitEditing={accept}
            errorMessage={fieldErrors.password}
          />
          <AppText variant="caption" tone="subtle">
            {texts.passwordHint}
          </AppText>
        </>
      )}
      <Button
        label={invitation.hasAccount ? translate("invitationAnswer.accept") : texts.createAccount}
        onPress={accept}
        disabled={!isFilled || isDeclining}
        isLoading={isAccepting}
      />
      <Button
        variant="outline"
        label={translate("invitationAnswer.decline")}
        onPress={onDecline}
        disabled={isAccepting}
        isLoading={isDeclining}
      />
    </>
  );
}
