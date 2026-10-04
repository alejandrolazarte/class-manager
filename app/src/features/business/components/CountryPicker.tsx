import { useState } from "react";
import { Pressable, View } from "react-native";
import {
  CountrySelection,
  countryPresets,
  formatTimeZoneForDisplay,
  getCountryPreset,
  selectCountry,
} from "@/features/business/countryPresets";
import { translate } from "@/i18n/translate";
import { Chip } from "@/ui/Chip";
import { AppText } from "@/ui/AppText";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";

interface CountryPickerProps {
  selection: CountrySelection;
  onSelectionChange: (selection: CountrySelection) => void;
}

export function CountryPicker({ selection, onSelectionChange }: CountryPickerProps) {
  const [isTimeZoneListExpanded, setIsTimeZoneListExpanded] = useState(false);
  const selectedCountryPreset = getCountryPreset(selection.countryCode);
  const hasSeveralTimeZones = selectedCountryPreset.timeZoneIds.length > 1;

  return (
    <View className="gap-2">
      <AppText variant="label" tone="muted">
        {withRequiredMark(translate("business.country"))}
      </AppText>
      <View className="flex-row flex-wrap gap-2">
        {countryPresets.map((countryPreset) => (
          <Chip
            key={countryPreset.countryCode}
            label={translate(`countries.${countryPreset.countryCode}`)}
            isSelected={countryPreset.countryCode === selection.countryCode}
            onPress={() => {
              setIsTimeZoneListExpanded(false);
              onSelectionChange(selectCountry(countryPreset.countryCode));
            }}
          />
        ))}
      </View>
      <View className="flex-row flex-wrap items-center gap-2">
        <AppText variant="caption" tone="muted">
          {translate("business.countrySummary", {
            timeZone: formatTimeZoneForDisplay(selection.timeZoneId),
            currency: selectedCountryPreset.currencyCode,
            callingCode: selectedCountryPreset.callingCode,
          })}
        </AppText>
        {hasSeveralTimeZones && !isTimeZoneListExpanded ? (
          <Pressable
            accessibilityRole="button"
            accessibilityLabel={translate("business.changeTimeZone")}
            onPress={() => setIsTimeZoneListExpanded(true)}
          >
            <AppText variant="label" tone="primary">
              {translate("common.change")}
            </AppText>
          </Pressable>
        ) : null}
      </View>
      {hasSeveralTimeZones && isTimeZoneListExpanded ? (
        <View className="flex-row flex-wrap gap-2">
          {selectedCountryPreset.timeZoneIds.map((timeZoneId) => (
            <Chip
              key={timeZoneId}
              label={formatTimeZoneForDisplay(timeZoneId)}
              isSelected={timeZoneId === selection.timeZoneId}
              onPress={() => onSelectionChange({ countryCode: selection.countryCode, timeZoneId })}
            />
          ))}
        </View>
      ) : null}
    </View>
  );
}
