import { useLocalSearchParams } from "expo-router";
import { ClientDetailScreen } from "@/features/clients/screens/ClientDetailScreen";

export default function ClientDetailRoute() {
  const { clientId } = useLocalSearchParams<{ clientId: string }>();
  return <ClientDetailScreen clientId={clientId} />;
}
