import { PropsWithChildren } from "react";
import { View } from "react-native";
import { PressableScale } from "@/ui/motion/PressableScale";

interface CardProps extends PropsWithChildren {
  className?: string;
  onPress?: () => void;
  accessibilityLabel?: string;
  disabled?: boolean;
  testID?: string;
}

export function Card({
  className,
  onPress,
  accessibilityLabel,
  disabled = false,
  testID,
  children,
}: CardProps) {
  const classNames = `overflow-hidden rounded-[14px] border border-border-subtle bg-surface ${className ?? ""}`;
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
      accessibilityState={{ disabled }}
      disabled={disabled}
      onPress={onPress}
      className={`${classNames} active:opacity-80`}
      testID={testID}
    >
      {children}
    </PressableScale>
  );
}
