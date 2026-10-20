import { useLocalSearchParams } from "expo-router";
import { GiveAddOnScreen } from "@/backoffice/brands/GiveAddOnScreen";

export default function BackofficeGiveAddOnRoute() {
  const { brandId } = useLocalSearchParams<{ brandId: string }>();
  return <GiveAddOnScreen brandId={brandId} />;
}
