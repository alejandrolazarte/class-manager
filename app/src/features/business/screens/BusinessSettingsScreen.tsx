import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { isApiError } from "@/api/httpClient";
import { getFieldErrors } from "@/api/problemDetails";
import {
  BusinessLocationValues,
  isLocationComplete,
  locationValuesOf,
  toLocationRequest,
} from "@/features/business/businessLocation";
import { BusinessLocationFields } from "@/features/business/components/BusinessLocationFields";
import { currencyOptions } from "@/features/business/currencyOptions";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import {
  businessSettingsFieldNames,
  BusinessSettingsFormValues,
  businessSettingsSchema,
} from "@/features/business/businessSettingsSchema";
import { useUpdateBusinessSettings } from "@/features/business/useUpdateBusinessSettings";
import { CountryPicker } from "@/features/countries/CountryPicker";
import { findCountry } from "@/features/countries/countrySelection";
import { CountrySelection } from "@/features/countries/types";
import { useCountries } from "@/features/countries/useCountries";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { applyServerFieldErrors } from "@/forms/applyServerFieldErrors";
import { translate } from "@/i18n/translate";
import { View } from "react-native";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";
import { AppText } from "@/ui/AppText";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";
import { RequiredFieldsLegend, withRequiredMark } from "@/ui/RequiredFieldsLegend";

const badRequestStatus = 400;

interface LocationErrors {
  locationName?: string;
  locationMapsUrl?: string;
}

interface RegionalSettings {
  countrySelection: CountrySelection;
  currencyCode: string;
}

export function BusinessSettingsScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const business = useCurrentBusiness();
  const updateBusinessSettingsMutation = useUpdateBusinessSettings();
  const { data: countries = [] } = useCountries();
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const [regionalSettings, setRegionalSettings] = useState<RegionalSettings>(() => ({
    countrySelection: { countryCode: business.countryCode, timeZoneId: business.timeZoneId },
    currencyCode: business.currencyCode,
  }));
  const [location, setLocation] = useState<BusinessLocationValues>(() =>
    locationValuesOf(business),
  );
  const [locationErrors, setLocationErrors] = useState<LocationErrors>({});
  const form = useForm<BusinessSettingsFormValues>({
    resolver: zodResolver(businessSettingsSchema),
    defaultValues: { name: business.name },
    mode: "onTouched",
  });
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, ["name"]);

  const selectCountry = (countrySelection: CountrySelection) =>
    setRegionalSettings((current) => ({
      countrySelection,
      currencyCode:
        findCountry(countries, countrySelection.countryCode)?.currencyCode ?? current.currencyCode,
    }));

  const selectCurrency = (currencyCode: string) =>
    setRegionalSettings((current) => ({ ...current, currencyCode }));
  const countryCurrencyCode =
    findCountry(countries, regionalSettings.countrySelection.countryCode)?.currencyCode ??
    regionalSettings.currencyCode;

  const changeLocation = (values: BusinessLocationValues) => {
    setLocation(values);
    setLocationErrors({});
  };

  const save = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    try {
      await updateBusinessSettingsMutation.mutateAsync({
        name: formValues.name.trim(),
        countryCode: regionalSettings.countrySelection.countryCode,
        timeZoneId: regionalSettings.countrySelection.timeZoneId,
        currencyCode: regionalSettings.currencyCode,
        ...toLocationRequest(location),
      });
      showToast(translate("businessSettings.saved"));
      router.back();
    } catch (saveError) {
      const isBadRequest = isApiError(saveError) && saveError.status === badRequestStatus;
      const fieldErrors = isBadRequest ? getFieldErrors(saveError.problem) : {};
      const serverLocationErrors: LocationErrors = {
        locationName: fieldErrors.locationName,
        locationMapsUrl: fieldErrors.locationMapsUrl,
      };
      setLocationErrors(serverLocationErrors);
      const hasFieldErrors =
        (isBadRequest &&
          applyServerFieldErrors(form, saveError.problem, businessSettingsFieldNames)) ||
        serverLocationErrors.locationName !== undefined ||
        serverLocationErrors.locationMapsUrl !== undefined;
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
      <RequiredFieldsLegend />
      <Controller
        control={form.control}
        name="name"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("businessSettings.name")}
            isRequired
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
          {withRequiredMark(translate("businessSettings.currency"))}
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
      <BusinessLocationFields
        values={location}
        onChange={changeLocation}
        locationNameError={locationErrors.locationName}
        locationMapsUrlError={locationErrors.locationMapsUrl}
      />
      <Button
        label={translate("common.save")}
        onPress={save}
        disabled={!areRequiredFieldsFilled || !isLocationComplete(location)}
        isLoading={updateBusinessSettingsMutation.isPending}
      />
    </SettingsFormScreenLayout>
  );
}
