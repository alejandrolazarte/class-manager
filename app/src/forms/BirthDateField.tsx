import { formatBirthDateAsTyped } from "@/features/students/birthDateFormatting";
import { translate } from "@/i18n/translate";
import { TextField, TextFieldSurface } from "@/ui/TextField";

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
    <TextField
      label={label}
      hint={hint}
      isRequired={isRequired}
      placeholder={translate("birthDate.placeholder")}
      keyboardType="number-pad"
      fieldSurface={fieldSurface}
      value={value}
      onChangeText={(typedText) => onChangeText(formatBirthDateAsTyped(typedText))}
      onBlur={onBlur}
      errorMessage={errorMessage}
    />
  );
}
