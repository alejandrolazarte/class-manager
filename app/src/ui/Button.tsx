import { ThemeColorToken } from "@/theme/themeColorTokens";
import { AppText, TextTone } from "@/ui/AppText";
import { Icon, IconName } from "@/ui/Icon";
import { PressableScale } from "@/ui/motion/PressableScale";
import { Spinner } from "@/ui/Spinner";

type ButtonVariant =
  "primary" | "secondary" | "danger" | "dangerOutline" | "dangerGhost" | "dashed" | "ghost";

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
  isExpanded?: boolean;
  accessibilityLabel?: string;
  testID?: string;
}

const containerClassNames: Record<ButtonVariant, string> = {
  primary: "bg-primary active:bg-primary-strong",
  secondary: "border border-border bg-surface active:bg-muted",
  danger: "bg-danger",
  dangerOutline: "border border-border bg-surface active:bg-danger-soft",
  dangerGhost: "bg-transparent active:bg-danger-soft",
  dashed: "border border-dashed border-primary/40 bg-transparent active:bg-primary-soft",
  ghost: "bg-transparent active:bg-primary-soft",
};

const sizeClassNames: Record<ButtonSize, string> = {
  large: "min-h-12 rounded-xl px-5",
  medium: "min-h-11 rounded-xl px-4",
};

const contentTones: Record<ButtonVariant, TextTone> = {
  primary: "onPrimary",
  secondary: "default",
  danger: "onDanger",
  dangerOutline: "danger",
  dangerGhost: "danger",
  dashed: "primary",
  ghost: "primary",
};

const disabledContainerClassNames: Record<ButtonVariant, string> = {
  primary: "bg-border",
  secondary: "border border-border bg-surface",
  danger: "bg-border",
  dangerOutline: "border border-border bg-surface",
  dangerGhost: "bg-transparent",
  dashed: "border border-dashed border-border bg-transparent",
  ghost: "bg-transparent",
};

const contentColorTokens: Record<ButtonVariant, ThemeColorToken> = {
  primary: "primary-foreground",
  secondary: "primary",
  danger: "danger-foreground",
  dangerOutline: "danger",
  dangerGhost: "danger",
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
  isExpanded,
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
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel ?? label}
      testID={testID}
      accessibilityState={{ disabled: isInactive, busy: isLoading, expanded: isExpanded }}
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
    </PressableScale>
  );
}
