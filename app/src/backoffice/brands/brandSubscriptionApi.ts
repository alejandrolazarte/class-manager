import { getBackofficeJson, sendBackofficeJson } from "@/backoffice/api/backofficeHttpClient";
import { BrandAddOn } from "@/backoffice/brands/brandsApi";

const catalogPath = "/api/subscription-catalog";
const brandsPath = "/api/brands";

export interface CatalogPlan {
  code: string;
  listPrice: number | null;
  currency: string;
}

export interface CatalogFeature {
  code: string;
  isCounted: boolean;
  isAddOn: boolean;
  addOnListPrice: number | null;
  currency: string | null;
}

export interface SubscriptionCatalog {
  plans: CatalogPlan[];
  features: CatalogFeature[];
}

export interface ChangeBrandPlanRequest {
  planCode: string;
  price: number;
  currency: string;
  note: string | null;
  expiredOn: string | null;
}

export interface GiveBrandAddOnRequest {
  featureCode: string;
  price: number;
  currency: string;
  limit: number | null;
  expiredOn: string | null;
}

function brandPath(brandId: string): string {
  return `${brandsPath}/${encodeURIComponent(brandId)}`;
}

function addOnPath(brandId: string, addOnId: string): string {
  return `${brandPath(brandId)}/add-ons/${encodeURIComponent(addOnId)}`;
}

export function getSubscriptionCatalog(): Promise<SubscriptionCatalog> {
  return getBackofficeJson<SubscriptionCatalog>(catalogPath);
}

export function changeBrandPlan(
  brandId: string,
  request: ChangeBrandPlanRequest,
): Promise<{ subscriptionId: string }> {
  return sendBackofficeJson("PUT", `${brandPath(brandId)}/subscription`, request);
}

export function giveBrandAddOn(
  brandId: string,
  request: GiveBrandAddOnRequest,
): Promise<BrandAddOn> {
  return sendBackofficeJson("POST", `${brandPath(brandId)}/add-ons`, request);
}

export function removeBrandAddOn(brandId: string, addOnId: string): Promise<BrandAddOn> {
  return sendBackofficeJson("DELETE", addOnPath(brandId, addOnId));
}

export function restoreBrandAddOn(brandId: string, addOnId: string): Promise<BrandAddOn> {
  return sendBackofficeJson("POST", `${addOnPath(brandId, addOnId)}/restore`);
}
