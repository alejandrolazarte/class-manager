export interface Country {
  code: string;
  callingCode: string;
  currencyCode: string;
  timeZoneIds: string[];
}

export interface CountrySelection {
  countryCode: string;
  timeZoneId: string;
}
