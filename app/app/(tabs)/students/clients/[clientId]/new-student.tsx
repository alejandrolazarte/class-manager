import { useLocalSearchParams } from "expo-router";
import { AddStudentScreen } from "@/features/students/screens/AddStudentScreen";

export default function AddStudentRoute() {
  const { clientId } = useLocalSearchParams<{ clientId: string }>();
  return <AddStudentScreen clientId={clientId} />;
}
