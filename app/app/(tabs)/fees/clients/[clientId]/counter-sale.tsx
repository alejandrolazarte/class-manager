import { useLocalSearchParams } from "expo-router";
import { CounterSaleScreen } from "@/features/orders/screens/CounterSaleScreen";

export default function ClientCounterSaleRoute() {
  const { clientId, studentId } = useLocalSearchParams<{ clientId: string; studentId?: string }>();
  return <CounterSaleScreen clientId={clientId} studentId={studentId} />;
}
