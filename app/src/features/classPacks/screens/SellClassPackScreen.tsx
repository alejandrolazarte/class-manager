import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "expo-router";
import { useState } from "react";
import { Controller, useForm, useWatch } from "react-hook-form";
import { View } from "react-native";
import { useBusinessCurrency } from "@/features/business/CurrentBusinessProvider";
import {
  SellClassPackFormValues,
  sellClassPackSchema,
  toSellClassPackRequest,
} from "@/features/classPacks/sellClassPackSchema";
import { useSellClassPack } from "@/features/classPacks/useClassPackMutations";
import { useClassPacks } from "@/features/classPacks/useClassPacks";
import { formatMoney, toAmountText } from "@/features/fees/money";
import { paymentMethods } from "@/features/fees/paymentSchema";
import { SettingsFormScreenLayout } from "@/features/settings/components/SettingsFormScreenLayout";
import { SubmissionFailure, toSubmissionFailure } from "@/features/settings/submissionFailure";
import { todayIsoDate } from "@/features/sessions/dates";
import {
  formatBirthDateAsTyped,
  formatBirthDateForDisplay,
} from "@/features/students/birthDateFormatting";
import { translate, translateCount, TranslationKey } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { AppText } from "@/ui/AppText";
import { Banner } from "@/ui/Banner";
import { Button } from "@/ui/Button";
import { Chip } from "@/ui/Chip";
import { LoadingScreen } from "@/ui/LoadingScreen";
import { TextField } from "@/ui/TextField";
import { useToast } from "@/ui/ToastProvider";

interface SellClassPackScreenProps {
  clientId: string;
}

export function SellClassPackScreen({ clientId }: SellClassPackScreenProps) {
  const router = useRouter();
  const { showToast } = useToast();
  const currencyCode = useBusinessCurrency();
  const { data: classPacks = [], isPending } = useClassPacks(false);
  const sellClassPackMutation = useSellClassPack(clientId);
  const [submissionFailure, setSubmissionFailure] = useState<SubmissionFailure | null>(null);
  const form = useForm<SellClassPackFormValues>({
    resolver: zodResolver(sellClassPackSchema),
    defaultValues: {
      classPackId: "",
      price: "",
      method: "Cash",
      purchasedOn: formatBirthDateForDisplay(todayIsoDate()),
      notes: "",
    },
    mode: "onTouched",
  });
  const selectedClassPackId = useWatch({ control: form.control, name: "classPackId" });

  const sell = form.handleSubmit(async (formValues) => {
    setSubmissionFailure(null);
    try {
      await sellClassPackMutation.mutateAsync(toSellClassPackRequest(formValues));
      showToast(translate("classPacks.sell.saved"));
      router.back();
    } catch (sellError) {
      setSubmissionFailure(toSubmissionFailure(sellError));
    }
  });

  if (isPending) {
    return <LoadingScreen />;
  }
  if (classPacks.length === 0) {
    return (
      <View className="flex-1 bg-background p-4">
        <Banner tone="warning" message={translate("classPacks.sell.noPacks")}>
          <Button
            variant="secondary"
            label={translate("classPacks.sell.createPacks")}
            onPress={() => router.push(routes.classPacks)}
          />
        </Banner>
      </View>
    );
  }

  return (
    <SettingsFormScreenLayout submissionFailure={submissionFailure} onRetry={sell}>
      <Controller
        control={form.control}
        name="classPackId"
        render={({ field }) => (
          <View className="gap-2">
            <AppText variant="label" tone="muted">
              {translate("classPacks.sell.pack")}
            </AppText>
            <View className="flex-row flex-wrap gap-2">
              {classPacks.map((classPack) => (
                <Chip
                  key={classPack.id}
                  label={classPack.name}
                  accessibilityLabel={classPack.name}
                  isSelected={field.value === classPack.id}
                  onPress={() => {
                    field.onChange(classPack.id);
                    form.setValue("price", toAmountText(classPack.price));
                  }}
                />
              ))}
            </View>
            {classPacks
              .filter((classPack) => classPack.id === field.value)
              .map((classPack) => (
                <AppText key={classPack.id} variant="caption" tone="muted">
                  {translateCount("classPacks.list.summary", classPack.classCount, {
                    price: formatMoney(classPack.price, currencyCode),
                  })}
                </AppText>
              ))}
          </View>
        )}
      />
      <Controller
        control={form.control}
        name="price"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("classPacks.sell.price")}
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
        name="purchasedOn"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("classPacks.sell.purchasedOn")}
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
        label={translate("classPacks.sell.submit")}
        onPress={sell}
        disabled={selectedClassPackId.length === 0}
        isLoading={sellClassPackMutation.isPending}
      />
    </SettingsFormScreenLayout>
  );
}
