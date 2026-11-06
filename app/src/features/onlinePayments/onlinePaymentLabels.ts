import { OnlinePaymentStatus } from "@/features/onlinePayments/types";
import { translate } from "@/i18n/translate";

export function onlinePaymentStatusLabel(status: OnlinePaymentStatus): string {
  switch (status) {
    case "Active":
      return translate("onlinePayments.status.active");
    case "Restricted":
      return translate("onlinePayments.status.restricted");
    case "Pending":
      return translate("onlinePayments.status.pending");
    case "NotConnected":
      return translate("onlinePayments.status.notConnected");
  }
}
