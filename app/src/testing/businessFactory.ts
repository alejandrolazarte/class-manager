import { Business } from "@/features/business/types";

export function buildBusiness(overrides: Partial<Business> = {}): Business {
  return {
    name: "Panadería Laura",
    timeZoneId: "America/Argentina/Buenos_Aires",
    currencyCode: "ARS",
    defaultCountryCallingCode: "54",
    noticedAbsencesKeepStreak: true,
    defaultMonthlyFee: 12000,
    defaultMonthlyFeeChanges: [{ effectiveFrom: "2026-01", amount: 12000 }],
    ...overrides,
  };
}
