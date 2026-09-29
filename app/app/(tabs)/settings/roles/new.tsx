import { useLocalSearchParams } from "expo-router";
import { RoleEditorScreen } from "@/features/roles/screens/RoleEditorScreen";

export default function NewRoleRoute() {
  const { copyFrom } = useLocalSearchParams<{ copyFrom?: string }>();
  return <RoleEditorScreen copyFromRoleKey={copyFrom} />;
}
