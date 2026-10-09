import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { getFieldErrors } from "@/api/problemDetails";
import { isApiError } from "@/api/httpClient";
import { useSession } from "@/features/authentication/useSession";
import { Branch } from "@/features/branches/types";
import { useCreateBranch } from "@/features/branches/useBranches";
import { CountryPicker } from "@/features/business/components/CountryPicker";
import {
  CountrySelection,
  getCountryPreset,
  getCountrySelectionForTimeZone,
} from "@/features/business/countryPresets";
import { currencyOptions } from "@/features/business/currencyOptions";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";
import { isFilled } from "@/forms/requiredFields";
import { RequiredFieldsLegend, withRequiredMark } from "@/ui/RequiredFieldsLegend";

const badRequestStatus = 400;

export function NewBranchScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const { switchBranch } = useSession();
  const business = useCurrentBusiness();
  const createBranchMutation = useCreateBranch();
  const [name, setName] = useState("");
  const [nameError, setNameError] = useState<string | undefined>();
  const [countrySelection, setCountrySelection] = useState<CountrySelection>(() =>
    getCountrySelectionForTimeZone(business.timeZoneId),
  );
  const [currencyCode, setCurrencyCode] = useState(business.currencyCode);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const countryPreset = getCountryPreset(countrySelection.countryCode);

  const selectCountry = (selection: CountrySelection) => {
    setCountrySelection(selection);
    setCurrencyCode(getCountryPreset(selection.countryCode).currencyCode);
  };

  const enterBranch = async (branch: Branch) => {
    try {
      await switchBranch(branch.businessId);
      showToast(translate("branches.switched", { name: branch.name }));
      router.replace(routes.today);
    } catch {
      showToast(translate("branches.switchFailed"));
    }
  };

  const create = async () => {
    setSubmissionFailure(null);
    setNameError(undefined);
    try {
      const branch = await createBranchMutation.mutateAsync({
        name: name.trim(),
        timeZoneId: countrySelection.timeZoneId,
        currencyCode,
        defaultCountryCallingCode: countryPreset.callingCode,
      });
      showToast(translate("branches.form.created", { name: branch.name }), {
        label: translate("branches.form.enter"),
        onPress: () => void enterBranch(branch),
      });
      router.back();
    } catch (createError) {
      if (isApiError(createError) && createError.status === badRequestStatus) {
        const fieldErrors = getFieldErrors(createError.problem);
        if (fieldErrors.name) {
          setNameError(fieldErrors.name);
          return;
        }
      }
      setSubmissionFailure(toSubmissionFailure(createError));
    }
  };

  return (
    <SettingsFormScreenLayout
      title={translate("branches.form.title")}
      submissionFailure={submissionFailure}
      onRetry={create}
    >
      <RequiredFieldsLegend />
      <TextField
        label={translate("branches.form.name")}
        isRequired
        hint={translate("branches.form.nameHint")}
        autoCapitalize="words"
        value={name}
        onChangeText={setName}
        errorMessage={nameError}
      />
      <CountryPicker selection={countrySelection} onSelectionChange={selectCountry} />
      <View className="gap-2">
        <AppText variant="label" tone="muted">
          {withRequiredMark(translate("businessSettings.currency"))}
        </AppText>
        <View className="flex-row flex-wrap gap-2">
          {currencyOptions(countryPreset.currencyCode, currencyCode).map((currencyOption) => (
            <Chip
              key={currencyOption}
              label={currencyOption}
              isSelected={currencyCode === currencyOption}
              onPress={() => setCurrencyCode(currencyOption)}
            />
          ))}
        </View>
      </View>
      <Button
        label={translate("branches.form.submit")}
        onPress={create}
        disabled={!isFilled(name)}
        isLoading={createBranchMutation.isPending}
      />
    </SettingsFormScreenLayout>
  );
}
