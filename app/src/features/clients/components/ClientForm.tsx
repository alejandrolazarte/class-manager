import { Controller, UseFormReturn } from "react-hook-form";
import { View } from "react-native";
import { AttendeesSection } from "@/features/clients/components/AttendeesSection";
import { BillingModeChoice } from "@/features/fees/components/BillingModeChoice";
import { formatPhoneNumberAsTyped } from "@/features/clients/phoneNumberFormatting";
import { RegisterClientFormValues } from "@/features/clients/registerClientSchema";
import { ContactAdultConfirmation } from "@/features/studentApp/components/ContactAdultConfirmation";
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
  asksBillingMode: boolean;
}

export function ClientForm({ form, onSubmit, isSubmitting, asksBillingMode }: ClientFormProps) {
  const { control, formState, watch } = form;
  const isAppInvitationOffered = watch("email").trim().length > 0;
  const isAppInvitationOn = isAppInvitationOffered && watch("sendAppInvitation");
  const isSubmitBlockedByErrors = formState.isSubmitted && !formState.isValid;
  const additionalStudents = watch("additionalStudents");
  const areRequiredFieldsFilled =
    areFilled([watch("fullName"), watch("phoneNumber")]) &&
    areFilled(additionalStudents.map((additionalStudent) => additionalStudent.fullName)) &&
    (!isAppInvitationOn || watch("confirmsContactIsAdult")) &&
    (!asksBillingMode || watch("billingMode") !== null);

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
            {isAppInvitationOn ? (
              <Controller
                control={control}
                name="confirmsContactIsAdult"
                render={({ field }) => (
                  <ContactAdultConfirmation
                    contactFullName={watch("fullName")}
                    isConfirmed={field.value}
                    onConfirmedChange={field.onChange}
                  />
                )}
              />
            ) : null}
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
      {asksBillingMode ? (
        <View className="gap-3">
          <SectionTitle title={translate("fees.billingPlan.title")} isOverline />
          <Controller
            control={control}
            name="billingMode"
            render={({ field }) => (
              <BillingModeChoice
                billingMode={field.value}
                onChange={field.onChange}
                note={translate("clients.register.billingModeNote")}
              />
            )}
          />
        </View>
      ) : null}
      <Button
        label={translate("clients.register.submit")}
        onPress={onSubmit}
        isLoading={isSubmitting}
        disabled={!areRequiredFieldsFilled || isSubmitBlockedByErrors}
      />
    </View>
  );
}
