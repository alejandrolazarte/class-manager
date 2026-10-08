import { Pressable } from "react-native";
import { ThemeColorToken } from "@/theme/themeColorTokens";
import { Icon, IconName } from "@/ui/Icon";

type IconButtonVariant = "plain" | "outlined";

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
  plain: "h-12 w-12 active:bg-muted",
  outlined: "h-11 w-11 border-[1.5px] border-border bg-surface active:bg-muted",
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
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel}
      accessibilityHint={accessibilityHint}
      accessibilityState={{ disabled }}
      disabled={disabled}
      onPress={onPress}
      hitSlop={4}
      className={`items-center justify-center rounded-full ${variantClassNames[variant]} ${disabled ? "opacity-40" : ""}`}
    >
      <Icon name={icon} size={variant === "plain" ? "extraLarge" : "large"} tone={tone} />
    </Pressable>
  );
}
