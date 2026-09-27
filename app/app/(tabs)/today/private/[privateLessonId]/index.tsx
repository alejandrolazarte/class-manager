import { useLocalSearchParams } from "expo-router";
import { PrivateLessonScreen } from "@/features/privateLessons/screens/PrivateLessonScreen";

export default function PrivateLessonRoute() {
  const { privateLessonId } = useLocalSearchParams<{ privateLessonId: string }>();
  return <PrivateLessonScreen privateLessonId={privateLessonId} />;
}
