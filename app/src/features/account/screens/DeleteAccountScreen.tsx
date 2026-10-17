import { useState } from "react";
import { isApiError, isNetworkError } from "@/api/httpClient";
import { deleteMyAccount } from "@/features/account/accountApi";
import { accountErrorCodes } from "@/features/account/accountErrorCodes";
import { useSession } from "@/features/authentication/useSession";
import { isFilled } from "@/forms/requiredFields";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { ConfirmDialog } from "@/ui/ConfirmDialog";
import { PasswordField } from "@/ui/PasswordField";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";
import { ScreenFooter, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { useToast } from "@/ui/ToastProvider";

type AccountDeletionFailure = "network" | "lastBrandOwner" | "unexpected";

export function DeleteAccountScreen() {
  const { signOut } = useSession();
  const { showToast } = useToast();
  const [currentPassword, setCurrentPassword] = useState("");
  const [currentPasswordError, setCurrentPasswordError] = useState<string | undefined>();
  const [failure, setFailure] = useState<AccountDeletionFailure | null>(null);
  const [isConfirming, setIsConfirming] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);

  const handleDeletionError = (deletionError: unknown) => {
    if (isNetworkError(deletionError)) {
      setFailure("network");
      return;
    }
    if (
      isApiError(deletionError) &&
      deletionError.hasCode(accountErrorCodes.invalidCurrentPassword)
    ) {
      setCurrentPasswordError(translate("profile.delete.wrongPassword"));
      return;
    }
    if (isApiError(deletionError) && deletionError.hasCode(accountErrorCodes.lastBrandOwner)) {
      setFailure("lastBrandOwner");
      return;
    }
    setFailure("unexpected");
  };

  const askToConfirm = () => {
    setFailure(null);
    setCurrentPasswordError(undefined);
    setIsConfirming(true);
  };

  const deleteAccount = async () => {
    setIsDeleting(true);
    try {
      await deleteMyAccount({ currentPassword });
      showToast(translate("profile.delete.done"));
      await signOut();
    } catch (deletionError) {
      setIsConfirming(false);
      handleDeletionError(deletionError);
    } finally {
      setIsDeleting(false);
    }
  };

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="close" title={translate("profile.delete.title")} />}
      footer={
        <ScreenFooter>
          <Button
            variant="dangerOutline"
            label={translate("profile.delete.submit")}
            onPress={askToConfirm}
            disabled={!isFilled(currentPassword)}
          />
        </ScreenFooter>
      }
    >
      <AppText variant="body" tone="subtle">
        {translate("profile.delete.hint")}
      </AppText>
      {failure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={askToConfirm} />
        </Banner>
      ) : null}
      {failure === "lastBrandOwner" ? (
        <Banner message={translate("profile.delete.lastBrandOwner")} />
      ) : null}
      {failure === "unexpected" ? <Banner message={translate("common.unexpectedError")} /> : null}
      <RequiredFieldsLegend />
      <PasswordField
        label={translate("profile.delete.currentPassword")}
        isRequired
        autoComplete="current-password"
        value={currentPassword}
        onChangeText={setCurrentPassword}
        errorMessage={currentPasswordError}
      />
      {isConfirming ? (
        <ConfirmDialog
          title={translate("profile.delete.question")}
          message={translate("profile.delete.consequence")}
          confirmLabel={translate("profile.delete.confirm")}
          onCancel={() => setIsConfirming(false)}
          onConfirm={deleteAccount}
          isConfirming={isDeleting}
        />
      ) : null}
    </ScrollScreen>
  );
}
