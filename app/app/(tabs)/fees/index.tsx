import { useLocalSearchParams } from "expo-router";
import { CollectionsScreen } from "@/features/fees/screens/CollectionsScreen";

export default function CollectionsRoute() {
  const { month } = useLocalSearchParams<{ month?: string }>();
  return <CollectionsScreen initialMonth={month} />;
}
