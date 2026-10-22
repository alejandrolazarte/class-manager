import { Pressable, Text } from "react-native";
import { useSession } from "@/features/authentication/useSession";

export const signOutProbeLabel = "sign out probe";
export const switchBranchProbeLabel = "switch branch probe";
export const probeBranchId = "branch-valencia";
export const switchToStudentAccountProbeLabel = "switch to student account probe";
export const switchToTeamAccountProbeLabel = "switch to team account probe";
export const probeAccountBusinessId = "business-school";

export function SessionProbe() {
  const { session, signOut, switchBranch, switchAccount } = useSession();
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
      <Pressable
        accessibilityRole="button"
        accessibilityLabel={switchToStudentAccountProbeLabel}
        onPress={() => switchAccount(probeAccountBusinessId, "student")}
      />
      <Pressable
        accessibilityRole="button"
        accessibilityLabel={switchToTeamAccountProbeLabel}
        onPress={() => switchAccount(probeAccountBusinessId, "team")}
      />
    </>
  );
}
