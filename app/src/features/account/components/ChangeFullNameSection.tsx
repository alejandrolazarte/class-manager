import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useState } from "react";
import { View } from "react-native";
import { changeMyFullName } from "@/features/account/accountApi";
import { accountQueryKeys } from "@/features/account/accountQueryKeys";
import { MyAccount } from "@/features/account/types";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { SectionTitle } from "@/ui/SectionTitle";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const fullNameMinimumLength = 2;

interface ChangeFullNameSectionProps {
  account: MyAccount;
}

export function ChangeFullNameSection({ account }: ChangeFullNameSectionProps) {
  const queryClient = useQueryClient();
  const { showToast } = useToast();
  const [fullName, setFullName] = useState(account.fullName);
  const [fullNameError, setFullNameError] = useState<string | undefined>();
  const fullNameMutation = useMutation({
    mutationFn: (newFullName: string) => changeMyFullName(newFullName),
    onSuccess: (updatedAccount) => {
      queryClient.setQueryData(accountQueryKeys.myAccount, updatedAccount);
      showToast(translate("profile.name.saved"));
    },
  });
  const trimmedFullName = fullName.trim();
  const hasChanged = trimmedFullName.length > 0 && trimmedFullName !== account.fullName;

  const save = () => {
    if (trimmedFullName.length < fullNameMinimumLength) {
      setFullNameError(translate("profile.name.tooShort"));
      return;
    }
    setFullNameError(undefined);
    fullNameMutation.mutate(trimmedFullName);
  };

  return (
    <View className="gap-3">
      <SectionTitle title={translate("profile.name.title")} />
      {fullNameMutation.isError ? <Banner message={translate("common.unexpectedError")} /> : null}
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
      <Button
        label={translate("profile.name.submit")}
        onPress={save}
        disabled={!hasChanged}
        isLoading={fullNameMutation.isPending}
      />
    </View>
  );
}
