import { FeeStatus } from "@/features/fees/types";

export interface FamilyNextClass {
  name: string;
  date: string;
  startTime: string;
  endTime: string;
  instructorFullName: string | null;
  location: string | null;
  isPrivateLesson: boolean;
  isCancelled: boolean;
}

export interface FamilyStudent {
  id: string;
  fullName: string;
  nextClasses: FamilyNextClass[];
}

export interface FamilyMonthlyFee {
  month: string;
  fee: number | null;
  paid: number;
  balance: number;
  status: FeeStatus;
}

export interface FamilyClassBalance {
  availableClasses: number;
  unpaidClasses: number;
}

export interface FamilyBilling {
  kind: "BusinessFee" | "CustomFee" | "ClassPacks";
  monthlyFee: FamilyMonthlyFee | null;
  classes: FamilyClassBalance | null;
}

export interface FamilyHome {
  businessName: string;
  currencyCode: string;
  clientFullName: string;
  students: FamilyStudent[];
  billing: FamilyBilling;
}

export interface InviteFamilyRequest {
  email: string;
}

export interface FamilyInvitation {
  id: string;
  email: string;
  expiresAt: string;
}
