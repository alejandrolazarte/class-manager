import { Pressable, Text } from "react-native";
import { useSession } from "@/features/authentication/useSession";

export const signOutProbeLabel = "sign out probe";
export const switchBranchProbeLabel = "switch branch probe";
export const probeBranchId = "branch-valencia";

export function SessionProbe() {
  const { session, signOut, switchBranch } = useSession();
  return (
    <>
      <Text>{session.status}</Text>
      {session.status === "signedIn" ? <Text>{session.kind}</Text> : null}
      <Pressable
        accessibilityRole="button"
        accessibilityLabel={signOutProbeLabel}
        onPress={signOut}
      />
      <Pressable
        accessibilityRole="button"
        accessibilityLabel={switchBranchProbeLabel}
        onPress={() => switchBranch(probeBranchId)}
      />
    </>
  );
}
