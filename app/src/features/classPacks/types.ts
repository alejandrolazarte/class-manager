import { CatalogImage } from "@/features/catalogImages/types";
import { PaymentMethod } from "@/features/fees/types";

export interface ClassPack {
  id: string;
  name: string;
  description: string | null;
  classCount: number | null;
  price: number;
  validityMonths: number | null;
  isActive: boolean;
  classDurationMinutes: number | null;
  classGroupIds: string[];
  coversAnyGroupClass: boolean;
  images: CatalogImage[];
  renewsMonthly: boolean;
  appliesFamilyDiscount: boolean;
  priceChanges: ClassPackPriceChange[];
}

export interface ClassPackPriceChange {
  effectiveFrom: string;
  price: number;
}

export type SoldOnceClassPack = ClassPack & { classCount: number; renewsMonthly: false };

export function isSoldOnce(classPack: ClassPack): classPack is SoldOnceClassPack {
  return !classPack.renewsMonthly && classPack.classCount !== null;
}

export interface SaveClassPackRequest {
  name: string;
  description: string | null;
  classCount: number | null;
  price: number;
  validityMonths: number | null;
  classDurationMinutes: number | null;
  classGroupIds: string[];
  coversAnyGroupClass: boolean;
  renewsMonthly: boolean;
  priceEffectiveFrom?: string;
}

export interface ClassPackPurchase {
  id: string;
  clientId: string;
  name: string;
  classCount: number;
  price: number;
  currencyCode: string;
  purchasedOn: string;
  expiresOn: string | null;
  method: PaymentMethod | null;
  notes: string | null;
  classDurationMinutes: number | null;
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
  currencyCode: string;
  purchasedOn: string;
  expiresOn: string | null;
  method: PaymentMethod | null;
  usedClasses: number;
  remainingClasses: number;
  status: ClassPackPurchaseStatus;
  classDurationMinutes: number | null;
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
