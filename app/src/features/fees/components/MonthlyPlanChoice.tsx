import { View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { ClassPack } from "@/features/classPacks/types";
import { monthlyPlanHint } from "@/features/fees/planDescriptions";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { OptionCard } from "@/ui/OptionCard";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";

export const paysWithClassPacks = "classPacks";

interface MonthlyPlanChoiceProps {
  monthlyPlans: readonly ClassPack[];
  choice: string | null;
  onChange: (choice: string) => void;
}

export function MonthlyPlanChoice({ monthlyPlans, choice, onChange }: MonthlyPlanChoiceProps) {
  const currencyCode = useBusinessCurrency();
  return (
    <View className="gap-2" accessibilityRole="radiogroup">
      <AppText variant="label" tone="muted">
        {withRequiredMark(translate("clients.register.planQuestion"))}
      </AppText>
      {monthlyPlans.map((plan) => (
        <OptionCard
          key={plan.id}
          label={plan.name}
          hint={monthlyPlanHint(plan, currencyCode)}
          isSelected={choice === plan.id}
          onPress={() => onChange(plan.id)}
        />
      ))}
      <OptionCard
        label={translate("fees.studentPlan.none")}
        hint={translate("fees.studentPlan.noneHint")}
        isSelected={choice === paysWithClassPacks}
        onPress={() => onChange(paysWithClassPacks)}
      />
      <AppText variant="caption" tone="muted">
        {translate("clients.register.planNote")}
      </AppText>
    </View>
  );
}
