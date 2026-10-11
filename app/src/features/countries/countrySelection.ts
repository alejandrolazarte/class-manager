import { hasTranslation, translate } from "@/i18n/translate";
import { Country, CountrySelection } from "@/features/countries/types";

export function findCountry(
  countries: readonly Country[],
  countryCode: string,
): Country | undefined {
  return countries.find((country) => country.code === countryCode);
}

export function selectCountry(country: Country): CountrySelection {
  return { countryCode: country.code, timeZoneId: country.timeZoneIds[0] ?? "" };
}

export function countrySelectionForTimeZone(
  countries: readonly Country[],
  timeZoneId: string,
): CountrySelection | null {
  const matchingCountry = countries.find((country) => country.timeZoneIds.includes(timeZoneId));
  if (matchingCountry) {
    return { countryCode: matchingCountry.code, timeZoneId };
  }
  const [firstCountry] = countries;
  return firstCountry ? selectCountry(firstCountry) : null;
}

export function getDeviceTimeZone(): string {
  return Intl.DateTimeFormat().resolvedOptions().timeZone;
}

export function formatTimeZoneForDisplay(timeZoneId: string): string {
  const cityName = timeZoneId.split("/").pop() ?? timeZoneId;
  return cityName.replaceAll("_", " ");
}

export function countryName(countryCode: string): string {
  const translationKey = `countries.${countryCode}`;
  return hasTranslation(translationKey) ? translate(translationKey) : countryCode;
}
