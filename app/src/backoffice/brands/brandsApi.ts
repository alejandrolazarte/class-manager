import { getBackofficeJson } from "@/backoffice/api/backofficeHttpClient";

const brandsPath = "/api/brands";

export type BrandStatus = "Trial" | "Active" | "Ended" | "WithoutSubscription";

export interface BrandSummary {
  id: string;
  name: string;
  ownerName: string | null;
  ownerEmail: string | null;
  planCode: string | null;
  status: BrandStatus;
  trialDaysLeft: number | null;
  expiredOn: string | null;
  branchCount: number;
  studentCount: number;
  createdAt: string;
}

export interface BrandCurrentSubscription {
  planCode: string;
  price: number;
  currency: string;
  note: string | null;
  createdOn: string;
  expiredOn: string | null;
  status: BrandStatus;
  trialDaysLeft: number | null;
}

export interface BrandFeature {
  code: string;
  limit: number | null;
}

export interface BrandBranch {
  id: string;
  name: string;
  countryCode: string;
  createdAt: string;
  studentCount: number;
  teamCount: number;
  storageMegabytes: number;
}

export interface BrandAddOn {
  id: string;
  featureCode: string;
  price: number;
  currency: string;
  limit: number | null;
  createdOn: string;
  expiredOn: string | null;
}

export interface BrandPastSubscription {
  planCode: string;
  price: number;
  currency: string;
  note: string | null;
  createdOn: string;
  expiredOn: string | null;
  deletedAt: string | null;
}

export interface BrandDetail {
  id: string;
  name: string;
  createdAt: string;
  owners: { fullName: string; email: string | null }[];
  status: BrandStatus;
  subscription: BrandCurrentSubscription | null;
  features: BrandFeature[];
  branches: BrandBranch[];
  addOns: BrandAddOn[];
  pastSubscriptions: BrandPastSubscription[];
}

export async function getBrands(): Promise<BrandSummary[]> {
  return (await getBackofficeJson<{ brands: BrandSummary[] }>(brandsPath)).brands;
}

export function getBrand(brandId: string): Promise<BrandDetail> {
  return getBackofficeJson<BrandDetail>(`${brandsPath}/${encodeURIComponent(brandId)}`);
}
