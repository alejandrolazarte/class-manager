import { useState } from "react";
import { Pressable, View } from "react-native";
import { useCurrentBusiness } from "@/features/business/CurrentBusinessProvider";
import { ClientDetails } from "@/features/clients/types";
import { formatMoney, parseAmount, toAmountText } from "@/features/fees/money";
import { formatMonth } from "@/features/fees/months";
import { useDeletePayment, useSetClientMonthlyFee } from "@/features/fees/useFeeMutations";
import { useClientPayments } from "@/features/fees/useFees";
import { formatBirthDateForDisplay } from "@/features/students/birthDateFormatting";
import { translate, TranslationKey } from "@/i18n/translate";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { TextField } from "@/ui/TextField";
import { AppText } from "@/ui/AppText";

interface ClientFeeSectionProps {
  client: ClientDetails;
}

export function ClientFeeSection({ client }: ClientFeeSectionProps) {
  const business = useCurrentBusiness();
  const { data: payments = [] } = useClientPayments(client.id);
  const setClientMonthlyFeeMutation = useSetClientMonthlyFee(client.id);
  const deletePaymentMutation = useDeletePayment();
  const [isEditing, setIsEditing] = useState(false);
  const [amountText, setAmountText] = useState(toAmountText(client.monthlyFee));
  const [hasFailed, setHasFailed] = useState(false);
  const money = (amount: number) => formatMoney(amount, business.currencyCode);
  const effectiveFee = client.monthlyFee ?? business.defaultMonthlyFee;
  const feeLabel =
    effectiveFee === null
      ? translate("fees.client.noFee")
      : translate(client.monthlyFee === null ? "fees.client.defaultFee" : "fees.client.ownFee", {
          fee: money(effectiveFee),
        });

  const saveFee = async (amount: number | null) => {
    setHasFailed(false);
    try {
      await setClientMonthlyFeeMutation.mutateAsync(amount);
      setIsEditing(false);
    } catch {
      setHasFailed(true);
    }
  };

  return (
    <View className="gap-2">
      <AppText variant="heading">{translate("fees.client.title")}</AppText>
      {hasFailed ? <Banner message={translate("common.unexpectedError")} /> : null}
      <AppText variant="body">{feeLabel}</AppText>
      {isEditing ? (
        <View className="gap-2 rounded-xl bg-surface p-3">
          <TextField
            label={translate("fees.client.ownFeeAmount")}
            keyboardType="decimal-pad"
            value={amountText}
            onChangeText={setAmountText}
          />
          <Button
            label={translate("common.save")}
            disabled={parseAmount(amountText) === null}
            onPress={() => saveFee(parseAmount(amountText))}
            isLoading={setClientMonthlyFeeMutation.isPending}
          />
          <Button
            variant="secondary"
            label={translate("fees.client.useDefault")}
            onPress={() => saveFee(null)}
          />
        </View>
      ) : (
        <Button
          variant="secondary"
          label={translate("fees.client.changeFee")}
          onPress={() => setIsEditing(true)}
        />
      )}
      {payments.length > 0 ? (
        <View className="gap-1">
          <AppText variant="label" tone="subtle">
            {translate("fees.client.payments")}
          </AppText>
          {payments.map((payment) => (
            <View
              key={payment.id}
              className="flex-row items-center gap-3 rounded-xl bg-surface p-3"
            >
              <View className="flex-1">
                <AppText variant="body">
                  {`${money(payment.amount)} · ${formatMonth(payment.month)}`}
                </AppText>
                <AppText variant="caption" tone="muted">
                  {`${formatBirthDateForDisplay(payment.paidOn)} · ${translate(`fees.methods.${payment.method}` as TranslationKey)}`}
                </AppText>
              </View>
              <Pressable
                accessibilityRole="button"
                accessibilityLabel={translate("fees.client.deletePayment")}
                onPress={() => deletePaymentMutation.mutate(payment.id)}
              >
                <AppText variant="label" tone="danger">
                  {translate("fees.client.deletePayment")}
                </AppText>
              </Pressable>
            </View>
          ))}
        </View>
      ) : null}
    </View>
  );
}
