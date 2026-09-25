export type CountryCode =
  "AR" | "BO" | "CL" | "CO" | "CR" | "EC" | "ES" | "GT" | "MX" | "PA" | "PE" | "PY" | "UY" | "VE";

export interface CountryPreset {
  countryCode: CountryCode;
  timeZoneIds: readonly [string, ...string[]];
  currencyCode: string;
  callingCode: string;
}

export interface CountrySelection {
  countryCode: CountryCode;
  timeZoneId: string;
}

export const countryPresets: readonly CountryPreset[] = [
  {
    countryCode: "AR",
    timeZoneIds: ["America/Argentina/Buenos_Aires"],
    currencyCode: "ARS",
    callingCode: "54",
  },
  {
    countryCode: "UY",
    timeZoneIds: ["America/Montevideo"],
    currencyCode: "UYU",
    callingCode: "598",
  },
  {
    countryCode: "CL",
    timeZoneIds: ["America/Santiago", "Pacific/Easter"],
    currencyCode: "CLP",
    callingCode: "56",
  },
  { countryCode: "PY", timeZoneIds: ["America/Asuncion"], currencyCode: "PYG", callingCode: "595" },
  { countryCode: "BO", timeZoneIds: ["America/La_Paz"], currencyCode: "BOB", callingCode: "591" },
  { countryCode: "PE", timeZoneIds: ["America/Lima"], currencyCode: "PEN", callingCode: "51" },
  {
    countryCode: "EC",
    timeZoneIds: ["America/Guayaquil", "Pacific/Galapagos"],
    currencyCode: "USD",
    callingCode: "593",
  },
  { countryCode: "CO", timeZoneIds: ["America/Bogota"], currencyCode: "COP", callingCode: "57" },
  { countryCode: "VE", timeZoneIds: ["America/Caracas"], currencyCode: "VES", callingCode: "58" },
  { countryCode: "PA", timeZoneIds: ["America/Panama"], currencyCode: "USD", callingCode: "507" },
  {
    countryCode: "CR",
    timeZoneIds: ["America/Costa_Rica"],
    currencyCode: "CRC",
    callingCode: "506",
  },
  {
    countryCode: "GT",
    timeZoneIds: ["America/Guatemala"],
    currencyCode: "GTQ",
    callingCode: "502",
  },
  {
    countryCode: "MX",
    timeZoneIds: [
      "America/Mexico_City",
      "America/Cancun",
      "America/Mazatlan",
      "America/Hermosillo",
      "America/Tijuana",
    ],
    currencyCode: "MXN",
    callingCode: "52",
  },
  {
    countryCode: "ES",
    timeZoneIds: ["Europe/Madrid", "Atlantic/Canary"],
    currencyCode: "EUR",
    callingCode: "34",
  },
];

const defaultCountryPreset = countryPresets[0];

export function getCountryPreset(countryCode: CountryCode): CountryPreset {
  return (
    countryPresets.find((countryPreset) => countryPreset.countryCode === countryCode) ??
    defaultCountryPreset
  );
}

export function selectCountry(countryCode: CountryCode): CountrySelection {
  return { countryCode, timeZoneId: getCountryPreset(countryCode).timeZoneIds[0] };
}

export function getDeviceTimeZone(): string {
  return Intl.DateTimeFormat().resolvedOptions().timeZone;
}

export function getCountrySelectionForTimeZone(deviceTimeZone: string): CountrySelection {
  const matchingCountryPreset = countryPresets.find((countryPreset) =>
    countryPreset.timeZoneIds.includes(deviceTimeZone),
  );
  return matchingCountryPreset
    ? { countryCode: matchingCountryPreset.countryCode, timeZoneId: deviceTimeZone }
    : selectCountry(defaultCountryPreset.countryCode);
}

export function formatTimeZoneForDisplay(timeZoneId: string): string {
  const cityName = timeZoneId.split("/").pop() ?? timeZoneId;
  return cityName.replaceAll("_", " ");
}
