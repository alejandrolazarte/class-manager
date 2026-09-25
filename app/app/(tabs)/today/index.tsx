import { useLocalSearchParams } from "expo-router";
import { DayScreen } from "@/features/sessions/screens/DayScreen";

export default function DayRoute() {
  const { date } = useLocalSearchParams<{ date?: string }>();
  return <DayScreen initialDate={date} />;
}
