import { useLocalSearchParams } from "expo-router";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";
import { isWeekday } from "@/dates/weekdays";

export default function NewClassGroupRoute() {
  const { weekday } = useLocalSearchParams<{ weekday?: string }>();
  return <ClassGroupFormScreen initialWeekday={isWeekday(weekday) ? weekday : undefined} />;
}
