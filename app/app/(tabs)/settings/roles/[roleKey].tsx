import { useLocalSearchParams } from "expo-router";
import { RoleScreen } from "@/features/roles/screens/RoleScreen";

export default function RoleRoute() {
  const { roleKey } = useLocalSearchParams<{ roleKey: string }>();
  return <RoleScreen roleKey={roleKey} />;
}
