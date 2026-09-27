import { Pressable } from "react-native";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { AppText, TextTone } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";
import { Spinner } from "@/ui/Spinner";

type ButtonVariant =
  "primary" | "secondary" | "outline" | "danger" | "dangerOutline" | "dashed" | "ghost";

type ButtonSize = "large" | "medium";

interface ButtonProps {
  label: string;
  onPress: () => void;
  variant?: ButtonVariant;
  size?: ButtonSize;
  icon?: IconName;
  disabled?: boolean;
  isLoading?: boolean;
  accessibilityLabel?: string;
  testID?: string;
}

const containerClassNames: Record<ButtonVariant, string> = {
  primary: "bg-primary active:bg-primary-strong",
  secondary: "border-[1.5px] border-border bg-surface active:bg-muted",
  outline: "border-[1.5px] border-primary bg-transparent active:bg-primary-soft",
  danger: "bg-danger",
  dangerOutline: "border-[1.5px] border-border bg-surface active:bg-danger-soft",
  dashed: "border-[1.5px] border-dashed border-primary bg-transparent active:bg-primary-soft",
  ghost: "bg-transparent active:bg-primary-soft",
};

const sizeClassNames: Record<ButtonSize, string> = {
  large: "min-h-[52px] rounded-2xl px-5",
  medium: "min-h-12 rounded-[14px] px-4",
};

const contentTones: Record<ButtonVariant, TextTone> = {
  primary: "onPrimary",
  secondary: "default",
  outline: "primary",
  danger: "onDanger",
  dangerOutline: "danger",
  dashed: "primary",
  ghost: "primary",
};

const contentColorTokens: Record<ButtonVariant, ThemeColorToken> = {
  primary: "primary-foreground",
  secondary: "primary",
  outline: "primary",
  danger: "danger-foreground",
  dangerOutline: "danger",
  dashed: "primary",
  ghost: "primary",
};

export function Button({
  label,
  onPress,
  variant = "primary",
  size = "large",
  icon,
  disabled = false,
  isLoading = false,
  accessibilityLabel,
  testID,
}: ButtonProps) {
  const isInactive = disabled || isLoading;
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel ?? label}
      testID={testID}
      accessibilityState={{ disabled: isInactive, busy: isLoading }}
      disabled={isInactive}
      onPress={onPress}
      className={`flex-row items-center justify-center gap-2 ${sizeClassNames[size]} ${containerClassNames[variant]} ${isInactive ? "opacity-50" : ""}`}
    >
      {isLoading ? (
        <Spinner tone={contentColorTokens[variant]} testID="button-spinner" />
      ) : icon ? (
        <Icon name={icon} size="medium" tone={contentColorTokens[variant]} />
      ) : null}
      <AppText variant={size === "large" ? "button" : "bodyStrong"} tone={contentTones[variant]}>
        {label}
      </AppText>
    </Pressable>
  );
}
