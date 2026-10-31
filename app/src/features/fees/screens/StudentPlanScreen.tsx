import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { ClassPack } from "@/features/classPacks/types";
import { useClassBalance, useClassPacks } from "@/features/classPacks/useClassPacks";
import { ClientDetails } from "@/features/clients/types";
import { useClient } from "@/features/clients/useClient";
import { EffectiveMonthPicker } from "@/features/fees/components/EffectiveMonthPicker";
import { monthOf } from "@/features/fees/months";
import { monthlyPlanHint } from "@/features/fees/planDescriptions";
import { StudentPlan } from "@/features/fees/types";
import { useSetStudentPlan } from "@/features/fees/useFeeMutations";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { currencySymbol, parseAmount, toAmountText } from "@/formatting/money";
import { translate, translateCount } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Notice } from "@/ui/Notice";
import { OptionCard } from "@/ui/OptionCard";
import { RequiredFieldsLegend, withRequiredMark } from "@/ui/RequiredFieldsLegend";
import { ScreenFooter, ScrollScreen } from "@/ui/Screen";
import { ScreenHeader } from "@/ui/ScreenHeader";
import { SkeletonScreen } from "@/ui/SkeletonScreen";
import { TextField } from "@/ui/TextField";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { useToast } from "@/ui/ToastProvider";

const noPlan = "none";

interface StudentPlanScreenProps {
  clientId: string;
  studentId: string;
}

interface StudentPlanEditorProps {
  client: ClientDetails;
  studentPlan: StudentPlan;
  monthlyPlans: ClassPack[];
}

function firstNameOf(fullName: string): string {
  return fullName.trim().split(/\s+/)[0] ?? fullName;
}

function StudentPlanEditor({ client, studentPlan, monthlyPlans }: StudentPlanEditorProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const currencyCode = useBusinessCurrency();
  const canViewClassPacks = useCan(permissions.classPacksView);
  const { data: balance } = useClassBalance(canViewClassPacks ? client.id : undefined);
  const setStudentPlanMutation = useSetStudentPlan(client.id);
  const [planId, setPlanId] = useState<string>(studentPlan.classPackId ?? noPlan);
  const [hasCustomPrice, setHasCustomPrice] = useState(studentPlan.customPrice !== null);
  const [customPriceText, setCustomPriceText] = useState(toAmountText(studentPlan.customPrice));
  const [isCustomPriceTouched, setIsCustomPriceTouched] = useState(false);
  const [effectiveFrom, setEffectiveFrom] = useState(monthOf());
  const [hasFailed, setHasFailed] = useState(false);
  const hasPlan = planId !== noPlan;
  const customPrice = parseAmount(customPriceText);
  const isCustomPriceValid = customPrice !== null && customPrice >= 0;
  const canSave = !hasPlan || !hasCustomPrice || isCustomPriceValid;
  const availableClasses = balance?.availableClasses ?? 0;

  const save = async () => {
    setHasFailed(false);
    try {
      await setStudentPlanMutation.mutateAsync({
        studentId: studentPlan.studentId,
        request: {
          classPackId: hasPlan ? planId : null,
          customPrice: hasPlan && hasCustomPrice ? customPrice : null,
          effectiveFrom,
        },
      });
      showToast(translate("fees.studentPlan.saved"));
      router.back();
    } catch {
      setHasFailed(true);
    }
  };

  return (
    <ScrollScreen
      header={<ScreenHeader navigation="close" title={translate("fees.studentPlan.title")} />}
      footer={
        <ScreenFooter>
          <Button
            label={translate("common.save")}
            disabled={!canSave}
            onPress={save}
            isLoading={setStudentPlanMutation.isPending}
          />
        </ScreenFooter>
      }
    >
      {hasFailed ? <Banner message={translate("common.unexpectedError")} /> : null}
      <RequiredFieldsLegend />
      <View className="gap-2" accessibilityRole="radiogroup">
        <AppText variant="label" tone="muted">
          {withRequiredMark(
            translate("fees.studentPlan.question", {
              name: firstNameOf(studentPlan.studentFullName),
            }),
          )}
        </AppText>
        {monthlyPlans.map((plan) => (
          <OptionCard
            key={plan.id}
            label={plan.name}
            hint={monthlyPlanHint(plan, currencyCode)}
            isSelected={planId === plan.id}
            onPress={() => setPlanId(plan.id)}
          />
        ))}
        <OptionCard
          label={translate("fees.studentPlan.none")}
          hint={translate("fees.studentPlan.noneHint")}
          isSelected={!hasPlan}
          onPress={() => setPlanId(noPlan)}
        />
      </View>
      {monthlyPlans.length === 0 ? (
        <Notice tone="muted" message={translate("fees.studentPlan.noPlansYet")} />
      ) : null}
      {hasPlan ? (
        <View className="gap-2">
          <ToggleSwitch
            label={translate("fees.studentPlan.customPrice")}
            hint={translate("fees.studentPlan.customPriceHint")}
            value={hasCustomPrice}
            onValueChange={setHasCustomPrice}
          />
          {hasCustomPrice ? (
            <TextField
              label={translate("fees.studentPlan.customPriceAmount")}
              isRequired
              prefix={currencySymbol(currencyCode)}
              keyboardType="decimal-pad"
              value={customPriceText}
              onChangeText={setCustomPriceText}
              onBlur={() => setIsCustomPriceTouched(true)}
              errorMessage={
                isCustomPriceTouched && !isCustomPriceValid
                  ? translate("fees.billingPlan.amountRequired")
                  : undefined
              }
            />
          ) : null}
        </View>
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
            hasPlan
              ? translateCount("fees.billingPlan.keepClasses", availableClasses)
              : translateCount("fees.billingPlan.useClasses", availableClasses)
          }
        />
      ) : null}
    </ScrollScreen>
  );
}

export function StudentPlanScreen({ clientId, studentId }: StudentPlanScreenProps) {
  const { data: client } = useClient(clientId);
  const { data: catalog } = useClassPacks(false);
  const studentPlan = client?.plans.students.find((plan) => plan.studentId === studentId);
  if (client === undefined || catalog === undefined || studentPlan === undefined) {
    return <SkeletonScreen />;
  }
  return (
    <StudentPlanEditor
      client={client}
      studentPlan={studentPlan}
      monthlyPlans={catalog.filter((classPack) => classPack.renewsMonthly)}
    />
  );
}
