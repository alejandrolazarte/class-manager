import { TextInput, View } from "react-native";
import { currencySymbol } from "@/features/fees/money";
import { useTheme } from "@/theme/useTheme";
import { AppText, textVariantClassNames } from "@/ui/AppText";
import { useElevationStyle } from "@/ui/elevation";
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
  const elevationStyle = useElevationStyle();
  return (
    <View
      style={elevationStyle}
      className={`items-center gap-1.5 rounded-3xl border-[1.5px] bg-surface p-[18px] ${errorMessage ? "border-danger" : "border-transparent"}`}
    >
      <AppText variant="label" tone="muted">
        {withRequiredMark(label, isRequired)}
      </AppText>
      <View className="flex-row items-center justify-center gap-1">
        <AppText variant="amount" tone="subtle">
          {currencySymbol(currencyCode)}
        </AppText>
        <TextInput
          accessibilityLabel={label}
          keyboardType="decimal-pad"
          value={value}
          onChangeText={onChangeText}
          onBlur={onBlur}
          placeholderTextColor={colors["subtle-foreground"]}
          className={`min-w-[120px] max-w-[220px] text-center ${textVariantClassNames.amount} text-foreground`}
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
