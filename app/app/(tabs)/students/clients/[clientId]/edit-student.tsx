import { useLocalSearchParams } from "expo-router";
import { EditStudentScreen } from "@/features/students/screens/EditStudentScreen";

export default function EditStudentRoute() {
  const { clientId, studentId } = useLocalSearchParams<{ clientId: string; studentId: string }>();
  return <EditStudentScreen clientId={clientId} studentId={studentId} />;
}
