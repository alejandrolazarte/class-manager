import { useLocalSearchParams } from "expo-router";
import { CollectionsScreen, CollectionsView } from "@/features/fees/screens/CollectionsScreen";

const ordersView: CollectionsView = "orders";

export default function CollectionsRoute() {
  const { month, view } = useLocalSearchParams<{ month?: string; view?: string }>();
  return (
    <CollectionsScreen
      initialMonth={month}
      initialView={view === ordersView ? ordersView : "fees"}
    />
  );
}
