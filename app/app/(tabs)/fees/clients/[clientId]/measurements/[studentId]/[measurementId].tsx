import { useLocalSearchParams } from "expo-router";
import { StudentMeasurementScreen } from "@/features/measurements/screens/StudentMeasurementScreen";

export default function StudentMeasurementRoute() {
  const { clientId, studentId, measurementId } = useLocalSearchParams<{
    clientId: string;
    studentId: string;
    measurementId: string;
  }>();
  return (
    <StudentMeasurementScreen
      clientId={clientId}
      studentId={studentId}
      measurementId={measurementId}
    />
  );
}
