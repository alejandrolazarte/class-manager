import { useLocalSearchParams } from "expo-router";
import { CounterSaleScreen } from "@/features/orders/screens/CounterSaleScreen";

export default function ClientCounterSaleRoute() {
  const { clientId } = useLocalSearchParams<{ clientId: string }>();
  return <CounterSaleScreen clientId={clientId} />;
}
