import { useLocalSearchParams } from "expo-router";
import { OrderDetailScreen } from "@/features/orders/screens/OrderDetailScreen";

export default function OrderRoute() {
  const { orderId } = useLocalSearchParams<{ orderId: string }>();
  return <OrderDetailScreen orderId={orderId} />;
}
