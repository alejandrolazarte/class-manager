import { useState } from "react";
import { useRouter } from "expo-router";
import { View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { ClassBalanceSection } from "@/features/classPacks/components/ClassBalanceSection";
import { ClientDetails } from "@/features/clients/types";
import { describeBillingPlan } from "@/features/fees/billingPlanDescription";
import { formatMoney } from "@/formatting/money";
import { formatMonth, monthOf } from "@/features/fees/months";
import { BillingPlanChange, Payment } from "@/features/fees/types";
import {
  useDeleteClientBillingPlanChange,
  useDeletePayment,
  useRestorePayment,
  useSetClientBillingPlan,
} from "@/features/fees/useFeeMutations";
import { useCanUndoCollection } from "@/features/fees/useCanUndoCollection";
import { useClientPayments } from "@/features/fees/useFees";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { formatBirthDateForDisplay } from "@/dates/birthDateFormatting";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { SectionTitle } from "@/ui/SectionTitle";
import { IconButton } from "@/ui/IconButton";
import { TextLink } from "@/ui/TextLink";
import { useToast } from "@/ui/ToastProvider";
import { clientTabs, routes } from "@/navigation/routes";
import { useCurrentTab } from "@/navigation/useCurrentTab";

interface ClientFeeSectionProps {
  client: ClientDetails;
}

function currentPlanStart(
  changes: readonly BillingPlanChange[],
  currentMonth: string,
): string | null {
  return changes.reduce<string | null>(
    (latestStart, change) =>
      change.effectiveFrom <= currentMonth &&
      (latestStart === null || change.effectiveFrom > latestStart)
        ? change.effectiveFrom
        : latestStart,
    null,
  );
}

export function ClientFeeSection({ client }: ClientFeeSectionProps) {
  const router = useRouter();
  const clientTab = useCurrentTab(clientTabs);
  const business = useCurrentBusiness();
  const { data: payments = [] } = useClientPayments(client.id);
  const canRecordPayments = useCan(permissions.paymentsRecord);
  const canViewClassPacks = useCan(permissions.classPacksView);
  const canUndoPayment = useCanUndoCollection(permissions.paymentsRecord);
  const deletePaymentMutation = useDeletePayment();
  const deletePlanChangeMutation = useDeleteClientBillingPlanChange(client.id);
  const restorePaymentMutation = useRestorePayment();
  const setPlanMutation = useSetClientBillingPlan(client.id);
  const { showToast } = useToast();
  const [isShowingUpcomingChanges, setIsShowingUpcomingChanges] = useState(false);
  const describePlan = (plan: BillingPlanChange | ClientDetails["billingPlan"]) =>
    describeBillingPlan(plan, business.defaultMonthlyFee, business.currencyCode);
  const currentMonth = monthOf();
  const upcomingChanges = client.billingPlanChanges.filter(
    (change) => change.effectiveFrom > currentMonth,
  );
  const planStart = currentPlanStart(client.billingPlanChanges, currentMonth);
  const isMonthlyPlan = client.billingPlan.kind !== "ClassPacks";

  const undo = (restore: () => Promise<unknown>) => ({
    label: translate("common.undo"),
    onPress: () => {
      restore().catch(() => showToast(translate("common.undoFailed")));
    },
  });

  const deletePlanChange = async (change: BillingPlanChange) => {
    try {
      await deletePlanChangeMutation.mutateAsync(change.effectiveFrom);
      showToast(
        translate("fees.client.upcomingChangeDeleted"),
        undo(() =>
          setPlanMutation.mutateAsync({
            effectiveFrom: change.effectiveFrom,
            kind: change.kind,
            customFee: change.customFee,
          }),
        ),
      );
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  const deletePayment = async (payment: Payment) => {
    try {
      await deletePaymentMutation.mutateAsync(payment.id);
      showToast(
        translate("fees.client.paymentDeleted"),
        undo(() => restorePaymentMutation.mutateAsync(payment.id)),
      );
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  return (
    <View className="gap-4">
      <View className="gap-2.5 border-b border-border-subtle pb-3">
        <View className="flex-row items-start gap-3">
          <View className="min-w-0 flex-1 gap-0.5">
            <SectionTitle title={translate("fees.client.planOverline")} isOverline />
            <AppText variant="body">{describePlan(client.billingPlan)}</AppText>
            {planStart ? (
              <AppText variant="caption" tone="muted">
                {translate("fees.client.since", { month: formatMonth(planStart) })}
              </AppText>
            ) : null}
          </View>
          {canRecordPayments ? (
            <Button
              variant="ghost"
              size="small"
              label={translate("common.change")}
              accessibilityLabel={translate("fees.client.changePlanAccessibility")}
              onPress={() => router.push(routes.clientBillingPlan(clientTab, client.id))}
            />
          ) : null}
        </View>
        {upcomingChanges.length > 0 ? (
          <View className="gap-1.5">
            <TextLink
              label={translateCount("fees.client.upcomingChanges", upcomingChanges.length)}
              trailingIcon={isShowingUpcomingChanges ? "collapse" : "expand"}
              isExpanded={isShowingUpcomingChanges}
              onPress={() => setIsShowingUpcomingChanges(!isShowingUpcomingChanges)}
            />
            {isShowingUpcomingChanges
              ? upcomingChanges.map((change) => (
                  <View key={change.effectiveFrom} className="flex-row items-center gap-2">
                    <AppText variant="caption" tone="subtle" className="min-w-0 flex-1">
                      {translate("fees.client.upcomingChange", {
                        month: formatMonth(change.effectiveFrom),
                        plan: describePlan(change),
                      })}
                    </AppText>
                    {canRecordPayments ? (
                      <IconButton
                        icon="delete"
                        tone="muted-foreground"
                        accessibilityLabel={translate("fees.client.deleteUpcomingChange", {
                          month: formatMonth(change.effectiveFrom),
                        })}
                        disabled={deletePlanChangeMutation.isPending}
                        onPress={() => deletePlanChange(change)}
                      />
                    ) : null}
                  </View>
                ))
              : null}
          </View>
        ) : null}
        {isMonthlyPlan && canRecordPayments ? (
          <Button
            size="medium"
            icon="cash"
            label={translate("fees.client.recordPayment")}
            onPress={() => router.push(routes.recordPayment(clientTab, client.id, currentMonth))}
          />
        ) : null}
      </View>
      {canViewClassPacks ? (
        <ClassBalanceSection clientId={client.id} isMonthlyPlan={isMonthlyPlan} />
      ) : null}
      <View>
        <SectionTitle title={translate("fees.client.payments")} isOverline />
        {payments.length === 0 ? (
          <AppText variant="body" tone="muted" className="py-2.5">
            {translate("fees.client.noPayments")}
          </AppText>
        ) : (
          payments.map((payment) => (
            <HairlineRow key={payment.id} className="flex-row items-center gap-2 py-2">
              <View className="min-w-0 flex-1">
                <AppText variant="bodyStrong">
                  {formatMoney(payment.amount, payment.currencyCode)}
                </AppText>
                <AppText variant="footnote" tone="subtle">
                  {[
                    formatBirthDateForDisplay(payment.paidOn),
                    translate(`fees.methods.${payment.method}` as TranslationKey),
                    ...(payment.recordedByFullName
                      ? [translate("fees.client.recordedBy", { name: payment.recordedByFullName })]
                      : []),
                  ].join(" · ")}
                </AppText>
              </View>
              <AppText variant="label" tone="muted">
                {formatMonth(payment.month)}
              </AppText>
              {canUndoPayment(payment.recordedByUserId) ? (
                <IconButton
                  icon="delete"
                  tone="muted-foreground"
                  accessibilityLabel={translate("fees.client.deletePaymentOf", {
                    amount: formatMoney(payment.amount, payment.currencyCode),
                  })}
                  disabled={deletePaymentMutation.isPending}
                  onPress={() => deletePayment(payment)}
                />
              ) : null}
            </HairlineRow>
          ))
        )}
      </View>
    </View>
  );
}
