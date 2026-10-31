export interface Business {
  name: string;
  countryCode: string;
  timeZoneId: string;
  currencyCode: string;
  defaultCountryCallingCode: string;
  locationName: string | null;
  locationMapsUrl: string | null;
}

export interface UpdateBusinessSettingsRequest {
  name: string;
  countryCode: string;
  timeZoneId: string;
  currencyCode: string;
  locationName: string | null;
  locationMapsUrl: string | null;
}

export type UpdateBusinessSettingsResponse = Business;
