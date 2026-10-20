import { BusinessBillingMode, MonthlyFeeChange } from "@/features/fees/types";

export interface Business {
  name: string;
  countryCode: string;
  timeZoneId: string;
  currencyCode: string;
  defaultCountryCallingCode: string;
  defaultMonthlyFee: number | null;
  defaultMonthlyFeeChanges: MonthlyFeeChange[];
  billingMode: BusinessBillingMode | null;
}

export interface UpdateBusinessSettingsRequest {
  name: string;
  countryCode: string;
  timeZoneId: string;
  currencyCode: string;
}

export type UpdateBusinessSettingsResponse = Omit<
  Business,
  "defaultMonthlyFee" | "defaultMonthlyFeeChanges" | "billingMode"
>;
