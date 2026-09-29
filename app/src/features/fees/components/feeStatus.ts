import { formatMoney } from "@/features/fees/money";
import { ClientFee, FeeStatus } from "@/features/fees/types";
import { translate } from "@/i18n/translate";
import { StatusTone } from "@/ui/StatusPill";

export const feeStatusTones: Record<FeeStatus, StatusTone> = {
  Unpaid: "danger",
  Partial: "warning",
  NoFee: "neutral",
  Paid: "success",
};

export type FeeStatusSummary = Pick<ClientFee, "status" | "balance" | "paid" | "fee">;

export function feeStatusLabel(clientFee: FeeStatusSummary, currencyCode: string): string {
  const money = (amount: number) => formatMoney(amount, currencyCode);
  return {
    Unpaid: translate("fees.status.unpaid", { balance: money(clientFee.balance) }),
    Partial: translate("fees.status.partial", {
      paid: money(clientFee.paid),
      fee: money(clientFee.fee ?? 0),
    }),
    NoFee: translate("fees.status.noFee"),
    Paid: translate("fees.status.paid"),
  }[clientFee.status];
}
