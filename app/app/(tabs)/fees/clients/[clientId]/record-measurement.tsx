import { useLocalSearchParams } from "expo-router";
import { RecordMeasurementScreen } from "@/features/measurements/screens/RecordMeasurementScreen";

export default function RecordMeasurementRoute() {
  const { clientId, studentId, measurementId } = useLocalSearchParams<{
    clientId: string;
    studentId: string;
    measurementId?: string;
  }>();
  return (
    <RecordMeasurementScreen
      clientId={clientId}
      studentId={studentId}
      measurementId={measurementId}
    />
  );
}
