export type OnlinePaymentStatus = "NotConnected" | "Pending" | "Restricted" | "Active";

export interface OnlinePaymentAccount {
  id: string;
  status: OnlinePaymentStatus;
  branchNames: string[];
}

export interface OnlinePayments {
  status: OnlinePaymentStatus;
  isAvailable: boolean;
  account: OnlinePaymentAccount | null;
  brandAccounts: OnlinePaymentAccount[];
}

export interface OnlinePaymentRedirect {
  url: string;
}
