export interface FeatureLimit {
  code: string;
  limit: number | null;
  used?: number | null;
}

export interface CurrentSubscription {
  planCode: string;
  isActive: boolean;
  expiredOn: string | null;
  features: FeatureLimit[];
}

export interface Plan {
  code: string;
  listPrice: number | null;
  currency: string;
  billingPeriod: "Monthly" | "Yearly";
  durationInDays: number | null;
  features: FeatureLimit[];
}

export interface OrganizationSubscription {
  planCode: string;
  price: number;
  currency: string;
  createdOn: string;
  expiredOn: string | null;
  isActive: boolean;
  selfService: boolean;
  isExternal: boolean;
  paymentStatus: PaymentStatus | null;
  offer: PlanOffer | null;
}

export interface PlanOffer {
  planCode: string;
  price: number;
  currency: string;
  expiredOn: string | null;
}

export type PaymentStatus = "Current" | "PastDue";

export interface BillingRedirect {
  url: string;
}
