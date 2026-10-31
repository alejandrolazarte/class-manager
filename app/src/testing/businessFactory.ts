import { Business } from "@/features/business/types";

export function buildBusiness(overrides: Partial<Business> = {}): Business {
  return {
    name: "Panadería Laura",
    countryCode: "AR",
    timeZoneId: "America/Argentina/Buenos_Aires",
    currencyCode: "ARS",
    defaultCountryCallingCode: "54",
    locationName: null,
    locationMapsUrl: null,
    ...overrides,
  };
}
