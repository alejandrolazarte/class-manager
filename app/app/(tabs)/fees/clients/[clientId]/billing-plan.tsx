import { useLocalSearchParams } from "expo-router";
import { ClientBillingPlanScreen } from "@/features/fees/screens/ClientBillingPlanScreen";

export default function ClientBillingPlanRoute() {
  const { clientId } = useLocalSearchParams<{ clientId: string }>();
  return <ClientBillingPlanScreen clientId={clientId} />;
}
