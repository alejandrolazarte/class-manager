import { getBackofficeJson } from "@/backoffice/api/backofficeHttpClient";

const expiringAddOnsPath = "/api/expiring-add-ons";

export interface ExpiringAddOn {
  brandId: string;
  brandName: string;
  featureCode: string;
  expiredOn: string;
}

export async function getExpiringAddOns(): Promise<ExpiringAddOn[]> {
  return (await getBackofficeJson<{ addOns: ExpiringAddOn[] }>(expiringAddOnsPath)).addOns;
}
