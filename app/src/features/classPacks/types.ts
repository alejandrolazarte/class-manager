import { PaymentMethod } from "@/features/fees/types";

export interface ClassPack {
  id: string;
  name: string;
  classCount: number;
  price: number;
  validityMonths: number | null;
  isActive: boolean;
}

export interface SaveClassPackRequest {
  name: string;
  classCount: number;
  price: number;
  validityMonths: number | null;
}

export interface ClassPackPurchase {
  id: string;
  clientId: string;
  name: string;
  classCount: number;
  price: number;
  purchasedOn: string;
  expiresOn: string | null;
  method: PaymentMethod;
  notes: string | null;
}

export interface SellClassPackRequest {
  classPackId: string;
  price: number;
  purchasedOn: string;
  method: PaymentMethod;
  notes: string | null;
}

export type ClassPackPurchaseStatus = "Active" | "UsedUp" | "Expired";

export interface ClassPackUsage {
  id: string;
  name: string;
  classCount: number;
  price: number;
  purchasedOn: string;
  expiresOn: string | null;
  method: PaymentMethod;
  usedClasses: number;
  remainingClasses: number;
  status: ClassPackPurchaseStatus;
}

export interface AttendedClass {
  date: string;
  studentFullName: string;
  classGroupName: string;
}

export interface ClassBalance {
  availableClasses: number;
  unpaidClasses: number;
  purchases: ClassPackUsage[];
  unpaidAttendances: AttendedClass[];
}
