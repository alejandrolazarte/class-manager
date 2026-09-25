import { useState } from "react";
import { Pressable, Text, View } from "react-native";
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
      <Text className="text-lg font-semibold text-gray-900">{translate("fees.client.title")}</Text>
      {hasFailed ? <Banner message={translate("common.unexpectedError")} /> : null}
      <Text className="text-base text-gray-800">{feeLabel}</Text>
      {isEditing ? (
        <View className="gap-2 rounded-xl bg-white p-3">
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
          <Text className="text-sm font-medium text-gray-500">
            {translate("fees.client.payments")}
          </Text>
          {payments.map((payment) => (
            <View key={payment.id} className="flex-row items-center gap-3 rounded-xl bg-white p-3">
              <View className="flex-1">
                <Text className="text-base text-gray-900">
                  {`${money(payment.amount)} · ${formatMonth(payment.month)}`}
                </Text>
                <Text className="text-sm text-gray-600">
                  {`${formatBirthDateForDisplay(payment.paidOn)} · ${translate(`fees.methods.${payment.method}` as TranslationKey)}`}
                </Text>
              </View>
              <Pressable
                accessibilityRole="button"
                accessibilityLabel={translate("fees.client.deletePayment")}
                onPress={() => deletePaymentMutation.mutate(payment.id)}
              >
                <Text className="text-sm font-medium text-red-600">
                  {translate("fees.client.deletePayment")}
                </Text>
              </Pressable>
            </View>
          ))}
        </View>
      ) : null}
    </View>
  );
}
