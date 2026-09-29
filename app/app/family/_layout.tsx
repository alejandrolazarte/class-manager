import { Stack } from "expo-router";
import { RequireSessionKind } from "@/features/authentication/components/RequireSessionKind";

export default function FamilyLayout() {
  return (
    <RequireSessionKind kind="family">
      <Stack screenOptions={{ headerShown: false }} />
    </RequireSessionKind>
  );
}
