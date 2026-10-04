import { BillingPlan, BillingPlanChange } from "@/features/fees/types";
import { NewStudentRequest, Student } from "@/features/students/types";

export interface Client {
  id: string;
  fullName: string;
  phoneNumber: string;
  email: string | null;
  notes: string | null;
  createdAt: string;
}

export type StudentAppAccessStatus = "NotInvited" | "Invited" | "Active";

export interface StudentAppAccess {
  status: StudentAppAccessStatus;
  invitedEmail: string | null;
}

export interface ClientDetails extends Client {
  billingPlan: BillingPlan;
  billingPlanChanges: BillingPlanChange[];
  students: Student[];
  appAccess: StudentAppAccess;
}

export interface UpdateClientRequest {
  fullName: string;
  phoneNumber: string;
  email: string | null;
  notes: string | null;
}

export interface RegisterClientRequest {
  fullName: string;
  phoneNumber: string;
  email?: string;
  notes?: string;
  students: NewStudentRequest[];
}
