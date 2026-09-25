import { useLocalSearchParams } from "expo-router";
import { EnrollStudentScreen } from "@/features/enrollments/screens/EnrollStudentScreen";

export default function EnrollStudentRoute() {
  const { classGroupId } = useLocalSearchParams<{ classGroupId: string }>();
  return <EnrollStudentScreen classGroupId={classGroupId} />;
}
