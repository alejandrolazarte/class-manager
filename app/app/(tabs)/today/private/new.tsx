import { useLocalSearchParams } from "expo-router";
import { PrivateLessonFormScreen } from "@/features/privateLessons/screens/PrivateLessonFormScreen";

export default function NewPrivateLessonRoute() {
  const { date } = useLocalSearchParams<{ date?: string }>();
  return <PrivateLessonFormScreen initialDate={date} />;
}
