import { Pressable } from "react-native";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { AppText, TextTone } from "@/ui/AppText";
import { Spinner } from "@/ui/Spinner";

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
  primary: "bg-primary",
  secondary: "border border-primary bg-surface",
  danger: "bg-danger",
};

const labelTones: Record<ButtonVariant, TextTone> = {
  primary: "onPrimary",
  secondary: "primary",
  danger: "onDanger",
};

const spinnerTones: Record<ButtonVariant, ThemeColorToken> = {
  primary: "primary-foreground",
  secondary: "primary",
  danger: "danger-foreground",
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
      {isLoading ? <Spinner tone={spinnerTones[variant]} testID="button-spinner" /> : null}
      <AppText variant="bodyStrong" tone={labelTones[variant]}>
        {label}
      </AppText>
    </Pressable>
  );
}
