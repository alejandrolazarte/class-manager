import { useQueryClient } from "@tanstack/react-query";
import { useLocalSearchParams } from "expo-router";
import { useEffect, useState } from "react";
import { studentAppQueryKeys } from "@/features/studentApp/studentAppQueryKeys";
import { translate } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";

export const orderPaymentSucceeded = "success";
export const orderPaymentPollingIntervalInMilliseconds = 3000;
export const orderPaymentPollingAttempts = 10;

export function OrderPaymentReturnNotice() {
  const { payment } = useLocalSearchParams<{ payment?: string }>();
  const queryClient = useQueryClient();
  const [attempts, setAttempts] = useState(0);
  const isPolling = payment === orderPaymentSucceeded && attempts < orderPaymentPollingAttempts;

  useEffect(() => {
    if (!isPolling) {
      return undefined;
    }
    const timer = setTimeout(() => {
      void queryClient.invalidateQueries({ queryKey: studentAppQueryKeys.orders() });
      setAttempts((previousAttempts) => previousAttempts + 1);
    }, orderPaymentPollingIntervalInMilliseconds);
    return () => clearTimeout(timer);
  }, [isPolling, attempts, queryClient]);

  if (payment !== orderPaymentSucceeded) {
    return null;
  }
  return <Banner tone="success" message={translate("student.orders.paymentReceived")} />;
}
