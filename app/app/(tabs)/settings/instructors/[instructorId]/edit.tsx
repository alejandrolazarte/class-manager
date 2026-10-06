import { useLocalSearchParams } from "expo-router";
import { InstructorFormScreen } from "@/features/instructors/screens/InstructorFormScreen";

export default function EditInstructorRoute() {
  const { instructorId } = useLocalSearchParams<{ instructorId: string }>();
  return <InstructorFormScreen instructorId={instructorId} />;
}
