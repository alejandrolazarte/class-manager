import { useLocalSearchParams } from "expo-router";
import { SellClassPackScreen } from "@/features/classPacks/screens/SellClassPackScreen";

export default function SellClassPackRoute() {
  const { clientId } = useLocalSearchParams<{ clientId: string }>();
  return <SellClassPackScreen clientId={clientId} />;
}
