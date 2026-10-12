import { useLocalSearchParams } from "expo-router";
import { ClassTestDayScreen } from "@/features/measurements/screens/ClassTestDayScreen";

export default function ClassTestDayRoute() {
  const { classGroupId, date } = useLocalSearchParams<{ classGroupId: string; date: string }>();
  return <ClassTestDayScreen classGroupId={classGroupId} sessionDate={date} />;
}
