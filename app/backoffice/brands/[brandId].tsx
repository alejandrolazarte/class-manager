import { useLocalSearchParams } from "expo-router";
import { BrandScreen } from "@/backoffice/brands/BrandScreen";

export default function BackofficeBrandRoute() {
  const { brandId } = useLocalSearchParams<{ brandId: string }>();
  return <BrandScreen brandId={brandId} />;
}
