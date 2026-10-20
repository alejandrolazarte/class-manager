import { useLocalSearchParams } from "expo-router";
import { ChangePlanScreen } from "@/backoffice/brands/ChangePlanScreen";

export default function BackofficeChangePlanRoute() {
  const { brandId } = useLocalSearchParams<{ brandId: string }>();
  return <ChangePlanScreen brandId={brandId} />;
}
