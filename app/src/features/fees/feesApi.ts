import { httpClient } from "@/api/httpClient";
import { Business } from "@/features/business/types";
import {
  ClientBilling,
  MonthlyFees,
  Payment,
  RecordPaymentRequest,
  SetBillingPlanRequest,
} from "@/features/fees/types";

const feesPath = "/api/fees";
const businessMonthlyFeePath = "/api/business/monthly-fee";
const clientsPath = "/api/clients";
const paymentsPath = "/api/payments";
const billingPlanSegment = "billing-plan";
const paymentsSegment = "payments";

function clientPath(clientId: string): string {
  return `${clientsPath}/${encodeURIComponent(clientId)}`;
}

export function listMonthlyFees(month: string): Promise<MonthlyFees> {
  return httpClient.get<MonthlyFees>(feesPath, { month });
}

export function setDefaultMonthlyFee(
  amount: number | null,
  effectiveFrom: string,
): Promise<Business> {
  return httpClient.put<Business>(businessMonthlyFeePath, { amount, effectiveFrom });
}

export function setClientBillingPlan(
  clientId: string,
  request: SetBillingPlanRequest,
): Promise<ClientBilling> {
  return httpClient.put<ClientBilling>(`${clientPath(clientId)}/${billingPlanSegment}`, request);
}

export function recordPayment(clientId: string, request: RecordPaymentRequest): Promise<Payment> {
  return httpClient.post<Payment>(`${clientPath(clientId)}/${paymentsSegment}`, request);
}

export function listClientPayments(clientId: string): Promise<Payment[]> {
  return httpClient.get<Payment[]>(`${clientPath(clientId)}/${paymentsSegment}`);
}

export function deletePayment(paymentId: string): Promise<void> {
  return httpClient.delete<void>(`${paymentsPath}/${encodeURIComponent(paymentId)}`);
}
