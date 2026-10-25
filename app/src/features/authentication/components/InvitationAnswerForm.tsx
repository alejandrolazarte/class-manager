import { useState } from "react";
import { CheckedInvitation } from "@/features/authentication/types";
import {
  formatBirthDateForDisplay,
  isValidBirthDate,
  parseBirthDate,
} from "@/dates/birthDateFormatting";
import { LegalTermsConsent } from "@/features/legal/components/LegalTermsConsent";
import { BirthDateField } from "@/forms/BirthDateField";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { PasswordField } from "@/ui/PasswordField";
import { PasswordManagerUsername } from "@/ui/PasswordManagerUsername";
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
  birthDate?: string;
}

interface InvitationAnswerFormProps {
  invitation: CheckedInvitation;
  texts: InvitationAnswerTexts;
  fieldErrors: InvitationFieldErrors;
  isAccepting: boolean;
  isDeclining: boolean;
  onAccept: (
    fullName: string,
    password: string,
    birthDate?: string,
    acceptsLegalTerms?: boolean,
  ) => void;
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
  const knownFullName = invitation.fullName ?? null;
  const [fullName, setFullName] = useState("");
  const [birthDate, setBirthDate] = useState(
    invitation.birthDate ? formatBirthDateForDisplay(invitation.birthDate) : "",
  );
  const [birthDateError, setBirthDateError] = useState<string | undefined>();
  const [password, setPassword] = useState("");
  const [passwordConfirmation, setPasswordConfirmation] = useState("");
  const [confirmationError, setConfirmationError] = useState<string | undefined>();
  const [acceptsLegalTerms, setAcceptsLegalTerms] = useState(false);
  const isFilled =
    invitation.hasAccount ||
    (password.length > 0 &&
      passwordConfirmation.length > 0 &&
      birthDate.length > 0 &&
      acceptsLegalTerms &&
      (knownFullName !== null || fullName.trim().length > 0));

  const accept = () => {
    if (invitation.hasAccount) {
      onAccept("", "");
      return;
    }
    const hasValidBirthDate = isValidBirthDate(birthDate);
    const passwordsMatch = password === passwordConfirmation;
    setBirthDateError(hasValidBirthDate ? undefined : translate("birthDate.invalid"));
    setConfirmationError(
      passwordsMatch ? undefined : translate("invitationAnswer.passwordsDoNotMatch"),
    );
    if (!hasValidBirthDate || !passwordsMatch) {
      return;
    }
    onAccept(
      knownFullName ?? fullName.trim(),
      password,
      parseBirthDate(birthDate) ?? undefined,
      acceptsLegalTerms,
    );
  };

  return (
    <>
      {invitation.hasAccount ? (
        <AppText variant="body" tone="muted">
          {texts.existingAccount}
        </AppText>
      ) : (
        <>
          <AppText variant="body" tone="muted">
            {knownFullName
              ? translate("invitationAnswer.greeting", { name: knownFullName })
              : texts.newAccountSubtitle}
          </AppText>
          {knownFullName ? null : (
            <TextField
              label={texts.fullName}
              autoCapitalize="words"
              autoComplete="name"
              value={fullName}
              onChangeText={setFullName}
              errorMessage={fieldErrors.fullName}
            />
          )}
          <BirthDateField
            hint={
              invitation.birthDate
                ? translate("invitationAnswer.birthDateFromTeam", {
                    business: invitation.businessName,
                  })
                : translate("invitationAnswer.birthDateHint")
            }
            value={birthDate}
            onChangeText={setBirthDate}
            errorMessage={birthDateError ?? fieldErrors.birthDate}
          />
          <PasswordManagerUsername email={invitation.email} />
          <PasswordField
            label={texts.password}
            autoComplete="new-password"
            value={password}
            onChangeText={setPassword}
            errorMessage={fieldErrors.password}
          />
          <PasswordField
            label={translate("invitationAnswer.passwordConfirmation")}
            autoComplete="new-password"
            value={passwordConfirmation}
            onChangeText={setPasswordConfirmation}
            onSubmitEditing={accept}
            errorMessage={confirmationError}
          />
          <AppText variant="caption" tone="subtle">
            {knownFullName ? translate("invitationAnswer.nameHint") : texts.passwordHint}
          </AppText>
          <LegalTermsConsent
            isAccepted={acceptsLegalTerms}
            onAcceptedChange={setAcceptsLegalTerms}
          />
        </>
      )}
      <Button
        label={invitation.hasAccount ? translate("invitationAnswer.accept") : texts.createAccount}
        onPress={accept}
        disabled={!isFilled || isDeclining}
        isLoading={isAccepting}
      />
      {invitation.hasAccount ? (
        <Button
          variant="secondary"
          label={translate("invitationAnswer.decline")}
          onPress={onDecline}
          disabled={isAccepting}
          isLoading={isDeclining}
        />
      ) : null}
    </>
  );
}
