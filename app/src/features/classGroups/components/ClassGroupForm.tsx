import { Controller, UseFormReturn } from "react-hook-form";
import { View } from "react-native";
import { WeekdayChips } from "@/features/classGroups/components/WeekdayChips";
import {
  ClassGroupFormValues,
  classGroupLimits,
  commonDurationsInMinutes,
} from "@/features/classGroups/classGroupSchema";
import { formatStartTimeAsTyped } from "@/features/classGroups/startTimeFormatting";
import { Instructor } from "@/features/instructors/types";
import { translate } from "@/i18n/translate";
import { Chip } from "@/ui/Chip";
import { NumberStepper } from "@/ui/NumberStepper";
import { TextField } from "@/ui/TextField";
import { AppText } from "@/ui/AppText";

interface ClassGroupFormProps {
  form: UseFormReturn<ClassGroupFormValues>;
  instructors: Instructor[];
}

function FieldError({ message }: { message?: string }) {
  return message ? (
    <AppText variant="label" tone="danger" accessibilityRole="alert">
      {message}
    </AppText>
  ) : null;
}

function FieldLabel({ label }: { label: string }) {
  return (
    <AppText variant="label" tone="muted">
      {label}
    </AppText>
  );
}

export function ClassGroupForm({ form, instructors }: ClassGroupFormProps) {
  const { control } = form;
  return (
    <View className="gap-[18px]">
      <Controller
        control={control}
        name="name"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("classGroups.form.name")}
            placeholder={translate("classGroups.form.namePlaceholder")}
            autoCapitalize="sentences"
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={control}
        name="weekdays"
        render={({ field, fieldState }) => (
          <View className="gap-2">
            <FieldLabel label={translate("classGroups.form.weekdays")} />
            <WeekdayChips
              selectedWeekdays={field.value}
              onToggleWeekday={(weekday) =>
                field.onChange(
                  field.value.includes(weekday)
                    ? field.value.filter((selectedWeekday) => selectedWeekday !== weekday)
                    : [...field.value, weekday],
                )
              }
            />
            <FieldError message={fieldState.error?.message} />
          </View>
        )}
      />
      <View className="flex-row gap-3">
        <View className="flex-1">
          <Controller
            control={control}
            name="startTime"
            render={({ field, fieldState }) => (
              <TextField
                label={translate("classGroups.form.startTime")}
                placeholder={translate("classGroups.form.startTimePlaceholder")}
                keyboardType="number-pad"
                value={field.value}
                onChangeText={(typedText) => field.onChange(formatStartTimeAsTyped(typedText))}
                onBlur={field.onBlur}
                errorMessage={fieldState.error?.message}
              />
            )}
          />
        </View>
        <View className="flex-1">
          <Controller
            control={control}
            name="capacity"
            render={({ field, fieldState }) => (
              <NumberStepper
                label={translate("classGroups.form.capacity")}
                value={field.value}
                onChange={field.onChange}
                onBlur={field.onBlur}
                minimum={classGroupLimits.minimumCapacity}
                maximum={classGroupLimits.maximumCapacity}
                decreaseLabel={translate("classGroups.form.decreaseCapacity")}
                increaseLabel={translate("classGroups.form.increaseCapacity")}
                errorMessage={fieldState.error?.message}
              />
            )}
          />
        </View>
      </View>
      <Controller
        control={control}
        name="durationMinutes"
        render={({ field, fieldState }) => (
          <View className="gap-2">
            <FieldLabel label={translate("classGroups.form.duration")} />
            <View className="flex-row flex-wrap gap-2">
              {commonDurationsInMinutes.map((durationMinutes) => (
                <Chip
                  key={durationMinutes}
                  label={translate("classGroups.form.durationOption", {
                    minutes: durationMinutes,
                  })}
                  isSelected={field.value === String(durationMinutes)}
                  onPress={() => field.onChange(String(durationMinutes))}
                />
              ))}
            </View>
            <TextField
              label={translate("classGroups.form.customDuration")}
              keyboardType="number-pad"
              value={field.value}
              onChangeText={field.onChange}
              onBlur={field.onBlur}
            />
            <FieldError message={fieldState.error?.message} />
          </View>
        )}
      />
      <Controller
        control={control}
        name="instructorId"
        render={({ field, fieldState }) => (
          <View className="gap-2">
            <FieldLabel label={translate("classGroups.form.instructor")} />
            <View className="flex-row flex-wrap gap-2">
              {instructors.map((instructor) => (
                <Chip
                  key={instructor.id}
                  label={instructor.fullName}
                  isSelected={field.value === instructor.id}
                  onPress={() => field.onChange(instructor.id)}
                />
              ))}
            </View>
            <FieldError message={fieldState.error?.message} />
          </View>
        )}
      />
      <Controller
        control={control}
        name="location"
        render={({ field, fieldState }) => (
          <TextField
            label={translate("classGroups.form.location")}
            placeholder={translate("classGroups.form.locationPlaceholder")}
            value={field.value}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
    </View>
  );
}
