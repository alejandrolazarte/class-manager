import { useLocalSearchParams } from "expo-router";
import { BrandSalesFeeScreen } from "@/backoffice/salesFees/BrandSalesFeeScreen";

export default function BackofficeBrandSalesFeeRoute() {
  const { brandId } = useLocalSearchParams<{ brandId: string }>();
  return <BrandSalesFeeScreen brandId={brandId} />;
}
