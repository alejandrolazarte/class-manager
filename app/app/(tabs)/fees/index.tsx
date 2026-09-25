import { useLocalSearchParams } from "expo-router";
import { MonthlyFeesScreen } from "@/features/fees/screens/MonthlyFeesScreen";

export default function MonthlyFeesRoute() {
  const { month } = useLocalSearchParams<{ month?: string }>();
  return <MonthlyFeesScreen initialMonth={month} />;
}
