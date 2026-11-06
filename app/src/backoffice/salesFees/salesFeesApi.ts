import { getBackofficeJson, sendBackofficeJson } from "@/backoffice/api/backofficeHttpClient";
import { CatalogPlan } from "@/backoffice/brands/brandSubscriptionApi";

export interface BrandSalesFee {
  percent: number;
  isBrandFee: boolean;
  planPercent: number;
  note: string | null;
}

export interface GiveBrandSalesFeeRequest {
  percent: number;
  note: string | null;
}

function brandSalesFeePath(brandId: string): string {
  return `/api/brands/${encodeURIComponent(brandId)}/sales-fee`;
}

export function getBrandSalesFee(brandId: string): Promise<BrandSalesFee> {
  return getBackofficeJson<BrandSalesFee>(brandSalesFeePath(brandId));
}

export function giveBrandSalesFee(
  brandId: string,
  request: GiveBrandSalesFeeRequest,
): Promise<BrandSalesFee> {
  return sendBackofficeJson("PUT", brandSalesFeePath(brandId), request);
}

export function removeBrandSalesFee(brandId: string): Promise<BrandSalesFee> {
  return sendBackofficeJson("DELETE", brandSalesFeePath(brandId));
}

export function changePlanSalesFee(planCode: string, percent: number): Promise<CatalogPlan> {
  return sendBackofficeJson("PUT", `/api/plans/${encodeURIComponent(planCode)}/sales-fee`, {
    percent,
  });
}
