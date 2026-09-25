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

export interface MonthlyFees {
  month: string;
  totalDue: number;
  totalPaid: number;
  clients: ClientFee[];
}

export interface Payment {
  id: string;
  clientId: string;
  amount: number;
  month: string;
  paidOn: string;
  method: PaymentMethod;
  notes: string | null;
}

export interface RecordPaymentRequest {
  amount: number;
  month: string;
  paidOn: string;
  method: PaymentMethod;
  notes: string | null;
}
