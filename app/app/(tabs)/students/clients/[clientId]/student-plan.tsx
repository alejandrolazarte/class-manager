import { useLocalSearchParams } from "expo-router";
import { StudentPlanScreen } from "@/features/fees/screens/StudentPlanScreen";

export default function StudentPlanRoute() {
  const { clientId, studentId } = useLocalSearchParams<{ clientId: string; studentId: string }>();
  return <StudentPlanScreen clientId={clientId} studentId={studentId} />;
}
