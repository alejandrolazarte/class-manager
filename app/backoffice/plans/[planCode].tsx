import { useLocalSearchParams } from "expo-router";
import { PlanSalesFeeScreen } from "@/backoffice/salesFees/PlanSalesFeeScreen";

export default function BackofficePlanSalesFeeRoute() {
  const { planCode } = useLocalSearchParams<{ planCode: string }>();
  return <PlanSalesFeeScreen planCode={planCode} />;
}
