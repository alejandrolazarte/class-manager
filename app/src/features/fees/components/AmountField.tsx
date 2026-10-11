import { TextInput, View } from "react-native";
import { currencySymbol } from "@/features/fees/money";
import { useTheme } from "@/theme/useTheme";
import { AppText, textVariantClassNames } from "@/ui/AppText";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";

interface AmountFieldProps {
  label: string;
  currencyCode: string;
  value: string;
  onChangeText: (value: string) => void;
  onBlur?: () => void;
  errorMessage?: string;
  isRequired?: boolean;
}

export function AmountField({
  label,
  currencyCode,
  value,
  onChangeText,
  onBlur,
  errorMessage,
  isRequired = false,
}: AmountFieldProps) {
  const { colors } = useTheme();
  return (
    <View className="gap-1.5">
      <AppText variant="label" tone="muted">
        {withRequiredMark(label, isRequired)}
      </AppText>
      <View
        className={`h-[52px] flex-row items-center gap-1.5 rounded-[10px] border bg-surface px-3.5 ${errorMessage ? "border-danger" : "border-border"}`}
      >
        <AppText variant="headline" tone="subtle">
          {currencySymbol(currencyCode)}
        </AppText>
        <TextInput
          accessibilityLabel={label}
          keyboardType="decimal-pad"
          value={value}
          onChangeText={onChangeText}
          onBlur={onBlur}
          placeholderTextColor={colors["subtle-foreground"]}
          className={`h-full min-w-0 flex-1 ${textVariantClassNames.headline} text-foreground`}
        />
      </View>
      {errorMessage ? (
        <AppText variant="label" tone="danger" accessibilityRole="alert">
          {errorMessage}
        </AppText>
      ) : null}
    </View>
  );
}
