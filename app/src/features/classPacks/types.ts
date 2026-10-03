import { PaymentMethod } from "@/features/fees/types";

export interface ClassPack {
  id: string;
  name: string;
  classCount: number;
  price: number;
  validityMonths: number | null;
  isActive: boolean;
  classDurationMinutes: number | null;
  materialUrl: string | null;
  classGroupIds: string[];
}

export interface SaveClassPackRequest {
  name: string;
  classCount: number;
  price: number;
  validityMonths: number | null;
  classDurationMinutes: number | null;
  materialUrl: string | null;
  classGroupIds: string[];
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
  classDurationMinutes: number | null;
  materialUrl: string | null;
  trialLessonId: string | null;
}

export interface SellClassPackRequest {
  classPackId: string;
  price: number;
  purchasedOn: string;
  method: PaymentMethod;
  notes: string | null;
  trialLessonId: string | null;
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
  classDurationMinutes: number | null;
  materialUrl: string | null;
  recordedByUserId: string | null;
}

export interface DeductibleTrial {
  privateLessonId: string;
  date: string;
  studentFullName: string;
  trialPrice: number;
}

export interface AttendedClass {
  date: string;
  studentFullName: string;
  classGroupName: string;
  isPrivateLesson: boolean;
}

export interface ClassBalance {
  availableClasses: number;
  unpaidClasses: number;
  purchases: ClassPackUsage[];
  unpaidAttendances: AttendedClass[];
  deductibleTrials: DeductibleTrial[];
}
