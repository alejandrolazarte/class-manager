export type FeeStatus = "Unpaid" | "Partial" | "NoFee" | "Paid";

export type PaymentMethod = "Cash" | "Transfer" | "Card" | "Other";

export interface ClientFee {
  clientId: string;
  clientFullName: string;
  clientPhoneNumber: string;
  studentNames: string[];
  fee: number | null;
  paid: number;
  balance: number;
  status: FeeStatus;
}

export interface ClassPackClient {
  clientId: string;
  clientFullName: string;
  clientPhoneNumber: string;
  studentNames: string[];
  availableClasses: number;
  unpaidClasses: number;
}

export interface MonthlyFees {
  month: string;
  totalDue: number;
  totalPaid: number;
  clients: ClientFee[];
  classPackClients: ClassPackClient[];
}

export interface StudentPlanChange {
  effectiveFrom: string;
  classPackId: string | null;
  planName: string | null;
  customPrice: number | null;
}

export interface StudentPlan {
  studentId: string;
  studentFullName: string;
  classPackId: string | null;
  planName: string | null;
  price: number | null;
  customPrice: number | null;
  since: string | null;
  upcomingChanges: StudentPlanChange[];
}

export interface FamilyPlans {
  monthlyDue: number | null;
  students: StudentPlan[];
}

export interface SetStudentPlanRequest {
  classPackId: string | null;
  customPrice: number | null;
  effectiveFrom: string;
}

export interface Payment {
  id: string;
  clientId: string;
  amount: number;
  currencyCode: string;
  month: string;
  paidOn: string;
  method: PaymentMethod;
  notes: string | null;
  recordedByUserId: string | null;
  recordedByFullName: string | null;
}

export interface RecordPaymentRequest {
  amount: number;
  month: string;
  paidOn: string;
  method: PaymentMethod;
  notes: string | null;
}
