import { Account } from "@/features/authentication/types";

export function buildAccount(overrides: Partial<Account> = {}): Account {
  return {
    businessId: "business-own",
    businessName: "Natación Sol",
    kind: "team",
    isCurrent: true,
    ...overrides,
  };
}
