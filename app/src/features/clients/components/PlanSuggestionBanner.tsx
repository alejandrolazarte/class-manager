import { useRouter } from "expo-router";
import { View } from "react-native";
import { useClassBalance } from "@/features/classPacks/useClassPacks";
import { ClientDetails } from "@/features/clients/types";
import { StudentPlan } from "@/features/fees/types";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { useCanSellAtCounter } from "@/features/orders/useOrderPermissions";
import { translate } from "@/i18n/translate";
import { ClientTab, routes } from "@/navigation/routes";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";

interface PlanSuggestionBannerProps {
  client: ClientDetails;
  clientTab: ClientTab;
}

function hasNoPlan(studentPlan: StudentPlan): boolean {
  return (
    studentPlan.classPackId === null &&
    !studentPlan.upcomingChanges.some((change) => change.classPackId !== null)
  );
}

export function PlanSuggestionBanner({ client, clientTab }: PlanSuggestionBannerProps) {
  const router = useRouter();
  const canSellAtCounter = useCanSellAtCounter();
  const canRecordPayments = useCan(permissions.paymentsRecord);
  const canViewClassPacks = useCan(permissions.classPacksView);
  const { data: balance } = useClassBalance(canViewClassPacks ? client.id : undefined);
  const studentWithoutPlan = client.plans.students.find(hasNoPlan);
  const hasClassesLeft = (balance?.availableClasses ?? 0) > 0;
  const isBalanceKnown = !canViewClassPacks || balance !== undefined;
  if (
    studentWithoutPlan === undefined ||
    hasClassesLeft ||
    !isBalanceKnown ||
    !(canSellAtCounter || canRecordPayments)
  ) {
    return null;
  }

  const message =
    client.students.length > 1
      ? translate("clients.detail.studentHasNoPlanNorPacks", {
          name: studentWithoutPlan.studentFullName,
        })
      : translate("clients.detail.noPlanNorPacks");
  return (
    <Banner tone="info" message={message}>
      <View className="flex-row">
        {canSellAtCounter ? (
          <Button
            variant="secondary"
            size="medium"
            icon="cash"
            label={translate("orders.counterSale.charge")}
            onPress={() =>
              router.push(
                routes.clientCounterSale(clientTab, client.id, studentWithoutPlan.studentId),
              )
            }
          />
        ) : (
          <Button
            variant="secondary"
            size="medium"
            label={translate("clients.detail.choosePlan")}
            onPress={() =>
              router.push(routes.studentPlan(clientTab, client.id, studentWithoutPlan.studentId))
            }
          />
        )}
      </View>
    </Banner>
  );
}
