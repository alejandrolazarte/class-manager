import { httpClient } from "@/api/httpClient";
import {
  FamilyPlans,
  MonthlyFees,
  Payment,
  RecordPaymentRequest,
  SetStudentPlanRequest,
} from "@/features/fees/types";

const feesPath = "/api/fees";
const studentsPath = "/api/students";
const clientsPath = "/api/clients";
const paymentsPath = "/api/payments";
const restoreSegment = "restore";
const planSegment = "plan";
const paymentsSegment = "payments";

function clientPath(clientId: string): string {
  return `${clientsPath}/${encodeURIComponent(clientId)}`;
}

export function listMonthlyFees(month: string): Promise<MonthlyFees> {
  return httpClient.get<MonthlyFees>(feesPath, { month });
}

function studentPlanPath(studentId: string): string {
  return `${studentsPath}/${encodeURIComponent(studentId)}/${planSegment}`;
}

export function setStudentPlan(
  studentId: string,
  request: SetStudentPlanRequest,
): Promise<FamilyPlans> {
  return httpClient.put<FamilyPlans>(studentPlanPath(studentId), request);
}

export function deleteStudentPlanChange(studentId: string, month: string): Promise<FamilyPlans> {
  return httpClient.delete<FamilyPlans>(
    `${studentPlanPath(studentId)}/${encodeURIComponent(month)}`,
  );
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

export function restorePayment(paymentId: string): Promise<void> {
  return httpClient.post<void>(
    `${paymentsPath}/${encodeURIComponent(paymentId)}/${restoreSegment}`,
    {},
  );
}
