import { View } from "react-native";
import { formatMoney } from "@/features/fees/money";
import { FeatureList } from "@/features/subscriptions/components/FeatureList";
import { planCodes } from "@/features/subscriptions/subscriptionCodes";
import { planLabel } from "@/features/subscriptions/subscriptionLabels";
import { Plan } from "@/features/subscriptions/types";
import { usePlans } from "@/features/subscriptions/useSubscription";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { Spinner } from "@/ui/Spinner";
import { StatusPill } from "@/ui/StatusPill";
import { TitleWithPills } from "@/ui/TitleWithPills";

function priceLabel(plan: Plan): string {
  if (plan.durationInDays !== null && plan.listPrice === 0) {
    return translate("subscriptions.trialDuration", { days: plan.durationInDays });
  }
  if (plan.listPrice === null) {
    return translate("subscriptions.priceOnRequest");
  }
  return translate("subscriptions.pricePerMonth", {
    price: formatMoney(plan.listPrice, plan.currency),
  });
}

interface PlanCatalogProps {
  currentPlanCode?: string;
  includesTrial?: boolean;
}

export function PlanCatalog({ currentPlanCode, includesTrial = true }: PlanCatalogProps) {
  const plansQuery = usePlans();
  if (plansQuery.isPending) {
    return <Spinner className="mt-2" />;
  }
  if (plansQuery.isError) {
    return (
      <Banner message={translate("common.unexpectedError")}>
        <Button
          variant="secondary"
          size="medium"
          label={translate("common.retry")}
          onPress={() => plansQuery.refetch()}
        />
      </Banner>
    );
  }
  return (
    <View className="gap-3">
      {plansQuery.data
        .filter((plan) => includesTrial || plan.code !== planCodes.free)
        .map((plan) => (
          <Card key={plan.code} className="gap-2.5 px-4 py-3.5">
            <TitleWithPills title={planLabel(plan.code)}>
              {plan.code === currentPlanCode ? (
                <StatusPill label={translate("subscriptions.currentPlan")} tone="primary" isSmall />
              ) : null}
            </TitleWithPills>
            <AppText variant="bodyStrong" tone="primary">
              {priceLabel(plan)}
            </AppText>
            <FeatureList features={plan.features} />
          </Card>
        ))}
    </View>
  );
}
