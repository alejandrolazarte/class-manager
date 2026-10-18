import { useLocalSearchParams } from "expo-router";
import { ClassMeasurementsScreen } from "@/features/measurements/screens/ClassMeasurementsScreen";

export default function ClassMeasurementsRoute() {
  const { classGroupId, date } = useLocalSearchParams<{ classGroupId: string; date: string }>();
  return <ClassMeasurementsScreen classGroupId={classGroupId} sessionDate={date} />;
}
