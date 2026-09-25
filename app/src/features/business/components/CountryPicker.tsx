import { useState } from "react";
import { Pressable, Text, View } from "react-native";
import {
  CountrySelection,
  countryPresets,
  formatTimeZoneForDisplay,
  getCountryPreset,
  selectCountry,
} from "@/features/business/countryPresets";
import { translate } from "@/i18n/translate";
import { Chip } from "@/ui/Chip";

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
      <Text className="text-sm font-medium text-gray-700">{translate("business.country")}</Text>
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
        <Text className="text-sm text-gray-600">
          {translate("business.countrySummary", {
            timeZone: formatTimeZoneForDisplay(selection.timeZoneId),
            currency: selectedCountryPreset.currencyCode,
            callingCode: selectedCountryPreset.callingCode,
          })}
        </Text>
        {hasSeveralTimeZones && !isTimeZoneListExpanded ? (
          <Pressable
            accessibilityRole="button"
            accessibilityLabel={translate("business.changeTimeZone")}
            onPress={() => setIsTimeZoneListExpanded(true)}
          >
            <Text className="text-sm font-medium text-brand">{translate("common.change")}</Text>
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
