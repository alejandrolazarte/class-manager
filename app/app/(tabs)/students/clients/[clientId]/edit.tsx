import { useLocalSearchParams } from "expo-router";
import { EditClientScreen } from "@/features/clients/screens/EditClientScreen";

export default function EditClientRoute() {
  const { clientId } = useLocalSearchParams<{ clientId: string }>();
  return <EditClientScreen clientId={clientId} />;
}
