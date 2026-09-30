import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { isApiError } from "@/api/httpClient";
import { CountryPicker } from "@/features/business/components/CountryPicker";
import {
  CountrySelection,
  getCountryPreset,
  getCountrySelectionForTimeZone,
} from "@/features/business/countryPresets";
import { currencyOptions } from "@/features/business/currencyOptions";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import {
  businessSettingsFieldNames,
  BusinessSettingsFormValues,
  businessSettingsSchema,
} from "@/features/business/businessSettingsSchema";
import { useUpdateBusinessSettings } from "@/features/business/useUpdateBusinessSettings";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { translate } from "@/i18n/translate";
import { View } from "react-native";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { TextField } from "@/ui/TextField";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { useToast } from "@/ui/ToastProvider";
import { AppText } from "@/ui/AppText";

const badRequestStatus = 400;

interface RegionalSettings {
  countrySelection: CountrySelection;
  currencyCode: string;
  defaultCountryCallingCode: string;
}

export function BusinessSettingsScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const business = useCurrentBusiness();
  const updateBusinessSettingsMutation = useUpdateBusinessSettings();
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const [regionalSettings, setRegionalSettings] = useState<RegionalSettings>(() => ({
    countrySelection: getCountrySelectionForTimeZone(business.timeZoneId),
    currencyCode: business.currencyCode,
    defaultCountryCallingCode: business.defaultCountryCallingCode,
  }));
  const [noticedAbsencesKeepStreak, setNoticedAbsencesKeepStreak] = useState(
    business.noticedAbsencesKeepStreak,
  );
  const form = useForm<BusinessSettingsFormValues>({
    resolver: zodResolver(businessSettingsSchema),
    defaultValues: { name: business.name },
    mode: "onTouched",
  });

  const selectCountry = (countrySelection: CountrySelection) => {
    const countryPreset = getCountryPreset(countrySelection.countryCode);
    setRegionalSettings({
      countrySelection,
      currencyCode: countryPreset.currencyCode,
      defaultCountryCallingCode: countryPreset.callingCode,
    });
  };

  const selectCurrency = (currencyCode: string) =>
    setRegionalSettings((current) => ({ ...current, currencyCode }));
  const countryCurrencyCode = getCountryPreset(
    regionalSettings.countrySelection.countryCode,
  ).currencyCode;

  const save = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    const timeZoneId = regionalSettings.countrySelection.timeZoneId;
    try {
      await updateBusinessSettingsMutation.mutateAsync({
        name: formValues.name.trim(),
        timeZoneId,
        currencyCode: regionalSettings.currencyCode,
        defaultCountryCallingCode: regionalSettings.defaultCountryCallingCode,
        noticedAbsencesKeepStreak,
      });
      showToast(translate("businessSettings.saved"));
      router.back();
    } catch (saveError) {
      const hasFieldErrors =
        isApiError(saveError) &&
        saveError.status === badRequestStatus &&
        applyServerFieldErrors(form, saveError.problem, businessSettingsFieldNames);
      if (!hasFieldErrors) {
        setSubmissionFailure(toSubmissionFailure(saveError));
      }
    }
  });

  return (
    <SettingsFormScreenLayout
      title={translate("businessSettings.title")}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      <Controller
        control={form.control}
        name="name"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("businessSettings.name")}
            autoCapitalize="words"
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <CountryPicker
        selection={regionalSettings.countrySelection}
        onSelectionChange={selectCountry}
      />
      <View className="gap-2">
        <AppText variant="label" tone="muted">
          {translate("businessSettings.currency")}
        </AppText>
        <View className="flex-row flex-wrap gap-2">
          {currencyOptions(countryCurrencyCode, regionalSettings.currencyCode).map(
            (currencyCode) => (
              <Chip
                key={currencyCode}
                label={currencyCode}
                isSelected={regionalSettings.currencyCode === currencyCode}
                onPress={() => selectCurrency(currencyCode)}
              />
            ),
          )}
        </View>
        {regionalSettings.currencyCode === business.currencyCode ? null : (
          <AppText variant="caption" tone="warning">
            {translate("businessSettings.currencyChangeNotice")}
          </AppText>
        )}
      </View>
      <View className="gap-2">
        <ToggleSwitch
          label={translate("businessSettings.noticedAbsencesKeepStreak")}
          value={noticedAbsencesKeepStreak}
          onValueChange={setNoticedAbsencesKeepStreak}
        />
        <AppText variant="caption" tone="muted">
          {translate(
            noticedAbsencesKeepStreak
              ? "businessSettings.noticedAbsencesKeepStreakHint"
              : "businessSettings.noticedAbsencesBreakStreakHint",
          )}
        </AppText>
      </View>
      <Button
        label={translate("common.save")}
        onPress={save}
        isLoading={updateBusinessSettingsMutation.isPending}
      />
    </SettingsFormScreenLayout>
  );
}
