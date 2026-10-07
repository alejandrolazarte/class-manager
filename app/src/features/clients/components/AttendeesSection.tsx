import { Controller, useFieldArray, UseFormReturn } from "react-hook-form";
import { Pressable, View } from "react-native";
import {
  maximumStudentsPerRegistration,
  RegisterClientFormValues,
} from "@/features/clients/registerClientSchema";
import { StudentFields } from "@/features/students/components/StudentFields";
import { emptyStudentFormValues } from "@/features/students/studentSchema";
import { translate } from "@/i18n/translate";
import { Button } from "@/ui/Button";
import { AppText } from "@/ui/AppText";
import { Card } from "@/ui/Card";
import { SectionTitle } from "@/ui/SectionTitle";
import { ToggleSwitch } from "@/ui/ToggleSwitch";

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
    <View className="gap-3">
      <SectionTitle title={translate("clients.register.attendeesSection")} isOverline />
      <Controller
        control={control}
        name="clientAttends"
        render={({ field }) => (
          <ToggleSwitch
            label={translate("clients.register.clientAttends")}
            value={field.value}
            onValueChange={(clientAttendsValue) => {
              field.onChange(clientAttendsValue);
              void form.trigger("clientAttends");
            }}
          />
        )}
      />
      {fields.map((additionalStudentField, index) => (
        <Card key={additionalStudentField.id} className="gap-3 p-4">
          <View className="flex-row items-center justify-between">
            <AppText variant="link" tone="primary">
              {translate("clients.register.attendeeCard")}
            </AppText>
            <Pressable accessibilityRole="button" className="p-1" onPress={() => remove(index)}>
              <AppText tone="danger" variant="link">
                {translate("common.remove")}
              </AppText>
            </Pressable>
          </View>
          <StudentFields
            control={control}
            paths={{
              fullName: `additionalStudents.${index}.fullName`,
              birthDate: `additionalStudents.${index}.birthDate`,
              notes: `additionalStudents.${index}.notes`,
              email: `additionalStudents.${index}.email`,
            }}
            autoFocus
            isInsideCard
          />
        </Card>
      ))}
      {attendeeCount < maximumStudentsPerRegistration ? (
        <Button
          variant="dashed"
          size="medium"
          icon="enroll"
          label={translate("clients.register.addAttendee")}
          onPress={() => append({ ...emptyStudentFormValues })}
        />
      ) : null}
      {attendeesErrorMessage ? (
        <AppText variant="label" tone="danger" accessibilityRole="alert">
          {attendeesErrorMessage}
        </AppText>
      ) : null}
    </View>
  );
}
