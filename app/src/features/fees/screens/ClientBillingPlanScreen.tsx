import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { useClassBalance } from "@/features/classPacks/useClassPacks";
import { ClientDetails } from "@/features/clients/types";
import { useClient } from "@/features/clients/useClient";
import { EffectiveMonthPicker } from "@/features/fees/components/EffectiveMonthPicker";
import { currencySymbol, formatMoney, parseAmount, toAmountText } from "@/features/fees/money";
import { monthOf } from "@/features/fees/months";
import { BillingPlanKind } from "@/features/fees/types";
import { useSetClientBillingPlan } from "@/features/fees/useFeeMutations";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { LoadingScreen } from "@/ui/LoadingScreen";
import { OptionCard } from "@/ui/OptionCard";
import { ScreenFooter, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

const billingPlanKinds: readonly BillingPlanKind[] = ["BusinessFee", "CustomFee", "ClassPacks"];

interface ClientBillingPlanScreenProps {
  clientId: string;
}

interface BillingPlanEditorProps {
  client: ClientDetails;
}

function firstNameOf(fullName: string): string {
  return fullName.trim().split(/\s+/)[0] ?? fullName;
}

function BillingPlanEditor({ client }: BillingPlanEditorProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const business = useCurrentBusiness();
  const canViewClassPacks = useCan(permissions.classPacksView);
  const { data: balance } = useClassBalance(canViewClassPacks ? client.id : undefined);
  const setBillingPlanMutation = useSetClientBillingPlan(client.id);
  const [kind, setKind] = useState<BillingPlanKind>(client.billingPlan.kind);
  const [amountText, setAmountText] = useState(toAmountText(client.billingPlan.customFee));
  const [isAmountTouched, setIsAmountTouched] = useState(false);
  const [effectiveFrom, setEffectiveFrom] = useState(monthOf());
  const [hasFailed, setHasFailed] = useState(false);
  const customFee = parseAmount(amountText);
  const isAmountValid = customFee !== null && customFee > 0;
  const canSave = kind !== "CustomFee" || isAmountValid;
  const availableClasses = balance?.availableClasses ?? 0;

  const hintOf = (planKind: BillingPlanKind): string => {
    if (planKind !== "BusinessFee") {
      return translate(`fees.billingPlan.hint.${planKind}`);
    }
    return business.defaultMonthlyFee === null
      ? translate("fees.billingPlan.hint.BusinessFeeMissing")
      : translate("fees.billingPlan.hint.BusinessFee", {
          fee: formatMoney(business.defaultMonthlyFee, business.currencyCode),
        });
  };

  const savePlan = async () => {
    setHasFailed(false);
    try {
      await setBillingPlanMutation.mutateAsync({
        kind,
        customFee: kind === "CustomFee" ? customFee : null,
        effectiveFrom,
      });
      showToast(translate("fees.client.saved"));
      router.back();
    } catch {
      setHasFailed(true);
    }
  };

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="close" title={translate("fees.billingPlan.title")} />}
      footer={
        <ScreenFooter>
          <Button
            label={translate("common.save")}
            disabled={!canSave}
            onPress={savePlan}
            isLoading={setBillingPlanMutation.isPending}
          />
        </ScreenFooter>
      }
    >
      {hasFailed ? <Banner message={translate("common.unexpectedError")} /> : null}
      <View className="gap-2" accessibilityRole="radiogroup">
        <AppText variant="label" tone="muted">
          {translate("fees.billingPlan.question", { name: firstNameOf(client.fullName) })}
        </AppText>
        {billingPlanKinds.map((planKind) => (
          <OptionCard
            key={planKind}
            label={translate(`fees.plan.${planKind}`)}
            hint={hintOf(planKind)}
            isSelected={kind === planKind}
            onPress={() => setKind(planKind)}
          />
        ))}
      </View>
      {kind === "CustomFee" ? (
        <TextField
          label={translate("fees.client.ownFeeAmount")}
          isRequired
          prefix={currencySymbol(business.currencyCode)}
          keyboardType="decimal-pad"
          value={amountText}
          onChangeText={setAmountText}
          onBlur={() => setIsAmountTouched(true)}
          hint={translate("fees.billingPlan.perMonth")}
          errorMessage={
            isAmountTouched && !isAmountValid
              ? translate("fees.billingPlan.amountRequired")
              : undefined
          }
        />
      ) : null}
      <EffectiveMonthPicker
        label={translate("fees.billingPlan.from")}
        month={effectiveFrom}
        onChange={setEffectiveFrom}
      />
      {availableClasses > 0 ? (
        <Banner
          tone="success"
          message={
            kind === "ClassPacks"
              ? translateCount("fees.billingPlan.useClasses", availableClasses)
              : translateCount("fees.billingPlan.keepClasses", availableClasses)
          }
        />
      ) : null}
    </ScrollScreen>
  );
}

export function ClientBillingPlanScreen({ clientId }: ClientBillingPlanScreenProps) {
  const { data: client } = useClient(clientId);
  if (client === undefined) {
    return <LoadingScreen />;
  }
  return <BillingPlanEditor client={client} />;
}
