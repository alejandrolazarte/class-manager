import { PropsWithChildren } from "react";
import { AccessibilityRole, AccessibilityState, View } from "react-native";
import { PressableScale } from "@/ui/motion/PressableScale";

interface HairlineRowProps extends PropsWithChildren {
  onPress?: () => void;
  accessibilityLabel?: string;
  accessibilityRole?: AccessibilityRole;
  accessibilityState?: AccessibilityState;
  disabled?: boolean;
  isDimmed?: boolean;
  className?: string;
  testID?: string;
}

export function HairlineRow({
  onPress,
  accessibilityLabel,
  accessibilityRole = "button",
  accessibilityState,
  disabled = false,
  isDimmed = false,
  className,
  testID,
  children,
}: HairlineRowProps) {
  const classNames = `border-b border-border-subtle py-3 ${isDimmed ? "opacity-70" : ""} ${className ?? ""}`;
  if (onPress === undefined) {
    return (
      <View className={classNames} testID={testID}>
        {children}
      </View>
    );
  }
  return (
    <PressableScale
      accessibilityRole={accessibilityRole}
      accessibilityLabel={accessibilityLabel}
      accessibilityState={accessibilityState ?? { disabled }}
      disabled={disabled}
      onPress={onPress}
      className={`${classNames} active:bg-muted`}
      testID={testID}
    >
      {children}
    </PressableScale>
  );
}
