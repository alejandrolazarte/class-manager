import { useLocalSearchParams } from "expo-router";
import { PrivateLessonFormScreen } from "@/features/privateLessons/screens/PrivateLessonFormScreen";

export default function EditPrivateLessonRoute() {
  const { privateLessonId } = useLocalSearchParams<{ privateLessonId: string }>();
  return <PrivateLessonFormScreen privateLessonId={privateLessonId} />;
}
