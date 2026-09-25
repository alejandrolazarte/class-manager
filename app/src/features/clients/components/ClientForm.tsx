import { useState } from "react";
import { Controller, UseFormReturn } from "react-hook-form";
import { Pressable, Text, View } from "react-native";
import { AttendeesSection } from "@/features/clients/components/AttendeesSection";
import { formatPhoneNumberAsTyped } from "@/features/clients/phoneNumberFormatting";
import { RegisterClientFormValues } from "@/features/clients/registerClientSchema";
import { translate } from "@/i18n/translate";
import { Button } from "@/ui/Button";
import { TextField } from "@/ui/TextField";

interface ClientFormProps {
  form: UseFormReturn<RegisterClientFormValues>;
  onSubmit: () => void;
  isSubmitting: boolean;
}

export function ClientForm({ form, onSubmit, isSubmitting }: ClientFormProps) {
  const { control, formState } = form;
  const [areMoreDetailsExpanded, setAreMoreDetailsExpanded] = useState(false);
  const isEmailVisible = areMoreDetailsExpanded || formState.errors.email !== undefined;
  const isSubmitBlockedByErrors = formState.isSubmitted && !formState.isValid;

  return (
    <View className="gap-4">
      <Text className="text-lg font-semibold text-gray-900">
        {translate("clients.register.contactSection")}
      </Text>
      <Controller
        control={control}
        name="fullName"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("clients.register.fullName")}
            autoFocus
            autoCapitalize="words"
            autoComplete="name"
            returnKeyType="next"
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={control}
        name="phoneNumber"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("clients.register.phoneNumber")}
            keyboardType="phone-pad"
            autoComplete="tel"
            value={field.value}
            onChangeText={(typedText) => field.onChange(formatPhoneNumberAsTyped(typedText))}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={control}
        name="notes"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("clients.register.notes")}
            placeholder={translate("clients.register.notesPlaceholder")}
            multiline
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      {isEmailVisible ? (
        <Controller
          control={control}
          name="email"
          render={({ field, fieldState }) => (
            <TextField
              label={translate("clients.register.email")}
              keyboardType="email-address"
              autoCapitalize="none"
              autoComplete="email"
              value={field.value}
              onChangeText={field.onChange}
              onBlur={field.onBlur}
              errorMessage={fieldState.error?.message}
            />
          )}
        />
      ) : (
        <Pressable accessibilityRole="button" onPress={() => setAreMoreDetailsExpanded(true)}>
          <Text className="text-base font-medium text-brand">
            {translate("clients.register.moreDetails")}
          </Text>
        </Pressable>
      )}
      <AttendeesSection form={form} />
      <Button
        label={translate("clients.register.submit")}
        onPress={onSubmit}
        isLoading={isSubmitting}
        disabled={isSubmitBlockedByErrors}
      />
    </View>
  );
}
