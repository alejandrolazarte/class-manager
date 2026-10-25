import { useQueryClient } from "@tanstack/react-query";
import { useRouter } from "expo-router";
import { useState } from "react";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { passkeyErrorCodes } from "@/features/passkeys/passkeyErrorCodes";
import {
  isPasskeyAlreadyOnDevice,
  isPasskeyPromptCancelled,
} from "@/features/passkeys/passkeyErrors";
import {
  beginPasskeyRegistration,
  finishPasskeyRegistration,
} from "@/features/passkeys/passkeysApi";
import { passkeyQueryKeys } from "@/features/passkeys/passkeyQueryKeys";
import { createPasskeyCredential, suggestedPasskeyName } from "@/features/passkeys/webAuthn";
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
import { useToast } from "@/ui/ToastProvider";

const passkeyNameMaxLength = 60;

type AddPasskeyFailure = "alreadyOnDevice" | "notVerified" | "network" | "unexpected";

export function AddPasskeyScreen() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const [currentPassword, setCurrentPassword] = useState("");
  const [name, setName] = useState(suggestedPasskeyName);
  const [currentPasswordError, setCurrentPasswordError] = useState<string | undefined>();
  const [failure, setFailure] = useState<AddPasskeyFailure | null>(null);
  const [isAdding, setIsAdding] = useState(false);
  const areRequiredFieldsFilled = isFilled(currentPassword) && isFilled(name);

  const handleAddError = (addError: unknown) => {
    if (isPasskeyPromptCancelled(addError)) {
      return;
    }
    if (isPasskeyAlreadyOnDevice(addError)) {
      setFailure("alreadyOnDevice");
      return;
    }
    if (isNetworkError(addError)) {
      setFailure("network");
      return;
    }
    if (isApiError(addError) && addError.hasCode(passkeyErrorCodes.invalidCurrentPassword)) {
      setCurrentPasswordError(translate("profile.email.wrongPassword"));
      return;
    }
    if (
      isApiError(addError) &&
      (addError.hasCode(passkeyErrorCodes.invalidPasskey) ||
        addError.hasCode(passkeyErrorCodes.invalidPasskeyCeremony))
    ) {
      setFailure("notVerified");
      return;
    }
    setFailure("unexpected");
  };

  const addPasskey = async () => {
    setFailure(null);
    setCurrentPasswordError(undefined);
    setIsAdding(true);
    try {
      const ceremony = await beginPasskeyRegistration(currentPassword);
      const credential = await createPasskeyCredential(ceremony.options);
      await finishPasskeyRegistration({
        ceremonyId: ceremony.ceremonyId,
        credential,
        name: name.trim(),
      });
      await queryClient.invalidateQueries({ queryKey: passkeyQueryKeys.mine });
      showToast(translate("profile.passkeys.added"));
      router.back();
    } catch (addError) {
      handleAddError(addError);
    } finally {
      setIsAdding(false);
    }
  };

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="close" title={translate("profile.passkeys.add")} />}
      footer={
        <ScreenFooter>
          <Button
            label={translate("profile.passkeys.add")}
            onPress={addPasskey}
            disabled={!areRequiredFieldsFilled}
            isLoading={isAdding}
          />
        </ScreenFooter>
      }
    >
      <AppText variant="body" tone="subtle">
        {translate("profile.passkeys.addHint")}
      </AppText>
      {failure === "alreadyOnDevice" ? (
        <Banner tone="warning" message={translate("profile.passkeys.alreadyOnDevice")} />
      ) : null}
      {failure === "notVerified" ? (
        <Banner message={translate("profile.passkeys.notVerified")} />
      ) : null}
      {failure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={addPasskey} />
        </Banner>
      ) : null}
      {failure === "unexpected" ? <Banner message={translate("common.unexpectedError")} /> : null}
      <RequiredFieldsLegend />
      <PasswordField
        label={translate("profile.email.currentPassword")}
        isRequired
        autoComplete="current-password"
        value={currentPassword}
        onChangeText={setCurrentPassword}
        errorMessage={currentPasswordError}
      />
      <TextField
        label={translate("profile.passkeys.name")}
        isRequired
        placeholder={translate("profile.passkeys.namePlaceholder")}
        maxLength={passkeyNameMaxLength}
        value={name}
        onChangeText={setName}
        onSubmitEditing={addPasskey}
      />
    </ScrollScreen>
  );
}
