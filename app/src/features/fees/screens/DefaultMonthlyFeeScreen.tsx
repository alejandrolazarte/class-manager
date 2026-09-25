import { useRouter } from "expo-router";
import { useState } from "react";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { parseAmount, toAmountText } from "@/features/fees/money";
import { useSetDefaultMonthlyFee } from "@/features/fees/useFeeMutations";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { translate } from "@/i18n/translate";
import { Button } from "@/ui/Button";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

export function DefaultMonthlyFeeScreen() {
  const router = useRouter();
  const { showToast } = useToast();
  const business = useCurrentBusiness();
  const setDefaultMonthlyFeeMutation = useSetDefaultMonthlyFee();
  const [amountText, setAmountText] = useState(toAmountText(business.defaultMonthlyFee));
  const [amountError, setAmountError] = useState<string | undefined>(undefined);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);

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
      await setDefaultMonthlyFeeMutation.mutateAsync(amount);
      showToast(translate("fees.defaultFee.saved"));
      router.back();
    } catch (saveError) {
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  };

  return (
    <SettingsFormScreenLayout submissionFailure={submissionFailure} onRetry={save}>
      <TextField
        label={translate("fees.defaultFee.amount")}
        placeholder={translate("fees.defaultFee.placeholder")}
        keyboardType="decimal-pad"
        value={amountText}
        onChangeText={setAmountText}
        errorMessage={amountError}
      />
      <Button
        label={translate("common.save")}
        onPress={save}
        isLoading={setDefaultMonthlyFeeMutation.isPending}
      />
    </SettingsFormScreenLayout>
  );
}
