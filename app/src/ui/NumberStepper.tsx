import { TextInput, View } from "react-native";
import { AppText, textVariantClassNames } from "@/ui/AppText";
import { IconButton } from "@/ui/IconButton";
import { withRequiredMark } from "@/ui/RequiredFieldsLegend";

interface NumberStepperProps {
  label: string;
  value: string;
  onChange: (value: string) => void;
  onBlur?: () => void;
  minimum: number;
  maximum: number;
  decreaseLabel: string;
  increaseLabel: string;
  errorMessage?: string;
  isRequired?: boolean;
}

const decimalRadix = 10;

export function NumberStepper({
  label,
  value,
  onChange,
  onBlur,
  minimum,
  maximum,
  decreaseLabel,
  increaseLabel,
  errorMessage,
  isRequired = false,
}: NumberStepperProps) {
  const parsedValue = Number.parseInt(value, decimalRadix);
  const currentValue = Number.isNaN(parsedValue) ? minimum : parsedValue;
  const step = (difference: number) =>
    onChange(String(Math.max(minimum, Math.min(maximum, currentValue + difference))));
  return (
    <View className="gap-1.5">
      <AppText variant="label" tone="muted">
        {withRequiredMark(label, isRequired)}
      </AppText>
      <View
        className={`h-[52px] flex-row items-center justify-between rounded-2xl border-[1.5px] bg-surface px-1 ${errorMessage ? "border-danger" : "border-border"}`}
      >
        <IconButton
          icon="remove"
          tone="primary"
          accessibilityLabel={decreaseLabel}
          disabled={currentValue <= minimum}
          onPress={() => step(-1)}
        />
        <TextInput
          accessibilityLabel={label}
          keyboardType="number-pad"
          value={value}
          onChangeText={onChange}
          onBlur={onBlur}
          className={`min-w-0 flex-1 text-center ${textVariantClassNames.input} text-foreground`}
        />
        <IconButton
          icon="add"
          tone="primary"
          accessibilityLabel={increaseLabel}
          disabled={currentValue >= maximum}
          onPress={() => step(1)}
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
