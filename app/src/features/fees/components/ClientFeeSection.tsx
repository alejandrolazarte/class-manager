import { useState } from "react";
import { useRouter } from "expo-router";
import { View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { ClassBalanceSection } from "@/features/classPacks/components/ClassBalanceSection";
import { ClientDetails } from "@/features/clients/types";
import { describeBillingPlan } from "@/features/fees/billingPlanDescription";
import { EffectiveMonthPicker } from "@/features/fees/components/EffectiveMonthPicker";
import { formatMoney, parseAmount, toAmountText } from "@/features/fees/money";
import { formatMonth, monthOf } from "@/features/fees/months";
import { BillingPlanKind } from "@/features/fees/types";
import { useDeletePayment, useSetClientBillingPlan } from "@/features/fees/useFeeMutations";
import { useClientPayments } from "@/features/fees/useFees";
import { formatBirthDateForDisplay } from "@/features/students/birthDateFormatting";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";
import { Card } from "@/ui/Card";
import { IconButton } from "@/ui/IconButton";
import { SectionTitle } from "@/ui/SectionTitle";
import { routes } from "@/navigation/routes";

const billingPlanKinds: readonly BillingPlanKind[] = ["BusinessFee", "CustomFee", "ClassPacks"];

interface ClientFeeSectionProps {
  client: ClientDetails;
}

export function ClientFeeSection({ client }: ClientFeeSectionProps) {
  const router = useRouter();
  const business = useCurrentBusiness();
  const { showToast } = useToast();
  const { data: payments = [] } = useClientPayments(client.id);
  const setBillingPlanMutation = useSetClientBillingPlan(client.id);
  const deletePaymentMutation = useDeletePayment();
  const [isEditing, setIsEditing] = useState(false);
  const [kind, setKind] = useState<BillingPlanKind>(client.billingPlan.kind);
  const [amountText, setAmountText] = useState(toAmountText(client.billingPlan.customFee));
  const [effectiveFrom, setEffectiveFrom] = useState(monthOf());
  const [hasFailed, setHasFailed] = useState(false);
  const money = (amount: number) => formatMoney(amount, business.currencyCode);
  const currentMonth = monthOf();
  const upcomingChanges = client.billingPlanChanges.filter(
    (change) => change.effectiveFrom > currentMonth,
  );
  const customFee = parseAmount(amountText);
  const canSave = kind !== "CustomFee" || (customFee !== null && customFee > 0);

  const savePlan = async () => {
    setHasFailed(false);
    try {
      await setBillingPlanMutation.mutateAsync({
        kind,
        customFee: kind === "CustomFee" ? customFee : null,
        effectiveFrom,
      });
      showToast(translate("fees.client.saved"));
      setIsEditing(false);
    } catch {
      setHasFailed(true);
    }
  };

  return (
    <View className="gap-4">
      <SectionTitle title={translate("fees.client.title")} />
      {hasFailed ? <Banner message={translate("common.unexpectedError")} /> : null}
      <Card className="gap-3 p-4">
        <View className="gap-0.5">
          <AppText variant="title">
            {describeBillingPlan(
              client.billingPlan,
              business.defaultMonthlyFee,
              business.currencyCode,
            )}
          </AppText>
          {upcomingChanges.map((change) => (
            <AppText key={change.effectiveFrom} variant="caption" tone="subtle">
              {translate("fees.client.upcomingChange", {
                month: formatMonth(change.effectiveFrom),
                plan: describeBillingPlan(
                  change,
                  business.defaultMonthlyFee,
                  business.currencyCode,
                ),
              })}
            </AppText>
          ))}
        </View>
        {client.billingPlan.kind === "ClassPacks" ? null : (
          <Button
            size="medium"
            icon="cash"
            label={translate("fees.client.recordPayment")}
            onPress={() => router.push(routes.recordPayment(client.id, currentMonth))}
          />
        )}
        {isEditing ? (
          <View className="gap-3 rounded-2xl bg-muted p-3">
            <AppText variant="label" tone="muted">
              {translate("fees.client.planKind")}
            </AppText>
            <View className="flex-row flex-wrap gap-2">
              {billingPlanKinds.map((planKind) => (
                <Chip
                  key={planKind}
                  label={translate(`fees.plan.${planKind}` as TranslationKey)}
                  isSelected={kind === planKind}
                  onPress={() => setKind(planKind)}
                />
              ))}
            </View>
            {kind === "CustomFee" ? (
              <TextField
                label={translate("fees.client.ownFeeAmount")}
                keyboardType="decimal-pad"
                value={amountText}
                onChangeText={setAmountText}
              />
            ) : null}
            <EffectiveMonthPicker month={effectiveFrom} onChange={setEffectiveFrom} />
            <Button
              size="medium"
              label={translate("common.save")}
              disabled={!canSave}
              onPress={savePlan}
              isLoading={setBillingPlanMutation.isPending}
            />
          </View>
        ) : (
          <Button
            variant="secondary"
            size="medium"
            label={translate("fees.client.changeFee")}
            onPress={() => setIsEditing(true)}
          />
        )}
        <AppText variant="overline" tone="subtle" className="mt-0.5">
          {translate("fees.client.payments")}
        </AppText>
        {payments.length === 0 ? (
          <AppText variant="body" tone="muted">
            {translate("fees.client.noPayments")}
          </AppText>
        ) : (
          payments.map((payment) => (
            <View key={payment.id} className="flex-row items-center gap-2">
              <View className="min-w-0 flex-1 gap-px">
                <AppText variant="bodyStrong">{money(payment.amount)}</AppText>
                <AppText variant="caption" tone="subtle">
                  {`${formatBirthDateForDisplay(payment.paidOn)} · ${translate(`fees.methods.${payment.method}` as TranslationKey)}`}
                </AppText>
              </View>
              <AppText variant="label" tone="muted">
                {formatMonth(payment.month)}
              </AppText>
              <IconButton
                icon="delete"
                tone="subtle-foreground"
                accessibilityLabel={translate("fees.client.deletePaymentOf", {
                  amount: money(payment.amount),
                })}
                onPress={() => deletePaymentMutation.mutate(payment.id)}
              />
            </View>
          ))
        )}
      </Card>
      {client.billingPlan.kind === "ClassPacks" ? (
        <ClassBalanceSection clientId={client.id} />
      ) : null}
    </View>
  );
}
