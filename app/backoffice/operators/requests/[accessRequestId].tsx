import { useLocalSearchParams } from "expo-router";
import { EnableOperatorScreen } from "@/backoffice/operators/EnableOperatorScreen";

export default function BackofficeEnableOperatorRoute() {
  const { accessRequestId } = useLocalSearchParams<{ accessRequestId: string }>();
  return <EnableOperatorScreen accessRequestId={accessRequestId} />;
}
