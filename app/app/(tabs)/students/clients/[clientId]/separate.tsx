import { useLocalSearchParams } from "expo-router";
import { SeparateClientPersonScreen } from "@/features/users/screens/SeparateClientPersonScreen";

export default function SeparateClientPersonRoute() {
  const { clientId, studentId } = useLocalSearchParams<{ clientId: string; studentId?: string }>();
  return <SeparateClientPersonScreen clientId={clientId} studentId={studentId} />;
}
