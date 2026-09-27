import { Controller, useFieldArray, UseFormReturn } from "react-hook-form";
import { Pressable, Switch, View } from "react-native";
import {
  maximumStudentsPerRegistration,
  RegisterClientFormValues,
} from "@/features/clients/registerClientSchema";
import { StudentFields } from "@/features/students/components/StudentFields";
import { emptyStudentFormValues } from "@/features/students/studentSchema";
import { translate } from "@/i18n/translate";
import { Button } from "@/ui/Button";
import { AppText } from "@/ui/AppText";

interface AttendeesSectionProps {
  form: UseFormReturn<RegisterClientFormValues>;
}

export function AttendeesSection({ form }: AttendeesSectionProps) {
  const { control, formState, watch } = form;
  const { fields, append, remove } = useFieldArray({ control, name: "additionalStudents" });
  const clientAttends = watch("clientAttends");
  const attendeeCount = (clientAttends ? 1 : 0) + fields.length;
  const attendeesErrorMessage = formState.errors.clientAttends?.message;

  return (
    <View className="gap-4">
      <AppText variant="heading">{translate("clients.register.attendeesSection")}</AppText>
      <Controller
        control={control}
        name="clientAttends"
        render={({ field }) => (
          <View className="flex-row items-center justify-between">
            <AppText variant="body">{translate("clients.register.clientAttends")}</AppText>
            <Switch
              accessibilityLabel={translate("clients.register.clientAttends")}
              value={field.value}
              onValueChange={(clientAttendsValue) => {
                field.onChange(clientAttendsValue);
                void form.trigger("clientAttends");
              }}
            />
          </View>
        )}
      />
      {fields.map((additionalStudentField, index) => (
        <View
          key={additionalStudentField.id}
          className="gap-3 rounded-xl border border-border bg-surface p-3"
        >
          <StudentFields
            control={control}
            paths={{
              fullName: `additionalStudents.${index}.fullName`,
              birthDate: `additionalStudents.${index}.birthDate`,
              notes: `additionalStudents.${index}.notes`,
            }}
            autoFocus
          />
          <Pressable accessibilityRole="button" onPress={() => remove(index)}>
            <AppText tone="danger" variant="link">
              {translate("common.remove")}
            </AppText>
          </Pressable>
        </View>
      ))}
      {attendeeCount < maximumStudentsPerRegistration ? (
        <Button
          variant="secondary"
          label={translate("clients.register.addAttendee")}
          onPress={() => append({ ...emptyStudentFormValues })}
        />
      ) : null}
      {attendeesErrorMessage ? (
        <AppText variant="caption" tone="danger" accessibilityRole="alert">
          {attendeesErrorMessage}
        </AppText>
      ) : null}
    </View>
  );
}
