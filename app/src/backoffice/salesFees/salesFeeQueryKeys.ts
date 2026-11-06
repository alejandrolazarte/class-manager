import { brandQueryKey } from "@/backoffice/brands/brandQueryKeys";

export function brandSalesFeeQueryKey(brandId: string) {
  return [...brandQueryKey(brandId), "salesFee"];
}
