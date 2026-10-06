import { useState } from "react";
import { useRouter } from "expo-router";
import { Pressable, View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { ClassBalanceSection } from "@/features/classPacks/components/ClassBalanceSection";
import { ClientDetails } from "@/features/clients/types";
import { describeBillingPlan } from "@/features/fees/billingPlanDescription";
import { formatMoney } from "@/features/fees/money";
import { formatMonth, monthOf } from "@/features/fees/months";
import { BillingPlanChange } from "@/features/fees/types";
import {
  useDeleteClientBillingPlanChange,
  useDeletePayment,
} from "@/features/fees/useFeeMutations";
import { useCanUndoCollection } from "@/features/fees/useCanUndoCollection";
import { useClientPayments } from "@/features/fees/useFees";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { formatBirthDateForDisplay } from "@/features/students/birthDateFormatting";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { Card } from "@/ui/Card";
import { DeleteConfirmation } from "@/ui/DeleteConfirmation";
import { Icon } from "@/ui/Icon";
import { IconButton } from "@/ui/IconButton";
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
  const { showToast } = useToast();
  const [isShowingUpcomingChanges, setIsShowingUpcomingChanges] = useState(false);
  const [monthPendingDeletion, setMonthPendingDeletion] = useState<string | null>(null);
  const money = (amount: number) => formatMoney(amount, business.currencyCode);
  const describePlan = (plan: BillingPlanChange | ClientDetails["billingPlan"]) =>
    describeBillingPlan(plan, business.defaultMonthlyFee, business.currencyCode);
  const currentMonth = monthOf();
  const upcomingChanges = client.billingPlanChanges.filter(
    (change) => change.effectiveFrom > currentMonth,
  );
  const planStart = currentPlanStart(client.billingPlanChanges, currentMonth);
  const isMonthlyPlan = client.billingPlan.kind !== "ClassPacks";

  const deletePendingPlanChange = async (month: string) => {
    try {
      await deletePlanChangeMutation.mutateAsync(month);
      showToast(translate("fees.client.upcomingChangeDeleted"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
    setMonthPendingDeletion(null);
  };

  return (
    <View className="gap-3">
      <Card className="gap-3 p-4">
        <View className="flex-row items-start gap-3">
          <View className="min-w-0 flex-1 gap-0.5">
            <AppText variant="overline" tone="subtle">
              {translate("fees.client.planOverline")}
            </AppText>
            <AppText variant="headline">{describePlan(client.billingPlan)}</AppText>
            {planStart ? (
              <AppText variant="caption" tone="muted">
                {translate("fees.client.since", { month: formatMonth(planStart) })}
              </AppText>
            ) : null}
          </View>
          {canRecordPayments ? (
            <Pressable
              accessibilityRole="button"
              accessibilityLabel={translate("fees.client.changePlanAccessibility")}
              onPress={() => router.push(routes.clientBillingPlan(clientTab, client.id))}
              className="rounded-full bg-primary-soft px-3.5 py-2 active:opacity-80"
            >
              <AppText variant="link" tone="primarySoft">
                {translate("common.change")}
              </AppText>
            </Pressable>
          ) : null}
        </View>
        {upcomingChanges.length > 0 ? (
          <View className="gap-1.5">
            <Pressable
              accessibilityRole="button"
              accessibilityState={{ expanded: isShowingUpcomingChanges }}
              onPress={() => setIsShowingUpcomingChanges(!isShowingUpcomingChanges)}
              className="flex-row items-center gap-1 self-start py-1"
            >
              <AppText variant="link" tone="primary">
                {translateCount("fees.client.upcomingChanges", upcomingChanges.length)}
              </AppText>
              <Icon
                name={isShowingUpcomingChanges ? "collapse" : "expand"}
                size="medium"
                tone="primary"
              />
            </Pressable>
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
                        tone="subtle-foreground"
                        accessibilityLabel={translate("fees.client.deleteUpcomingChange", {
                          month: formatMonth(change.effectiveFrom),
                        })}
                        onPress={() => setMonthPendingDeletion(change.effectiveFrom)}
                      />
                    ) : null}
                  </View>
                ))
              : null}
            {monthPendingDeletion ? (
              <DeleteConfirmation
                question={translate("fees.client.deleteUpcomingChangeQuestion", {
                  month: formatMonth(monthPendingDeletion),
                })}
                onCancel={() => setMonthPendingDeletion(null)}
                onConfirm={() => deletePendingPlanChange(monthPendingDeletion)}
                isDeleting={deletePlanChangeMutation.isPending}
              />
            ) : null}
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
      </Card>
      {canViewClassPacks ? (
        <ClassBalanceSection clientId={client.id} isMonthlyPlan={isMonthlyPlan} />
      ) : null}
      <Card className="gap-3 p-4">
        <AppText variant="overline" tone="subtle">
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
                  tone="subtle-foreground"
                  accessibilityLabel={translate("fees.client.deletePaymentOf", {
                    amount: money(payment.amount),
                  })}
                  onPress={() => deletePaymentMutation.mutate(payment.id)}
                />
              ) : null}
            </View>
          ))
        )}
      </Card>
    </View>
  );
}
