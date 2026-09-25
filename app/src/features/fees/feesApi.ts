import { httpClient } from "@/api/httpClient";
import { Business } from "@/features/business/types";
import { Client } from "@/features/clients/types";
import { MonthlyFees, Payment, RecordPaymentRequest } from "@/features/fees/types";

const feesPath = "/api/fees";
const businessMonthlyFeePath = "/api/business/monthly-fee";
const clientsPath = "/api/clients";
const paymentsPath = "/api/payments";
const monthlyFeeSegment = "monthly-fee";
const paymentsSegment = "payments";

function clientPath(clientId: string): string {
  return `${clientsPath}/${encodeURIComponent(clientId)}`;
}

export function listMonthlyFees(month: string): Promise<MonthlyFees> {
  return httpClient.get<MonthlyFees>(feesPath, { month });
}

export function setDefaultMonthlyFee(amount: number | null): Promise<Business> {
  return httpClient.put<Business>(businessMonthlyFeePath, { amount });
}

export function setClientMonthlyFee(clientId: string, amount: number | null): Promise<Client> {
  return httpClient.put<Client>(`${clientPath(clientId)}/${monthlyFeeSegment}`, { amount });
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
