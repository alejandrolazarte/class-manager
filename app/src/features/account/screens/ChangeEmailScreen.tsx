import { useState } from "react";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { requestEmailChange } from "@/features/account/accountApi";
import { accountErrorCodes } from "@/features/account/accountErrorCodes";
import { useMyAccount } from "@/features/account/useMyAccount";
import { emailAddressPattern } from "@/forms/emailAddress";
import { isFilled } from "@/forms/requiredFields";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { PasswordField } from "@/ui/PasswordField";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";
import { ScreenFooter, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { TextField } from "@/ui/TextField";

type EmailChangeFailure = "network" | "unexpected";

export function ChangeEmailScreen() {
  const accountQuery = useMyAccount();
  const [newEmail, setNewEmail] = useState("");
  const [currentPassword, setCurrentPassword] = useState("");
  const [newEmailError, setNewEmailError] = useState<string | undefined>();
  const [currentPasswordError, setCurrentPasswordError] = useState<string | undefined>();
  const [failure, setFailure] = useState<EmailChangeFailure | null>(null);
  const [sentTo, setSentTo] = useState<string | null>(null);
  const [isSending, setIsSending] = useState(false);
  const areRequiredFieldsFilled = isFilled(newEmail) && isFilled(currentPassword);

  const handleRequestError = (requestError: unknown) => {
    if (isNetworkError(requestError)) {
      setFailure("network");
      return;
    }
    if (
      isApiError(requestError) &&
      requestError.hasCode(accountErrorCodes.invalidCurrentPassword)
    ) {
      setCurrentPasswordError(translate("profile.email.wrongPassword"));
      return;
    }
    if (isApiError(requestError) && requestError.hasCode(accountErrorCodes.emailTaken)) {
      setNewEmailError(translate("profile.email.taken"));
      return;
    }
    if (isApiError(requestError) && requestError.hasCode(accountErrorCodes.sameEmail)) {
      setNewEmailError(translate("profile.email.same"));
      return;
    }
    setFailure("unexpected");
  };

  const sendLink = async () => {
    const trimmedEmail = newEmail.trim();
    setFailure(null);
    setCurrentPasswordError(undefined);
    if (!emailAddressPattern.test(trimmedEmail)) {
      setNewEmailError(translate("profile.email.invalid"));
      return;
    }
    setNewEmailError(undefined);
    setIsSending(true);
    try {
      await requestEmailChange({ newEmail: trimmedEmail, currentPassword });
      setSentTo(trimmedEmail);
      setNewEmail("");
      setCurrentPassword("");
    } catch (requestError) {
      handleRequestError(requestError);
    } finally {
      setIsSending(false);
    }
  };

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="close" title={translate("profile.email.title")} />}
      footer={
        <ScreenFooter>
          <Button
            label={translate("profile.email.submit")}
            onPress={sendLink}
            disabled={!areRequiredFieldsFilled}
            isLoading={isSending}
          />
        </ScreenFooter>
      }
    >
      <AppText variant="body" tone="subtle">
        {translate("profile.email.hint")}
      </AppText>
      {accountQuery.data ? (
        <AppText variant="caption" tone="muted">
          {translate("profile.signsInWith", { email: accountQuery.data.email })}
        </AppText>
      ) : null}
      {sentTo ? (
        <Banner tone="success" message={translate("profile.email.sent", { email: sentTo })} />
      ) : null}
      {failure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={sendLink} />
        </Banner>
      ) : null}
      {failure === "unexpected" ? <Banner message={translate("common.unexpectedError")} /> : null}
      <RequiredFieldsLegend />
      <TextField
        label={translate("profile.email.newEmail")}
        isRequired
        keyboardType="email-address"
        autoCapitalize="none"
        autoComplete="email"
        value={newEmail}
        onChangeText={setNewEmail}
        errorMessage={newEmailError}
      />
      <PasswordField
        label={translate("profile.email.currentPassword")}
        isRequired
        autoComplete="current-password"
        value={currentPassword}
        onChangeText={setCurrentPassword}
        onSubmitEditing={sendLink}
        errorMessage={currentPasswordError}
      />
    </ScrollScreen>
  );
}
