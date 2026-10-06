import { useRouter } from "expo-router";
import { useState } from "react";
import { View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { EffectiveMonthPicker } from "@/features/fees/components/EffectiveMonthPicker";
import { formatMoney, parseAmount, toAmountText } from "@/features/fees/money";
import { formatMonth, monthOf } from "@/features/fees/months";
import {
  useDeleteDefaultMonthlyFeeChange,
  useSetDefaultMonthlyFee,
} from "@/features/fees/useFeeMutations";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate } from "@/i18n/translate";
import { AppText } from "@/ui/AppText";
import { Button } from "@/ui/Button";
import { DeleteConfirmation } from "@/ui/DeleteConfirmation";
import { IconButton } from "@/ui/IconButton";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

export function DefaultMonthlyFeeScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const business = useCurrentBusiness();
  const setDefaultMonthlyFeeMutation = useSetDefaultMonthlyFee();
  const deleteChangeMutation = useDeleteDefaultMonthlyFeeChange();
  const currentMonth = monthOf();
  const [amountText, setAmountText] = useState(toAmountText(business.defaultMonthlyFee));
  const [effectiveFrom, setEffectiveFrom] = useState(currentMonth);
  const [amountError, setAmountError] = useState<string | undefined>(undefined);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const [monthPendingDeletion, setMonthPendingDeletion] = useState<string | null>(null);

  const deletePendingChange = async (month: string) => {
    try {
      await deleteChangeMutation.mutateAsync(month);
      showToast(translate("fees.defaultFee.changeDeleted"));
    } catch {
      showToast(translate("common.unexpectedError"));
    }
    setMonthPendingDeletion(null);
  };

  const save = async () => {
    setSubmissionFailure(null);
    const trimmedAmount = amountText.trim();
    const amount = trimmedAmount.length === 0 ? null : parseAmount(trimmedAmount);
    if (trimmedAmount.length > 0 && (amount === null || amount <= 0)) {
      setAmountError(translate("fees.validation.amountInvalid"));
      return;
    }
    setAmountError(undefined);
    try {
      await setDefaultMonthlyFeeMutation.mutateAsync({ amount, effectiveFrom });
      showToast(translate("fees.defaultFee.saved"));
      router.back();
    } catch (saveError) {
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  return (
    <SettingsFormScreenLayout
      title={translate("fees.defaultFee.title")}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      <TextField
        label={translate("fees.defaultFee.amount")}
        placeholder={translate("fees.defaultFee.placeholder")}
        keyboardType="decimal-pad"
        value={amountText}
        onChangeText={setAmountText}
        errorMessage={amountError}
      />
      <EffectiveMonthPicker month={effectiveFrom} onChange={setEffectiveFrom} />
      <AppText variant="caption" tone="muted">
        {translate("fees.defaultFee.effectiveHint")}
      </AppText>
      <Button
        label={translate("common.save")}
        onPress={save}
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
                {change.amount === null
                  ? translate("fees.defaultFee.historyNoFee", {
                      month: formatMonth(change.effectiveFrom),
                    })
                  : translate("fees.defaultFee.historyItem", {
                      month: formatMonth(change.effectiveFrom),
                      fee: formatMoney(change.amount, business.currencyCode),
                    })}
              </AppText>
              {change.effectiveFrom > currentMonth ? (
                <IconButton
                  icon="delete"
                  tone="subtle-foreground"
                  accessibilityLabel={translate("fees.defaultFee.deleteChange", {
                    month: formatMonth(change.effectiveFrom),
                  })}
                  onPress={() => setMonthPendingDeletion(change.effectiveFrom)}
                />
              ) : null}
            </View>
          ))}
          {monthPendingDeletion ? (
            <DeleteConfirmation
              question={translate("fees.defaultFee.deleteChangeQuestion", {
                month: formatMonth(monthPendingDeletion),
              })}
              onCancel={() => setMonthPendingDeletion(null)}
              onConfirm={() => deletePendingChange(monthPendingDeletion)}
              isDeleting={deleteChangeMutation.isPending}
            />
          ) : null}
        </View>
      ) : null}
    </SettingsFormScreenLayout>
  );
}
