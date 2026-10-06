import { useLocalSearchParams } from "expo-router";
import { InstructorDetailScreen } from "@/features/instructors/screens/InstructorDetailScreen";

export default function InstructorDetailRoute() {
  const { instructorId } = useLocalSearchParams<{ instructorId: string }>();
  return <InstructorDetailScreen instructorId={instructorId} />;
}
