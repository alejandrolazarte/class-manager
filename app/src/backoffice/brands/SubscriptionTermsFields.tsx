import { Control, Controller, FieldValues, Path } from "react-hook-form";
import { todayIsoDate } from "@/dates/dates";
import { DateField } from "@/forms/DateField";
import { translate } from "@/i18n/translate";
import { TextField } from "@/ui/TextField";

interface SubscriptionTermsValues extends FieldValues {
  price: string;
  currency: string;
  expiredOn: string;
}

interface SubscriptionTermsFieldsProps<TValues extends SubscriptionTermsValues> {
  control: Control<TValues>;
}

export function SubscriptionTermsFields<TValues extends SubscriptionTermsValues>({
  control,
}: SubscriptionTermsFieldsProps<TValues>) {
  return (
    <>
      <Controller
        control={control}
        name={"price" as Path<TValues>}
        render={({ field, fieldState }) => (
          <TextField
            label={translate("backoffice.subscriptionTerms.price")}
            isRequired
            keyboardType="decimal-pad"
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={control}
        name={"currency" as Path<TValues>}
        render={({ field, fieldState }) => (
          <TextField
            label={translate("backoffice.subscriptionTerms.currency")}
            hint={translate("backoffice.subscriptionTerms.currencyHint")}
            isRequired
            autoCapitalize="characters"
            maxLength={3}
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={control}
        name={"expiredOn" as Path<TValues>}
        render={({ field, fieldState }) => (
          <DateField
            label={translate("backoffice.subscriptionTerms.expiredOn")}
            hint={translate("backoffice.subscriptionTerms.expiredOnHint")}
            earliestIsoDate={todayIsoDate()}
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
    </>
  );
}
