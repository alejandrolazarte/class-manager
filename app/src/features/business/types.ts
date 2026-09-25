export interface Business {
  name: string;
  timeZoneId: string;
  currencyCode: string;
  defaultCountryCallingCode: string;
  defaultMonthlyFee: number | null;
}

export interface UpdateBusinessSettingsRequest {
  name: string;
  timeZoneId: string;
  currencyCode: string;
  defaultCountryCallingCode: string;
}

export type UpdateBusinessSettingsResponse = Omit<Business, "defaultMonthlyFee">;
