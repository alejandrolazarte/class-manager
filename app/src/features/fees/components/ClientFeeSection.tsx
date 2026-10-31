import { useRouter } from "expo-router";
import { View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { ClassBalanceSection } from "@/features/classPacks/components/ClassBalanceSection";
import { ClientDetails } from "@/features/clients/types";
import { describeStudentPlan, describeStudentPlanChange } from "@/features/fees/planDescriptions";
import { formatMoney } from "@/formatting/money";
import { formatMonth, monthOf } from "@/features/fees/months";
import { Payment, StudentPlan, StudentPlanChange } from "@/features/fees/types";
import {
  useDeletePayment,
  useDeleteStudentPlanChange,
  useRestorePayment,
  useSetStudentPlan,
} from "@/features/fees/useFeeMutations";
import { useCanUndoCollection } from "@/features/fees/useCanUndoCollection";
import { useClientPayments } from "@/features/fees/useFees";
import { useCan } from "@/features/members/CurrentMemberProvider";
import { permissions } from "@/features/members/permissions";
import { formatBirthDateForDisplay } from "@/dates/birthDateFormatting";
import { translate, TranslationKey } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { HairlineRow } from "@/ui/HairlineRow";
import { SectionTitle } from "@/ui/SectionTitle";
import { IconButton } from "@/ui/IconButton";
import { useToast } from "@/ui/ToastProvider";
import { clientTabs, routes } from "@/navigation/routes";
import { useCurrentTab } from "@/navigation/useCurrentTab";

interface ClientFeeSectionProps {
  client: ClientDetails;
}

export function ClientFeeSection({ client }: ClientFeeSectionProps) {
  const router = useRouter();
  const clientTab = useCurrentTab(clientTabs);
  const currencyCode = useBusinessCurrency();
  const { data: payments = [] } = useClientPayments(client.id);
  const canRecordPayments = useCan(permissions.paymentsRecord);
  const canViewClassPacks = useCan(permissions.classPacksView);
  const canUndoPayment = useCanUndoCollection(permissions.paymentsRecord);
  const deletePaymentMutation = useDeletePayment();
  const deletePlanChangeMutation = useDeleteStudentPlanChange(client.id);
  const restorePaymentMutation = useRestorePayment();
  const setPlanMutation = useSetStudentPlan(client.id);
  const { showToast } = useToast();
  const currentMonth = monthOf();
  const isMonthlyPlan = client.plans.students.some(
    (studentPlan) => studentPlan.classPackId !== null,
  );

  const undo = (restore: () => Promise<unknown>) => ({
    label: translate("common.undo"),
    onPress: () => {
      restore().catch(() => showToast(translate("common.undoFailed")));
    },
  });

  const deletePlanChange = async (studentPlan: StudentPlan, change: StudentPlanChange) => {
    try {
      await deletePlanChangeMutation.mutateAsync({
        studentId: studentPlan.studentId,
        month: change.effectiveFrom,
      });
      showToast(
        translate("fees.client.upcomingChangeDeleted"),
        undo(() =>
          setPlanMutation.mutateAsync({
            studentId: studentPlan.studentId,
            request: {
              classPackId: change.classPackId,
              customPrice: change.customPrice,
              effectiveFrom: change.effectiveFrom,
            },
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
        <SectionTitle title={translate("fees.client.planOverline")} isOverline />
        {client.plans.students.map((studentPlan) => (
          <View key={studentPlan.studentId} className="gap-1">
            <View className="flex-row items-start gap-3">
              <View className="min-w-0 flex-1 gap-0.5">
                <AppText variant="bodyStrong">{studentPlan.studentFullName}</AppText>
                <AppText variant="body">{describeStudentPlan(studentPlan, currencyCode)}</AppText>
                {studentPlan.since && studentPlan.classPackId !== null ? (
                  <AppText variant="caption" tone="muted">
                    {translate("fees.client.since", { month: formatMonth(studentPlan.since) })}
                  </AppText>
                ) : null}
              </View>
              {canRecordPayments ? (
                <Button
                  variant="ghost"
                  size="small"
                  label={translate("common.change")}
                  accessibilityLabel={translate("fees.client.changeStudentPlanAccessibility", {
                    name: studentPlan.studentFullName,
                  })}
                  onPress={() =>
                    router.push(routes.studentPlan(clientTab, client.id, studentPlan.studentId))
                  }
                />
              ) : null}
            </View>
            {studentPlan.upcomingChanges.map((change) => (
              <View key={change.effectiveFrom} className="flex-row items-center gap-2">
                <AppText variant="caption" tone="subtle" className="min-w-0 flex-1">
                  {translate("fees.client.upcomingChange", {
                    month: formatMonth(change.effectiveFrom),
                    plan: describeStudentPlanChange(change, currencyCode),
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
                    onPress={() => deletePlanChange(studentPlan, change)}
                  />
                ) : null}
              </View>
            ))}
          </View>
        ))}
        {client.plans.monthlyDue !== null ? (
          <AppText variant="bodyStrong">
            {translate("fees.client.monthTotal", {
              amount: formatMoney(client.plans.monthlyDue, currencyCode),
            })}
          </AppText>
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
