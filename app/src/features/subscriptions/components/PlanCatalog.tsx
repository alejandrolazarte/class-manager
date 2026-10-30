import { View } from "react-native";
import { formatMoney } from "@/formatting/money";
import { FeatureList } from "@/features/subscriptions/components/FeatureList";
import { planCodes, selfServicePlanCodes } from "@/features/subscriptions/subscriptionCodes";
import { planLabel } from "@/features/subscriptions/subscriptionLabels";
import { Plan } from "@/features/subscriptions/types";
import { usePlans } from "@/features/subscriptions/useSubscription";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Spinner } from "@/ui/Spinner";
import { StatusPill } from "@/ui/StatusPill";
import { TitleWithPills } from "@/ui/TitleWithPills";
import { HairlineRow } from "@/ui/HairlineRow";

function priceLabel(plan: Plan, pricesPlusVat: boolean): string {
  if (plan.durationInDays !== null && plan.listPrice === 0) {
    return translate("subscriptions.trialDuration", { days: plan.durationInDays });
  }
  if (plan.listPrice === null) {
    return translate("subscriptions.priceOnRequest");
  }
  const price = formatMoney(plan.listPrice, plan.currency);
  return pricesPlusVat
    ? translate("subscriptions.pricePerMonthPlusVat", { price })
    : translate("subscriptions.pricePerMonth", { price });
}

interface PlanCatalogProps {
  currentPlanCode?: string;
  includesTrial?: boolean;
  onChoosePlan?: (planCode: string) => void;
  choosingPlanCode?: string | null;
}

export function PlanCatalog({
  currentPlanCode,
  includesTrial = true,
  onChoosePlan,
  choosingPlanCode = null,
}: PlanCatalogProps) {
  const canChoose = onChoosePlan !== undefined;
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
    <View>
      {plansQuery.data
        .filter((plan) => includesTrial || plan.code !== planCodes.free)
        .map((plan) => (
          <HairlineRow key={plan.code} className="gap-2.5 py-3.5">
            <TitleWithPills title={planLabel(plan.code)}>
              {plan.code === currentPlanCode ? (
                <StatusPill label={translate("subscriptions.currentPlan")} tone="primary" isSmall />
              ) : null}
            </TitleWithPills>
            <AppText variant="bodyStrong" tone="primary">
              {priceLabel(plan, canChoose && selfServicePlanCodes.includes(plan.code))}
            </AppText>
            <FeatureList features={plan.features} />
            {onChoosePlan && selfServicePlanCodes.includes(plan.code) ? (
              <Button
                size="small"
                label={translate("subscriptions.choosePlan")}
                accessibilityLabel={translate("subscriptions.choosePlanNamed", {
                  plan: planLabel(plan.code),
                })}
                isLoading={choosingPlanCode === plan.code}
                disabled={choosingPlanCode !== null}
                onPress={() => onChoosePlan(plan.code)}
              />
            ) : null}
          </HairlineRow>
        ))}
    </View>
  );
}
