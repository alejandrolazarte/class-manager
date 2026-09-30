import { MonthlyFeeChange } from "@/features/fees/types";

export interface Business {
  name: string;
  timeZoneId: string;
  currencyCode: string;
  defaultCountryCallingCode: string;
  defaultMonthlyFee: number | null;
  defaultMonthlyFeeChanges: MonthlyFeeChange[];
}

export interface UpdateBusinessSettingsRequest {
  name: string;
  timeZoneId: string;
  currencyCode: string;
  defaultCountryCallingCode: string;
}

export type UpdateBusinessSettingsResponse = Omit<
  Business,
  "defaultMonthlyFee" | "defaultMonthlyFeeChanges"
>;
