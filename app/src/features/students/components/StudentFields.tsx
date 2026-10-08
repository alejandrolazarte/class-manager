import { Control, Controller, FieldValues, Path } from "react-hook-form";
import { View } from "react-native";
import { BirthDateField } from "@/forms/BirthDateField";
import { translate } from "@/i18n/translate";
import { TextField } from "@/ui/TextField";

export interface StudentFieldPaths<TFormValues extends FieldValues> {
  fullName: Path<TFormValues>;
  birthDate: Path<TFormValues>;
  notes: Path<TFormValues>;
  email: Path<TFormValues>;
}

interface StudentFieldsProps<TFormValues extends FieldValues> {
  control: Control<TFormValues>;
  paths: StudentFieldPaths<TFormValues>;
  autoFocus?: boolean;
  isInsideCard?: boolean;
}

function toText(value: unknown): string {
  return typeof value === "string" ? value : "";
}

export function StudentFields<TFormValues extends FieldValues>({
  control,
  paths,
  autoFocus = false,
  isInsideCard = false,
}: StudentFieldsProps<TFormValues>) {
  const fieldSurface = isInsideCard ? "background" : "surface";
  return (
    <View className="gap-3">
      <Controller
        control={control}
        name={paths.fullName}
        render={({ field, fieldState }) => (
          <TextField
            label={translate("students.fields.fullName")}
            isRequired
            fieldSurface={fieldSurface}
            autoFocus={autoFocus}
            autoCapitalize="words"
            value={toText(field.value)}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={control}
        name={paths.birthDate}
        render={({ field, fieldState }) => (
          <BirthDateField
            label={translate("students.fields.birthDate")}
            isRequired={false}
            fieldSurface={fieldSurface}
            value={toText(field.value)}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={control}
        name={paths.email}
        render={({ field, fieldState }) => (
          <TextField
            label={translate("students.fields.email")}
            hint={translate("students.fields.emailHint")}
            fieldSurface={fieldSurface}
            keyboardType="email-address"
            autoCapitalize="none"
            autoComplete="off"
            value={toText(field.value)}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
      <Controller
        control={control}
        name={paths.notes}
        render={({ field, fieldState }) => (
          <TextField
            label={translate("students.fields.notes")}
            fieldSurface={fieldSurface}
            placeholder={translate("students.fields.notesPlaceholder")}
            multiline
            value={toText(field.value)}
            onChangeText={field.onChange}
            onBlur={field.onBlur}
            errorMessage={fieldState.error?.message}
          />
        )}
      />
    </View>
  );
}
