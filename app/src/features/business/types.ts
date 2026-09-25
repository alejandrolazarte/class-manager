export interface Business {
  name: string;
  timeZoneId: string;
  currencyCode: string;
  defaultCountryCallingCode: string;
}

export interface UpdateBusinessSettingsRequest {
  name: string;
  timeZoneId: string;
  currencyCode: string;
  defaultCountryCallingCode: string;
}

export type UpdateBusinessSettingsResponse = Business;
