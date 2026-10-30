import { useLocalSearchParams } from "expo-router";
import { OfferPlanScreen } from "@/backoffice/brands/OfferPlanScreen";

export default function BackofficeOfferPlanRoute() {
  const { brandId } = useLocalSearchParams<{ brandId: string }>();
  return <OfferPlanScreen brandId={brandId} />;
}
