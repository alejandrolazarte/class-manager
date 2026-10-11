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
  trailingIcon?: IconName;
  disabled?: boolean;
  isLoading?: boolean;
  accessibilityLabel?: string;
  testID?: string;
}

const containerClassNames: Record<ButtonVariant, string> = {
  primary: "bg-primary active:bg-primary-strong",
  secondary: "border border-border bg-surface active:bg-muted",
  outline: "border border-primary bg-transparent active:bg-primary-soft",
  danger: "bg-danger",
  dangerOutline: "border border-border bg-surface active:bg-danger-soft",
  dashed: "border border-dashed border-primary bg-transparent active:bg-primary-soft",
  ghost: "bg-transparent active:bg-primary-soft",
};

const sizeClassNames: Record<ButtonSize, string> = {
  large: "min-h-12 rounded-xl px-5",
  medium: "min-h-11 rounded-xl px-4",
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

const disabledContainerClassNames: Record<ButtonVariant, string> = {
  primary: "bg-border",
  secondary: "border border-border bg-surface",
  outline: "border border-border bg-transparent",
  danger: "bg-border",
  dangerOutline: "border border-border bg-surface",
  dashed: "border border-dashed border-border bg-transparent",
  ghost: "bg-transparent",
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
  trailingIcon,
  disabled = false,
  isLoading = false,
  accessibilityLabel,
  testID,
}: ButtonProps) {
  const isInactive = disabled || isLoading;
  const isDisabled = disabled && !isLoading;
  const containerClassName = isDisabled
    ? disabledContainerClassNames[variant]
    : `${containerClassNames[variant]} ${isLoading ? "opacity-70" : ""}`;
  const contentColorToken: ThemeColorToken = isDisabled
    ? "disabled-foreground"
    : contentColorTokens[variant];
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel ?? label}
      testID={testID}
      accessibilityState={{ disabled: isInactive, busy: isLoading }}
      disabled={isInactive}
      onPress={onPress}
      className={`flex-row items-center justify-center gap-2 ${sizeClassNames[size]} ${containerClassName}`}
    >
      {isLoading ? (
        <Spinner tone={contentColorToken} testID="button-spinner" />
      ) : icon ? (
        <Icon name={icon} size="medium" tone={contentColorToken} />
      ) : null}
      <AppText variant="button" tone={isDisabled ? "disabled" : contentTones[variant]}>
        {label}
      </AppText>
      {trailingIcon ? <Icon name={trailingIcon} size="medium" tone={contentColorToken} /> : null}
    </Pressable>
  );
}
