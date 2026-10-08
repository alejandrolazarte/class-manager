import { DateField } from "@/forms/DateField";
import { earliestBirthDate } from "@/features/students/birthDateFormatting";
import { todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { TextFieldSurface } from "@/ui/TextField";

interface BirthDateFieldProps {
  label?: string;
  hint?: string;
  value: string;
  onChangeText: (typedBirthDate: string) => void;
  onBlur?: () => void;
  errorMessage?: string;
  isRequired?: boolean;
  fieldSurface?: TextFieldSurface;
}

export function BirthDateField({
  label = translate("birthDate.label"),
  hint,
  value,
  onChangeText,
  onBlur,
  errorMessage,
  isRequired = true,
  fieldSurface,
}: BirthDateFieldProps) {
  return (
    <DateField
      label={label}
      hint={hint}
      isRequired={isRequired}
      placeholder={translate("birthDate.placeholder")}
      fieldSurface={fieldSurface}
      value={value}
      onChangeText={onChangeText}
      onBlur={onBlur}
      errorMessage={errorMessage}
      earliestIsoDate={earliestBirthDate}
      latestIsoDate={todayIsoDate()}
    />
  );
}
