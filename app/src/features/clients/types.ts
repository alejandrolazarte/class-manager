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

export interface ClientDetails extends Client {
  billingPlan: BillingPlan;
  billingPlanChanges: BillingPlanChange[];
  students: Student[];
}

export interface RegisterClientRequest {
  fullName: string;
  phoneNumber: string;
  email?: string;
  notes?: string;
  students: NewStudentRequest[];
}
