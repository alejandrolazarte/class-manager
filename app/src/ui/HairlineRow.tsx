import { PropsWithChildren } from "react";
import { AccessibilityState, View } from "react-native";
import { PressableScale } from "@/ui/motion/PressableScale";

interface HairlineRowProps extends PropsWithChildren {
  onPress?: () => void;
  accessibilityLabel?: string;
  accessibilityHint?: string;
  accessibilityState?: AccessibilityState;
  hasDivider?: boolean;
  isDimmed?: boolean;
  className?: string;
  testID?: string;
}

const rowClassName = "-mx-2 px-2";
const defaultLayoutClassName = "flex-row items-center gap-3 py-3";

export function HairlineRow({
  onPress,
  accessibilityLabel,
  accessibilityHint,
  accessibilityState,
  hasDivider = true,
  isDimmed = false,
  className,
  testID,
  children,
}: HairlineRowProps) {
  const classNames = [
    rowClassName,
    hasDivider ? "border-b border-border-subtle" : "",
    isDimmed ? "opacity-60" : "",
    className ?? defaultLayoutClassName,
  ].join(" ");
  if (onPress === undefined) {
    return (
      <View className={classNames} testID={testID}>
        {children}
      </View>
    );
  }
  return (
    <PressableScale
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel}
      accessibilityHint={accessibilityHint}
      accessibilityState={accessibilityState}
      onPress={onPress}
      testID={testID}
      className={`${classNames} active:bg-muted`}
    >
      {children}
    </PressableScale>
  );
}
