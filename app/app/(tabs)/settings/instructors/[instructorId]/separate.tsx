import { useLocalSearchParams } from "expo-router";
import { SeparateInstructorScreen } from "@/features/users/screens/SeparateInstructorScreen";

export default function SeparateInstructorRoute() {
  const { instructorId } = useLocalSearchParams<{ instructorId: string }>();
  return <SeparateInstructorScreen instructorId={instructorId} />;
}
