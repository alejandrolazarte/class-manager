import { OnlinePaymentAccount, OnlinePayments } from "@/features/onlinePayments/types";

export const firstBranchName = "Sede Centro";
export const secondBranchName = "Sede Norte";

export function buildOnlinePaymentAccount(
  overrides: Partial<OnlinePaymentAccount> = {},
): OnlinePaymentAccount {
  return { id: "account-1", status: "Active", branchNames: [firstBranchName], ...overrides };
}

export function buildOnlinePayments(overrides: Partial<OnlinePayments> = {}): OnlinePayments {
  return {
    status: "NotConnected",
    isAvailable: true,
    account: null,
    brandAccounts: [],
    ...overrides,
  };
}
