import { PropsWithChildren } from "react";
import { Pressable, View } from "react-native";
import { useElevationStyle } from "@/ui/elevation";

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
  const elevationStyle = useElevationStyle();
  const classNames = `overflow-hidden rounded-[20px] bg-surface ${className ?? ""}`;
  if (onPress === undefined) {
    return (
      <View style={elevationStyle} className={classNames} testID={testID}>
        {children}
      </View>
    );
  }
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={accessibilityLabel}
      accessibilityState={{ disabled }}
      disabled={disabled}
      onPress={onPress}
      style={elevationStyle}
      className={`${classNames} active:opacity-80`}
      testID={testID}
    >
      {children}
    </Pressable>
  );
}
