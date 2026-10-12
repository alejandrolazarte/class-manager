import { useLocalSearchParams } from "expo-router";
import { StudentAppRecordMeasurementScreen } from "@/features/measurements/screens/StudentAppRecordMeasurementScreen";

export default function StudentAppRecordMeasurementRoute() {
  const { studentId } = useLocalSearchParams<{ studentId: string }>();
  return <StudentAppRecordMeasurementScreen studentId={studentId} />;
}
