import { Controller, UseFormReturn } from "react-hook-form";
import { View } from "react-native";
import { AttendeesSection } from "@/features/clients/components/AttendeesSection";
import { formatPhoneNumberAsTyped } from "@/features/clients/phoneNumberFormatting";
import { RegisterClientFormValues } from "@/features/clients/registerClientSchema";
import { emailAddressPattern } from "@/forms/emailAddress";
import { translate } from "@/i18n/translate";
import { Button } from "@/ui/Button";
import { TextField } from "@/ui/TextField";
import { AppText } from "@/ui/AppText";
import { SectionTitle } from "@/ui/SectionTitle";
import { ToggleSwitch } from "@/ui/ToggleSwitch";
import { RequiredFieldsLegend } from "@/ui/RequiredFieldsLegend";
import { areFilled } from "@/forms/requiredFields";

interface ClientFormProps {
  form: UseFormReturn<RegisterClientFormValues>;
  onSubmit: () => void;
  isSubmitting: boolean;
}

export function ClientForm({ form, onSubmit, isSubmitting }: ClientFormProps) {
  const { control, formState, watch } = form;
  const isAppInvitationOffered = emailAddressPattern.test(watch("email").trim());
  const isSubmitBlockedByErrors = formState.isSubmitted && !formState.isValid;
  const additionalStudents = watch("additionalStudents");
  const areRequiredFieldsFilled =
    areFilled([watch("fullName"), watch("phoneNumber")]) &&
    (!watch("clientAttends") || areFilled([watch("clientBirthDate")])) &&
    areFilled(additionalStudents.map((additionalStudent) => additionalStudent.fullName));

  return (
    <View className="gap-[18px]">
      <RequiredFieldsLegend />
      <View className="gap-3">
        <SectionTitle title={translate("clients.register.contactSection")} isOverline />
        <Controller
          control={control}
          name="fullName"
          render={({ field, fieldState }) => (
            <TextField
              label={translate("clients.register.fullName")}
              isRequired
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
              isRequired
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
        {isAppInvitationOffered ? (
          <View className="gap-1.5">
            <Controller
              control={control}
              name="sendAppInvitation"
              render={({ field }) => (
                <ToggleSwitch
                  label={translate("clients.register.sendAppInvitation")}
                  value={field.value}
                  onValueChange={field.onChange}
                />
              )}
            />
            <AppText variant="caption" tone="subtle">
              {translate("clients.register.sendAppInvitationHint")}
            </AppText>
          </View>
        ) : null}
        <Controller
          control={control}
          name="notes"
          render={({ field, fieldState }) => (
            <TextField
              label={translate("clients.register.notes")}
              placeholder={translate("clients.register.notesPlaceholder")}
              value={field.value}
              onChangeText={field.onChange}
              onBlur={field.onBlur}
              errorMessage={fieldState.error?.message}
            />
          )}
        />
      </View>
      <AttendeesSection form={form} />
      <Button
        label={translate("clients.register.submit")}
        onPress={onSubmit}
        isLoading={isSubmitting}
        disabled={!areRequiredFieldsFilled || isSubmitBlockedByErrors}
      />
    </View>
  );
}
