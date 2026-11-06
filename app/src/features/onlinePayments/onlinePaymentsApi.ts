import { httpClient } from "@/api/httpClient";
import { OnlinePaymentRedirect, OnlinePayments } from "@/features/onlinePayments/types";

const onlinePaymentsPath = "/api/online-payments";
const onboardingPath = `${onlinePaymentsPath}/onboarding`;
const refreshPath = `${onlinePaymentsPath}/refresh`;
const accountPath = `${onlinePaymentsPath}/account`;

export function getOnlinePayments(): Promise<OnlinePayments> {
  return httpClient.get<OnlinePayments>(onlinePaymentsPath);
}

export function startOnlinePaymentOnboarding(): Promise<OnlinePaymentRedirect> {
  return httpClient.post<OnlinePaymentRedirect>(onboardingPath, {});
}

export function refreshOnlinePaymentAccount(): Promise<OnlinePayments> {
  return httpClient.post<OnlinePayments>(refreshPath, {});
}

export function chooseOnlinePaymentAccount(accountId: string): Promise<OnlinePayments> {
  return httpClient.put<OnlinePayments>(accountPath, { accountId });
}

export function stopUsingOnlinePaymentAccount(): Promise<OnlinePayments> {
  return httpClient.delete<OnlinePayments>(accountPath);
}
