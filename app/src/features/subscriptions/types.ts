export interface FeatureLimit {
  code: string;
  limit: number | null;
}

export interface CurrentSubscription {
  planCode: string;
  isActive: boolean;
  endsOn: string | null;
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
  startsOn: string;
  endsOn: string | null;
  isActive: boolean;
}
