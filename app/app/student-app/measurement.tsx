import { useLocalSearchParams } from "expo-router";
import { StudentAppMeasurementScreen } from "@/features/measurements/screens/StudentAppMeasurementScreen";

export default function StudentAppMeasurementRoute() {
  const { studentId, measurementId } = useLocalSearchParams<{
    studentId: string;
    measurementId: string;
  }>();
  return <StudentAppMeasurementScreen studentId={studentId} measurementId={measurementId} />;
}
