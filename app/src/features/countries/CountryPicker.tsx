import { useState } from "react";
import { Pressable, View } from "react-native";
import {
  countryName,
  findCountry,
  formatTimeZoneForDisplay,
  selectCountry,
} from "@/features/countries/countrySelection";
import { CountrySelection } from "@/features/countries/types";
import { useCountries } from "@/features/countries/useCountries";
import { translate } from "@/i18n/translate";
import { Chip } from "@/ui/Chip";
import { AppText } from "@/ui/AppText";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";
import { Spinner } from "@/ui/Spinner";

interface CountryPickerProps {
  selection: CountrySelection;
  onSelectionChange: (selection: CountrySelection) => void;
}

export function CountryPicker({ selection, onSelectionChange }: CountryPickerProps) {
  const [isTimeZoneListExpanded, setIsTimeZoneListExpanded] = useState(false);
  const { data: countries = [], isPending } = useCountries();
  const selectedCountry = findCountry(countries, selection.countryCode);
  const hasSeveralTimeZones = (selectedCountry?.timeZoneIds.length ?? 0) > 1;

  return (
    <View className="gap-2">
      <AppText variant="label" tone="muted">
        {withRequiredMark(translate("business.country"))}
      </AppText>
      {isPending ? <Spinner /> : null}
      <View className="flex-row flex-wrap gap-2">
        {countries.map((country) => (
          <Chip
            key={country.code}
            label={countryName(country.code)}
            isSelected={country.code === selection.countryCode}
            onPress={() => {
              setIsTimeZoneListExpanded(false);
              onSelectionChange(selectCountry(country));
            }}
          />
        ))}
      </View>
      {selectedCountry ? (
        <View className="flex-row flex-wrap items-center gap-2">
          <AppText variant="caption" tone="muted">
            {translate("business.countrySummary", {
              timeZone: formatTimeZoneForDisplay(selection.timeZoneId),
              currency: selectedCountry.currencyCode,
              callingCode: selectedCountry.callingCode,
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
      ) : null}
      {selectedCountry && hasSeveralTimeZones && isTimeZoneListExpanded ? (
        <View className="flex-row flex-wrap gap-2">
          {selectedCountry.timeZoneIds.map((timeZoneId) => (
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
