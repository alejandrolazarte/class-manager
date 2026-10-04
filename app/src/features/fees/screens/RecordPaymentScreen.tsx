import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import { AmountField } from "@/features/fees/components/AmountField";
import { PaymentMethodPicker } from "@/features/fees/components/PaymentMethodPicker";
import { formatMoney, toAmountText } from "@/features/fees/money";
import { formatMonth } from "@/features/fees/months";
import {
  PaymentFormValues,
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
import { translate } from "@/i18n/translate";
import { Button } from "@/ui/Button";
import { LoadingScreen } from "@/ui/LoadingScreen";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";
import { useRequiredFieldsFilled } from "@/forms/requiredFields";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";

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
  const areRequiredFieldsFilled = useRequiredFieldsFilled(form.control, ["amount", "paidOn"]);

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
    <SettingsFormScreenLayout
      eyebrow={translate("fees.payment.title")}
      title={clientFee?.clientFullName ?? ""}
      subtitle={[
        clientFee?.fee
          ? translate("fees.payment.feeOfMonth", {
              month: formatMonth(month),
              fee: formatMoney(clientFee.fee, currencyCode),
            })
          : formatMonth(month),
        clientFee?.paid
          ? translate("fees.payment.alreadyPaid", {
              paid: formatMoney(clientFee.paid, currencyCode),
            })
          : null,
      ]
        .filter(Boolean)
        .join(" · ")}
      submissionFailure={submissionFailure}
      onRetry={save}
    >
      <RequiredFieldsLegend />
      <Controller
        control={form.control}
        name="amount"
        render={({ field, fieldState }) => (
          <AmountField
            label={translate("fees.payment.amount")}
            isRequired
            currencyCode={currencyCode}
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
          <PaymentMethodPicker value={field.value} onChange={field.onChange} />
        )}
      />
      <Controller
        control={form.control}
        name="paidOn"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("fees.payment.paidOn")}
            isRequired
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
        disabled={!areRequiredFieldsFilled}
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
