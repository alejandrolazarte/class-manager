import { ActivityIndicator, Pressable, Text } from "react-native";

type ButtonVariant = "primary" | "secondary" | "danger";

interface ButtonProps {
  label: string;
  onPress: () => void;
  variant?: ButtonVariant;
  disabled?: boolean;
  isLoading?: boolean;
  testID?: string;
}

const containerClassNames: Record<ButtonVariant, string> = {
  primary: "bg-brand",
  secondary: "border border-brand bg-white",
  danger: "bg-red-600",
};

const labelClassNames: Record<ButtonVariant, string> = {
  primary: "text-white",
  secondary: "text-brand",
  danger: "text-white",
};

export function Button({
  label,
  onPress,
  variant = "primary",
  disabled = false,
  isLoading = false,
  testID,
}: ButtonProps) {
  const isInactive = disabled || isLoading;
  return (
    <Pressable
      accessibilityRole="button"
      testID={testID}
      accessibilityState={{ disabled: isInactive, busy: isLoading }}
      disabled={isInactive}
      onPress={onPress}
      className={`min-h-12 flex-row items-center justify-center rounded-xl px-4 ${containerClassNames[variant]} ${isInactive ? "opacity-50" : ""}`}
    >
      {isLoading ? <ActivityIndicator color="#ffffff" /> : null}
      <Text className={`text-base font-semibold ${labelClassNames[variant]}`}>{label}</Text>
    </Pressable>
  );
}
