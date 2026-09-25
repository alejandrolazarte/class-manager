import { Pressable, Text } from "react-native";
import { useSession } from "@/features/authentication/useSession";

export const signOutProbeLabel = "sign out probe";

export function SessionProbe() {
  const { session, signOut } = useSession();
  return (
    <>
      <Text>{session.status}</Text>
      <Pressable
        accessibilityRole="button"
        accessibilityLabel={signOutProbeLabel}
        onPress={signOut}
      />
    </>
  );
}
