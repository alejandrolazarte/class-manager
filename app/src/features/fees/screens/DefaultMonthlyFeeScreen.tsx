import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { hasChosenBillingMode } from "@/features/fees/billingMode";
import { BillingModeChoice } from "@/features/fees/components/BillingModeChoice";
import { EffectiveMonthPicker } from "@/features/fees/components/EffectiveMonthPicker";
import { formatMoney, parseAmount, toAmountText } from "@/formatting/money";
import { formatMonth, monthOf } from "@/features/fees/months";
import {
  BusinessBillingMode,
  MonthlyFeeChange,
  SetDefaultMonthlyFeeRequest,
} from "@/features/fees/types";
import {
  useDeleteDefaultMonthlyFeeChange,
  useSetDefaultMonthlyFee,
} from "@/features/fees/useFeeMutations";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { IconButton } from "@/ui/IconButton";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

function historyItemText(change: MonthlyFeeChange, currencyCode: string): string {
  const month = formatMonth(change.effectiveFrom);
  if (change.billingMode === "ClassPacks") {
    return translate("fees.defaultFee.historyClassPacks", { month });
  }
  return change.amount === null
    ? translate("fees.defaultFee.historyNoFee", { month })
    : translate("fees.defaultFee.historyItem", {
        month,
        fee: formatMoney(change.amount, currencyCode),
      });
}

export function DefaultMonthlyFeeScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const business = useCurrentBusiness();
  const setDefaultMonthlyFeeMutation = useSetDefaultMonthlyFee();
  const deleteChangeMutation = useDeleteDefaultMonthlyFeeChange();
  const currentMonth = monthOf();
  const [billingMode, setBillingMode] = useState<BusinessBillingMode | null>(
    hasChosenBillingMode(business) ? (business.billingMode ?? "MonthlyFee") : null,
  );
  const [amountText, setAmountText] = useState(toAmountText(business.defaultMonthlyFee));
  const [effectiveFrom, setEffectiveFrom] = useState(currentMonth);
  const [amountError, setAmountError] = useState<string | undefined>(undefined);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);

  const deleteChange = async (change: MonthlyFeeChange) => {
    try {
      await deleteChangeMutation.mutateAsync(change.effectiveFrom);
      showToast(translate("fees.defaultFee.changeDeleted"), {
        label: translate("common.undo"),
        onPress: () => {
          setDefaultMonthlyFeeMutation
            .mutateAsync({
              amount: change.amount,
              effectiveFrom: change.effectiveFrom,
              billingMode: change.billingMode,
            })
            .catch(() => showToast(translate("common.undoFailed")));
        },
      });
    } catch {
      showToast(translate("common.unexpectedError"));
    }
  };

  const save = async () => {
    if (billingMode === null) {
      return;
    }
    setSubmissionFailure(null);
    if (billingMode === "ClassPacks") {
      await saveChange({ amount: business.defaultMonthlyFee, effectiveFrom, billingMode });
      return;
    }
    const trimmedAmount = amountText.trim();
    const amount = trimmedAmount.length === 0 ? null : parseAmount(trimmedAmount);
    if (trimmedAmount.length > 0 && (amount === null || amount <= 0)) {
      setAmountError(translate("fees.validation.amountInvalid"));
      return;
    }
    setAmountError(undefined);
    await saveChange({ amount, effectiveFrom, billingMode });
  };

  const saveChange = async (request: SetDefaultMonthlyFeeRequest) => {
    try {
      await setDefaultMonthlyFeeMutation.mutateAsync(request);
      showToast(
        translate(
          request.billingMode === "ClassPacks" ? "fees.client.saved" : "fees.defaultFee.saved",
        ),
      );
      router.back();
    } catch (saveError) {
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  return (
    <SettingsFormScreenLayout
      title={translate("fees.billingPlan.title")}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      <RequiredFieldsLegend />
      <BillingModeChoice billingMode={billingMode} onChange={setBillingMode} />
      {billingMode === "MonthlyFee" ? (
        <TextField
          label={translate("fees.payment.amount")}
          placeholder={translate("fees.defaultFee.placeholder")}
          hint={translate("fees.billingPlan.perMonth")}
          keyboardType="decimal-pad"
          value={amountText}
          onChangeText={setAmountText}
          errorMessage={amountError}
        />
      ) : null}
      <EffectiveMonthPicker month={effectiveFrom} onChange={setEffectiveFrom} />
      <AppText variant="caption" tone="muted">
        {translate("fees.defaultFee.effectiveHint")}
      </AppText>
      <Button
        label={translate("common.save")}
        onPress={save}
        disabled={billingMode === null}
        isLoading={setDefaultMonthlyFeeMutation.isPending}
      />
      {business.defaultMonthlyFeeChanges.length > 0 ? (
        <View className="gap-1">
          <AppText variant="label" tone="subtle">
            {translate("fees.defaultFee.history")}
          </AppText>
          {[...business.defaultMonthlyFeeChanges].reverse().map((change) => (
            <View key={change.effectiveFrom} className="flex-row items-center gap-2">
              <AppText variant="body" className="min-w-0 flex-1">
                {historyItemText(change, business.currencyCode)}
              </AppText>
              {change.effectiveFrom > currentMonth ? (
                <IconButton
                  icon="delete"
                  tone="muted-foreground"
                  accessibilityLabel={translate("fees.defaultFee.deleteChange", {
                    month: formatMonth(change.effectiveFrom),
                  })}
                  disabled={deleteChangeMutation.isPending}
                  onPress={() => deleteChange(change)}
                />
              ) : null}
            </View>
          ))}
        </View>
      ) : null}
    </SettingsFormScreenLayout>
  );
}
