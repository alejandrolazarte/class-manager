import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useRouter } from "expo-router";
import { useState } from "react";
import { isNetworkError } from "@/api/httpClient";
import { updateMyProfile } from "@/features/account/accountApi";
import { accountQueryKeys } from "@/features/account/accountQueryKeys";
import { MyAccount, UpdateMyProfileRequest } from "@/features/account/types";
import { useMyAccount } from "@/features/account/useMyAccount";
import { BirthDateField } from "@/forms/BirthDateField";
import { isFilled } from "@/forms/requiredFields";
import {
  formatBirthDateForDisplay,
  isValidBirthDate,
  parseBirthDate,
} from "@/features/students/birthDateFormatting";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";
import { ScreenFooter, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { Spinner } from "@/ui/Spinner";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const fullNameMinimumLength = 2;

type SaveFailure = "network" | "unexpected";

export function EditMyProfileScreen() {
  const accountQuery = useMyAccount();
  if (accountQuery.data) {
    return <EditMyProfileForm account={accountQuery.data} />;
  }
  return (
    <ScrollScreen
      header={<ScreenHeader navigation="close" title={translate("profile.edit.title")} />}
    >
      {accountQuery.isError ? (
        <Banner message={translate("common.unexpectedError")}>
          <Button
            variant="secondary"
            label={translate("common.retry")}
            onPress={() => accountQuery.refetch()}
          />
        </Banner>
      ) : (
        <Spinner className="mt-6" />
      )}
    </ScrollScreen>
  );
}

interface EditMyProfileFormProps {
  account: MyAccount;
}

function EditMyProfileForm({ account }: EditMyProfileFormProps) {
  const router = useRouter();
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const [fullName, setFullName] = useState(account.fullName);
  const [fullNameError, setFullNameError] = useState<string | undefined>();
  const [birthDate, setBirthDate] = useState(
    account.birthDate ? formatBirthDateForDisplay(account.birthDate) : "",
  );
  const [birthDateError, setBirthDateError] = useState<string | undefined>();
  const [failure, setFailure] = useState<SaveFailure | null>(null);
  const profileMutation = useMutation({
    mutationFn: (request: UpdateMyProfileRequest) => updateMyProfile(request),
  });

  const save = async () => {
    const trimmedFullName = fullName.trim();
    setFailure(null);
    const isoBirthDate = isValidBirthDate(birthDate) ? parseBirthDate(birthDate) : null;
    const isFullNameLongEnough = trimmedFullName.length >= fullNameMinimumLength;
    setFullNameError(isFullNameLongEnough ? undefined : translate("profile.name.tooShort"));
    setBirthDateError(isoBirthDate ? undefined : translate("birthDate.invalid"));
    if (!isFullNameLongEnough || !isoBirthDate) {
      return;
    }
    try {
      const updatedAccount = await profileMutation.mutateAsync({
        fullName: trimmedFullName,
        birthDate: isoBirthDate,
      });
      queryClient.setQueryData(accountQueryKeys.myAccount, updatedAccount);
    } catch (saveError) {
      setFailure(isNetworkError(saveError) ? "network" : "unexpected");
      return;
    }
    showToast(translate("profile.edit.saved"));
    router.back();
  };

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="close" title={translate("profile.edit.title")} />}
      footer={
        <ScreenFooter>
          <Button
            label={translate("profile.edit.submit")}
            onPress={save}
            disabled={!isFilled(fullName) || !isFilled(birthDate)}
            isLoading={profileMutation.isPending}
          />
        </ScreenFooter>
      }
    >
      {failure === "network" ? (
        <Banner message={translate("common.networkError")}>
          <Button variant="secondary" label={translate("common.retry")} onPress={save} />
        </Banner>
      ) : null}
      {failure === "unexpected" ? <Banner message={translate("common.unexpectedError")} /> : null}
      <RequiredFieldsLegend />
      <TextField
        label={translate("profile.name.fullName")}
        isRequired
        autoCapitalize="words"
        autoComplete="name"
        value={fullName}
        onChangeText={setFullName}
        onSubmitEditing={save}
        errorMessage={fullNameError}
      />
      <BirthDateField value={birthDate} onChangeText={setBirthDate} errorMessage={birthDateError} />
    </ScrollScreen>
  );
}
