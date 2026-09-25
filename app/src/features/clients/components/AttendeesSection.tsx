import { Controller, useFieldArray, UseFormReturn } from "react-hook-form";
import { Pressable, Switch, Text, View } from "react-native";
import {
  maximumStudentsPerRegistration,
  RegisterClientFormValues,
} from "@/features/clients/registerClientSchema";
import { StudentFields } from "@/features/students/components/StudentFields";
import { emptyStudentFormValues } from "@/features/students/studentSchema";
import { translate } from "@/i18n/translate";
import { Button } from "@/ui/Button";

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
      <Text className="text-lg font-semibold text-gray-900">
        {translate("clients.register.attendeesSection")}
      </Text>
      <Controller
        control={control}
        name="clientAttends"
        render={({ field }) => (
          <View className="flex-row items-center justify-between">
            <Text className="text-base text-gray-800">
              {translate("clients.register.clientAttends")}
            </Text>
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
          className="gap-3 rounded-xl border border-gray-200 bg-white p-3"
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
            <Text className="text-base font-medium text-red-600">{translate("common.remove")}</Text>
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
        <Text accessibilityRole="alert" className="text-sm text-red-600">
          {attendeesErrorMessage}
        </Text>
      ) : null}
    </View>
  );
}
