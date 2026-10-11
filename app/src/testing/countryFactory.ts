import { Country } from "@/features/countries/types";

export function buildCountries(): Country[] {
  return [
    {
      code: "AR",
      callingCode: "54",
      currencyCode: "ARS",
      timeZoneIds: ["America/Argentina/Buenos_Aires"],
    },
    { code: "UY", callingCode: "598", currencyCode: "UYU", timeZoneIds: ["America/Montevideo"] },
    {
      code: "ES",
      callingCode: "34",
      currencyCode: "EUR",
      timeZoneIds: ["Europe/Madrid", "Atlantic/Canary"],
    },
  ];
}
