import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { formatMoney, toAmountText } from "@/features/fees/money";
import { formatMonth } from "@/features/fees/months";
import {
  PaymentFormValues,
  paymentMethods,
  paymentSchema,
  toRecordPaymentRequest,
} from "@/features/fees/paymentSchema";
import { ClientFee } from "@/features/fees/types";
import { useRecordPayment } from "@/features/fees/useFeeMutations";
import { useMonthlyFees } from "@/features/fees/useFees";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import {
  formatBirthDateAsTyped,
  formatBirthDateForDisplay,
} from "@/features/students/birthDateFormatting";
import { todayIsoDate } from "@/features/sessions/dates";
import { translate, TranslationKey } from "@/i18n/translate";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { LoadingScreen } from "@/ui/LoadingScreen";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";
import { AppText } from "@/ui/AppText";

interface RecordPaymentScreenProps {
  clientId: string;
  month: string;
}

interface PaymentEditorProps {
  clientId: string;
  month: string;
  clientFee?: ClientFee;
}

function PaymentEditor({ clientId, month, clientFee }: PaymentEditorProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const currencyCode = useBusinessCurrency();
  const recordPaymentMutation = useRecordPayment(clientId);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const form = useForm<PaymentFormValues>({
    resolver: zodResolver(paymentSchema),
    defaultValues: {
      amount: toAmountText(clientFee?.balance || clientFee?.fee || null),
      method: "Cash",
      paidOn: formatBirthDateForDisplay(todayIsoDate()),
      notes: "",
    },
    mode: "onTouched",
  });

  const save = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    try {
      await recordPaymentMutation.mutateAsync(toRecordPaymentRequest(formValues, month));
      showToast(translate("fees.payment.saved"));
      router.back();
    } catch (saveError) {
      setSubmissionFailure(toSubmissionFailure(saveError));
    }
  });

  return (
    <SettingsFormScreenLayout submissionFailure={submissionFailure} onRetry={save}>
      <View className="gap-1">
        <AppText variant="headline">{clientFee?.clientFullName ?? ""}</AppText>
        <AppText variant="body" tone="muted">
          {clientFee?.fee
            ? translate("fees.payment.feeOfMonth", {
                month: formatMonth(month),
                fee: formatMoney(clientFee.fee, currencyCode),
              })
            : formatMonth(month)}
        </AppText>
      </View>
      <Controller
        control={form.control}
        name="amount"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("fees.payment.amount")}
            keyboardType="decimal-pad"
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={form.control}
        name="method"
        render={({ field }) => (
          <View className="gap-2">
            <AppText variant="label" tone="muted">
              {translate("fees.payment.method")}
            </AppText>
            <View className="flex-row flex-wrap gap-2">
              {paymentMethods.map((method) => (
                <Chip
                  key={method}
                  label={translate(`fees.methods.${method}` as TranslationKey)}
                  isSelected={field.value === method}
                  onPress={() => field.onChange(method)}
                />
              ))}
            </View>
          </View>
        )}
      />
      <Controller
        control={form.control}
        name="paidOn"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("fees.payment.paidOn")}
            keyboardType="number-pad"
            value={field.value}
            onChangeText={(typedText) => field.onChange(formatBirthDateAsTyped(typedText))}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={form.control}
        name="notes"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("fees.payment.notes")}
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Button
        label={translate("fees.payment.submit")}
        onPress={save}
        isLoading={recordPaymentMutation.isPending}
      />
    </SettingsFormScreenLayout>
  );
}

export function RecordPaymentScreen({ clientId, month }: RecordPaymentScreenProps) {
  const { data: monthlyFees, isPending } = useMonthlyFees(month);
  if (isPending) {
    return <LoadingScreen />;
  }
  const clientFee = monthlyFees?.clients.find((candidate) => candidate.clientId === clientId);
  return <PaymentEditor clientId={clientId} month={month} clientFee={clientFee} />;
}
