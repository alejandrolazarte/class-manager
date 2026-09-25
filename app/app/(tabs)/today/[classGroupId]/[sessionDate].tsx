import { useLocalSearchParams } from "expo-router";
import { SessionScreen } from "@/features/sessions/screens/SessionScreen";

export default function SessionRoute() {
  const { classGroupId, sessionDate } = useLocalSearchParams<{
    classGroupId: string;
    sessionDate: string;
  }>();
  return <SessionScreen classGroupId={classGroupId} sessionDate={sessionDate} />;
}
