import { useLocalSearchParams } from "expo-router";
import { monthOf } from "@/features/fees/months";
import { RecordPaymentScreen } from "@/features/fees/screens/RecordPaymentScreen";

export default function RecordPaymentRoute() {
  const { clientId, month } = useLocalSearchParams<{ clientId: string; month?: string }>();
  return <RecordPaymentScreen clientId={clientId} month={month ?? monthOf()} />;
}
