import { ThemeColorToken } from "@/theme/themeColorTokens";
import { Icon, IconName } from "@/ui/Icon";
import { PressableScale } from "@/ui/motion/PressableScale";

type IconButtonVariant = "plain" | "compact" | "outlined";

interface IconButtonProps {
  icon: IconName;
  accessibilityLabel: string;
  accessibilityHint?: string;
  onPress: () => void;
  variant?: IconButtonVariant;
  tone?: ThemeColorToken;
  disabled?: boolean;
}

const variantClassNames: Record<IconButtonVariant, string> = {
  plain: "h-11 w-11 active:bg-muted",
  compact: "h-9 w-9 active:bg-muted",
  outlined: "h-10 w-10 border border-border bg-surface active:bg-muted",
};

export function IconButton({
  icon,
  accessibilityLabel,
  accessibilityHint,
  onPress,
  variant = "plain",
  tone = "foreground",
  disabled = false,
}: IconButtonProps) {
  return (
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel}
      accessibilityHint={accessibilityHint}
      accessibilityState={{ disabled }}
      disabled={disabled}
      onPress={onPress}
      hitSlop={4}
      className={`items-center justify-center rounded-full ${variantClassNames[variant]}`}
    >
      <Icon
        name={icon}
        size={variant === "plain" ? "large" : "medium"}
        tone={disabled ? "disabled-foreground" : tone}
      />
    </PressableScale>
  );
}
