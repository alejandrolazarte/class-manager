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
  classPackSales: number;
  classPackClients: ClassPackClient[];
}

export interface MonthlyFeeChange {
  effectiveFrom: string;
  amount: number | null;
}

export type BillingPlanKind = "BusinessFee" | "CustomFee" | "ClassPacks";

export interface BillingPlan {
  kind: BillingPlanKind;
  customFee: number | null;
}

export interface BillingPlanChange extends BillingPlan {
  effectiveFrom: string;
}

export interface SetBillingPlanRequest extends BillingPlan {
  effectiveFrom: string;
}

export interface ClientBilling {
  billingPlan: BillingPlan;
  billingPlanChanges: BillingPlanChange[];
}

export interface Payment {
  id: string;
  clientId: string;
  amount: number;
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
